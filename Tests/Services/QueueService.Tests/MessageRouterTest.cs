// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.QueueService;
using Microsoft.Research.DataOnboarding.QueueService.Fakes;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace QueueService.Tests
{
    [TestClass]
    public class MessageRouterTest
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
        /// Contains the list of messages.
        /// </summary>
        private List<BaseMessage> messages;

        /// <summary>
        /// contains the reference to processed message.
        /// </summary>
        private List<BaseMessage> processedMessage;

        [TestMethod]
        public void Allow_Process()
        {
            this.messages = new List<BaseMessage>();
            this.messages.Add(new PublishMessage() { RepositoryId = 1, FileId = 1 });
            this.messages.Add(new VerifyFileMessage() { RepositoryId = 1, FileId = 1 });
            

            MessageRouter messageRouter = this.GetMessageRouter(false);
            messageRouter.ProcessQueue();
            Assert.IsTrue(this.messages.Count == this.processedMessage.Count);
        }

        [TestMethod]
        public void Skip_Process_When_ProcessOn_Is_Greater_Than_Now()
        {
            this.messages = new List<BaseMessage>();
            this.messages.Add(new PublishMessage() { RepositoryId = 1, FileId = 1, ProcessOn=DateTime.UtcNow.AddHours(1) });
            this.messages.Add(new VerifyFileMessage() { RepositoryId = 1, FileId = 1, ProcessOn = DateTime.UtcNow.AddMinutes(5)});

            MessageRouter messageRouter = this.GetMessageRouter(false);
            messageRouter.ProcessQueue();

            Assert.IsTrue(this.processedMessage.Count == 0);
        }

        [TestMethod]
        public void Skip_Process_When_Handler_Is_Null()
        {
            this.messages = new List<BaseMessage>();
            this.messages.Add(new PublishMessage() { RepositoryId = 1, FileId = 1});
            this.messages.Add(new VerifyFileMessage() { RepositoryId = 1, FileId = 1});

            MessageRouter messageRouter = this.GetMessageRouter(true);
            messageRouter.ProcessQueue();

            Assert.IsTrue(this.processedMessage.Count == 0);
        }

        /// <summary>
        /// Reuturns the message Router instance.
        /// </summary>
        /// <param name="nullMessageHandler">Indicates if the handler has to be returned or not.</param>
        /// <returns>MessaeRouter instance.</returns>
        private MessageRouter GetMessageRouter(bool nullMessageHandler)
        {
            this.processedMessage = new List<BaseMessage>();
            this.fileServiceFactory = new StubIFileServiceFactory();
            this.repositoryService = new StubIRepositoryService();
            this.blobRepository = new StubIBlobDataRepository();
            IMessageHandler messageHandler = null;

            if (!nullMessageHandler)
            {
                messageHandler = new StubIMessageHandler()
                {
                    ProcessMessageBaseMessage = (message) =>
                        {
                            this.processedMessage.Add(message);
                        }
                };
            }
            
            this.messageHandlerFactory = new StubIMessageHandlerFactory
            {
                GetMessageHandlerMessageHandlerEnum = (handler) => {
                    
                    return messageHandler;
                }
            };

            this.queueRepository = new StubIQueueRepository()
            {
                GetQueuedMessagesInt32Int32 = (noofmessages, visibilityTimeout) =>
                    {
                        return this.messages;
                    }
            };

            MessageRouter messageRouter = new MessageRouter(this.messageHandlerFactory, this.queueRepository);
            return messageRouter;
        }
    }
}
