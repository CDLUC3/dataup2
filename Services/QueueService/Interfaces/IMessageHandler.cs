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
    /// Interface for IMessageHandler.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Process the messages.
        /// </summary>
        /// <param name="baseMessage">BaseMessage instance.</param>
        void ProcessMessage(BaseMessage baseMessage);
    }
}
