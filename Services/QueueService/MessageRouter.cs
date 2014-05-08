// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

namespace Microsoft.Research.DataOnboarding.QueueService
{
    /// <summary>
    /// Retrieves the messages from the Queue and routes the messages to respective handler.
    /// </summary>
    public class MessageRouter : IMessageRouter
    {
        /// <summary>
        /// Constant specifies number of message to read at a time.
        /// </summary>
        private const int NumberOfMessagesToRead = 32;

        /// <summary>
        /// Constant for VisibilityTimeOutAppSettingName.
        /// </summary>
        private const string VisibilityTimeOutAppSettingName = "QueueMessageVisibilityTimeOut";

        /// <summary>
        /// Holds the value for VisibilityTimeout for messages
        /// </summary>
        private readonly int VisibilityTimeOut = 1;

        /// <summary>
        /// Contains the reference to IMessageHandlerFactory.
        /// </summary>
        private IMessageHandlerFactory messageHandlerFactory;

        /// <summary>
        /// Contains the reference to queueRepository.
        /// </summary>
        private IQueueRepository queueRepository;

        /// <summary>
        /// Reference to Diagnostic provider.
        /// </summary>
        private DiagnosticsProvider diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRouter"/> class.
        /// </summary>
        /// <param name="messageHandlerFactory">IMessageHandlerFactory to instantiate IMessageHandler.</param>
        /// <param name="queueRepository">IQueueRepository instance.</param>
        public MessageRouter(IMessageHandlerFactory messageHandlerFactory, IQueueRepository queueRepository)
        {
            this.messageHandlerFactory = messageHandlerFactory;
            this.queueRepository = queueRepository;
            this.VisibilityTimeOut = ConfigReader<int>.GetSetting(VisibilityTimeOutAppSettingName);
            diagnostics = new DiagnosticsProvider(this.GetType());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRouter"/> class.
        /// </summary>
        /// <param name="queueRepository">IQueueRepository instance.</param>
        public MessageRouter(IQueueRepository queueRepository)
        {
            this.queueRepository = queueRepository;
        }

        /// <summary>
        /// Gets the Messages from the Queue and process each message.
        /// </summary>
        public void ProcessQueue()
        {
            try
            {
                List<BaseMessage> messages = queueRepository.GetQueuedMessages(NumberOfMessagesToRead, VisibilityTimeOut);

                diagnostics.WriteInformationTrace(TraceEventId.Flow, string.Format("number of messages {0}", messages.Count));
                foreach (BaseMessage message in messages)
                {
                    RouteMessage(message);
                }
            }
            catch (Exception exception)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, "exception occured while processing the queue");
                diagnostics.WriteErrorTrace(TraceEventId.Exception, exception);
            }
        }

        /// <summary>
        /// Instantiates the message handler and calls the ProcessMessage method.
        /// </summary>
        /// <param name="baseMessage">BaseMessage instance.</param>
        private void RouteMessage(BaseMessage baseMessage)
        {
            try
            {
                // do not process the message if the ProcessOn date time is less then the current time.
                if (baseMessage.ProcessOn > DateTime.UtcNow)
                {
                    diagnostics.WriteInformationTrace(TraceEventId.Flow, string.Format("message not procesed ProcessOntime {0} currentUTCTime {1}", baseMessage.ProcessOn.ToString(), DateTime.UtcNow.ToString()));
                    return;
                }

                IMessageHandler messageHandler = null;
                messageHandler = this.messageHandlerFactory.GetMessageHandler(baseMessage.MessageHandler);

                if (messageHandler == null)
                {
                    diagnostics.WriteInformationTrace(TraceEventId.Flow, string.Format("Message Handler not found FileId:{0} RepositoryId {1}", baseMessage.FileId, baseMessage.RepositoryId));
                    return;
                }

                messageHandler.ProcessMessage(baseMessage);
            }
            catch (Exception exception)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, exception);
            }
        }
    }
}
