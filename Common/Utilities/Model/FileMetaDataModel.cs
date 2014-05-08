// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    /// <summary>
    /// File MetaData model
    /// </summary>
    public class FileMetaDataModel
    {
        public FileMetaDataModel()
        {
            this.FileMetaDataFields = new List<FileMetadataField>();
            this.FileColumns = new List<FileColumn>();
        }

        /// <summary>
        /// Gets of sets the File Id
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Gets or sets the collection of File meta data fields
        /// </summary>
        public ICollection<FileMetadataField> FileMetaDataFields { get; set; }

        /// <summary>
        /// Gets or sets the collection of file columns
        /// </summary>
        public ICollection<FileColumn> FileColumns { get; set; }
    }
}
