// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Research.DataOnboarding.WebApplication.ViewModels
{
    /// <summary>
    /// View model class for uploaded file details.
    /// </summary>
    public class FileToBeDeletedViewModel
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
        /// Gets or sets Lifeline in hours.
        /// </summary>
        public short? LifelineInHours { get; set; }

        /// <summary>
        /// Gets or sets file type.
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets file mime type.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets file uploaded date time.
        /// </summary>
        public string UploadedDateTime { get; set; }

        /// <summary>
        /// Gets or sets file size.
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Gets or sets the list of available repositories for this file.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Property is used to add and get the values")]
        public IList<RepositoryPopupViewModel> RepositoryList { get; set; }
    }
}