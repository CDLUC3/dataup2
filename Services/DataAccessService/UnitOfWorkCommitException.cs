// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.DataAccessService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using Microsoft.Research.DataOnboarding.Core;

    /// <summary>
    /// This exception is thrown when the unit of work commit operation fails
    /// </summary>
    [ExcludeFromCodeCoverage]   // Justification: Boilerplate code for custom exception
    [Serializable]
    public class UnitOfWorkCommitException : BaseException
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkCommitException"/> class.
        /// </summary>
        public UnitOfWorkCommitException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkCommitException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        public UnitOfWorkCommitException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkCommitException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Nested exception</param>
        public UnitOfWorkCommitException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkCommitException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="errors">Error collection</param>
        public UnitOfWorkCommitException(string message, ICollection<string> errors)
            : base(message, errors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkCommitException"/> class.
        /// </summary>
        /// <param name="info">Serialized object data</param>
        /// <param name="context">Source and destination of a given serialized stream</param>
        private UnitOfWorkCommitException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
