// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces;
using Microsoft.Research.DataOnboarding.Services.UserService;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.WebApi.Helpers;
using Microsoft.Research.DataOnboarding.WebApi.Resources;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace Microsoft.Research.DataOnboarding.WebApi.Api
{
    /// <summary>
    /// class to manage the file related API methods.
    /// </summary>
    public class AuthTokenController : ApiController
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
        /// Interface IRepositoryService variable.
        /// </summary>
        protected IRepositoryService repositoryService;

        /// <summary>
        /// Holds the reference to IUserService
        /// </summary>
        protected IUserService userService;

        /// <summary>
        /// Holds the Reference to user object
        /// </summary>
        protected User user;

        /// <summary>
        /// contains the referene to DiagnosticsProvider
        /// </summary>
        private readonly DiagnosticsProvider diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthTokenController" /> class.
        /// </summary>
        /// <param name="repositoryService">IRepositoryService</param>
        /// <param name="userService">IUserService</param>
        /// <param name="repositoryAdapterFactory">IRepositoryAdapterFactory</param>
        public AuthTokenController(IRepositoryService repositoryService, IUserService userService)
        {
            this.repositoryService = repositoryService;
            this.userService = userService;
            this.diagnostics = new DiagnosticsProvider(this.GetType());
            this.user = IdentityHelper.GetCurrentUser(this.userService, this.User as ClaimsPrincipal);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        public HttpResponseMessage AddUpdateAuthToken(AuthToken token)
        {
            try
            {
                // Create file service instance
                Repository repository = this.repositoryService.GetRepositoryById(token.RespositoryId);
                if (null == repository)
                {
                    diagnostics.WriteErrorTrace(TraceEventId.Exception, "Repository is null");
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.Invalid_Repository_id);
                }
                token.UserId = this.user.UserId;
                this.userService.AddUpdateAuthToken(token);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (ArgumentException ex)
            {
                message = string.Format(CultureInfo.CurrentCulture, MessageStrings.Argument_Error_Message_Template, ex.ParamName);
                status = HttpStatusCode.BadRequest;
            }

            return Request.CreateErrorResponse(status, message);
        }
    }
}
