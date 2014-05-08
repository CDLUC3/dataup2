// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.RepositoryAdapters.SkyDrive
{
    /// <summary>
    /// Contains all the constants related to SkyDrive
    /// </summary>
    public class SkyDriveConstants
    {
        /// <summary>
        /// Constant for scope.
        /// </summary>
        public const string Scope = "scope";
       
        /// <summary>
        /// Constant for access_Token.
        /// </summary>
        public const string AccessToken = "access_token";

        /// <summary>
        /// Constant for authentication_token.
        /// </summary>
        public const string AuthenticationToken = "authentication_token";

        /// <summary>
        /// Constant for expires_in.
        /// </summary>
        public const string ExpiresIn = "expires_in";

        /// <summary>
        /// Constant for refresh_token.
        /// </summary>
        public const string RefreshToken = "refresh_token";

        /// <summary>
        /// Constant for grant_type.
        /// </summary>
        public const string GrantType = "grant_type";

        /// <summary>
        /// Constant for error.
        /// </summary>
        public const string Error = "error";

        /// <summary>
        /// Constant for error_description.
        /// </summary>
        public const string ErrorDescription = "error_description";

        /// <summary>
        /// Constant for display.
        /// </summary>
        public const string Display = "display";

        /// <summary>
        /// Constant for code.
        /// </summary>
        public const string OAuth_Code = "code";

        /// <summary>
        /// Constant for A300x.
        /// </summary>
        public const string skyDriveMultiPartRequestBoundary = "A300x";

        /// <summary>
        /// Constant for WindowsLiveLoginUrlFormat.
        /// </summary>
        public const string WindowsLiveLoginUrlFormat = "{0}?client_id={1}&scope={2}&response_type=code&redirect_uri={3}";
       
        /// <summary>
        /// Constant for OAuthUrl.
        /// </summary>
        public const string OAuthUrl = "OAuthUrl";

        /// <summary>
        /// Constant for OAuthAuthZUrl.
        /// </summary>
        public const string OAuthAuthZUrl = "OAuthAuthZUrl";

        /// <summary>
        /// Constant for ClientId.
        /// </summary>
        public const string ClientId = "ClientId";

        /// <summary>
        /// Constant for ClientSecret.
        /// </summary>
        public const string ClientSecret = "ClientSecret";

        /// <summary>
        /// Constant for RedicrectionUrl.
        /// </summary>
        public const string RedicrectionUrl = "RedicrectionUrl";

        /// <summary>
        /// Constant for SkyDriveUpdateScope.
        /// </summary>
        public const string SkyDriveUpdateScope = "SkyDriveUpdateScope";

        /// <summary>
        /// Constant for WindowsLiveOfflinseAccessScope.
        /// </summary>
        public const string WindowsLiveOfflinseAccessScope = "WindowsLiveOfflinseAccessScope";
    }
}
