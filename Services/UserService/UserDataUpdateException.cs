// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.Services.UserService
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.Research.DataOnboarding.Core;
    using Microsoft.Research.DataOnboarding.DomainModel;
    using Microsoft.Research.DataOnboarding.Utilities;

    public class UserDataUpdateException : BaseException
    {
        private const string UserKeyName = "User";
        private User user;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDataUpdateException"/> class.
        /// </summary>
        public UserDataUpdateException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDataUpdateException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        public UserDataUpdateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDataUpdateException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Nested exception</param>
        public UserDataUpdateException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDataUpdateException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="user">user object</param>
        /// <param name="errors">Error collection</param>
        public UserDataUpdateException(string message, User user, ICollection<string> errors)
            : base(message, errors)
        {
            this.user = user;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDataUpdateException"/> class.
        /// </summary>
        /// <param name="info">Serialized object data</param>
        /// <param name="context">Source and destination of a given serialized stream</param>
        private UserDataUpdateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.user = info.GetValue(UserKeyName, typeof(User)) as User;
        }

        #endregion

        #region Properties

        public User UserData
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

            // Add name identifier to serialization info
            info.AddValue(UserKeyName, this.user);

            base.GetObjectData(info, context);
        }

        #endregion
    }
}
