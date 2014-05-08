// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Research.DataOnboarding.WebApplication.ViewModels
{
    /// <summary>
    /// View model class for list of files.
    /// </summary>
    public class FileListViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileListViewModel" /> class.
        /// </summary>
        public FileListViewModel()
        {
            UploadedFiles = new List<UploadFileViewModel>();
            PostedFiles = new List<PostFileViewModel>();
            FilesToBeDeletedList = new List<FileToBeDeletedViewModel>();
        }

        /// <summary>
        /// Gets or sets list of uploaded files.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification="Used to set and get the values")]
        public IList<UploadFileViewModel> UploadedFiles { get; set; }

        /// <summary>
        /// Gets or sets list of posted files.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Used to set and get the values")]
        public IList<PostFileViewModel> PostedFiles { get; set; }

        /// <summary>
        /// Gets or sets list of files going to get deleted.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Used to set and get the values")]
        public IList<FileToBeDeletedViewModel> FilesToBeDeletedList { get; set; }
    }
}