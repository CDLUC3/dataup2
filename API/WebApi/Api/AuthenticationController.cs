// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
      
namespace Microsoft.Research.DataOnboarding.WebApi.Api
{
    using Microsoft.IdentityModel.Tokens.JWT;
    using Microsoft.Research.DataOnboarding.Utilities;
    using Microsoft.Research.DataOnboarding.Utilities.Enums;
    using Microsoft.Research.DataOnboarding.WebApi.Resources;
    using System;
    using System.Collections.ObjectModel;
    using System.IdentityModel.Services;
    using System.IdentityModel.Tokens;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    /// <summary>
    /// Implements the methods to Authenticate and Singout a particular user
    /// </summary>
    public class AuthenticationController : ApiController
    {
        /// <summary>
        /// Constant for AuthCookie
        /// </summary>
        private const string AuthCookie = "FedAuth";

        /// <summary>
        /// Constant to hold the Teplate for URL to retreive HRDMedata
        /// </summary>
        private const string HrdMetadataUrlTemplate = "{0}v2/metadata/IdentityProviders.js?protocol=wsfederation&realm={1}&reply_to={2}&context=&request_id=&version=1.0&callback=";

        /// <summary>
        /// Constant to hold the AppSetting Name for "Tenant"
        /// </summary>
        private const string TenantAppSettingName = "Tenant";

        /// <summary>
        /// Holds the reference to DiagnosticsProvider
        /// </summary>
        private readonly DiagnosticsProvider diagnostics;

        /// <summary>
        /// Instantiates the AuthenticationController
        /// </summary>
        public AuthenticationController()
            :base()
        {
            diagnostics = new DiagnosticsProvider(this.GetType());
        }

        /// <summary>
        /// The method serves as callback. ACS invokes the method after the Authentication.
        /// </summary>
        /// <param name="callback">URL to redirect to.</param>
        /// <returns>Redirection to the URL specified in callback parameter.</returns>
        [HttpPost]
        public HttpResponseMessage SignInCallBack(string callback)
        {
            diagnostics.WriteInformationTrace(TraceEventId.InboundParameters,
                                     "Client callback uri:{0}", callback);

            ClaimsPrincipal principal = this.User as ClaimsPrincipal;
            BootstrapContext context = principal.Identities.First().BootstrapContext as BootstrapContext;
            JWTSecurityToken jwtToken = context.SecurityToken as JWTSecurityToken;

            UriBuilder builder = new UriBuilder(new Uri(callback));


            TimeSpan span = jwtToken.ValidTo.Subtract(jwtToken.ValidFrom);
            double  seconds = span.TotalSeconds;

            string queryparam = string.Format("{0}={1}&{2}={3}", Constants.JWTCOOKIETOKEN_PARAM, HttpUtility.UrlEncode(jwtToken.RawData), Constants.JWTCOOKIETOKEN_VALIDTO_PARAM, HttpUtility.UrlEncode(seconds.ToString()));
            builder.Query = queryparam;
            
            var response = Request.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = builder.Uri;

            diagnostics.WriteVerboseTrace(TraceEventId.OutboundParameters,
                                        "Redirect Uri post authentication process:{0}",
                                        response.Headers.Location);
            return response;
        }

        /// <summary>
        /// Logs out from Web API
        /// </summary>
        /// <param name="callback">URL to call back</param>
        /// <returns>Redirection to callback</returns>
        [HttpGet]
        public HttpResponseMessage SignOut([FromUri]string callback)
        {
          
            diagnostics.WriteInformationTrace(TraceEventId.InboundParameters,
                                    "Client callback uri:{0}", callback);

            if (string.IsNullOrEmpty(callback))
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, "callback parameter is null or empty");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(MessageStrings.Argument_Error_Message_Template, callback));
            }

            FederatedAuthentication.WSFederationAuthenticationModule.SignOut(true);
            var acsConfig = FederatedAuthentication.FederationConfiguration.WsFederationConfiguration;
            string wtrealm = acsConfig.Realm;

            // construct the call back Url
            UriBuilder wreply = new UriBuilder(this.Request.RequestUri.Scheme, this.Request.RequestUri.Host);
            wreply.Path = "SignOutCallback";
            string queryparam = string.Concat("callback=", callback);
            wreply.Query = queryparam;
            string url = string.Format("{0}?wa=wsignout1.0&wreply={1}&wtrealm={2}", acsConfig.Issuer, HttpUtility.UrlEncode(wreply.ToString()), wtrealm);

            // Construct the Response
            var response = Request.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = new Uri(url);

            diagnostics.WriteVerboseTrace(TraceEventId.OutboundParameters,
                                        "Redirect Uri to Acs logout {0}",
                                        response.Headers.Location);
            return response;
        }

        /// <summary>
        /// Redirection handler for ACS logout
        /// </summary>
        /// <param name="callback">URL to call back</param>
        /// <returns>Redirection Response</returns>
        [HttpGet]
        public HttpResponseMessage SignOutCallback([FromUri]string callback)
        {
            diagnostics.WriteInformationTrace(TraceEventId.InboundParameters,
                                    "Client callback uri:{0}", callback);

            if(string.IsNullOrEmpty(callback))
            {
                    diagnostics.WriteErrorTrace(TraceEventId.Exception,"callback parameter is null or empty");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(MessageStrings.Argument_Error_Message_Template,callback));
            }

            // Construct the Response
            var response = Request.CreateResponse(HttpStatusCode.Moved);
            HttpCookie myCookie = new HttpCookie(AuthCookie);
            myCookie.Expires = DateTime.Now.AddDays(-1d);
            CookieHeaderValue authCookie = new CookieHeaderValue(AuthCookie, string.Empty);
            authCookie.Expires = DateTime.Now.AddDays(-1);
            response.Headers.AddCookies(new Collection<CookieHeaderValue>() { authCookie });

            // create the return Url
            UriBuilder builder = new UriBuilder(new Uri(HttpUtility.UrlDecode(callback)));
            response.Headers.Location = builder.Uri;

            diagnostics.WriteVerboseTrace(TraceEventId.OutboundParameters,
                                        "Redirect Uri post SignOut process:{0}",
                                        response.Headers.Location);
            return response;
        }

        /// <summary>
        /// Returns the List of Identity Providers supported by the API
        /// </summary>
        /// <param name="callback">Indicates a URL that Identity Provider need to redirect after authentication</param>
        /// <returns>JSON string containing Indetity Provider Login URLs and Information</returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetSupportedIdentityProviders([FromUri]string callback)
        {
            var acsConfig = FederatedAuthentication.FederationConfiguration.WsFederationConfiguration;
            UriBuilder callbackUribuilder = new UriBuilder(Request.RequestUri.Scheme, Request.RequestUri.Host);
            callbackUribuilder.Path = "api/SignInCallBack";
            callbackUribuilder.Query = string.Format("callback={0}", HttpUtility.UrlEncode(callback));
            string TenantUrl = Utilities.ConfigReader<string>.GetSetting(TenantAppSettingName);
            string url = string.Format(HrdMetadataUrlTemplate, TenantUrl, HttpUtility.UrlEncode(acsConfig.Realm), HttpUtility.UrlEncode(callbackUribuilder.ToString()));
            string webResponse = string.Empty;

            // Create a New HttpClient object.
            HttpClient client = new HttpClient();
            webResponse = await client.GetStringAsync(url); ;

            return Request.CreateResponse<string>(HttpStatusCode.OK, webResponse);
        }
    }
}
