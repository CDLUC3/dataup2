// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Research.DataOnboarding.WebApplication.Models;
using System;
using System.Net;
using Microsoft.Research.DataOnboarding.Utilities;

namespace Microsoft.Research.DataOnboarding.WebApplication.Helpers
{
    /// <summary>
    /// class to manage the session values and common data that are used across the application
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Holds the Url for LandingPage
        /// </summary>
        public static string LandingPage = "/Authenticate/Index";

        /// <summary>
        /// Gets the logged in user Id
        /// </summary>
        public static int UserId
        {
            get
            {
                if ((User)System.Web.HttpContext.Current.Session[Constants.UserData] != null)
                {
                    return ((User)System.Web.HttpContext.Current.Session[Constants.UserData]).UserId;
                }

                return -1;
            }
        }

        /// <summary>
        /// Gets or sets the user data
        /// </summary>
        public static User UserData
        {
            get
            {
                if (System.Web.HttpContext.Current.Session[Constants.UserData] != null)
                {
                    return (User)System.Web.HttpContext.Current.Session[Constants.UserData];
                }
                return null;
            }
            set
            {
                System.Web.HttpContext.Current.Session[Constants.UserData] = value;
            }
        }

        /// <summary>
        /// Gets the base publish file API path from config 
        /// </summary>
        public static string BaseWebApiPublishPath
        {
            get
            {
                if (ConfigurationManager.AppSettings[Constants.PUBLISHAPIURI] != null)
                {
                    return ConfigurationManager.AppSettings[Constants.PUBLISHAPIURI].ToString();
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the base web file API path from config 
        /// </summary>
        public static string BaseWebApiFilePath
        {
            get
            {
                if (ConfigurationManager.AppSettings[Constants.FILEAPIURIPATH] != null)
                {
                    return ConfigurationManager.AppSettings[Constants.FILEAPIURIPATH].ToString();
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the base web file API path from config 
        /// </summary>
        public static string BaseWebApiRepositoryPath
        {
            get
            {
                if (ConfigurationManager.AppSettings[Constants.REPOSITORYAPIURIPATH] != null)
                {
                    return ConfigurationManager.AppSettings[Constants.REPOSITORYAPIURIPATH].ToString();
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the base web file API path from config 
        /// </summary>
        public static string BaseWebApiRepositoryTypesPath
        {
            get
            {
                if (ConfigurationManager.AppSettings[Constants.REPOSITORYTYPESAPIURI] != null)
                {
                    return ConfigurationManager.AppSettings[Constants.REPOSITORYTYPESAPIURI].ToString();
                }
                return string.Empty;
            }
        }


        /// <summary>
        /// Gets the base web user API path from config 
        /// </summary>
        public static string BaseWebApiUserPath
        {
            get
            {
                if (ConfigurationManager.AppSettings[Constants.USERAPIURIPATH] != null)
                {
                    return ConfigurationManager.AppSettings[Constants.USERAPIURIPATH].ToString();
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the base web authentication API path from config 
        /// </summary>
        public static string BaseWebApiAuthenticationPath
        {
            get
            {
                if (ConfigurationManager.AppSettings[Constants.AUTHENTICATIONAPIURIPATH] != null)
                {
                    return ConfigurationManager.AppSettings[Constants.AUTHENTICATIONAPIURIPATH].ToString();
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the base web file API path from config 
        /// </summary>
        public static string BaseWebApiQCPath
        {
            get
            {
                if (ConfigurationManager.AppSettings[Constants.QCAPIURIPATH] != null)
                {
                    return ConfigurationManager.AppSettings[Constants.QCAPIURIPATH].ToString();
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the merritt related methods API path from config 
        /// </summary>
        public static string BaseMerrittApiPath
        {
            get
            {
                if (ConfigurationManager.AppSettings[Constants.MERRITTAPIURI] != null)
                {
                    return ConfigurationManager.AppSettings[Constants.MERRITTAPIURI].ToString();
                }
                return string.Empty;
            }
        }
        
        /// <summary>
        /// Gets or sets te Property to keep token value
        /// </summary>
        public static string TokenValue
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.Cookies[Constants.APPJWTCOOKIETOKEN] != null)
                {
                    return System.Web.HttpContext.Current.Request.Cookies[Constants.APPJWTCOOKIETOKEN].ToString();
                };

                return string.Empty;
            }
            set
            {
                System.Web.HttpContext.Current.Response.Cookies.Add(new HttpCookie(Constants.APPJWTCOOKIETOKEN, value));
            }
        }

        /// <summary>
        /// Gets the base web authentication API path from config 
        /// </summary>
        public static int PostFileTimeOutMinutes
        {
            get
            {
                // By default assigning 30 miniuts as timeout
                int returnValue = 30;
                if (ConfigurationManager.AppSettings[Constants.POSTFILETIMEOUTMINUTES] != null)
                {
                    var val = ConfigurationManager.AppSettings[Constants.POSTFILETIMEOUTMINUTES].ToString();
                    if (!int.TryParse(val, out returnValue))
                    {
                        returnValue = 30;
                    }
                }
                return returnValue;
            }
        }

        /// <summary>
        /// Gets the base web authentication API path from config 
        /// </summary>
        public static string FileTypeDelimiter
        {
            get
            {
                if (ConfigurationManager.AppSettings[Constants.FILETYPEDELIMITER] != null)
                {
                    return ConfigurationManager.AppSettings[Constants.FILETYPEDELIMITER].ToString();
                }
                return ";";
            }
        }

        /// <summary>
        /// Override method for on action executing to set the time zone cooke.
        /// </summary>
        /// <param name="filterContext">ActionExecutingContext object</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Request.Cookies.AllKeys.Contains(Constants.OFFSET_COOKIE))
            {
                Session[Constants.OFFSET_COOKIE] = HttpContext.Request.Cookies[Constants.OFFSET_COOKIE].Value;
            }

            var context = System.Web.HttpContext.Current;

            // TODO need to check for token expiry after fixing the api
            if (IsSessionRequired(filterContext) && (context.Request.Cookies[Constants.APPJWTCOOKIETOKEN] == null || string.IsNullOrEmpty(context.Request.Cookies[Constants.APPJWTCOOKIETOKEN].Value)))
            {
                // Do not redirect in case of AJAX. We do not want to check for token in web app and this is first step towards that.
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new RedirectResult(GetLandingPageUri());
                }
                
                return;
            }
            
            // check if session is supported
            if (context.Session != null)
            {
                // check if a new session id was generated
                if (context.Session.IsNewSession)
                {
                    if (IsSessionRequired(filterContext))
                    {
                        // If it says it is a new session, but an existing cookie exists, then it must
                        // have timed out
                        string sessionCookie = context.Request.Headers["Cookie"];

                        if (null != sessionCookie)
                        {
                            // Do not redirect in case of AJAX. We do not want to check for token in web app and this is first step towards that.
                            if (!filterContext.HttpContext.Request.IsAjaxRequest())
                            {
                                filterContext.Result = new RedirectResult(GetLandingPageUri());
                            }
                        }
                    }
                }
                else
                {
                    if (IsSessionRequired(filterContext) && (context.Request.Cookies[Constants.APPJWTCOOKIETOKEN] == null || string.IsNullOrEmpty(context.Request.Cookies[Constants.APPJWTCOOKIETOKEN].Value)))
                    {
                        filterContext.Result = new RedirectResult(GetLandingPageUri());
                        return;
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }
       
        /// <summary>
        /// Method to check is session required
        /// </summary>
        /// <param name="filterContext">filter context</param>
        /// <returns>returns boolean status </returns>
        private static bool IsSessionRequired(ActionExecutingContext filterContext)
        {
            bool result = true;
            if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Authenticate")
            {
                result = false;
            }
            else if ((filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Home") && (filterContext.ActionDescriptor.ActionName == "index"))
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Gets the BaseWebApiWindowsLiveAuthPath
        /// </summary>
        public static string BaseBlobApiPath
        {
            get
            {
                if (ConfigurationManager.AppSettings[Constants.BLOB_API_PATH] != null)
                {
                    return ConfigurationManager.AppSettings[Constants.BLOB_API_PATH].ToString();
                }
                return string.Empty;
            }
        }


        /// <summary>
        /// Gets the BaseWebApiWindowsLiveAuthPath
        /// </summary>
        public static string BaseWebApiWindowsLiveAuthPath
        {
            get
            {
                if (ConfigurationManager.AppSettings[Constants.WINDOWSLIVE_AUTH_PATH] != null)
                {
                    return ConfigurationManager.AppSettings[Constants.WINDOWSLIVE_AUTH_PATH].ToString();
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the base web signout API path from config 
        /// </summary>
        public static string BaseWebApiSignoutPath
        {
            get
            {
                if (ConfigurationManager.AppSettings[Constants.SIGN_OUT_API_PATH] != null)
                {
                    return ConfigurationManager.AppSettings[Constants.SIGN_OUT_API_PATH].ToString();
                }
                return string.Empty;
            }
        }

        public static string BaseWebApiGetSupportedIdentityProvidersPath
        {
            get
            {
                if (ConfigurationManager.AppSettings[Constants.GET_SUPPORTED_IDENTITY_PROVIDERS_API_PATH] != null)
                {
                    return ConfigurationManager.AppSettings[Constants.GET_SUPPORTED_IDENTITY_PROVIDERS_API_PATH].ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Creates and modifies the .
        /// </summary>
        /// <param name="value">Jwt token Raw value.</param>
        /// <param name="expiresOn">cookie expiry DateTime.</param>
        protected void SetJWTCookie(string value, DateTime expiresOn)
        {
            HttpCookie jwtCookie;
            var context = System.Web.HttpContext.Current;

            if (context.Request.Cookies[Constants.APPJWTCOOKIETOKEN] != null)
            {
                jwtCookie = context.Request.Cookies[Constants.APPJWTCOOKIETOKEN];
            }
            else
            {
                jwtCookie = new HttpCookie(Constants.APPJWTCOOKIETOKEN);
            }

            //int offsetMinutes = Convert.ToInt32(Session[Constants.OFFSET_COOKIE].ToString());
            //jwtCookie.Expires = expiresOn.AddMinutes(-offsetMinutes);

            jwtCookie.Expires = expiresOn;
            jwtCookie.Value = value;
            context.Response.Cookies.Add(jwtCookie);
        }

        /// <summary>
        /// constructs the Url to SignOut callback API which has a call back to landing page
        /// </summary>
        /// <returns>string</returns>
        protected string GetLandingPageUri()
        {
            string ladingpageUri = HttpUtility.UrlEncode(string.Format("{0}://{1}/", Request.Url.Scheme, Request.Url.Authority));
            string signOutUri =  string.Format(ConfigurationManager.AppSettings[ConfigurationConstants.SIGNOUT_CALLBACK_API], ConfigurationManager.AppSettings[ConfigurationConstants.BASE_API]);
            return string.Format("{0}?callback={1}", signOutUri, ladingpageUri);
        }

    }
}
