// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.Research.DataOnboarding.FileService.Extensions
{
    /// <summary>
    /// class to keep the extension methods related to data detail
    /// </summary>
    public static class DataDetailExtensions
    {
        /// <summary>
        /// Method to set the values from data file object to data detail object
        /// </summary>
        /// <param name="dataDetail">Data Detail Object</param>
        /// <param name="dataFile">Data File Object</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
        public static void SetValuesFrom(this DataDetail dataDetail, DataFile dataFile)
        {
            Check.IsNotNull(dataDetail, "dataDetail");
            Check.IsNotNull(dataFile, "dataFile");

            dataDetail.FileDetail.CreatedBy = dataFile.CreatedBy;
            dataDetail.FileDetail.CreatedOn = DateTime.UtcNow;
            dataDetail.FileDetail.MimeType = dataFile.ContentType;
            dataDetail.FileDetail.Name = dataFile.FileName;
            dataDetail.FileDetail.Size = dataFile.FileContent.Length;
            dataDetail.FileDetail.Title = Path.GetFileNameWithoutExtension(dataFile.FileName);
            dataDetail.FileDetail.Status = FileStatus.Uploaded.ToString();
            dataDetail.FileDetail.CreatedBy = dataFile.CreatedBy;
            dataDetail.FileDetail.isDeleted = false;
        }
    }
}
