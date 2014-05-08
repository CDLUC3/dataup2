// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.RepositoryAdapters.SkyDrive;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.Http;

namespace Microsoft.Research.DataOnboarding.WebApi.Api
{
    /// <summary>
    /// Class contains the methods to manage the Windows Live Authentication
    /// </summary>
    public class WindowsLiveAuthorizationController : ApiController
    {
        /// <summary>
        /// Statis message.
        /// </summary>
        string message = string.Empty;

        /// <summary>
        /// Status code.
        /// </summary>
        HttpStatusCode status = HttpStatusCode.OK;

        /// <summary>
        /// Reference to Diagnosic provider
        /// </summary>
        private DiagnosticsProvider diagnostics;

        /// <summary>
        /// Holds the oAuth URL.
        /// </summary>
        private string oAuthURL;

        /// <summary>
        /// Holds the oAuthZURL
        /// </summary>
        private string oAuthAuthZUrl;

        /// <summary>
        /// Holds the client Id.
        /// </summary>
        private string clientId;

        /// <summary>
        /// Holds the client Secret.
        /// </summary>
        private string clientSecret;

        /// <summary>
        /// Holds the Redirection Url.
        /// </summary>
        private string redirectionURL;

        /// <summary>
        /// Contains SkyDriveUpdate Scope value.
        /// </summary>
        private string skyDriveUpdateScope;

        /// <summary>
        /// Contains the OfflineAccess Scope value.
        /// </summary>
        private string offlineAccessScope;
      
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsLiveAuthorizationController" /> class.
        /// </summary>
        public WindowsLiveAuthorizationController()
        {
            this.diagnostics = new DiagnosticsProvider(this.GetType());
            
            // Read the config settings;
            string baseRepositoryName = BaseRepositoryEnum.SkyDrive.ToString();
            this.oAuthURL = ConfigReader<string>.GetRepositoryConfigValues(baseRepositoryName, SkyDriveConstants.OAuthUrl);
            this.oAuthAuthZUrl = ConfigReader<string>.GetRepositoryConfigValues(baseRepositoryName, SkyDriveConstants.OAuthAuthZUrl);
            this.clientId = ConfigReader<string>.GetRepositoryConfigValues(baseRepositoryName, SkyDriveConstants.ClientId);
            this.redirectionURL = ConfigReader<string>.GetRepositoryConfigValues(baseRepositoryName, SkyDriveConstants.RedicrectionUrl);
            this.clientSecret = ConfigReader<string>.GetRepositoryConfigValues(baseRepositoryName, SkyDriveConstants.ClientSecret);
            this.skyDriveUpdateScope = ConfigReader<string>.GetRepositoryConfigValues(baseRepositoryName, SkyDriveConstants.SkyDriveUpdateScope);
            this.offlineAccessScope = ConfigReader<string>.GetRepositoryConfigValues(baseRepositoryName, SkyDriveConstants.WindowsLiveOfflinseAccessScope);
        }

