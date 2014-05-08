// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.WebApplication.ViewModels
{
    /// <summary>
    /// View model class for repository popup.
    /// </summary>
    public class RepositoryPopupViewModel
    {
        /// <summary>
        /// Gets or sets repository id.
        /// </summary>
        public int RepositoryId { get; set; }

        /// <summary>
        /// Gets or sets repository name.
        /// </summary>
        public string RepositoryName { get; set; }

        /// <summary>
        /// Gets or sets base repository id.
        /// </summary>
        public int BaseRepositoryId { get; set; }

        /// <summary>
        /// Gets or sets indicating whether credentials are required or not.
        /// </summary>
        public bool IsCredintialsRequired { get; set; }
    }
}