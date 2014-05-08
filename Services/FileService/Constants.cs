// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace Microsoft.Research.DataOnboarding.FileService
{
    public class Constants
    {
        /// <summary>
        /// flag to check to upload data to azure or not
        /// </summary>
        public const bool UploadToAzure = true;

        /// <summary>
        /// Path separator for azure folders.
        /// </summary>
        public const string PathSeparator = "/";

        /// <summary>
        /// Configuration setting name for getting the azure connection string.
        /// </summary>
        public const string StorageSettingName = "DataOnBoardingStorage";

        /// <summary>
        /// Default Mime Type for blobs.
        /// </summary>
        public const string DefaultMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        /// <summary>
        /// Zip files mime type.
        /// </summary>
        public const string ZipMimeType = "  application/zip";

        /// <summary>
        /// Excel file extension
        /// </summary>
        public const string XLFileExtension = ".xlsx";

        /// <summary>
        /// CSV file extension
        /// </summary>
        public const string CSVFileExtension = ".csv";

        /// <summary>
        /// NC file extension
        /// </summary>
        public const string NCFileExtension = ".nc";

        /// <summary>
        /// zip file extension
        /// </summary>
        public const string ZipFileExtension = ".zip";

        /// <summary>
        /// citation constant
        /// </summary>
        public const string CitationContent = "Citation";

        /// <summary>
        /// Identifier Field Name
        /// </summary>
        public const string IdentifierKeyName = "Identifier";

        /// <summary>
        /// Metadata sheet name
        /// </summary>
        public const string MetadataSheetNamePrefix = "Meta_";

        /// <summary>
        /// Metadata Range name
        /// </summary>
        public const string MetadataRangeName = "MetadataRange";

        /// <summary>
        /// Parameter Metadata Range name
        /// </summary>
        public const string ParaMetadataRangeName = "ParaMetadataRange";

        /// <summary>
        /// Metadata Table Style Name
        /// </summary>
        public const string MetadataTableStyleName = "TableStyleMedium9";

        public const string DefaultColumnTypeText = "Text";

        /// <summary>
        /// Constant for merrit repository type.
        /// </summary>
        public const int MerritRepositoryTypeID = 1;

        /// <summary>
        /// Gets Container name for files in azure.
        /// </summary>
        public static string ContainerName
        {
            get
            {
                return ConfigReader<string>.GetSetting("PrimaryContainer");
            }
        }

        /// <summary>
        /// Gets Retry policy for azure connections.
        /// </summary>
        public static IRetryPolicy DefaultRetryPolicy
        {
            get
            {
                return new LinearRetry(TimeSpan.FromSeconds(1), 3);
            }
        }

        public static string SpecialCharKeySequence
        {
            get
            {
                return ConfigReader<string>.GetSetting("SpecialCharKeySequence");
            }
        }
    }
}
