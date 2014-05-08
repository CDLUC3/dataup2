// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    /// <summary>
    /// class is used to pass the file data from client application to API Service
    /// </summary>
    public class DataFile
    {
        /// <summary>
        /// filename filed
        /// </summary>
        private string fileName;

        /// <summary>
        /// file content filed
        /// </summary>
        private byte[] fileContent;

        /// <summary>
        /// file extension filed
        /// </summary>
        private string fileExtentsion;

        /// <summary>
        /// content type
        /// </summary>
        private string contentType;

        /// <summary>
        /// created by 
        /// </summary>
        private int createdBy;

        /// <summary>
        /// isCompressed field
        /// </summary>
        private bool isCompressed;

        /// <summary>
        /// File information variable.
        /// </summary>
        private File fileInfo;

        /// <summary>
        /// Gets or sets file information.
        /// </summary>
        public File FileInfo
        {
            get { return this.fileInfo; }
            set { this.fileInfo = value; }
        }

        /// <summary>
        /// Gets or sets  File Name
        /// </summary>
        public string FileName
        {
            get { return this.fileName; }
            set { this.fileName = value; }
        }

        /// <summary>
        /// Gets or sets File Content
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "file byte array is required")]
        public byte[] FileContent
        {
            get { return this.fileContent; }
            set { this.fileContent = value; }
        }

        /// <summary>
        /// Gets or sets File Extension
        /// </summary>
        public string FileExtentsion
        {
            get { return this.fileExtentsion; }
            set { this.fileExtentsion = value; }
        }

        /// <summary>
        /// Gets or sets CreatedBy
        /// </summary>
        public int CreatedBy
        {
            get { return this.createdBy; }
            set { this.createdBy = value; }
        }

        /// <summary>
        /// Gets or sets content type
        /// </summary>
        public string ContentType
        {
            get { return this.contentType; }
            set { this.contentType = value; }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the value of Is Compressed
        /// </summary>
        public bool IsCompressed
        {
            get { return this.isCompressed; }
            set { this.isCompressed = value; }
        }
    }
}
