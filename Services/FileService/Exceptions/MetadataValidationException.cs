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
    public class MetadataValidationException : BaseException
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
        /// Constant for FieldIdKey.
        /// </summary>
        public const string FieldIdKey = "FieldId";

        /// <summary>
        /// Constant for MetadataValue
        /// </summary>
        public const string MetaDataValueKey = "MetadataValue";

        /// <summary>
        /// Constant for IsRequired
        /// </summary>
        public const string IsRequiredKey = "IsRequired";

        /// <summary>
        /// Constant for IsRequired
        /// </summary>
        public const string NotFoundKey = "NotFound";

        /// <summary>
        /// Constant for TypeMismatch
        /// </summary>
        public const string TypeMismatchKey = "TypeMismatch";

        /// <summary>
        /// Constant for FieldName.
        /// </summary>
        public const string FieldNameKey = "FieldName";

        public const string MetadataTypeNotFoundKey = "MetadataTypeNotFound";

        /// <summary>
        /// Gets or sets the File Id.
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public int RepositoryId { get; set; }

        /// <summary>
        /// Gets or sets the FieldId.
        /// </summary>
        public int FieldId { get; set; }

        /// <summary>
        /// Gets or sets MetdataValue.
        /// </summary>
        public string MetadataValue { get; set; }

        /// <summary>
        /// Gets or sets IsRequired
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets NotFound
        /// </summary>
        public bool NotFound { get; set; }

        /// <summary>
        /// Gets or sets TypeMismatch
        /// </summary>
        public bool TypeMismatch { get; set; }

        /// <summary>
        /// Gets or sets Field Name.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the MetadataTypeNotFound
        /// </summary>
        public bool MetadataTypeNotFound { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaDataValidationException"/> class.
        /// </summary>
        public MetadataValidationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaDataValidationException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        public MetadataValidationException(string message)
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaDataValidationException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Nested exception</param>
        public MetadataValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaDataValidationException"/> class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="nameIdentifier">Name identifier</param>
        /// <param name="errors">Error collection</param>
        public MetadataValidationException(string message, int fileId, int repositoryId, int fieldId, string fieldName, string metadataValue, bool isRequired, bool notFound, bool typeMismatch, bool metadataTypeNotFound, ICollection<string> errors)
            : base(message, errors)
        {
            this.FileId = fileId;
            this.RepositoryId = repositoryId;
            this.FieldId = fieldId;
            this.MetadataValue = metadataValue;
            this.IsRequired = IsRequired;
            this.NotFound = IsRequired;
            this.FieldName = fieldName;
            this.TypeMismatch = typeMismatch;
            this.MetadataTypeNotFound = metadataTypeNotFound;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
        /// </summary>
        /// <param name="info">Serialized object data</param>
        /// <param name="context">Source and destination of a given serialized stream</param>
        private MetadataValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.FileId = info.GetInt32(FileIdKey);
            this.RepositoryId = info.GetInt32(RepositoryIdKey);
            this.FieldId = info.GetInt32(FieldIdKey);
            this.MetadataValue = info.GetString(MetaDataValueKey);
            this.NotFound = info.GetBoolean(NotFoundKey);
            this.TypeMismatch = info.GetBoolean(TypeMismatchKey);
            this.FieldName = info.GetString(FieldNameKey);
            this.MetadataTypeNotFound = info.GetBoolean(MetadataTypeNotFoundKey);
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
                    MetadataValidationException.RepositoryIdKey,
                    this.RepositoryId
                },
                {
                    MetadataValidationException.FileIdKey,
                    this.FileId
                },
                {
                    MetadataValidationException.FieldIdKey,
                    this.FieldId
                },
                 {
                    MetadataValidationException.MetaDataValueKey,
                    this.MetadataValue
                },
                {
                    MetadataValidationException.NotFoundKey,
                    this.NotFound
                },
                {
                    MetadataValidationException.TypeMismatchKey,
                    this.TypeMismatch
                },
                {
                    MetadataValidationException.FieldNameKey,
                    this.FieldName
                },
                {
                    MetadataValidationException.MetadataTypeNotFoundKey,
                    this.MetadataTypeNotFound
                }
            };

            return error;
        }
    }
}
