// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.Services.UserService
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using Microsoft.Research.DataOnboarding.Core;
    using Microsoft.Research.DataOnboarding.Utilities;

    /// <summary>
    /// This exception is thrown if the requested user is not found in the data store
    /// </summary>
    [ExcludeFromCodeCoverage]   // Justification: Boilerplate code for custom exception
    [Serializable]
    public sealed class UserNotFoundException : BaseException
    {
        private const string UserIdKeyName = "UserId";
        private const string NameIdentifierKeyName = "NameIdentifier";
        private const string IdentityProviderKeyName = "IdentityProviderKeyName";

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
        /// </summary>
        public UserNotFoundException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        public UserNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Nested exception</param>
        public UserNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="nameIdentifier">Name identifier</param>
        /// <param name="errors">Error collection</param>
        public UserNotFoundException(string message, string nameIdentifier, ICollection<string> errors)
            : base(message, errors)
        {
            this.NameIdentifier = nameIdentifier;
            this.UserId = 0; // default value
        }

        
        public UserNotFoundException(string message, int userId, ICollection<string> errors)
            : base(message, errors)
        {
            this.UserId = userId;
            this.NameIdentifier = string.Empty; // default value
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
        /// </summary>
        /// <param name="info">Serialized object data</param>
        /// <param name="context">Source and destination of a given serialized stream</param>
        private UserNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.NameIdentifier = info.GetString(NameIdentifierKeyName);
            this.UserId = info.GetInt32(UserIdKeyName);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a Name identifier to uniquely identify the user
        /// </summary>
        public string NameIdentifier
        {
            get;
            set;
        }

        public int UserId
        {
            get;
            set;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Adds exception properties to the serialization object. 
        /// </summary>
        /// <param name="info">Serialized object data</param>
        /// <param name="context">Source and destination of a given serialized stream</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Using Check helper method to validate.")]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Check.IsNotNull(info, "info");

            info.AddValue(NameIdentifierKeyName, this.NameIdentifier);
            info.AddValue(UserIdKeyName, this.UserId);

            base.GetObjectData(info, context);
        }

        #endregion
    }
}
