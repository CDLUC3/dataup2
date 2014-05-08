// ----------------------------------------------------------------------- 
// <copyright file="MerritConstants.cs" company="Microsoft"> 
// copyright 2012 
// </copyright> 
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.Utilities
{
    public static class MerritConstants
    {
        /// <summary>
        /// Default Mime Type for blobs.
        /// </summary>
        public const string XLSXMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
       // public const string XLSXMimeType = "application/vnd.ms-excel";

        /// <summary>
        /// Zip files mime type.
        /// </summary>
        public const string ZipMimeType = "application/zip";

        /// <summary>
        /// XLSX constant
        /// </summary>
        public const string XLSX = ".xlsx";

        /// <summary>
        /// XLSX constant
        /// </summary>
        public const string CSV = ".csv";

        /// <summary>
        /// XLS constant
        /// </summary>
        public const string XLS = ".xls";

        /// <summary>
        /// ZIP constant
        /// </summary>
        public const string ZIP = ".zip";

        /// <summary>
        /// TAR constant
        /// </summary>
        public const string TAR = ".tar";

        /// <summary>
        /// zip extensions
        /// </summary>
        public const string GZIP = ".gz";

        /// <summary>
        /// Boundary constant
        /// </summary>
        public const string Boundary = "dhfYTTEWYfbn38d";

        /// <summary>
        /// MerrittRepositoryName constant
        /// </summary>
        public const string MerrittRepositoryName = "merritt";

        /// <summary>
        /// One share repository name.
        /// </summary>
        public const string OneShare = "OneShare";

        /// <summary>
        /// MerrittIdentifierLink constant
        /// </summary>
        public const string MerrittIdentifierLink = "https://merritt-stage.cdlib.org/object/mint";

        /// <summary>
        /// MerrittPostFileLink constant
        /// </summary>
        public const string MerrittPostFileLink = "https://merritt-stage.cdlib.org/object/ingest";

        /// <summary>
        /// MerrittRepositoryType constant
        /// </summary>
        public const string MerrittRepositoryType = "DataONE";

        /// <summary>
        /// CLRF constant
        /// </summary>
        public const string CLRF = "\r\n";

        /// <summary>
        /// The application registry path.
        /// </summary>
        public const string AppRegistryPath = "Software\\Microsoft\\Office\\Excel\\Addins\\Excel.Addin";

        /// <summary>
        /// The application registry key.
        /// </summary>
        public const string AppRegistryKey = "Manifest";

        /// <summary>
        /// Path delimiter
        /// </summary>
        public static readonly string PathDelimter = "\\";

        /// <summary>
        /// Temp Folder
        /// </summary>
        public static readonly string TempFolder = @"DCXL\";

        /// <summary>
        /// Temp File Name
        /// </summary>
        public static readonly string TempFileName = @"DcxlObject.{0}";

        /// <summary>
        /// merritt EML file
        /// </summary>
        public static readonly string MrtEmlFile = "mrt-eml.xml";

        /// <summary>
        /// File name
        /// </summary>
        public static readonly string MrtErcFile = "mrt-erc.txt";

        /// <summary>
        /// Manifest File
        /// </summary>
        public static readonly string MrtManifestFile = "mrt-dataone-manifest.txt";

        /// <summary>
        /// metadata xml node
        /// </summary>
        public static readonly string Mapping = "/metadatalist/metadatagroup/metadata";

        /// <summary>
        /// metadata xml node
        /// </summary>
        public static readonly string EmlNamespace = "eml://ecoinformatics.org/eml2.1.0";

        /// <summary>
        /// metadata xml node Attribute List Node XPath
        /// </summary>
        public static readonly string AttributListNodeXPath = "./dataset/dataTable/attributeList";

        /// <summary>
        /// metadata xml node SheetTableXPath
        /// </summary>
        public static readonly string SheetTableXPath = "./dataset/dataTable";

        /// <summary>
        /// metadata xml node XPathDatasetProject
        /// </summary>
        public static readonly string XPathDatasetProject = "dataset/project";

        /// <summary>
        /// metadata xml node XPathDatasetDataTable
        /// </summary>
        public static readonly string XPathDatasetDataTable = "dataset/dataTable";

        /// <summary>
        /// metadata xml node XPathDatasetContact
        /// </summary>
        public static readonly string XPathDatasetContact = "dataset/contact";

        /// <summary>
        /// metadata xml node XPathDataSetGeo Description
        /// </summary>
        public static readonly string XPathDataSetGeoDesc = "dataset/coverage/geographicCoverage/geographicDescription";

        /// <summary>
        /// metadata xml node XPathDataSetGeoCoordinates
        /// </summary>
        public static readonly string XPathDataSetGeoCoordinates = "dataset/coverage/geographicCoverage/boundingCoordinates";

        /// <summary>
        /// metadata xml node
        /// </summary>
        public static readonly string XPathDataSetSurName = "dataset/creator/individualName/surName";

        /// <summary>
        /// metadata xml node XPathDataSetTemporalCoverage
        /// </summary>
        public static readonly string XPathDataSetTemporalCoverage = "dataset/coverage/temporalCoverage";

        /// <summary>
        /// default text values
        /// </summary>
        public static readonly string MissingRequiredElement = "Missing required element";

        /// <summary>
        /// Default CoordinateValue
        /// </summary>
        public static readonly string DefaultCoordinateValue = "0.0";

        /// <summary>
        /// file Creation types
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Used across multiple projects and this can not made as invisible")]
        public enum FileCreationType
        {
            /// <summary>
            /// ERC file type
            /// </summary>
            ERC,

            /// <summary>
            /// EML File type
            /// </summary>
            EML,

            /// <summary>
            /// ManiFest type
            /// </summary>
            ManiFest
        }

        /// <summary>
        /// Gets the TempDownloadPath
        /// </summary>
        public static string TempDownloadPath
        {
            get
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{1}{2}",
                    System.IO.Path.GetTempPath(),
                    MerritConstants.TempFolder,
                    Guid.NewGuid().ToString());
            }
        }

        /// <summary>
        /// Gets the AdminEmail
        /// </summary>
        public static string AdminEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["AdminEmail"];
            }
        }

        /// <summary>
        /// Gets the New scientific Metadata
        /// </summary>
        public static string NewscientificMetadata
        {
            get
            {
                return ConfigurationManager.AppSettings["NewscientificMetadata"];
            }
        }

        /// <summary>
        /// Gets the DataUpMetadata
        /// </summary>
        public static string DataUpMetadata
        {
            get
            {
                return ConfigurationManager.AppSettings["EMLMetadata"];
            }
        }

        /// <summary>
        /// Gets the StandardUnitsPath
        /// </summary>
        public static string StandardUnitsPath
        {
            get
            {
                return ConfigurationManager.AppSettings["StandardUnitsPath"];
            }
        }

        /// <summary>
        /// Method to get the hosting environment location
        /// </summary>
        /// <returns>returns location</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ignore this exception and return empty value")]
        public static string RetrieveAppInstallationLocation()
        {
            string installationLocation = string.Empty;
            try
            {
                // installationLocation = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
            }
            catch (Exception)
            {
            }

            return installationLocation;
        }
    }
}
