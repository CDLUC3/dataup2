// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System;

namespace Microsoft.Research.DataOnboarding.QueueService
{
    /// <summary>
    /// Serves as base class for QueueService.
    /// </summary>
    public abstract class BaseQueueService
    {
        /// <summary>
        /// Constant for RetryCount configuration.
        /// </summary>
        protected const string MaxRetry_Count = "RetryCount";

        /// <summary>
        /// Constant for VerifyFileInterval Configuration.
        /// </summary>
        protected const string VerifyFile_Interval = "VerifyFileInterval";

        /// <summary>
        /// Constant for PublishInterval Configuration.
        /// </summary>
        protected const string Publish_Interval = "PublishInterval";
        
        /// <summary>
        /// Gets or sets FileService.
        /// </summary>
        protected IFileService FileService { get; set; }
        
        /// <summary>
        /// Gets or sets MazRetryCount.
        /// </summary>
        protected int MaxRetryCount { get; set; }

        /// <summary>
        /// Gets or sets VerifyFileInterval.
        /// </summary>
        protected int VerifyFileInterval { get; set; }

        /// <summary>
        /// Gets or sets QueueRepository.
        /// </summary>
        protected IQueueRepository QueueRepository { get; set; }

        /// <summary>
        /// Gets or sets RepositoryService.
        /// </summary>
        protected IRepositoryService RepositoryService { get; set; }

        /// <summary>
        /// Gets or sets Diagnostics.
        /// </summary>
        protected DiagnosticsProvider Diagnostics { get; set; }
      
        /// <summary>
        /// Updates the retry count and updates the Message in the queue queue 
        /// if the retry count is greater then the max retry count updates the file status as error and deletes the message from queue
        /// </summary>
        /// <param name="message">BaseMessage instance.</param>
        /// <param name="intervalInminutes">Interval In Minutes.</param>
        protected void UpdateMessageInQueue(BaseMessage message, int interval)
        {
            if (message.RetryCount < MaxRetryCount - 1)
            {
                message.RetryCount += 1;
                message.ProcessOn = DateTime.UtcNow.AddSeconds(interval);
                this.QueueRepository.UpdateMessage(message);
                this.Diagnostics.WriteInformationTrace(TraceEventId.Flow, string.Format("Adding the message back to the queue, after exception. RetryCount: {0}", message.RetryCount));
            }
            else
            {
                this.Diagnostics.WriteInformationTrace(TraceEventId.Flow, string.Format("Exceeded Max no of allowed Retries: {0}", message.RetryCount));
                UpdateFileStatusAsError(message);
            }
        }

        /// <summary>
        /// Updates the file status as error.
        /// </summary>
        /// <param name="message">BaseMessage instance.</param>
        protected void UpdateFileStatusAsError(BaseMessage message)
        {
            File file = this.FileService.GetFileByFileId(message.FileId);
            file.RepositoryId = message.RepositoryId;
            file.ModifiedOn = DateTime.UtcNow;
            file.Status = FileStatus.Error.ToString();
            this.FileService.UpdateFile(file);
            this.QueueRepository.DeleteFromQueue(message);
        }
    }
}
