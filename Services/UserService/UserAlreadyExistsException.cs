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
    /// This exception is thrown when adding a user fails due to an existing user with same keys
    /// </summary>
    [ExcludeFromCodeCoverage] // Justification: Boilerplate code for custom exception
    [Serializable]
    public sealed class UserAlreadyExistsException : BaseException
    {
        private const string NameIdentifierKeyName = "NameIdentifier";
        private const string IdentityProviderKeyName = "IdentityProviderKeyName";

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAlreadyExistsException"/> class.
        /// </summary>
        public UserAlreadyExistsException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        public UserAlreadyExistsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Nested exception</param>
        public UserAlreadyExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="nameIdentifier">Name identifier</param>
        /// <param name="identityProvider">Identity provider that issued the name identifier</param>
        /// <param name="errors">Error collection</param>
        public UserAlreadyExistsException(string message, string nameIdentifier, string identityProvider, ICollection<string> errors)
            : base(message, errors)
        {
            this.NameIdentifier = nameIdentifier;
            this.IdentityProvider = identityProvider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="nameIdentifier">Name identifier</param>
        /// <param name="identityProvider">Identity provider that issued the name identifier</param>
        public UserAlreadyExistsException(string message, string nameIdentifier, string identityProvider)
            : this(message, nameIdentifier, identityProvider, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="info">Serialized object data</param>
        /// <param name="context">Source and destination of a given serialized stream</param>
        private UserAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.NameIdentifier = info.GetString(NameIdentifierKeyName);
            this.IdentityProvider = info.GetString(IdentityProviderKeyName);
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the Name identifier to uniquely identify the user
        /// </summary>
        public string NameIdentifier 
        { 
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Identity provider that issued the name identifier
        /// </summary>
        public string IdentityProvider
        {
            get;
            set;
        }

        #endregion

        #region Overriden methods

        /// <summary>
        /// Adds exception properties to the serialization object. 
        /// </summary>
        /// <param name="info">Serialized object data</param>
        /// <param name="context">Source and destination of a given serialized stream</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Using Check helper method to validate.")]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Check.IsNotNull(info, "info");

            // Add name identifier to serialization info
            info.AddValue(NameIdentifierKeyName, this.NameIdentifier);

            // Add identity provider to serialization info
            info.AddValue(IdentityProviderKeyName, this.IdentityProvider);

            base.GetObjectData(info, context);
        }

        #endregion
    }
}
