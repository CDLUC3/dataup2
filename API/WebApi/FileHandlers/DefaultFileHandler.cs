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

namespace Microsoft.Research.DataOnboarding.WebApi.FileHandlers
{
    /// <summary>
    /// Default file handler for incoming and ourgoing file requests.
    /// </summary>
    public class DefaultFileHandler : FileHandler, IFileHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFileHandler"/> class.
        /// </summary>
        /// <param name="fileService">IFileService instance.</param>
        public DefaultFileHandler(IFileService fileService) : base(fileService)
        {
            Check.IsNotNull(fileService, "fileService");
        }

        /// <summary>
        /// Uploads files to the repository.
        /// </summary>
        /// <param name="dataFile">File to be uploaded.</param>
        /// <returns>Uploaded files.</returns>
        public IEnumerable<DataDetail> Upload(DataFile dataFile)
        {
            Check.IsNotNull(dataFile, "dataFile");
            List<DataDetail> collection = new List<DataDetail>();
            DataDetail dataDetail = base.Upload(dataFile);
            collection.Add(dataDetail);
            return collection;
        }
    }
}
