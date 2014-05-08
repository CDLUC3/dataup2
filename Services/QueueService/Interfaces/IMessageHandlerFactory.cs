// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.Utilities.Enums;

namespace Microsoft.Research.DataOnboarding.QueueService
{
    /// <summary>
    /// Interface methods for MessageHandlerFactory class.
    /// </summary>
    public interface IMessageHandlerFactory
    {
        /// <summary>
        /// Returns the instance of MessageHandler.
        /// </summary>
        /// <param name="handlerName">Handler Name.</param>
        /// <returns>IMessageHandler Instance.</returns>
        IMessageHandler GetMessageHandler(MessageHandlerEnum handlerName);
    }
}
