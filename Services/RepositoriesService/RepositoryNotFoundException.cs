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
    public class RepositoryNotFoundException : BaseException
    {
        /// <summary>
        /// Constant for the RepositoryId
        /// </summary>
        public const string RepositoryIdKey = "RepositoryId";

        /// <summary>
        /// Constant for the name
        /// </summary>
        public const string RepositoryNameKey = "Name";

        /// <summary>
        /// Gets or sets the Repository Id
        /// </summary>
        public int RepositoryId { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryNotFoundException"/> class.
        /// </summary>
        public RepositoryNotFoundException()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        public RepositoryNotFoundException(string message)
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Nested exception</param>
        public RepositoryNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="nameIdentifier">Name identifier</param>
        /// <param name="errors">Error collection</param>
        public RepositoryNotFoundException(string message, int repositoryId, string repositoryName, ICollection<string> errors)
            : base(message, errors)
        {
            this.RepositoryId = repositoryId;
            this.Name = repositoryName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
        /// </summary>
        /// <param name="info">Serialized object data</param>
        /// <param name="context">Source and destination of a given serialized stream</param>
        private RepositoryNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.RepositoryId = info.GetInt32(RepositoryIdKey);
            this.Name = info.GetString(RepositoryNameKey);
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
                        RepositoryNotFoundException.RepositoryIdKey,
                        this.RepositoryId
                    }
                };

            return error;
        }
    }
}
