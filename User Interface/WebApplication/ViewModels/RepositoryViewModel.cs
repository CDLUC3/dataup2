// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.WebApplication.ViewModels
{
    /// <summary>
    /// View model class for repository data.
    /// </summary>
    public class RepositoryViewModel
    {
        /// <summary>
        /// Gets or sets the id of the repository.
        /// </summary>
        public int RepositoryId { get; set; }

        /// <summary>
        /// Gets or sets the name of the repository.
        /// </summary>
        public string RepositoryName { get; set; }

        /// <summary>
        /// Gets or sets the authentication mode for the repository.
        /// </summary>
        public string AuthenticationMode { get; set; }

        /// <summary>
        /// Gets or sets the allowed file types of the repository.
        /// </summary>
        public string FileTypes { get; set; }

        /// <summary>
        /// Gets or sets indicating whether the repository is visible to all or for admins only.
        /// </summary>
        public bool IsVisibleToAll { get; set; }

        /// <summary>
        /// Gets or sets the created date of the repository.
        /// </summary>
        public string CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the created user of the repository.
        /// </summary>
        public string CreatedUser { get; set; }
    }
}