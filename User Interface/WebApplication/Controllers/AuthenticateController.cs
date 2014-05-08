// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.Research.DataOnboarding.WebApplication.Helpers;
using Microsoft.Research.DataOnboarding.WebApplication.Infrastructure;
using System.Configuration;
using Microsoft.Research.DataOnboarding.WebApplication.Resource;

namespace Microsoft.Research.DataOnboarding.WebApplication.Controllers
{
    /// <summary>
    /// class to manage the login and register action result methods
    /// </summary>
    public class AuthenticateController : BaseController
    {
        public ActionResult Index()
        {
            return new ViewResult();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return new ViewResult();
        }

        /// <summary>
        /// Signs out from the web app and redirects to API SignOut
        /// </summary>
        /// <returns>Redirects to API</returns>
        public ActionResult  SignOut()
        {
            // if cookie already expired then redirect to landing page.
            if (this.Request.Cookies[Microsoft.Research.DataOnboarding.WebApplication.Helpers.Constants.APPJWTCOOKIETOKEN] == null)
            {
                return Redirect(base.GetLandingPageUri());
            }

            // Clear the Cookies
            HttpCookie myCookie = new HttpCookie(Constants.APPJWTCOOKIETOKEN);
            myCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(myCookie);
            Session.Clear();

            // Redirect to API Signout with call back as landing page
            return Redirect(string.Format(BaseWebApiSignoutPath, this.GetLandingPageURL()));
            
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
        /// Returns the landing page Url
        /// </summary>
        /// <returns>Landing Page URL</returns>
        private string GetLandingPageURL()
        {
            var url = this.Request.Url;
            UriBuilder builder = new UriBuilder(url.Scheme,url.Host,url.Port);
            return builder.Uri.ToString();

        }
    }
}
