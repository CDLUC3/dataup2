// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.WindowsAzure.Storage;
using System;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace Microsoft.Research.DataOnboarding.Utilities
{
    /// <summary>
    /// class to store constants that are used across the application
    /// </summary>
    public static class Constants
    {
        public const string ERROR = "Error";

        /// <summary>
        /// Range separator for range stsart and range end.
        /// </summary>
        public const string RangeSeparator = "to";

        /// <summary>
        /// String literal for MetaData types string.
        /// </summary>
        public const string METADATATYPES = "METADATATYPES";

        /// <summary>
        /// String literal for quality check column types string.
        /// </summary>
        public const string QCCOLUMNTYPES = "QCCOLUMNTYPES";

        /// <summary>
        /// String literal for quality check file column unit.
        /// </summary>
        public const string FILECOLUMNUNIT = "FILECOLUMNUNIT";

        /// <summary>
        /// String literal for quality check file column type.
        /// </summary>
        public const string FILECOLUMNTYPE = "FILECOLUMNTYPE";

        /// <summary>
        /// application x zip
        /// </summary>
        public const string APPLICATION_X_ZIP = "application/x-zip-compressed";

        /// <summary>
        /// String literal for identifying a backslash.
        /// </summary>
        public const string Quote = "\"";

        /// <summary>
        /// String literal for identifying a escaped backslash.
        /// </summary>
        public const string EscapedQuote = "\"\"";

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
        public const string DefaultMimeType = "DefaultMimeType";

        /// <summary>
        /// XLSX constant
        /// </summary>
        public static readonly string XLSX = ".xlsx";

        /// <summary>
        /// XLSX constant
        /// </summary>
        public static readonly string CSV = ".csv";

        /// <summary>
        /// Date format
        /// </summary>
        public const string Dateformate = "yyyy-MM-dd";

        /// <summary>
        /// used to fetch the data from xml file
        /// </summary>
        public const string DC = @"http://purl.org/dc/elements/1.1/";

        /// <summary>
        /// used to set the data in Terms
        /// </summary>
        public const string DCTerms = @"http://purl.org/dc/terms/";
        /// Gets Container name for files in azure.
        /// </summary>
        public static string ContainerName
        {
            get
            {
                return ConfigReader<string>.GetSetting("PrimaryContainer", "filecontainer");
            }
        }

        /// <summary>
        /// Gets post file time out in minuts.
        /// </summary>
        public static int PostFileTimeOutMinutes
        {
            get
            {
                return ConfigReader<int>.GetSetting("PostFileTimeOutMinutes", 30);
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

        /// <summary>
        /// x-zip compressed
        /// </summary>
        public const string APPLICATION_XZIP = "application/x-zip-compressed";

        /// <summary>
        /// Default
        /// </summary>
        public const string Default = "default";

        /// <summary>
        /// Gets stream read or write chunk size
        /// </summary>
        public static int StreamReadOrWriteChunkSize
        {
            get
            {
                return ConfigReader<int>.GetSetting("StreamReadOrWriteChunkSize", 4096);
            }
        }

        /// <summary>
        /// Costant to hold the setting name for Queue Name.
        /// </summary>
        public const string PublishQueueName = "QueueName";

        /// <summary>
        /// constant to hold the RepositoryCredentialHeaderName.
        /// </summary>
        public const string RepositoryCredentialHeaderName = "RepositoryCredentials";

        /// <summary>
        /// Constant for Excel Mime type.
        /// </summary>
        public const string APPLICATION_EXCEL = "application/vnd.ms-excel";

        /// <summary>
        /// jwt token value
        /// </summary>
        public const string JWTCOOKIETOKEN_PARAM = "jwt";

        public const string JWTCOOKIETOKEN_VALIDTO_PARAM = "validto";

        /// <summary>
        /// Local resource key name configured for temporary file storage
        /// </summary>
        public const string TransientFileStorage_AzureLocalResource = "TransientFileStorage";

        public const string TempPathFolderName1 = "TMP";
        public const string TempPathFolderName2 = "TEMP";
        public const string CitationStringFormat = "{0}({1}): {2}. {3}. {0}. {4}";
    }
}
