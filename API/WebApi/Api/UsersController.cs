// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Research.DataOnboarding.WebApi.Api
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Web.Http;
    using Microsoft.Research.DataOnboarding.DomainModel;
    using Microsoft.Research.DataOnboarding.Services.UserService;
    using Microsoft.Research.DataOnboarding.Utilities;
    using Microsoft.Research.DataOnboarding.Utilities.Enums;
    using Microsoft.Research.DataOnboarding.WebApi.Models;
    using Microsoft.Research.DataOnboarding.WebApi.Resources;
    using System.Globalization;
    using System.Diagnostics;
    using Microsoft.Research.DataOnboarding.Utilities.Model;

    /// <summary>
    /// This class provides web API implementation for User model
    /// </summary>
    public class UsersController : ApiController
    {
        /// <summary>
        /// User service variable.
        /// </summary>
        private IUserService userService;
        private readonly DiagnosticsProvider diagnostics;

        /// <summary>
        /// Status code.
        /// </summary>
        HttpStatusCode status = HttpStatusCode.OK;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
            diagnostics = new DiagnosticsProvider(this.GetType());
        }

        /// <summary>
        /// Retrieves the user data by querying for the name identifier 
        /// input parameter
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
                            "CA1024:UsePropertiesWhereAppropriate", 
                            Justification = "This is a Web API method")]
        [HttpGet]
        public HttpResponseMessage GetUsersByNameIdentifier()
        {
            string message = string.Empty;
            string nameIdentifier = string.Empty;
            HttpStatusCode status = HttpStatusCode.OK;
            try
            {
                // Check if the user service is valid
                Check.IsNotNull(this.userService, "userService");
                nameIdentifier = Helpers.IdentityHelper.GetNameClaimTypeValue(this.User as ClaimsPrincipal);

                diagnostics.WriteVerboseTrace(TraceEventId.InboundParameters,
                                              "Retrieving user with name identifier:{0}",
                                              nameIdentifier);

                User retrievedUser = this.userService.GetUserWithRolesByNameIdentifier(nameIdentifier);
                return Request.CreateResponse<UserInformation>(status, new UserInformation(retrievedUser));
            }
            catch (ArgumentNullException ane)
            {
                if (ane.ParamName.Equals("userService"))
                {
                    message = MessageStrings.User_Service_Is_Null;
                    status = HttpStatusCode.InternalServerError;
                }
            }
            catch (ArgumentException ae)
            {
                message = string.Format(CultureInfo.CurrentCulture, MessageStrings.Argument_Error_Message_Template, ae.ParamName);
                status = HttpStatusCode.BadRequest;
            }
            catch (UserNotFoundException)
            {
                message = MessageStrings.User_Not_Found;
                status = HttpStatusCode.NotFound;
                diagnostics.WriteErrorTrace(TraceEventId.Exception,
                                        "User with nameidentifier {0} not found",
                                        nameIdentifier);
            }

            return Request.CreateErrorResponse(status, message);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
                        "Microsoft.Design", 
                        "CA1062:Validate arguments of public methods", 
                        MessageId = "0", 
                        Justification = "Using Check helper to validate input")]
        [HttpPost]
        public HttpResponseMessage PostUsers([FromBody] UserInformation user)
        {
            string message = string.Empty;
            HttpStatusCode status = HttpStatusCode.Created;

            // Check if the model binding was successful and is in a valid state
            if (!ModelState.IsValid)
            {
                diagnostics.WriteInformationTrace(TraceEventId.InboundParameters, "Model state invalid");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Empty);
            }
            try
            {
                // Check if the user service is valid
                Check.IsNotNull(this.userService, "userService");
                Check.IsNotNull(user, "user");

                user.NameIdentifier = Helpers.IdentityHelper.GetNameClaimTypeValue(this.User as ClaimsPrincipal);
                User registeredUser = this.userService.RegisterUser(user.ToEntity());

                HttpResponseMessage response = Request.CreateResponse<UserInformation>(status,
                                                        new UserInformation(registeredUser)
                                                        );
                response.Headers.Location = new Uri(
                                                Url.Link(Microsoft.Research.DataOnboarding.WebApi.Helpers.RouteConstants.DefaultApiRouteName,
                                                new { id = registeredUser.UserId }
                                            ));
                return response;
            }
            catch (ArgumentNullException ane)
            {
                if (ane.ParamName.Equals("userService"))
                {
                    message = MessageStrings.User_Service_Is_Null;
                    status = HttpStatusCode.InternalServerError;
                }
                else if (ane.ParamName.Equals("user"))
                {
                    message = MessageStrings.Invalid_User_Data;
                    status = HttpStatusCode.BadRequest;
                }
                else
                {
                    diagnostics.WriteErrorTrace(TraceEventId.Exception,
                                   "Invalid argument: {0}, {1}, {2}",
                                   ane.ParamName, ane.Message, ane.StackTrace);
                    status = HttpStatusCode.InternalServerError;
                }
            }
            catch (ArgumentException ae)
            {
                message = string.Format(CultureInfo.CurrentCulture, MessageStrings.Argument_Error_Message_Template, ae.ParamName);
                status = HttpStatusCode.BadRequest;
            }
            catch (UserAlreadyExistsException)
            {
                message = MessageStrings.User_Already_Exists;
                status = HttpStatusCode.BadRequest;
            }
            catch (UserDataUpdateException)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception,
                                   "UserDataUpdateException");

                message = string.Concat(MessageStrings.User_Data_Update_Error_Message,
                                        MessageStrings.Contact_Support);
                status = HttpStatusCode.InternalServerError;
            }

            return Request.CreateErrorResponse(status, message);
        }

        [Authorize(Roles = "Administrator, User")]
        public HttpResponseMessage GetUserAuthTokenStatus(int repositoryId)
        {
            string message = string.Empty;
            string nameIdentifier = Helpers.IdentityHelper.GetNameClaimTypeValue(this.User as ClaimsPrincipal);

            try
            {
                User retrievedUser = this.userService.GetUserWithRolesByNameIdentifier(nameIdentifier);
                UserAuthTokenStatusModel userAuthTokenStatus = this.userService.GetAuthTokenStatus(retrievedUser.UserId , repositoryId);
                return Request.CreateResponse<UserAuthTokenStatusModel>(status, userAuthTokenStatus);
            }
            catch (Exception exception)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, exception);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, message);
            }
        }
    }
}