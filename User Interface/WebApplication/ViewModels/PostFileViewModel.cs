// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.WebApplication.ViewModels
{
    /// <summary>
    /// View model class for published file details.
    /// </summary>
    public class PostFileViewModel
    {
        /// <summary>
        /// Gets or sets file id.
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Gets or sets file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets file type.
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets repository name that file posted.
        /// </summary>
        public string Repository { get; set; }

        /// <summary>
        /// Gets or sets file identifier value.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets file citation information.
        /// </summary>
        public string Citation { get; set; }

        /// <summary>
        /// Gets or sets file published date time.
        /// </summary>
        public string PublishedDateTime { get; set; }

        /// <summary>
        /// Gets or sets file size.
        /// </summary>
        public string Size { get; set; }        

        /// <summary>
        ///  Gets or sets Base Repository Id
        /// </summary>
        public int BaseRepositoryId { get; set; }

        /// <summary>
        /// Gets or sets Impersonation
        /// </summary>
        public bool? IsImpersonating { get; set; }

        /// <summary>
        /// Gets or sets Repository Id
        /// </summary>
        public int RepositoryId { get; set; }
    }
}