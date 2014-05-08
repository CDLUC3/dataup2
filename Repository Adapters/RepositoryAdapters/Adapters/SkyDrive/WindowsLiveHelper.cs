// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Research.DataOnboarding.RepositoryAdapters.SkyDrive
{
    /// <summary>
    /// Contains the Helper Methods required for WindowsLive Authentication
    /// </summary>
    public class WindowsLiveHelper
    {
        /// <summary>
        /// Checks if the wlCookie exists
        /// </summary>
        /// <returns>Boolean</returns>
        public static bool HasOAuthCookie()
        {
            if (!HttpContext.Current.Request.Cookies.AllKeys.Contains(SkyDriveConfiguration.wlCookie))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the param "code" exists in the questring params 
        /// </summary>
        /// <returns>Boolean</returns>
        public static bool HasOAuthCode()
        {
            if (!HttpContext.Current.Request.QueryString.AllKeys.Contains(SkyDriveConfiguration.OAuth_Code))
            {
                return false;
            }
            
            return true;
        }
      
        /// <summary>
        /// Constructs the URL for WindowsLive Auth
        /// </summary>
        /// <param name="callBack">call back url</param>
        /// <returns>String Url</returns>
        public static string GetWindowsLiveLoginUrl(string callBack)
        {
            string redirectionUrl = SkyDriveConfiguration.RedicrectionUrl;
            UriBuilder builder = new UriBuilder(new Uri(redirectionUrl));
            string callBackParam = string.Concat("callBackUrl=",  HttpUtility.UrlEncode(callBack));
           
            if (builder.Query!= null && builder.Query.Length > 1)
            {
                builder.Query = string.Join("&", builder.Query, callBackParam);
            }
            else
            {
                builder.Query = callBackParam;
            }

            string windowsLiveLoginUrl = String.Format(SkyDriveConfiguration.WindowsLiveLoginUrlFormat,
                                            SkyDriveConfiguration.oauthAuthZUrl,
                                            SkyDriveConfiguration.ClientId,
                                            HttpUtility.UrlEncode(string.Format("{0} {1}",SkyDriveConfiguration.SkyDriveUpdateScope,SkyDriveConfiguration.WindowsLiveOfflinseAccessScope)),
                                             HttpUtility.UrlEncode(builder.Uri.ToString())
                                            );
            return windowsLiveLoginUrl;
        }

        /// <summary>
        /// Constructs the Callback URL to be used while fetching the AccessToken and RefreshToken. The call back URL should be exactly the same as that specifed during
        /// the call to get the Authorization Code
        /// </summary>
        /// <param name="callBack">String Callback Url</param>
        /// <returns>String</returns>
        public static string ConstructCallBack(string callBack)
        {
            string redirectionUrl = SkyDriveConfiguration.RedicrectionUrl;
            UriBuilder builder = new UriBuilder(new Uri(redirectionUrl));
            string callBackParam = string.Concat("callBackUrl=", HttpUtility.UrlEncode(callBack));

            if (builder.Query.Length > 0)
            {
                builder.Query = string.Join("&", builder.Query, callBackParam);
            }
            else
            {
                builder.Query = callBackParam;
            }

           return builder.Uri.ToString();
        }
    }
}