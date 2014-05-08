// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Model;

namespace Microsoft.Research.DataOnboarding.QueueService
{
    /// <summary>
    /// Interface for PublishQueueService.
    /// </summary>
    public interface IPublishQueueService
    {
        /// <summary>
        /// Posts the file to Queue.
        /// </summary>
        /// <param name="message">BaseMessage Instance.</param>
        void PostFileToQueue(BaseMessage message);
    }
}