        [HttpGet]
        public HttpResponseMessage AuthorizeUser()
        {
            try
            {
                // Check if OAuthCode exists if not then redict to Windows Live
                if (!this.HasOAuthCode())
                {
                    diagnostics.WriteInformationTrace(TraceEventId.Flow, "Redirecting to live");

                    // Retreive the call back param
                    string callBackUrl = this.Request.GetQueryNameValuePairs().Where(p => p.Key == "callBackUrl").FirstOrDefault().Value;
                    string windowsLiveLoginUrl = this.GetWindowsLiveLoginUrl(callBackUrl);

                    // Construct the Response
                    var response = Request.CreateResponse(HttpStatusCode.Moved);
                    response.Headers.Location = new Uri(windowsLiveLoginUrl);

                    return response;
                }
                else
                {
                    diagnostics.WriteInformationTrace(TraceEventId.Flow, "Call back from live");

                    string code = this.Request.GetQueryNameValuePairs().Where(p => p.Key == SkyDriveConstants.OAuth_Code).FirstOrDefault().Value;
                    string callBackUrl = this.Request.GetQueryNameValuePairs().Where(p => p.Key == "callBackUrl").FirstOrDefault().Value;

                    // Get the AccessToken and Refresh Token by calling WindowsLive rest API
                    OAuthToken token = GetWindowsLiveTokens(code, callBackUrl);

                    diagnostics.WriteInformationTrace(TraceEventId.Flow, "Redirecting to client");

                    var response = Request.CreateResponse(HttpStatusCode.Moved);
                    string clientCallBackUrl = AppendAuthTokensToCallBackUrl(token, callBackUrl);
                    response.Headers.Location = new Uri(clientCallBackUrl);
                    return response;
                }
            }
            catch (Exception ex)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ex);
                string error = ex.Message + ex.StackTrace + ex.GetType().ToString();
                if (null != ex.InnerException)
                {
                    error += ex.InnerException.Message + ex.InnerException.StackTrace + ex.InnerException.GetType().ToString();
                }

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, error);
            }
        }

        /// <summary>
        /// Appends the Auth tokens as parameters to the callback Url 
        /// </summary>
        /// <param name="token">OAuthToken</param>
        /// <param name="callBackUrl">callback Url</param>
        /// <returns>String callback Url</returns>
        private string AppendAuthTokensToCallBackUrl(OAuthToken token, string callBackUrl)
        {
            string redirectionUrl = callBackUrl;
            UriBuilder builder = new UriBuilder(new Uri(callBackUrl));
            string accessTokenParam = string.Concat("accessToken=", token.AccessToken);
            string refreshTokenParam = string.Concat("refreshToken=", token.RefreshToken);
            string tokenExpiryDateTime = string.Concat("tokenExpiresOn=", token.TokenExpiresOn);

            if (builder.Query != null && builder.Query.Length > 1)
            {
                builder.Query = string.Join("&", builder.Query.Substring(1), accessTokenParam);
            }
            else
            {
                builder.Query = accessTokenParam;
            }

            builder.Query = string.Join("&", builder.Query.Substring(1), refreshTokenParam);
            builder.Query = string.Join("&", builder.Query.Substring(1), tokenExpiryDateTime);
            return builder.Uri.ToString();
        }

        private OAuthToken GetAccessTokenFromRefreshToken(string accessToken, string refreshToken)
        {
            HttpWebRequest request = WebRequest.Create(this.oAuthURL) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";

            string content = String.Format("client_id={0}&redirect_uri={1}&client_secret={2}&refresh_token={3}&grant_type=refresh_token",
                HttpUtility.UrlEncode(this.clientId),
                HttpUtility.UrlEncode(this.redirectionURL),
                HttpUtility.UrlEncode(this.clientSecret),
                HttpUtility.UrlEncode(refreshToken));

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(content);
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            OAuthToken token = null;
            if (response != null)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(OAuthToken));
                token = serializer.ReadObject(response.GetResponseStream()) as OAuthToken;

                if (token != null)
                {
                    return token;
                }
            }

            return null;
        }

        /// <summary>
        /// Makes Rest call to Retreive the AccessToken and RefreshToken
        /// </summary>
        /// <param name="code">Authorization Code</param>
        /// <returns>OAuthToken</returns>
        private OAuthToken GetWindowsLiveTokens(string code, string redirectUrl)
        {
           
            HttpWebRequest request = WebRequest.Create(this.oAuthURL) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";

            string content = String.Format("client_id={0}&redirect_uri={1}&client_secret={2}&code={3}&grant_type=authorization_code",
                HttpUtility.UrlEncode(this.clientId),
                HttpUtility.UrlEncode(this.ConstructCallBack(redirectUrl)),
                HttpUtility.UrlEncode(this.clientSecret),
                HttpUtility.UrlEncode(code));

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(content);
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            OAuthToken token = null;
            if (response != null)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(OAuthToken));
                token = serializer.ReadObject(response.GetResponseStream()) as OAuthToken;

                // Calculate the Expiry DateTime in UTC
                if (token != null)
                {
                    int expiresInSeconds = Convert.ToInt32(token.ExpiresIn);
                    token.TokenExpiresOn = DateTime.UtcNow.AddSeconds(expiresInSeconds);
                    return token;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if the param "code" exists in the questring params 
        /// </summary>
        /// <returns>Boolean</returns>
        private bool HasOAuthCode()
        {
            if (!HttpContext.Current.Request.QueryString.AllKeys.Contains(SkyDriveConstants.OAuth_Code))
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
        private string GetWindowsLiveLoginUrl(string callBack)
        {
            string redirectionUrl = this.redirectionURL;
            UriBuilder builder = new UriBuilder(new Uri(redirectionUrl));
            string callBackParam = string.Concat("callBackUrl=", HttpUtility.UrlEncode(callBack));

            if (builder.Query != null && builder.Query.Length > 1)
            {
                builder.Query = string.Join("&", builder.Query, callBackParam);
            }
            else
            {
                builder.Query = callBackParam;
            }

            string windowsLiveLoginUrl = String.Format(SkyDriveConstants.WindowsLiveLoginUrlFormat,
                                            this.oAuthAuthZUrl,
                                            this.clientId,
                                            HttpUtility.UrlEncode(string.Format("{0} {1}", this.skyDriveUpdateScope, this.offlineAccessScope)),
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
        private string ConstructCallBack(string callBack)
        {
            string redirectionUrl =this.redirectionURL;
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
