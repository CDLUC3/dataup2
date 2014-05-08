//--------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.Utilities.Enums;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    /// <summary>
    /// Model class for VerifyFileMessage.
    /// </summary>
    public class VerifyFileMessage : BaseMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VerifyFileMessage" /> class.
        /// </summary>
        public VerifyFileMessage()
        {
            base.MessageHandler = MessageHandlerEnum.VerifyMessageHandler;
        }
    }
}
