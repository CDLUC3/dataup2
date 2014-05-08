// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System.Collections.Generic;

namespace Microsoft.Research.DataOnboarding.WebApi.FileHandlers
{
    /// <summary>
    /// Interface for file handler .
    /// </summary>
    public interface IFileHandler
    {
        /// <summary>
        /// Uploads files to the repository.
        /// </summary>
        /// <param name="dataFile">File to be uploaded.</param>
        /// <returns>Uploaded files.</returns>
        IEnumerable<DataDetail> Upload(DataFile dataFile);
    }
}