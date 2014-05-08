// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Microsoft.Research.DataOnboarding.QueueService
{
    /// <summary>
    /// Contains the methods to interact with azure queue.
    /// </summary>
    public class QueueRepository : IQueueRepository
    {
        /// <summary>
        /// Contains the list of known types.
        /// </summary>
        private static List<Type> knownTypes;

        /// <summary>
        /// Holds the reference to queue.
        /// </summary>
        private CloudQueue queue;

        /// <summary>
        /// Storage account variable.
        /// </summary>
        private CloudStorageAccount storageAccount;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueRepository"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Code does not grant its callers access to operations or resources that can be used in a destructive manner.")]
        public QueueRepository()
        {
            string storageAccountConnection = ConfigReader<string>.GetConfigSetting(Constants.StorageSettingName, string.Empty);
            storageAccount = CloudStorageAccount.Parse(storageAccountConnection);
            CloudQueueClient queueClient = this.storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to the queue
            string queueName = ConfigReader<string>.GetConfigSetting(Constants.PublishQueueName, string.Empty);
            this.queue = queueClient.GetQueueReference(queueName);

            // Create the queue if it doesn't already exist
            this.queue.CreateIfNotExists(new QueueRequestOptions() { RetryPolicy = Constants.DefaultRetryPolicy }, new OperationContext());
        }

        /// <summary>
        /// Reads the Message from queue.
        /// </summary>
        /// <param name="numberOfMessages">Number of messages to be read.</param>
        /// <param name="visibiltiyPeriodInSeconds">Number of seconds after which the message will be visible again.</param>
        /// <returns>List of Base Messages.</returns>
        public List<BaseMessage> GetQueuedMessages(int numberOfMessages, int visibiltiyPeriodInSeconds)
        {
            List<BaseMessage> messages = new List<BaseMessage>();
            foreach (CloudQueueMessage cloudMessage in this.queue.GetMessages(numberOfMessages, TimeSpan.FromSeconds(visibiltiyPeriodInSeconds)).ToList())
            {
                BaseMessage baseMessage = this.Deserialize(cloudMessage);
                messages.Add(baseMessage);
            }

            return messages;
        }

        /// <summary>
        /// Adds the message to queue.
        /// </summary>
        /// <param name="message">BaseMessage instance.</param>
        public void AddMessageToQueue(BaseMessage message)
        {
            CloudQueueMessage cloudMessage = new CloudQueueMessage(this.Serialize(message));
            this.queue.AddMessage(cloudMessage);
        }

        /// <summary>
        /// Ads the message to queue asynchronously.
        /// </summary>
        /// <param name="message">BaseMessage instance.</param>
        /// <returns>Task instance.</returns>
        public Task<bool> AddMessageToQueueAsync(BaseMessage message)
        {
            CloudQueueMessage cloudMessage = new CloudQueueMessage(this.Serialize(message));
            var tcs = new TaskCompletionSource<bool>();

            this.queue.BeginAddMessage(
                cloudMessage,
                ar =>
                {
                    try
                    {
                        this.queue.EndAddMessage(ar);
                        tcs.SetResult(true);
                    }
                    catch (Exception exception)
                    {
                        tcs.SetException(exception);
                    }
                },
                    null);

            return tcs.Task;
        }

        /// <summary>
        /// Deletes the message from the queue.
        /// </summary>
        /// <param name="message">BaseMessage instance.</param>
        public void DeleteFromQueue(BaseMessage message)
        {
            this.queue.DeleteMessage(message.CloudMessage);
        }

        /// <summary>
        /// Updates the message content in the queue.
        /// </summary>
        /// <param name="message">BaseMessage instance.</param>
        public void UpdateMessage(BaseMessage message)
        {
            CloudQueueMessage cloudMessage = message.CloudMessage;
            cloudMessage.SetMessageContent(this.Serialize(message));
            this.queue.UpdateMessage(cloudMessage, TimeSpan.FromSeconds(0.0), MessageUpdateFields.Content | MessageUpdateFields.Visibility);
        }

        #region private methods

        /// <summary>
        /// Returns the list of known types.
        /// </summary>
        /// <returns>List of Types.</returns>
        private static List<Type> GetKnownTypes()
        {
            if (knownTypes == null)
            {
                knownTypes = new List<Type>();
                knownTypes.Add(typeof(VerifyFileMessage));
                knownTypes.Add(typeof(PublishMessage));
            }

            return knownTypes;
        }

        /// <summary>
        /// Serializes the BaseMessage.
        /// </summary>
        /// <param name="message">BaseMessage instance.</param>
        /// <returns>Message data as string.</returns>
        private string Serialize(BaseMessage message)
        {
            string messageData = null;
            DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(BaseMessage), GetKnownTypes());
            using (MemoryStream ms = new MemoryStream())
            {
                dataContractJsonSerializer.WriteObject(ms, message);
                messageData = Encoding.Default.GetString(ms.ToArray());
            }

            return messageData;
        }

        /// <summary>
        /// Deserializes the CloudMessage to BaseMessage.
        /// </summary>
        /// <param name="cloudMessage">CloudMessage instance.</param>
        /// <returns>BaseMessage instance.</returns>
        private BaseMessage Deserialize(CloudQueueMessage cloudMessage)
        {
            byte[] bytes = Encoding.Default.GetBytes(cloudMessage.AsString);
            DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(BaseMessage), GetKnownTypes());
            BaseMessage baseMessage;
            using (Stream stream = new MemoryStream(bytes))
            {
                baseMessage = dataContractJsonSerializer.ReadObject(stream) as BaseMessage;
                baseMessage.CloudMessage = cloudMessage;
            }

            return baseMessage;
        }

        #endregion
    }
}
