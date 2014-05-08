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
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Fakes;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace QueueService.Tests
{
    [TestClass]
    public class VerifyFileQueueServiceTest
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

        private BaseMessage deletedMessage;
        private File file;
        private bool fileUpdated;
        private bool updatedQueueContent;
        private bool isDeletedFromBlob;

        [TestMethod]
        public void Allow_ProcessMessage()
        {
            this.deletedMessage = null;
            this.fileUpdated = false;
            VerifyFileMessage message = new VerifyFileMessage() { RepositoryId = 1, FileId = 1 };
            IMessageHandler queueService = this.GetVerifyFileQueueService();

            using (ShimsContext.Create())
            {
                ShimConfigReader<int>.GetRepositoryConfigValuesStringString = (baseRepositoryName, SettingName) =>
                {
                    return 3;
                };

                queueService.ProcessMessage(message);
            }

            Assert.IsTrue(this.deletedMessage == message);
            Assert.IsTrue(this.isDeletedFromBlob);
            Assert.IsTrue(this.file.Status == FileStatus.Posted.ToString());
        }

        [TestMethod]
        public void Retry_When_CheckIfFileExistsOnExternalRepository_Returns_Failure()
        {
            this.deletedMessage = null;
            this.fileUpdated = false;

            int startCount = 1;
            VerifyFileMessage message = new VerifyFileMessage() { RepositoryId = 1, FileId = 1, RetryCount = startCount };

            VerifyFileQueueService queueService = this.GetVerifyFileQueueService();

            this.fileService = new StubIFileService()
            {
                CheckIfFileExistsOnExternalRepositoryVerifyFileMessage = (verifyMessage) =>
                {
                    return OperationStatus.CreateFailureStatus(string.Empty);
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

                queueService.ProcessMessage(message);
            }

            Assert.IsTrue(message.RetryCount == startCount + 1);
            Assert.IsTrue(updatedQueueContent);
            Assert.IsNull(this.deletedMessage);
            Assert.IsFalse(this.isDeletedFromBlob);
        }

        [TestMethod]
        public void Error_When_CheckIfFileExistsOnExternalRepository_Returns_Failure_AND_Exceeds_Maximum_Retries()
        {
            this.deletedMessage = null;
            this.fileUpdated = false;
            int startCount = 2;

            VerifyFileMessage message = new VerifyFileMessage() { RepositoryId = 1, FileId = 1, RetryCount = startCount };
            VerifyFileQueueService queueService = this.GetVerifyFileQueueService();

            this.fileService = new StubIFileService()
            {
                CheckIfFileExistsOnExternalRepositoryVerifyFileMessage = (verifyMessage) =>
                {
                    return OperationStatus.CreateFailureStatus(string.Empty);
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

                queueService.ProcessMessage(message);
            }

            Assert.IsTrue(this.file.Status == FileStatus.Error.ToString());
            Assert.IsTrue(this.deletedMessage == message);
            Assert.IsFalse(this.isDeletedFromBlob);
        }

        [TestMethod]
        public void Error_When_CheckIfFileExistsOnExternalRepository_Throws_Exception()
        {
            this.deletedMessage = null;
            this.fileUpdated = false;

            int startCount = 0;
            VerifyFileMessage message = new VerifyFileMessage() { RepositoryId = 1, FileId = 1, RetryCount = startCount };

            VerifyFileQueueService queueService = this.GetVerifyFileQueueService();

            this.fileService = new StubIFileService()
            {
                CheckIfFileExistsOnExternalRepositoryVerifyFileMessage = (verifyMessage) =>
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

                queueService.ProcessMessage(message);
            }

            Assert.IsTrue(this.file.Status == FileStatus.Error.ToString());
            Assert.IsTrue(this.deletedMessage == message);
            Assert.IsFalse(this.isDeletedFromBlob);
        }

        /// <summary>
        /// Reuturns the VerifyFileQueueService instance.
        /// </summary>
        /// <returns>VerifyFileQueueService instance.</returns>
        private VerifyFileQueueService GetVerifyFileQueueService()
        {
            this.fileService = new StubIFileService()
            {
                CheckIfFileExistsOnExternalRepositoryVerifyFileMessage = (message) =>
                    {
                        return OperationStatus.CreateSuccessStatus();
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
                DeleteFromQueueBaseMessage = (message) =>
                {
                    this.deletedMessage = message;
                },
                UpdateMessageBaseMessage = (message) =>
                {
                    this.updatedQueueContent = true;
                }
            };

            this.blobRepository = new StubIBlobDataRepository()
            {
                DeleteFileString = (filename) =>
                    {
                        this.isDeletedFromBlob = true;
                        return this.isDeletedFromBlob;
                    }
            };

            VerifyFileQueueService queueService = new VerifyFileQueueService(this.fileServiceFactory, this.repositoryService, this.queueRepository, this.blobRepository);
            return queueService;
        }

    }
}
