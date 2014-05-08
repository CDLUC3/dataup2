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
    public class FileAlreadyPublishedException : BaseException
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
        /// Constant for Error code
        /// </summary>
        public const string FileStatusKey = "FileStatus";

        /// <summary>
        /// Gets or sets the File Id
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public int RepositoryId { get; set; }

        /// <summary>
        /// Gets or sets the FileStatus
        /// </summary>
        public string FileStatus { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAlreadyPublishedException"/> class.
        /// </summary>
        public FileAlreadyPublishedException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAlreadyPublishedException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        public FileAlreadyPublishedException(string message)
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAlreadyPublishedException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Nested exception</param>
        public FileAlreadyPublishedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAlreadyPublishedException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="nameIdentifier">Name identifier</param>
        /// <param name="errors">Error collection</param>
        public FileAlreadyPublishedException(string message, int fileId, int repositoryId, string fileStatus, ICollection<string> errors)
            : base(message, errors)
        {
            this.FileId = fileId;
            this.RepositoryId = repositoryId;
            this.FileStatus = fileStatus;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
        /// </summary>
        /// <param name="info">Serialized object data</param>
        /// <param name="context">Source and destination of a given serialized stream</param>
        private FileAlreadyPublishedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.FileId = info.GetInt32(FileIdKey);
            this.RepositoryId = info.GetInt32(RepositoryIdKey);
            this.FileStatus = info.GetString(FileStatusKey);
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
                    FileAlreadyPublishedException.RepositoryIdKey,
                    this.RepositoryId
                },
                {
                    FileAlreadyPublishedException.FileIdKey,
                    this.FileId
                },
                {
                    FileAlreadyPublishedException.FileStatusKey,
                    this.FileStatus
                }
            };

            return error;
        }
    }
}
