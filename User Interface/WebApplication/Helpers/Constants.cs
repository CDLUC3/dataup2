// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.Utilities;
using System;
using System.Linq;

namespace Microsoft.Research.DataOnboarding.WebApplication.Helpers
{
    /// <summary>
    /// class to keep the constants related to web application
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// User Id
        /// </summary>
        public const string USERID = "UserId";

        /// <summary>
        /// User Data
        /// </summary>
        public const string UserData = "UserData";

        /// <summary>
        /// File API Uri
        /// </summary>
        public const string FILEAPIURIPATH = "FileApiUri";

        /// <summary>
        /// Publish Api Uri
        /// </summary>
        public const string PUBLISHAPIURI = "publishApiUri";

        /// <summary>
        /// User API Uri
        /// </summary>
        public const string USERAPIURIPATH = "UserApiUri";

        /// <summary>
        /// Repository Api Uri
        /// </summary>
        public const string REPOSITORYAPIURIPATH = "RepositoryApiUri";

           /// <summary>
        /// Repository Api Uri
        /// </summary>
        public const string REPOSITORYTYPESAPIURI = "RepositoryTypesApiUri";        

        /// <summary>
        /// Authentication API Uri
        /// </summary>
        public const string AUTHENTICATIONAPIURIPATH = "ApiAuthenticationUriTemplate";


        /// <summary>
        /// Merrit repository related methods API Uri
        /// </summary>
        public const string MERRITTAPIURI = "MerrittApiUri";

        /// <summary>
        /// Quality check API Uri
        /// </summary>
        public const string QCAPIURIPATH = "QCApiUri";

        /// <summary>
        /// Post file timeout minutes
        /// </summary>
        public const string POSTFILETIMEOUTMINUTES = "PostFileTimeOutMinutes";

        /// <summary>
        /// File type delimeter
        /// </summary>
        public const string FILETYPEDELIMITER = "FileTypeDelimiter";

        /// <summary>
        /// jwt token value
        /// </summary>
        public const string APPJWTCOOKIETOKEN = "x-api-jwt";

        public const string OFFSET_COOKIE = "__TimezoneOffset";

        /// <summary>
        /// Constant string for mega bytes unit name.
        /// </summary>
        public const string MEGABYTEUNITNAME = "MB";

        /// <summary>
        /// Constant string for mega bytes unit name.
        /// </summary>
        public const string KILOBYTEUNITNAME = "KB";

        /// <summary>
        /// Constant string for mega bytes unit name.
        /// </summary>
        public const string BYTESUNITNAME = "Bytes";

        /// <summary>
        /// Constant number of bytes for a mega byte.
        /// </summary>
        public const long MEGABYTESSIZE = 1048576;

        /// <summary>
        /// Constant number of bytes for a kilo byte.
        /// </summary>
        public const long KILOBYTESSIZE = 1024;

        /// <summary>
        /// multi part form data
        /// </summary>
        public const string MULTIPARTFORMDATA = "multipart/form-data";

        /// <summary>
        /// x-zip compressed
        /// </summary>
        public const string APPLICATION_XZIP = "application/x-zip-compressed";

        /// <summary>
        /// Gets or sets the repository view model
        /// </summary>
        public const string REPOSITORYVIEWMODEL = "RepositoryViewModel";

        /// <summary>
        /// Id
        /// </summary>
        public const string ID = "Id";

        /// <summary>
        /// Name
        /// </summary>
        public const string NAME = "Name";

        /// <summary>
        /// Sheet Name
        /// </summary>
        public const string SHEETNAME = "SheetName";

        /// <summary>
        /// Default Drop down value
        /// </summary>
        public const string DEFAULTDROPDOWNVALUE = "ALL";

        /// <summary>
        /// Constant string for base repository id.
        /// </summary>
        public const string BASEREPOSITORYID = "BaseRepositoryId";

        /// <summary>
        /// Constant string for Name.
        /// </summary>
        public const string NAMESTRING = "Name";

        /// <summary>
        /// WindowsLive Authentication URI
        /// </summary>
        public const string WINDOWSLIVE_AUTH_PATH = "WindowsLiveAuthUri";

        /// <summary>
        /// Blob Api Path
        /// </summary>
        public const string BLOB_API_PATH = "BlobApiUri";

        /// <summary>
        /// Signout Path
        /// </summary>
        public const string SIGN_OUT_API_PATH = "SignOutUri";

        /// <summary>
        /// Merrit repository related methods API Uri
        /// </summary>
        public const string PUBLISH_API_URI = "PublishApiUri";

        /// <summary>
        /// Signout Path
        /// </summary>
        public const string GET_SUPPORTED_IDENTITY_PROVIDERS_API_PATH = "supportedIdentityProvidersAPIUri";

        /// <summary>
        /// Gets the file deletion alert checkpoints.
        /// </summary>
        public static short[] FileDeletionAlertCheckpoints
        {
            get
            {
                return ConfigReader<string>.GetSetting("FileDeletionAlertCheckpoints", "32,24,8").Split(',').Select(cp => Convert.ToInt16(cp)).OrderBy(cp => cp).ToArray();
            }
        }

        /// <summary>
        /// Gets the file deletion alert display threshold.
        /// </summary>
        public static short FileDeletionAlertDisplayThreshold
        {
            get
            {
                return ConfigReader<short>.GetSetting("FileDeletionAlertDisplayThreshold", 24);
            }
        }
    }
}
