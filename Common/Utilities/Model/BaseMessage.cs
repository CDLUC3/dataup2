// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.Utilities.Enums;
using System;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    /// <summary>
    /// Model for BaseMessage.
    /// </summary>
    [DataContract]
    public class BaseMessage
    {
        /// <summary>
        /// Gets or sets the Field Id.
        /// </summary>
        [DataMember]
        public int FileId { get; set; }

        /// <summary>
        /// Gets or sets the Repository Id.
        /// </summary>
        [DataMember]
        public int RepositoryId { get; set; }

        /// <summary>
        /// Gets or sets the UserId.
        /// </summary>
        [DataMember]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the MessageHandler.
        /// </summary>
        [DataMember]
        public MessageHandlerEnum MessageHandler { get; set; }

        /// <summary>
        /// Gets or sets the ProcessOn.
        /// </summary>
        [DataMember]
        public DateTime ProcessOn { get; set; }

        /// <summary>
        /// Gets or sets the RetryCount.
        /// </summary>
        [DataMember]
        public int RetryCount { get; set; }

        /// <summary>
        /// Gets or sets user name.
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets password.
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets CloudMessage.
        /// </summary>
        public CloudQueueMessage CloudMessage { get; set; }
    }
}
