// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.WebApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.Research.DataOnboarding.WebApplication.ViewModels
{
    /// <summary>
    /// View model class for add edit repository.
    /// </summary>
    public class ManageReopsitoryViewModel
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="ManageReopsitoryViewModel" /> class.
        /// </summary>
        public ManageReopsitoryViewModel()
        {
            this.VisibilityOptions = Helper.GetVisibilityList();
            this.RepositoryMetaDataFieldList = new List<RepositoryMetadataFieldViewModel>();
        }

        /// <summary>
        /// Gets or sets repository id.
        /// </summary>
        public int RepositoryId { get; set; }

        /// <summary>
        /// Gets or sets repository name.
        /// </summary>
        public string RepositoryName { get; set; }

        /// <summary>
        /// Gets or sets rule created date time.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets rule created user id.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets indicating whether impersonating user ot not.
        /// </summary>
        public bool IsImpersonate { get; set; }

        /// <summary>
        /// Gets ors sets impersonate user name.
        /// </summary>
        public string ImpersonateUserName { get; set; }

        /// <summary>
        /// Gets ors sets impersonate password.
        /// </summary>
        public string ImpersonatePassword { get; set; }

        /// <summary>
        /// Gets or sets identifier url of the repository.
        /// </summary>
        public string GetIdentifierURL { get; set; }

        /// <summary>
        /// Gets or sets post file url of the repository.
        /// </summary>
        public string PostFileURL { get; set; }

        /// <summary>
        /// Gets or sets download fiel url of the repository.
        /// </summary>
        public string DownloadFileURL { get; set; }

        /// <summary>
        /// Gets or sets delete file url of the repository.
        /// </summary>
        public string DeleteFielURL { get; set; }

        /// <summary>
        /// Gets or sets user agreement text for the repository.
        /// </summary>
        [AllowHtml]
        public string UserAgreement { get; set; }

        /// <summary>
        /// Gets or sets allowed files types for the repository.
        /// </summary>
        public string AllowedFileTypes { get; set; }

        /// <summary>
        /// Gets or sets indicating whether the repository is visible to all or for admins only.
        /// </summary>
        public bool IsVisibleToAll { get; set; }

        /// <summary>
        /// Gets or sets base repository id (Repository type).
        /// </summary>
        public int BaseRepositoryId { get; set; }

        /// <summary>
        /// Gets or sets list of repository types.
        /// </summary>
        public SelectList RepositoryTypes { get; set; }

        /// <summary>
        /// Gets or sets selected visibility option.
        /// </summary>
        public int VisibilityOption { get; set; }

        /// <summary>
        /// Gets or sets list of visibility types.
        /// </summary>
        public SelectList VisibilityOptions { get; set; }

        /// <summary>
        /// Gets or sets the List of Repository view models
        /// </summary>
        public List<RepositoryMetadataFieldViewModel> RepositoryMetaDataFieldList { get; set; }

        /// <summary>
        /// Gets or sets the Repository meta data id
        /// </summary>
        public int RepositoryMetaDataId { get; set; }

        /// <summary>
        /// Gets or sets the Repository meta data Name
        /// </summary>
        public string RepositoryMetaDataName { get; set; }

        /// <summary>
        /// Gets or sets the Deleted MetaData field ids
        /// </summary>
        public string DeletedMetaDataFieldIds { get; set; }

        /// <summary>
        /// Gets or Sets the Access Token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or Sets the Refresh Token
        /// </summary>
        public string RefreshToken { get; set; }

        public DateTime? TokenExpiresOn { get; set; }
    }
}