// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.Research.DataOnboarding.RepositoriesService
{
    /// <summary>
    /// This exception is thrown if the requested repository is not found in the data store
    /// </summary>
    [ExcludeFromCodeCoverage]   // Justification: Boilerplate code for custom exception
    [Serializable]
    public class FilePublishValidationException : BaseException
    {
        /// <summary>
        /// Constant for the RepositoryId
        /// </summary>
        private const string FileIdKey = "FileId";

        /// <summary>
        /// Constant for the name
        /// </summary>
        private const string RepositoryIdKey = "RepositoryId";

        /// <summary>
        /// Constant for Error code
        /// </summary>
        private const string ErrorCodeKey = "ErrorCode";

        /// <summary>
        /// Gets or sets the File Id
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public int RepositoryId { get; set; }

        /// <summary>
        /// Gets or sets the Error Code
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePublishValidationException"/> class.
        /// </summary>
        public FilePublishValidationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePublishValidationException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        public FilePublishValidationException(string message)
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePublishValidationException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Nested exception</param>
        public FilePublishValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePublishValidationException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="nameIdentifier">Name identifier</param>
        /// <param name="errors">Error collection</param>
        public FilePublishValidationException(string message, int fileId, int repositoryId, string errorCode, ICollection<string> errors)
            : base(message, errors)
        {
            this.FileId = fileId;
            this.RepositoryId = repositoryId;
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
        /// </summary>
        /// <param name="info">Serialized object data</param>
        /// <param name="context">Source and destination of a given serialized stream</param>
        private FilePublishValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.FileId = info.GetInt32(FileIdKey);
            this.RepositoryId = info.GetInt32(RepositoryIdKey);
            this.ErrorCode = info.GetString(ErrorCodeKey);
        }

       
    }
}
