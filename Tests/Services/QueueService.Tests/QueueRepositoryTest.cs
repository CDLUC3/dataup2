// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.QueueService;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Fakes;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace QueueService.Tests
{
    [TestClass]
    public class QueueRepositoryTest
    {
        private const string StorageConnection = "UseDevelopmentStorage=true";
        private const string QueueName = "unittestqueue";
        private const int NumberOfMessagesToRead = 32;
        private const int VisibilityTimeOut = 1;
        
        [TestMethod]
        public void Allow_Add_Queue_Messages()
        {
            IQueueRepository queueRepository = this.GetQueueRepository();
            BaseMessage message = this.GetMessage();

            queueRepository.AddMessageToQueue(message);
            BaseMessage retrieveMessage = queueRepository.GetQueuedMessages(NumberOfMessagesToRead, VisibilityTimeOut).Find(m => m.FileId == message.FileId && m.RepositoryId == message.RepositoryId);
            Assert.IsNotNull(retrieveMessage);
        }

        [TestMethod]
        public void Allow_Update_Queue_Messages()
        {
            IQueueRepository queueRepository = this.GetQueueRepository();
            BaseMessage message = this.GetMessage();

            queueRepository.AddMessageToQueue(message);
            BaseMessage retrieveMessage = queueRepository.GetQueuedMessages(NumberOfMessagesToRead, VisibilityTimeOut).Find(m => m.FileId == message.FileId && m.RepositoryId == message.RepositoryId);
            retrieveMessage.RetryCount += 1;
            queueRepository.UpdateMessage(retrieveMessage);

            BaseMessage updatedMessage = queueRepository.GetQueuedMessages(NumberOfMessagesToRead, VisibilityTimeOut).Find(m => m.FileId == message.FileId && m.RepositoryId == message.RepositoryId);

            Assert.IsNotNull(retrieveMessage);
            Assert.IsTrue(updatedMessage.RetryCount == retrieveMessage.RetryCount);
        }

        [TestMethod]
        public void Allow_Delete_Queue_Message()
        {
            IQueueRepository queueRepository = this.GetQueueRepository();
            BaseMessage message = this.GetMessage();

            queueRepository.AddMessageToQueue(message);
            BaseMessage retrieveMessage = queueRepository.GetQueuedMessages(NumberOfMessagesToRead, VisibilityTimeOut).Find(m => m.FileId == message.FileId && m.RepositoryId == message.RepositoryId);
               
            queueRepository.DeleteFromQueue(retrieveMessage);

            BaseMessage deletedMessage = queueRepository.GetQueuedMessages(NumberOfMessagesToRead, VisibilityTimeOut).Find(m => m.FileId == message.FileId && m.RepositoryId == message.RepositoryId);

            Assert.IsNull(deletedMessage);
        }

        [TestCleanup]
        public void Cleanup()
        {
            IQueueRepository queueRepository = this.GetQueueRepository();
            List<BaseMessage> retrieveMessages = queueRepository.GetQueuedMessages(NumberOfMessagesToRead, VisibilityTimeOut);
            foreach(BaseMessage message in retrieveMessages)
            {
                queueRepository.DeleteFromQueue(message);
            }
        }

        private IQueueRepository GetQueueRepository()
        {
            IQueueRepository queueRepository;
            using (ShimsContext.Create())
            {
                ShimConfigReader<string>.GetConfigSettingStringT0 = (settingName, defaultValue) =>
                {
                    switch (settingName)
                    {
                        case Constants.StorageSettingName:
                            return StorageConnection;
                        case Constants.PublishQueueName:
                            return QueueName;
                        default:
                            return defaultValue;
                    }
                };

                queueRepository = new QueueRepository();
            }

            return queueRepository;
        }

        private BaseMessage GetMessage()
        {
            Random rnd = new Random(1);
            int fileId = rnd.Next(1, 1000);
            int repositoryId = rnd.Next(1, 1000);
            BaseMessage message = new PublishMessage()
            {
                FileId = fileId,
                RepositoryId = repositoryId,
                AuthToken = new AuthToken() { AccessToken = "accessToken", RefreshToken = "refreshToken", TokenExpiresOn = DateTime.UtcNow.AddHours(1) },
                UserName = "userName",
                Password = "password",
                ProcessOn = DateTime.Now.AddHours(1),
                RetryCount = 2
            };

            return message;
        }
    }
}
