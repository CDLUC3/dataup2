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
using System.Web.Http;

namespace Microsoft.Research.DataOnboarding.FileService.Exceptions
{
    /// <summary>
    /// This exception is thrown if the requested repository is not found in the data store
    /// </summary>
    [ExcludeFromCodeCoverage]   // Justification: Boilerplate code for custom exception
    [Serializable]
    public class ValidationException : BaseException
    {
        /// <summary>
        /// Constant for the RepositoryId.
        /// </summary>
        public const string FileIdKey = "FileId";

        /// <summary>
        /// Constant for the name.
        /// </summary>
        public const string RepositoryIdKey = "RepositoryId";

        /// <summary>
        /// Constant for FileName.
        /// </summary>
        public const string FileNameKey = "FileName";

        /// <summary>
        /// Constant for RepositoryName.
        /// </summary>
        public const string RepositoryNameKey = "RepositoryName";

        /// <summary>
        /// Constant for ValidationExceptionType.
        /// </summary>
        public const string ValidationExceptionTypeKey = "ValidationExceptionType";

        /// <summary>
        /// Constant for FileStatus.
        /// </summary>
        public const string FileStatusKey = "FileStatus";

        /// <summary>
        /// Gets or sets the File Id.
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Gets or sets the RepositoryId.
        /// </summary>
        public int RepositoryId { get; set; }

        /// <summary>
        /// Gets or sets the RepositoryName.
        /// </summary>
        public string RepositoryName { get; set; }

        /// <summary>
        /// Gets or sets FileName.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the ValidationId.
        /// </summary>
        public string ValidationExceptionType { get; set; }

        /// <summary>
        /// Gets or sets the FileStatus.
        /// </summary>
        public string FileStatus { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        public ValidationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        public ValidationException(string message)
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Nested exception</param>
        public ValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="nameIdentifier">Name identifier</param>
        /// <param name="errors">Error collection</param>
        public ValidationException(string message, int fileId, int repositoryId, string fileName, string repositoryName, string fileStatus, string validationExceptionType, ICollection<string> errors)
            : base(message, errors)
        {
            this.FileId = fileId;
            this.RepositoryId = repositoryId;
            this.FileName = FileName;
            this.RepositoryName = RepositoryName;
            this.FileStatus = FileStatus;
            this.ValidationExceptionType = validationExceptionType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
        /// </summary>
        /// <param name="info">Serialized object data</param>
        /// <param name="context">Source and destination of a given serialized stream</param>
        private ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.FileId = info.GetInt32(FileIdKey);
            this.RepositoryId = info.GetInt32(RepositoryIdKey);
            this.FileName = info.GetString(FileNameKey);
            this.RepositoryName = info.GetString(RepositoryNameKey);
            this.FileStatus = info.GetString(FileStatusKey);
            this.ValidationExceptionType = info.GetString(ValidationExceptionTypeKey);
        }

        /// <summary>
        /// constructs the HttpError object
        /// </summary>
        /// <param name="message">Message for the exception</param>
        /// <returns></returns>
        public HttpError GetHttpError(string message)
        {
            HttpError error = new HttpError(message)
            {
                {
                    ValidationException.RepositoryIdKey,
                    this.RepositoryId
                },
                {
                    ValidationException.FileIdKey,
                    this.FileId
                },
                {
                    ValidationException.FileNameKey,
                    this.FileName
                },
                 {
                    ValidationException.RepositoryNameKey,
                    this.RepositoryName
                },
                {
                    ValidationException.FileStatusKey,
                    this.FileStatus
                },
                {
                    ValidationException.ValidationExceptionTypeKey,
                    this.ValidationExceptionType
                }
            };

            return error;
        }
    }
}
