// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    /// <summary>
    /// Model class for post file details.
    /// </summary>
    public class PostFileModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostFileModel" /> class.
        /// </summary>
        public PostFileModel()
        {
            this.FileMetaData = new FileMetaDataModel();
            this.FileMetaData.FileColumns = new List<FileColumn>();
            this.FileMetaData.FileMetaDataFields = new List<FileMetadataField>();
            this.UserAuthToken = new AuthToken();
        }

        /// <summary>
        /// Gets or sets file id.
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Gets or sets file object.
        /// </summary>
        public File FileDetails { get; set; }

        /// <summary>
        /// Gets or sets selected repository id.
        /// </summary>
        public int SelectedRepositoryId { get; set; }

        /// <summary>
        /// Gets or sets base repository id.
        /// </summary>
        public int BaseRepositoryId { get; set; }

        /// <summary>
        /// Gets or sets user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Get or sets RPS authentication token.
        /// </summary>
        public string RPSToken { get; set; }

        /// <summary>
        /// Gets or sets who is positing the file.
        /// </summary>
        public string Who { get; set; }

        /// <summary>
        /// Gets or sets what user is posting.
        /// </summary>
        public string What { get; set; }

        /// <summary>
        /// Gets or sets indicating whether the call is to publish or not.
        /// </summary>
        public bool isPublish { get; set; }

        /// <summary>
        /// Gets or sets when user is posting.
        /// </summary>
        public DateTime When { get; set; }

        /// <summary>
        /// Gets or sets the File metadata object
        /// </summary>
        public FileMetaDataModel FileMetaData { get; set; }

        /// <summary>
        /// Gets or sets available repositories for the selected file.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification="Used to set and get the values" )]
        public IList<Repository> RepositoryList { get; set; }
        
        /// <summary>
        /// Gets or sets available File Meta Data Fields
        /// </summary>
        public ICollection<FileMetadataField> FileMetaDataFields { get; set; }

        /// <summary>
        /// Gets or sets the List of document sheets
        /// </summary>
        public IEnumerable<FileSheet> FileSheets { get; set; }

        /// <summary>
        /// Gets or sets the metadata that are as part of sheet
        /// used to set the metadata in post detail page from the UPLOADED file if metadata exists in the file
        /// </summary>
        public FileMetaDataModel FileMetaDataDetailsFromSheet { get; set; }

        /// <summary>
        /// Gets or sets the UserAuthToken.
        /// </summary>
        public AuthToken UserAuthToken { get; set; }
    }
}
