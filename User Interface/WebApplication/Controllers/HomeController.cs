// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Extensions;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.Research.DataOnboarding.WebApplication.Extensions;
using Microsoft.Research.DataOnboarding.WebApplication.Helpers;
using Microsoft.Research.DataOnboarding.WebApplication.Infrastructure;
using Microsoft.Research.DataOnboarding.WebApplication.Models;
using Microsoft.Research.DataOnboarding.WebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DM = Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.DomainModel.ConceptualModel;

namespace Microsoft.Research.DataOnboarding.WebApplication.Controllers
{
    /// <summary>
    /// controller class contains the action result methods related to upload,file list 
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public class HomeController : BaseController
    {
        // create the object of HttpWeb Request Manager
        private HttpWebRequestManager webRequestManager = null;

        /// <summary>
        /// Default Index methods
        /// </summary>
        /// <param name="IsPostSuccess">to say is the post happend successfully</param>
        /// <returns>returns view</returns>
        public ActionResult Index(bool? IsPostSuccess)
        {
            ViewBag.IsAfterPost = IsPostSuccess == null ? false : IsPostSuccess;
            bool validateUser = false;
            string jwtToken = string.Empty;
            JavaScriptSerializer searializer = new JavaScriptSerializer();
            // first time when the user comes to the application ,check the jwt token exists or not,
            // if the user does not exists redirect the user to regiser to the application
            if (this.Request.QueryString.AllKeys.Contains(Utilities.Constants.JWTCOOKIETOKEN_PARAM))
            {
                jwtToken = HttpUtility.UrlDecode(this.Request.QueryString[Utilities.Constants.JWTCOOKIETOKEN_PARAM]);
                string validTo = HttpUtility.UrlDecode(this.Request.QueryString[Utilities.Constants.JWTCOOKIETOKEN_VALIDTO_PARAM]);
                DateTime expiresOn;
                expiresOn = DateTime.UtcNow.AddSeconds((Convert.ToDouble(validTo)));

                if (expiresOn > DateTime.UtcNow)
                {
                    SetJWTCookie(jwtToken, expiresOn);
                    validateUser = true;
                }

            }// check if the cookie already exists , set the user data and load the home page
            else if (this.Request.Cookies[Microsoft.Research.DataOnboarding.WebApplication.Helpers.Constants.APPJWTCOOKIETOKEN] != null && !string.IsNullOrEmpty(this.Request.Cookies[Microsoft.Research.DataOnboarding.WebApplication.Helpers.Constants.APPJWTCOOKIETOKEN].Value))
            {
                validateUser = true;
            }

            if (validateUser)
            {
                // validate the user and if exists set the user to session
                if (!this.ValidateAndSetUser())
                {
                    return this.RedirectToAction("Register", "Authenticate");
                }

                if (UserData.Roles.Contains(Roles.Administrator.ToString()))
                {
                    RedirectToAction("Index", "Repository");
                }
               
                return View("Index");
            }
            else// redirect the user to login
            {
                return new RedirectResult(GetLandingPageUri());
            }
        }

        public ActionResult Home()
        {
            //var model = this.GetFileViewModel();
            return PartialView("Home");
        }
        
        /// <summary>
        /// Action method to download the file from the API.
        /// </summary>
        /// <param name="fileId">File id.</param>
        /// <param name="mimeType">Mime type.</param>
        /// <param name="fileName">File name.</param>
        /// <returns>File result to download.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2")]
        public FileResult DownloadFile(int fileId, string mimeType, string fileName)
        {
            Check.IsNotEmptyOrWhiteSpace(mimeType, "mimeType");
            Check.IsNotEmptyOrWhiteSpace(fileName, "fileName");

            Stream fileStream = null;

            // create the object of HttpWeb Request Manager
            webRequestManager = new HttpWebRequestManager();

            // set the request details
            webRequestManager.SetRequestDetails
                (new RequestParams()
                        {
                            RequestURL = string.Concat(BaseController.BaseBlobApiPath, "?fileId=" + fileId),
                            TimeOut = (BaseController.PostFileTimeOutMinutes * 60 * 1000)
                        }
                );

            HttpWebResponse response = webRequestManager.HttpWebRequest.GetResponse() as HttpWebResponse;
            // In case of CSV change the mime type as zip since the csv will have the metadata as a sepearte file
            if (Path.GetExtension(fileName) == FileService.Constants.CSVFileExtension)
            {
                mimeType = Helpers.Constants.APPLICATION_XZIP;
                fileName = fileName.Replace(Path.GetExtension(fileName), ".zip");
            }

            fileStream = response.GetResponseStream();
            return File(fileStream, mimeType, fileName);
        }
      
        /// <summary>
        /// Returns the Configuration
        /// </summary>
        /// <returns>AppConfiguration as Json</returns>
        [HttpGet]
        public ActionResult GetConfiguration()
        {
            string baseWebApiUri = ConfigReader<string>.GetConfigSetting(ConfigurationConstants.BASE_API, string.Empty);

            AppConfiguration appConfig = new AppConfiguration()
            {
                BaseWebApiUri = baseWebApiUri,
                BlobApiUri = string.Format(ConfigReader<string>.GetConfigSetting(ConfigurationConstants.BLOB_API, string.Empty), baseWebApiUri),
                FileApiUri = string.Format(ConfigReader<string>.GetConfigSetting(ConfigurationConstants.FILE_API, string.Empty), baseWebApiUri),
                PublishApiUri = string.Format(ConfigReader<string>.GetConfigSetting(ConfigurationConstants.PUBLISH_API, string.Empty), baseWebApiUri),
                QCApiUri = string.Format(ConfigReader<string>.GetConfigSetting(ConfigurationConstants.QC_API, string.Empty), baseWebApiUri),
                RepositoryApiUri = string.Format(ConfigReader<string>.GetConfigSetting(ConfigurationConstants.REPOSITORY_API, string.Empty), baseWebApiUri),
                UserApiUri = string.Format(ConfigReader<string>.GetConfigSetting(ConfigurationConstants.USER_API, string.Empty), baseWebApiUri),
                SignOutApiUri = string.Format(ConfigReader<string>.GetConfigSetting(ConfigurationConstants.SIGN_OUT_API, string.Empty), baseWebApiUri),
                RepositoryTypesApiUri= string.Format(ConfigReader<string>.GetConfigSetting(ConfigurationConstants.REPOSITORY_TYPES_API, string.Empty), baseWebApiUri),
                WindowsLiveAuthUri = string.Format(ConfigReader<string>.GetConfigSetting(ConfigurationConstants.WINDOWS_LIVE_URL, string.Empty), baseWebApiUri),
                SupportedIdentityProvidersAPIUri = string.Format(ConfigReader<string>.GetConfigSetting(ConfigurationConstants.SUPPORTED_IDENTITY_PROVIDERS_API, string.Empty), baseWebApiUri),
                FileTypeDelimeter = ConfigReader<string>.GetConfigSetting(ConfigurationConstants.FILETYPEDELIMITER, string.Empty),
                AuthTokenUri = string.Format(ConfigReader<string>.GetConfigSetting(ConfigurationConstants.AUTH_TOKEN, string.Empty), baseWebApiUri),
                SignOutCallbackUri = string.Format(ConfigReader<string>.GetConfigSetting(ConfigurationConstants.SIGNOUT_CALLBACK_API, string.Empty), baseWebApiUri),
                
            };

            return Json(appConfig, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Post()
        {
            return PartialView("_Post");
        }

        #region private methods

        /// <summary>
        /// Method will validate the user and if the user exists returns true else false
        /// </summary>
        /// <returns>returns true on success else false</returns>
        private bool ValidateAndSetUser()
        {
            // create the object of HttpWeb Request Manager
            webRequestManager = new HttpWebRequestManager();

            // set the request details
            webRequestManager.SetRequestDetails(new RequestParams()
                        {
                            RequestURL = BaseWebApiUserPath,
                            TimeOut = (BaseController.PostFileTimeOutMinutes * 60 * 1000)
                        }
                );

            string responseData = string.Empty;
            try
            {
                responseData = webRequestManager.RetrieveWebResponse();

                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = int.MaxValue;
                // set the data to base class
                UserData = js.Deserialize<User>(responseData);
            }
            catch (WebException we)
            {
                HttpWebResponse response = we.Response as HttpWebResponse;
                if (response.StatusCode.Equals(HttpStatusCode.NotFound))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
