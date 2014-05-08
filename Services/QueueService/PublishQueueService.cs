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
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using DM = Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.QueueService
{
    /// <summary>
    /// Handles the PublishMessages and Implements interface for adding the publish message to the queue.
    /// </summary>
    public class PublishQueueService : BaseQueueService, IPublishQueueService, IMessageHandler
    {
       
        /// <summary>
        /// Holds the PublishInterval value which is read from the configuration.
        /// </summary>
        private int publishInterval = 0;
        
        /// <summary>
        /// Holds the reference to IFileServiceFactory instance.
        /// </summary>
        private IFileServiceFactory fileServiceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishQueueService"/> class.
        /// </summary>
        /// <param name="fileServiceFactory">IFileServiceFactory instance.</param>
        /// <param name="repositoryService">IRepository instance.</param>
        /// <param name="queueRepository">QueueRepository instance.</param>
        public PublishQueueService(IFileServiceFactory fileServiceFactory, IRepositoryService repositoryService, IQueueRepository queueRepository)
        {
            this.fileServiceFactory = fileServiceFactory;
            this.RepositoryService = repositoryService;
            this.QueueRepository = queueRepository;
            this.Diagnostics = new DiagnosticsProvider(this.GetType());
        }

        /// <summary>
        /// Posts the message in the queue.
        /// </summary>
        /// <param name="message">BaseMessage instance.</param>
        public void PostFileToQueue(BaseMessage message)
        {
            message.ProcessOn = DateTime.UtcNow;
            this.QueueRepository.AddMessageToQueue(message);
        }

        /// <summary>
        /// Process the publish message.
        /// </summary>
        /// <param name="message">BaseMessage instance.</param>
        public void ProcessMessage(BaseMessage message)
        {
            PublishMessage publishMessage = message as PublishMessage;

            try
            {
                this.Diagnostics.WriteInformationTrace(TraceEventId.Flow, string.Format("begin publish {0}", message.FileId));
                Repository repository = this.RepositoryService.GetRepositoryById(publishMessage.RepositoryId);

                // Read the config values
                this.MaxRetryCount = ConfigReader<int>.GetRepositoryConfigValues(repository.BaseRepository.Name, BaseQueueService.MaxRetry_Count);
                this.publishInterval = ConfigReader<int>.GetRepositoryConfigValues(repository.BaseRepository.Name, BaseQueueService.Publish_Interval);
                this.VerifyFileInterval = ConfigReader<int>.GetRepositoryConfigValues(repository.BaseRepository.Name, BaseQueueService.VerifyFile_Interval);
                this.FileService = this.fileServiceFactory.GetFileService(repository.BaseRepository.Name);

                string fileIdentifier = this.FileService.PublishFile(publishMessage);

                if (!string.IsNullOrEmpty(fileIdentifier))
                {
                    this.AddVerifyFileMessageToQueue(publishMessage);
                    this.UpdateFileStatusAndDeleteFromQueue(publishMessage, fileIdentifier);
                }
                else
                {
                    this.UpdateMessageInQueue(publishMessage, this.publishInterval);
                }

                this.Diagnostics.WriteInformationTrace(TraceEventId.Flow, string.Format("Complete publish {0}", message.FileId));
            }
            catch (WebException webException)
            {
                this.Diagnostics.WriteErrorTrace(TraceEventId.Exception, webException);

                HttpWebResponse httpResponse = (HttpWebResponse)webException.Response;
                HttpStatusCode statusCode = httpResponse.StatusCode;
                if (statusCode == HttpStatusCode.Unauthorized || statusCode == HttpStatusCode.NotFound)
                {
                    this.UpdateFileStatusAsError(publishMessage);
                }
                else
                {
                    this.UpdateMessageInQueue(publishMessage, this.publishInterval);
                }
            }
            catch (Exception exception)
            {
                this.Diagnostics.WriteErrorTrace(TraceEventId.Exception, exception);
                this.UpdateFileStatusAsError(publishMessage);
            }
        }

        #region private methods

        /// <summary>
        /// Creates an instance of VerifyFileMessage from PublishMessage and Adds the VerifyFileMessage to the Queue.
        /// </summary>
        /// <param name="publishMessage">PublishMessage instance.</param>
        private void AddVerifyFileMessageToQueue(PublishMessage publishMessage)
        {
            VerifyFileMessage message = new VerifyFileMessage()
            {
                FileId = publishMessage.FileId,
                RepositoryId = publishMessage.RepositoryId,
                UserId = publishMessage.UserId,
                ProcessOn = DateTime.UtcNow.AddSeconds(this.VerifyFileInterval),
                UserName = publishMessage.UserName,
                Password = publishMessage.Password
            };
           
            this.QueueRepository.AddMessageToQueue(message);

            //// TODO Need to use the Async Method
            //// return this.queueRepository.AddMessageToQueueAsync(message);
        }

        /// <summary>
        /// Updates the FileStatus as Verifying.
        /// </summary>
        /// <param name="publishMessage">PublishMessage instance.</param>
        /// <param name="fileIdentifier">File Identifier.</param>
        private void UpdateFileStatusAndDeleteFromQueue(PublishMessage publishMessage, string fileIdentifier)
        {
            DM.File file = FileService.GetFileByFileId(publishMessage.FileId);
            file.RepositoryId = publishMessage.RepositoryId;
            file.Status = FileStatus.Verifying.ToString();
            file.ModifiedOn = DateTime.UtcNow;
            file.Identifier = fileIdentifier;
            this.FileService.UpdateFile(file);
            this.QueueRepository.DeleteFromQueue(publishMessage);
        }

        #endregion
    }
}
