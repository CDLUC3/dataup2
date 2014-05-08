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

namespace Microsoft.Research.DataOnboarding.RepositoriesService
{
    /// <summary>
    /// This exception is thrown if the requested repository is not found in the data store
    /// </summary>
    [ExcludeFromCodeCoverage]   // Justification: Boilerplate code for custom exception
    [Serializable]
    public class FileDownloadException : BaseException
    {
        /// <summary>
        /// Constant for the RepositoryId
        /// </summary>
        public const string FileIdKey = "FileId";

        /// <summary>
        /// Constant for the name
        /// </summary>
        public const string RepositoryIdKey = "RepositoryId";

        /// <summary>
        /// Constant for ValidationExceptionType.
        /// </summary>
        public const string FileDownLoadExceptionTypeKey = "FileDownloadExceptionType";

        /// <summary>
        /// Gets or sets the File Id
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public int RepositoryId { get; set; }

        /// <summary>
        /// Gets or sets the ValidationId.
        /// </summary>
        public string FileDownloadExceptionType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDownloadException"/> class.
        /// </summary>
        public FileDownloadException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDownloadException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        public FileDownloadException(string message)
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDownloadException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Nested exception</param>
        public FileDownloadException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDownloadException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="fileId">File Id.</param>
        /// <param name="errors">Error collection</param>
        public FileDownloadException(string message, int fileId, int repositoryId, string fileDownloadExceptionType, ICollection<string> errors)
            : base(message, errors)
        {
            this.FileId = fileId;
            this.RepositoryId = repositoryId;
            this.FileDownloadExceptionType = fileDownloadExceptionType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
        /// </summary>
        /// <param name="info">Serialized object data</param>
        /// <param name="context">Source and destination of a given serialized stream</param>
        private FileDownloadException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.FileId = info.GetInt32(FileIdKey);
            this.RepositoryId = info.GetInt32(RepositoryIdKey);
            this.FileDownloadExceptionType = info.GetString(FileDownloadExceptionType);
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
                    FileDownloadException.RepositoryIdKey,
                    this.RepositoryId
                },
                {
                    FileDownloadException.FileIdKey,
                    this.FileId
                },
                {
                    FileDownloadException.FileDownLoadExceptionTypeKey,
                    this.FileDownloadExceptionType
                }
            };

            return error;
        }
    }
}
