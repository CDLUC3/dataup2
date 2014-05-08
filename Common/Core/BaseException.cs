// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// Base exception for all the custom exception 
    /// </summary>
    [ExcludeFromCodeCoverage] // Justification: Boiler plate implementation of exception base class
    [Serializable]
    public abstract class BaseException : Exception
    {
        private const string ErrorsKeyName = "ErrorsCollection";

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class.
        /// </summary>
        protected BaseException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        protected BaseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Nested exception</param>
        protected BaseException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="errors">Collection of errors</param>
        protected BaseException(string message, ICollection<string> errors)
            : base(message)
        {
            this.Errors = errors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class.
        /// </summary>
        /// <param name="info">Serialized object data</param>
        /// <param name="context">Source and destination of a given serialized stream</param>
        protected BaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Errors = (ICollection<string>)info.GetValue(ErrorsKeyName, typeof(ICollection<string>));
        }

        #endregion

        #region Poperties

        /// <summary>
        /// Gets or sets the list errors
        /// </summary>
        public ICollection<string> Errors
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
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (null == info)
            {
                throw new ArgumentNullException("info");
            }

            // Add errors to serialization info
            info.AddValue(ErrorsKeyName, this.Errors, typeof(ICollection<string>));

            base.GetObjectData(info, context);
        }

        #endregion
    }
}
