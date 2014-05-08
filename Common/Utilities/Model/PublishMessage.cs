// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using System.Runtime.Serialization;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    /// <summary>
    /// Model for PublishMessage.
    /// </summary>
    [DataContract]
    public class PublishMessage : BaseMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublishMessage" /> class.
        /// </summary>
        public PublishMessage()
        {
            base.MessageHandler = MessageHandlerEnum.PublishMessageHandler;
        }

        /// <summary>
        /// Gets or sets the AuthToken.
        /// </summary>
        [DataMember]
        public AuthToken AuthToken { get; set; }
    }
}
