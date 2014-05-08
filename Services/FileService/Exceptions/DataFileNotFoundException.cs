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
    public class DataFileNotFoundException : BaseException
    {
        /// <summary>
        /// Constant for the RepositoryId
        /// </summary>
        public const string FileIdKey = "FileId";

        /// <summary>
        /// Constant for the name
        /// </summary>
        public const string FileNameKey = "Name";

        /// <summary>
        /// Gets or sets the File Id
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFileNotFoundException"/> class.
        /// </summary>
        public DataFileNotFoundException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFileNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        public DataFileNotFoundException(string message)
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFileNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Nested exception</param>
        public DataFileNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFileNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="nameIdentifier">Name identifier</param>
        /// <param name="errors">Error collection</param>
        public DataFileNotFoundException(string message, int fileId, string fileName, ICollection<string> errors)
            : base(message, errors)
        {
            this.FileId = fileId;
            this.Name = fileName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
        /// </summary>
        /// <param name="info">Serialized object data</param>
        /// <param name="context">Source and destination of a given serialized stream</param>
        private DataFileNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.FileId = info.GetInt32(FileIdKey);
            this.Name = info.GetString(FileNameKey);
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
                        DataFileNotFoundException.FileIdKey,
                        this.FileId
                    }
                };

            return error;
        }
    }
}
