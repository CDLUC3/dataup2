// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    /// <summary>
    /// class to keep the document sheet information
    /// </summary>
    public class FileSheet
    {
        public FileSheet()
        {
            this.FileErrors = new List<FileError>();
        }

        /// <summary>
        /// Gets or sets the Sheet Id
        /// </summary>
        public string SheetId { get; set; }

        /// <summary>
        /// Gets or sets the Sheet name
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// List of errors in this sheet
        /// </summary>
        public List<FileError> FileErrors { get; set; }
    }
}
