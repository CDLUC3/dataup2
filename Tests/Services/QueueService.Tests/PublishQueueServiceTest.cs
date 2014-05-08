// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.QueueService;
using Microsoft.Research.DataOnboarding.QueueService.Fakes;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Fakes;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace QueueService.Tests
{
    [TestClass]
    public class PublishQueueServiceTest
    {
        /// <summary>
        /// Contains the reference to queueRepository.
        /// </summary>
        private IQueueRepository queueRepository;

        /// <summary>
        /// Contains the reference to fileServiceFactory.
        /// </summary>
        private IFileServiceFactory fileServiceFactory;

        /// <summary>
        /// Contains the reference to repositoryService.
        /// </summary>
        private IRepositoryService repositoryService;

        /// <summary>
        /// Contains the reference to blobRepository.
        /// </summary>
        private IBlobDataRepository blobRepository;

        /// <summary>
        /// Contains the reference to fileServiceFactory.
        /// </summary>
        private IMessageHandlerFactory messageHandlerFactory;

        /// <summary>
        /// Contains the reference to fileService.
        /// </summary>
        private IFileService fileService;

        private BaseMessage addedMessage;
        private BaseMessage deletedMessage;
        private File file;
        private bool fileUpdated;

        private bool updatedQueueContent;

        [TestMethod]
        public void Allow_ProcessMessage()
        {
            this.addedMessage = null;
            this.deletedMessage = null;
            this.fileUpdated = false;
            PublishMessage message = new PublishMessage() { RepositoryId = 1, FileId = 1 };
            IMessageHandler publishQueueService = this.GetPublishQueueService();

            using (ShimsContext.Create())
            {
                ShimConfigReader<int>.GetRepositoryConfigValuesStringString = (baseRepositoryName, SettingName) =>
                    {
                        return 3;
                    };

                publishQueueService.ProcessMessage(message);
            }

            Assert.IsTrue(this.deletedMessage == message);
            Assert.IsTrue(this.file.Status == FileStatus.Verifying.ToString());
            
            var verifyFileMessage = this.addedMessage as VerifyFileMessage;
            Assert.IsNotNull(verifyFileMessage);
        }

        [TestMethod]
        public void Retry_When_PublishFile_Returns_Null()
        {
            this.addedMessage = null;
            this.deletedMessage = null;
            this.fileUpdated = false;

            int startCount = 1;
            PublishMessage message = new PublishMessage() { RepositoryId = 1, FileId = 1, RetryCount = startCount };

            PublishQueueService publishQueueService = this.GetPublishQueueService();

            this.fileService = new StubIFileService()
            {
                PublishFilePublishMessage = (publishMessage) =>
                {
                    return string.Empty;
                }
            };

            this.fileService = new StubIFileService()
            {
                PublishFilePublishMessage = (publishMessage) =>
                {
                    return null;
                }
            };

            using (ShimsContext.Create())
            {
                ShimConfigReader<int>.GetRepositoryConfigValuesStringString = (baseRepositoryName, SettingName) =>
                {
                    return 3;
                };

                publishQueueService.ProcessMessage(message);
            }

            Assert.IsTrue(message.RetryCount == startCount+1);
            Assert.IsTrue(updatedQueueContent);
            Assert.IsNull(this.deletedMessage);
        }

        [TestMethod]
        public void Error_When_PublishFile_Returns_Null_AND_Exceeds_Maximum_Retries()
        {
            this.addedMessage = null;
            this.deletedMessage = null;
            this.fileUpdated = false;

            int startCount = 2;
            PublishMessage message = new PublishMessage() { RepositoryId = 1, FileId = 1, RetryCount = startCount };

            PublishQueueService publishQueueService = this.GetPublishQueueService();

            this.fileService = new StubIFileService()
            {
                PublishFilePublishMessage = (publishMessage) =>
                    {
                        return null;
                    },
                GetFileByFileIdInt32 = (fileId) =>
                    {
                        this.file = new File() { FileId = 1 };
                        return file;
                    },
                UpdateFileFile = (file) =>
                    {
                        this.file = file;
                        return true;
                    },
            };

            using (ShimsContext.Create())
            {
                ShimConfigReader<int>.GetRepositoryConfigValuesStringString = (baseRepositoryName, SettingName) =>
                {
                    return 3;
                };

                publishQueueService.ProcessMessage(message);
            }

            Assert.IsTrue(this.file.Status == FileStatus.Error.ToString());
            Assert.IsTrue(this.deletedMessage == message);
        }

        [TestMethod]
        public void Error_When_PublishFile_Throws_Exception_AND_Exceeds_Maximum_Retries()
        {
            this.addedMessage = null;
            this.deletedMessage = null;
            this.fileUpdated = false;

            int startCount = 0;
            PublishMessage message = new PublishMessage() { RepositoryId = 1, FileId = 1, RetryCount = startCount };

            PublishQueueService publishQueueService = this.GetPublishQueueService();

            this.fileService = new StubIFileService()
            {
                PublishFilePublishMessage = (publishMessage) =>
                {
                    throw new Exception("test");
                },
                GetFileByFileIdInt32 = (fileId) =>
                {
                    this.file = new File() { FileId = 1 };
                    return file;
                },
                UpdateFileFile = (file) =>
                {
                    this.file = file;
                    return true;
                },
            };

            using (ShimsContext.Create())
            {
                ShimConfigReader<int>.GetRepositoryConfigValuesStringString = (baseRepositoryName, SettingName) =>
                {
                    return 3;
                };

                publishQueueService.ProcessMessage(message);
            }

            Assert.IsTrue(this.file.Status == FileStatus.Error.ToString());
            Assert.IsTrue(this.deletedMessage == message);
        }

        /// <summary>
        /// Reuturns the PublishQueueServicer instance.
        /// </summary>
        /// <param name="nullMessageHandler">Indicates if the handler has to be returned or not.</param>
        /// <returns>MessaeRouter instance.</returns>
        private PublishQueueService GetPublishQueueService()
        {
            this.fileService = new StubIFileService()
            {
                PublishFilePublishMessage = (publishMessage) =>
                    {
                        return "successfull";
                    },
                GetFileByFileIdInt32 = (fileId) =>
                    {
                        this.file = new File() { FileId = 1 };
                        return file;
                    },
                UpdateFileFile = (file) =>
                    {
                        this.file = file;
                        return true;
                    },
            };

            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) =>
                    {
                        return this.fileService;
                    }

            };

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = (repositoryId) =>
                    {
                        return new Repository() { BaseRepository = new BaseRepository() { Name = "SkyDrive", BaseRepositoryId = 2 }, RepositoryId = 1, Name = "test" };
                    }
            };

            this.queueRepository = new StubIQueueRepository()
            {
                AddMessageToQueueBaseMessage = (message) =>
                    {
                        this.addedMessage = message;
                    },
                DeleteFromQueueBaseMessage = (message) =>
                    {
                        this.deletedMessage = message;
                    },
                UpdateMessageBaseMessage = (message) =>
                    {
                        this.updatedQueueContent = true;
                    }
            };

            PublishQueueService publishQueueService = new PublishQueueService(this.fileServiceFactory, this.repositoryService, this.queueRepository);
            return publishQueueService;
        }
       
    }
}
