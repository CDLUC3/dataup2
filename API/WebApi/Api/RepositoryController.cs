// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.DomainModel.ConceptualModel;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.Services.UserService;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Extensions;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.Research.DataOnboarding.WebApi.Helpers;
using Microsoft.Research.DataOnboarding.WebApi.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using DM = Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.WebApi.Api
{
    /// <summary>
    /// class to manage the repository related API methods
    /// </summary>
    public class RepositoryController : ApiController
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
        /// Repository service interface.
        /// </summary>
        protected IRepositoryService repositoryService;
        protected DiagnosticsProvider diagnostics;

        /// <summary>
        /// Holds the Reference to user object
        /// </summary>
        protected User user;

        /// <summary>
        /// Holds the reference to IUserService
        /// </summary>
        protected IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryController" /> class.
        /// </summary>
        /// <param name="repositoryService">Repository service object.</param>
        /// <param name="userService">User service object.</param>
        public RepositoryController(IRepositoryService repositoryService, IUserService userService)
        {
            this.repositoryService = repositoryService;
            this.diagnostics = new DiagnosticsProvider(this.GetType());
            this.userService = userService;
            this.user = IdentityHelper.GetCurrentUser(this.userService, this.User as ClaimsPrincipal);
        }

        /// <summary>
        /// Method to get all available repositories.
        /// </summary>
        /// <returns>Repositories response.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching unknown exception.")]
        [HttpGet]
        public HttpResponseMessage GetRepositories()
        {
            // Check if the model binding was successful and is in a valid state
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Empty);
            }
            try
            {
                // Check if the file service is valid
                Check.IsNotNull(this.repositoryService, "repositoryService");

                bool isAdmin = this.user.UserRoles.Any(ur => ur.Role.Name.Equals(Roles.Administrator.ToString(), StringComparison.OrdinalIgnoreCase));

                IEnumerable<RepositoryDataModel> repositoryList = this.repositoryService.RetrieveRepositories(isAdmin);

                return Request.CreateResponse<IEnumerable<RepositoryDataModel>>(HttpStatusCode.OK, repositoryList);
            }
            catch (ArgumentNullException ane)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ane.Message);
            }
            catch (Exception ex)
            {
                string error = ex.Message + ex.StackTrace + ex.GetType().ToString();
                if (null != ex.InnerException)
                {
                    error += ex.InnerException.Message + ex.InnerException.StackTrace + ex.InnerException.GetType().ToString();
                }
                diagnostics.WriteErrorTrace(TraceEventId.Exception, "Exception:{0}", error);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, error);
            }
        }

        /// <summary>
        /// API method to get all the Repository  information
        /// </summary>
        /// <param name="id">Repository id.</param>
        /// <returns>Response with the Repository details.</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public HttpResponseMessage GetRepository(int id)
        {
            // Check if the model binding was successful and is in a valid state
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Empty);
            }
            try
            {
                // Check if the repository service is valid
                Check.IsNotNull(this.repositoryService, "repositoryService");

                DM.Repository repository = this.repositoryService.GetRepositoryById(id);

                return Request.CreateResponse<DM.Repository>(HttpStatusCode.OK, repository);
            }
            catch (ArgumentNullException ane)
            {
                if (ane.ParamName.Equals("repositoryService"))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, MessageStrings.Repository_Service_Is_Null);
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, MessageStrings.Invalid_Repository_id);
            }
            catch (Exception ex)
            {
                string error = ex.Message + ex.StackTrace + ex.GetType().ToString();
                if (null != ex.InnerException)
                {
                    error += ex.InnerException.Message + ex.InnerException.StackTrace + ex.InnerException.GetType().ToString();
                }
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, error);
            }
        }

 
        /// <summary>
        /// Method to check for duplicate repository names.
        /// </summary>
        /// <param name="repositoryName">Repository name to check.</param>
        /// <returns>Repository id if exists else 0.</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public HttpResponseMessage CheckRepositoryExists(string repositoryName)
        {
            // Check if the model binding was successful and is in a valid state
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Empty);
            }
            try
            {
                // Check if the repository service is valid
                Check.IsNotNull(this.repositoryService, "repositoryService");

                var repositoryId = this.repositoryService.CheckRepositoryExists(repositoryName);

                return Request.CreateResponse<int>(HttpStatusCode.OK, repositoryId);
            }
            catch (ArgumentNullException ane)
            {
                if (ane.ParamName.Equals("repositoryService"))
                {
                    message = MessageStrings.QC_Service_Is_Null;
                    status = HttpStatusCode.InternalServerError;
                }

                message = ane.Message;
                status = HttpStatusCode.BadRequest;
            }
            catch (Exception ex)
            {
                message = ex.Message + ex.StackTrace + ex.GetType().ToString();
                if (null != ex.InnerException)
                {
                    message += ex.InnerException.Message + ex.InnerException.StackTrace + ex.InnerException.GetType().ToString();
                }
                status = HttpStatusCode.InternalServerError;
            }

            return Request.CreateErrorResponse(status, message);
        }

        /// <summary>
        /// Method to add or update the repository data.
        /// </summary>
        /// <returns>Add or update operation result.</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public virtual HttpResponseMessage PostRepository()
        {
            try
            {
                // Check if the file service is valid
                Check.IsNotNull(this.repositoryService, "repositoryService");

                if (HttpContext.Current.Request["repositoryModel"] != null)
                {
                    var qcModelString = HttpContext.Current.Request["repositoryModel"];

                    if (qcModelString != null)
                    {
                        Repository repositoryModel = Helper.DeSerializeObject<Repository>(qcModelString.DecodeFrom64(), "repositoryModel");
                        bool result = this.repositoryService.AddUpdateRepository(repositoryModel);
                        return Request.CreateResponse<bool>(status, result);
                    }
                }
            }
            catch (ArgumentNullException ane)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ane);

                switch (ane.ParamName)
                {
                    case "qcService":
                        message = MessageStrings.QC_Service_Is_Null;
                        status = HttpStatusCode.InternalServerError;
                        break;
                    case "accessToken":
                        message = MessageStrings.Access_Token_Is_Null;
                        status = HttpStatusCode.InternalServerError;
                        break;
                    case "refreshToken":
                        message = MessageStrings.Refresh_Token_Is_Null;
                        status = HttpStatusCode.BadRequest;
                        break;
                    case "tokenExpiresOn":
                        message = MessageStrings.Token_Expires_Is_Null;
                        status = HttpStatusCode.BadRequest;
                        break;
                    default:
                         message = ane.Message;
                         status = HttpStatusCode.BadRequest;
                         break;
                }
            }
            catch (Exception ex)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ex);

                message = ex.Message + ex.StackTrace + ex.GetType().ToString();
                if (null != ex.InnerException)
                {
                    message += ex.InnerException.Message + ex.InnerException.StackTrace + ex.InnerException.GetType().ToString();
                }
                status = HttpStatusCode.InternalServerError;
            }
            return Request.CreateErrorResponse(status, message);
        }

        /// <summary>
        /// Web API method to delete the specified quality check rule.
        /// </summary>
        /// <param name="id">Quality check rule id.</param>
        /// <returns>Http response with the delete result.</returns>
        [HttpDelete]
        [Authorize(Roles = "Administrator")]
        public HttpResponseMessage Delete(int id)
        {
            // Check if the model binding was successful and is in a valid state
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Empty);
            }
            try
            {
                // Check if the qc service is valid
                Check.IsNotNull(this.repositoryService, "repositoryService");

                bool deleteResult = this.repositoryService.DeleteRepository(id);
                return Request.CreateResponse<bool>(HttpStatusCode.OK, deleteResult);
            }
            catch (ArgumentNullException ane)
            {
                if (ane.ParamName.Equals("repositoryService"))
                {
                    message = MessageStrings.QC_Service_Is_Null;
                    status = HttpStatusCode.InternalServerError;
                }
                message = ane.Message;
                status = HttpStatusCode.BadRequest;
            }
            catch (Exception ex)
            {
                message = ex.Message + ex.StackTrace + ex.GetType().ToString();
                if (null != ex.InnerException)
                {
                    message += ex.InnerException.Message + ex.InnerException.StackTrace + ex.InnerException.GetType().ToString();
                }
                status = HttpStatusCode.InternalServerError;
            }

            return Request.CreateErrorResponse(status, message);
        }

        /// <summary>
        /// Web API method to delete the specified repositoryMetData Fields that are marked for deletion in UI
        /// </summary>
        /// <param name="id">repositoryId.</param>
        /// <returns>Http response with the delete result.</returns>
        [HttpDelete]
        [Authorize(Roles = "Administrator")]
        public HttpResponseMessage DeleteRepositoryMetaDataFields(int repositorId, string repositoryMetadataFields)
        {
            // Check if the model binding was successful and is in a valid state
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Empty);
            }
            try
            {
                // Check if the qc service is valid
                Check.IsNotNull(this.repositoryService, "repositoryService");

                bool deleteResult = this.repositoryService.DeleteRepositoryMetaDataFields(repositorId, repositoryMetadataFields);
                return Request.CreateResponse<bool>(HttpStatusCode.OK, deleteResult);
            }
            catch (ArgumentNullException ane)
            {
                if (ane.ParamName.Equals("qcService"))
                {
                    message = MessageStrings.QC_Service_Is_Null;
                    status = HttpStatusCode.InternalServerError;
                }
                message = ane.Message;
                status = HttpStatusCode.BadRequest;
            }
            catch (Exception ex)
            {
                message = ex.Message + ex.StackTrace + ex.GetType().ToString();
                if (null != ex.InnerException)
                {
                    message += ex.InnerException.Message + ex.InnerException.StackTrace + ex.InnerException.GetType().ToString();
                }
                status = HttpStatusCode.InternalServerError;
            }

            return Request.CreateErrorResponse(status, message);
        }
    }
}
