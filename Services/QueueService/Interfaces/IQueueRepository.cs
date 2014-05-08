// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.Utilities.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.QueueService
{
    /// <summary>
    /// Interface for QueueRepository.
    /// </summary>
    public interface IQueueRepository
    {
        /// <summary>
        /// Retrieves the messages from the queue.
        /// </summary>
        /// <param name="numberOfMessages">Number of messages to be retrieved.</param>
        /// <param name="visibiltiyPeriodInSeconds">Number of seconds after which the message will be visible again.</param>
        /// <returns>List of Messages.</returns>
        List<BaseMessage> GetQueuedMessages(int numberOfMessages, int visibiltiyPeriodInSeconds);
        
        /// <summary>
        /// Ads the messages to queue.
        /// </summary>
        /// <param name="message">BaseMessage instance.</param>
        void AddMessageToQueue(BaseMessage message);

        /// <summary>
        /// Deletes the message from the queue.
        /// </summary>
        /// <param name="message">BaseMessage instance.</param>
        void DeleteFromQueue(BaseMessage message);

        /// <summary>
        /// Updates the Message.
        /// </summary>
        /// <param name="message">BaseMessage instance.</param>
        void UpdateMessage(BaseMessage message);

        /// <summary>
        /// Async method to add the message to queue.
        /// </summary>
        /// <param name="message">BaseMessage instance.</param>
        /// <returns>Task instance.</returns>
        Task<bool> AddMessageToQueueAsync(BaseMessage message);
    }
}
