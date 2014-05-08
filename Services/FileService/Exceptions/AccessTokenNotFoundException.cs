// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Http;

namespace Microsoft.Research.DataOnboarding.FileService.Exceptions
{
    public class AccessTokenNotFoundException : BaseException
    {
        /// <summary>
        /// constant for UserId
        /// </summary>
        public const string UserIdKeyName = "UserId";

        /// <summary>
        /// constant for RepositoryId
        /// </summary>
        public const string RepositoryIdKeyName = "RepositoryId";

        /// <summary>
        /// Gets or sets the RepositoryId
        /// </summary>
        public int RepositoryId { get; set; }

        /// <summary>
        /// Gets or sets the UserId
        /// </summary>
        public int UserId { get; set; }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenNotFoundException"/> class.
        /// </summary>
        public AccessTokenNotFoundException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        public AccessTokenNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Nested exception</param>
        public AccessTokenNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="userId">User Id</param>
        /// <param name="repositoryId">Repository Id</param>
        /// <param name="errors">Error collection</param>
        public AccessTokenNotFoundException(string message, int userId, int repositoryId, ICollection<string> errors)
            : base(message, errors)
        {
            this.UserId = userId;
            this.RepositoryId = repositoryId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenNotFoundException"/> class.
        /// </summary>
        /// <param name="info">Serialized object data</param>
        /// <param name="context">Source and destination of a given serialized stream</param>
        private AccessTokenNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.RepositoryId = info.GetInt32(RepositoryIdKeyName);
            this.UserId = info.GetInt32(UserIdKeyName);
        }

        #endregion

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
                        UserIdKeyName,
                        this.UserId
                    },
                    {
                        RepositoryIdKeyName,
                        this.RepositoryId
                    }
                };

            return error;
        }
    }
}
