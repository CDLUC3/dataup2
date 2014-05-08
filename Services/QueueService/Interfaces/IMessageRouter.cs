// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.QueueService
{
    /// <summary>
    /// Interface for IMessageRouter.
    /// </summary>
    public interface IMessageRouter
    {
        /// <summary>
        /// Process the Queue.
        /// </summary>
        void ProcessQueue();
    }
}
