// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Research.DataOnboarding.FileService.Models
{
    /// <summary>
    /// Class representing the model for files which are uploaded    
    /// </summary>
    public class DataDetail
    {
        public DataDetail()
        {
            FileDetail = new File();
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public DataDetail(DataFile dataFile)
            : this()
        {
            Check.IsNotNull(dataFile, "dataFile");

            this.DataStream = dataFile.FileContent;
            this.FileDetail.CreatedBy = dataFile.CreatedBy;
            this.FileDetail.CreatedOn = DateTime.UtcNow;
            this.FileDetail.MimeType = dataFile.ContentType;
            this.FileDetail.Name = dataFile.FileName;
            this.FileDetail.Size = dataFile.FileContent.Length;
            this.FileDetail.Title = System.IO.Path.GetFileNameWithoutExtension(dataFile.FileName);
            this.FileDetail.Status = FileStatus.Uploaded.ToString();
            this.FileDetail.CreatedBy = dataFile.CreatedBy;
            this.FileDetail.isDeleted = false;
        }

        /// <summary>
        /// Gets or sets the entity file information
        /// </summary>
        public File FileDetail { get; set; }

        /// <summary>
        /// Gets or sets file data stream.
        /// this is used to pass the data to azure blob service
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public byte[] DataStream { get; set; }

        /// <summary>
        /// Gets or sets file name to download.
        /// </summary>
        public string FileNameToDownLoad { get; set; }

        /// <summary>
        /// Gets or sets Mime type to download.
        /// </summary>
        public string MimeTypeToDownLoad { get; set; }
    }
}
