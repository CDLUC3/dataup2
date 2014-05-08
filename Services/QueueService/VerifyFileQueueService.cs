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
using System.Net;
using DM = Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.QueueService
{
    /// <summary>
    ///  Handles the VerifyFileMessage.
    /// </summary>
    public class VerifyFileQueueService : BaseQueueService, IMessageHandler
    {
        /// <summary>
        /// Holds the reference to IFileServiceFactory instance.
        /// </summary>
        private IFileServiceFactory fileServiceFactory;
       
        /// <summary>
        /// Holds the reference to IBlobRepository.
        /// </summary>
        private IBlobDataRepository blobDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="VerifyFileQueueService"/> class.
        /// </summary>
        /// <param name="fileServiceFactory">IFileServiceFactory instance.</param>
        /// <param name="repositoryService">IRepositoryService instance.</param>
        /// <param name="queueRepository">IQueueRepository instance.</param>
        /// <param name="blobDataRepository">IBlobRepository instance.</param>
        public VerifyFileQueueService(IFileServiceFactory fileServiceFactory, IRepositoryService repositoryService, IQueueRepository queueRepository, IBlobDataRepository blobDataRepository)
        {
            this.fileServiceFactory = fileServiceFactory;
            this.RepositoryService = repositoryService;
            this.QueueRepository = queueRepository;
            this.blobDataRepository = blobDataRepository;
            this.Diagnostics = new DiagnosticsProvider(this.GetType());
        }

        /// <summary>
        /// Processes VerifyFileMessage.
        /// </summary>
        /// <param name="message">BaseMessage instance.</param>
        public void ProcessMessage(BaseMessage message)
        {
            VerifyFileMessage verifyFileMessage = message as VerifyFileMessage;

            try
            {
                Repository repository = this.RepositoryService.GetRepositoryById(verifyFileMessage.RepositoryId);
                
                // Read the config values
                this.MaxRetryCount = ConfigReader<int>.GetRepositoryConfigValues(repository.BaseRepository.Name, BaseQueueService.MaxRetry_Count);
                this.VerifyFileInterval = ConfigReader<int>.GetRepositoryConfigValues(repository.BaseRepository.Name, BaseQueueService.VerifyFile_Interval);

                this.FileService = this.fileServiceFactory.GetFileService(repository.BaseRepository.Name);
                DM.File file = FileService.GetFileByFileId(verifyFileMessage.FileId);
                OperationStatus status = this.FileService.CheckIfFileExistsOnExternalRepository(verifyFileMessage);

                if (status.Succeeded)
                {
                    this.blobDataRepository.DeleteFile(file.BlobId);
                    this.UpdateFileAndDeleteFromQueue(verifyFileMessage);
                }
                else
                {
                    this.UpdateMessageInQueue(verifyFileMessage, this.VerifyFileInterval);
                }
            }
            catch (WebException webException)
            {
                this.Diagnostics.WriteErrorTrace(TraceEventId.Exception, webException);
                HttpWebResponse httpResponse = (HttpWebResponse)webException.Response;
                HttpStatusCode statusCode = httpResponse.StatusCode;
                if (statusCode == HttpStatusCode.NotFound || statusCode == HttpStatusCode.RequestTimeout)
                {
                    this.UpdateMessageInQueue(verifyFileMessage, this.VerifyFileInterval);
                }
                else
                {
                    this.UpdateFileStatusAsError(verifyFileMessage);
                }
            }
            catch (Exception exception)
            {
                this.Diagnostics.WriteErrorTrace(TraceEventId.Exception, exception);
                this.UpdateFileStatusAsError(verifyFileMessage);
            }
        }

        #region private methods
                
        /// <summary>
        /// Updates the file status as Posted.
        /// </summary>
        /// <param name="message">VerifyFileMessage instance.</param>
        private void UpdateFileAndDeleteFromQueue(VerifyFileMessage message)
        {
            DM.File file = this.FileService.GetFileByFileId(message.FileId);
            file.RepositoryId = message.RepositoryId;
            file.ModifiedOn = DateTime.UtcNow;
            file.PublishedOn = DateTime.UtcNow;
            file.BlobId = string.Empty;
            file.isDeleted = true;
            file.Status = FileStatus.Posted.ToString();
            this.FileService.UpdateFile(file);
            this.QueueRepository.DeleteFromQueue(message);
        }

        #endregion
    }
}
