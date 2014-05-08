// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Model;

namespace Microsoft.Research.DataOnboarding.WebApi.FileHandlers
{
    /// <summary>
    /// File handler for incoming and ourgoing file requests.
    /// </summary>
    public abstract class FileHandler
    {
        private IFileService fileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileHandler"/> class.
        /// </summary>
        /// <param name="fileService">IFileService instance.</param>
        public FileHandler(IFileService fileService)
        {
            Check.IsNotNull(fileService, "fileService");

            this.fileService = fileService;
        }

        /// <summary>
        /// Uploads files to the repository.
        /// </summary>
        /// <param name="dataFile">File to be uploaded.</param>
        /// <returns>Uploaded files.</returns>
        public virtual DataDetail Upload(DataFile dataFile)
        {
            var uploadedDataDetail = this.fileService.UploadFile(new DataDetail(dataFile));
            return uploadedDataDetail;
        }
    }
}
