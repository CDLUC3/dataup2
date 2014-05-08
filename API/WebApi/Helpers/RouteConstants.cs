// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.WebApi.Helpers
{
    /// <summary>
    /// Contains the route name constants
    /// </summary>
    public static class RouteConstants
    {
        /// <summary>
        /// default route name
        /// </summary>
        public static readonly string DefaultApiRouteName = "API";

        /// <summary>
        /// route name for the windowsLiveAuthorization
        /// </summary>
        public static readonly string WindowsLiveAuthRouteName = "WindowsLiveAuthorization";

        /// <summary>
        /// API Authentication Route Name
        /// </summary>
        public static readonly string AuthenticationRouteName = "Authentication";

        /// <summary>
        /// API SignOut Route Name
        /// </summary>
        public static readonly string SignOutRouteName = "SignOut";

        /// <summary>
        /// API SignOut callback Route Name
        /// </summary>
        public static readonly string SignOutCallbackRouteName = "SignOutCallback";

        /// <summary>
        /// API Files GetErrors Route Name
        /// </summary>
        public static readonly string FilesGetErrors = "FilesGetErrors";

        /// <summary>
        /// API Files GetErrors Route Name
        /// </summary>
        public static readonly string FilesRemoveErrors = "FilesRemoveErrors";

        /// <summary>
        /// API Files GetErrors Route Name
        /// </summary>
        public static readonly string GetPostFileDetails = "GetPostFileDetails";

        /// <summary>
        /// API Files GetFileLevelMetadata Route Name
        /// </summary>
        public static readonly string GetFileLevelMetadata = "GetFileLevelMetadata";

        /// <summary>
        /// API Files SaveFileLevelMetadata Route Name
        /// </summary>
        public static readonly string SaveFileLevelMetadata = "SaveFileLevelMetadata";

        /// <summary>
        /// API Files GetColumnLevelMetadata Route Name
        /// </summary>
        public static readonly string GetColumnLevelMetadata = "GetColumnLevelMetadata";

        /// <summary>
        /// API Files GetColumnLevelMetadataFromFile Route Name
        /// </summary>
        public static readonly string GetColumnLevelMetadataFromFile = "GetColumnLevelMetadataFromFile";

        /// <summary>
        /// API Files SaveColumnLevelMetadata Route Name
        /// </summary>
        public static readonly string SaveColumnLevelMetadata = "SaveColumnLevelMetadata";

        /// <summary>
        /// API Files GetCitation Route Name
        /// </summary>
        public static readonly string GetCitation = "GetCitation";

        /// <summary>
        /// API Files SaveCitation Route Name
        /// </summary>
        public static readonly string SaveCitation = "SaveCitation";

        /// <summary>
        /// DownloadFile Route Name
        /// </summary>
        public static readonly string DownloadFileRouteName = "DownloadFile";

        /// <summary>
        /// GetSupportedIdentityProviders Route Name
        /// </summary>
        public static readonly string GetSupportedIdentityProviderseRouteName = "GetSupportedIdentityProviders";

        /// <summary>
        /// SignInCallBack Route Name
        /// </summary>
        public static readonly string SignInCallBackRouteName = "SignInCallBack";
    }
}