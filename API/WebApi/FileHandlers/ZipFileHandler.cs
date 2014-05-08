// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.Research.DataOnboarding.WebApi.FileHandlers
{
    /// <summary>
    /// Ffile handler for incoming and ourgoing zip file requests.
    /// </summary>
    public class ZipFileHandler : FileHandler, IFileHandler
    {
        private int userId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFileHandler"/> class.
        /// </summary>
        /// <param name="fileService">IFileService instance.</param>
        /// <param name="userId">Id of the user.</param>
        public ZipFileHandler(IFileService fileService, int userId)
            : base(fileService)
        {
            Check.IsNotNull(fileService, "fileService");

            this.userId = userId;
        }

        /// <summary>
        /// Uploads files to the repository.
        /// </summary>
        /// <param name="dataFile">Zip file to be unzipped and uploaded.</param>
        /// <returns>Uploaded files.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public IEnumerable<DataDetail> Upload(DataFile dataFile)
        {
            Check.IsNotNull(dataFile, "dataFile");
            List<DataDetail> collection = new List<DataDetail>();
            List<DataFile> dataFiles = null;
            using (MemoryStream memoryStream = new MemoryStream(dataFile.FileContent))
            {
                dataFiles = ZipUtilities.GetListOfFilesFromStream(memoryStream, this.userId);
            }

            foreach (var df in dataFiles)
            {
                var uploadedDataFiles = base.Upload(df);
                collection.Add(uploadedDataFiles);
            }
            return collection;
        }
    }
}
