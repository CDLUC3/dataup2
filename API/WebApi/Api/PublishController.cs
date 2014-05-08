// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.Exceptions;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.QueueService;
using Microsoft.Research.DataOnboarding.RepositoriesService;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.Services.UserService;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.Research.DataOnboarding.WebApi.Helpers;
using Microsoft.Research.DataOnboarding.WebApi.Resources;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace Microsoft.Research.DataOnboarding.WebApi.Api
{
    /// <summary>
    /// Posts the publish message to queue.
    /// </summary>
    public class PublishController : ApiController
    {
        /// <summary>
        /// Holds the reference to DiagnosticsProvider
        /// </summary>
        private readonly DiagnosticsProvider diagnostics;

        /// <summary>
        /// Holds the reference to publishQueueService
        /// </summary>
        private IPublishQueueService publishQueueService;

        /// <summary>
        /// Holds the reference to IUserService
        /// </summary>
        private IUserService userService;

        /// <summary>
        /// Holds the Reference to user object
        /// </summary>
        private User user;

        /// <summary>
        /// Holds the reference to IRepositoryService
        /// </summary>
        private IRepositoryService repositoryService;

        /// <summary>
        /// Holds the reference to IFileServiceFactory
        /// </summary>
        private IFileServiceFactory fileServiceFactory;

        /// <summary>
        /// Holds the Refrence to IFileService
        /// </summary>
        private IFileService fileService;

        /// <summary>
        /// Instantiates the PublishController
        /// </summary>
        public PublishController(IPublishQueueService publishQueueService, IUserService userService, IRepositoryService repositoryService, IFileServiceFactory fileServiceFactory)
            : base()
        {
            diagnostics = new DiagnosticsProvider(this.GetType());
            this.publishQueueService = publishQueueService;
            this.userService = userService;
            this.repositoryService = repositoryService;
            this.fileServiceFactory = fileServiceFactory;
            this.user = IdentityHelper.GetCurrentUser(this.userService, this.User as ClaimsPrincipal);
        }

        /// <summary>
        /// API method to post publish message to queue.
        /// </summary>
        /// <returns>HttpResponseMessage instance.</returns>
        [HttpPost]
        public HttpResponseMessage Publish(PublishMessage publishMessage)
        {
            string message = string.Empty;
            HttpError error = null;

            try
            {
                if (null == publishMessage)
                {
                    diagnostics.WriteErrorTrace(TraceEventId.Exception, "publishMessage null");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(MessageStrings.Argument_Error_Message_Template, "publishMessage"));
                }

                publishMessage.UserId = this.user.UserId;

                Repository repository = this.repositoryService.GetRepositoryById(publishMessage.RepositoryId);
                if (null == repository)
                {
                    diagnostics.WriteErrorTrace(TraceEventId.Exception, "Repository is null");
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.Invalid_Repository_id);
                }

                // Create file service instance
                this.fileService = this.fileServiceFactory.GetFileService(repository.BaseRepository.Name);

                File file = this.fileService.GetFileByFileId(publishMessage.FileId);
                if (null == file)
                {
                    diagnostics.WriteErrorTrace(TraceEventId.Exception, "File is null");
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.Invalid_File_Id);
                }

                // Perform validations
                this.fileService.ValidateForPublish(publishMessage);

                // Publish the message to queue
                this.publishQueueService.PostFileToQueue(publishMessage);

                file.Status = FileStatus.Inqueue.ToString();
                file.RepositoryId = repository.RepositoryId;
                file.ModifiedOn = DateTime.UtcNow;
                // update the file status and associate repository
                this.fileService.UpdateFile(file);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (ArgumentException argumentException)
            {
                error = new HttpError(string.Format(MessageStrings.Argument_Error_Message_Template, argumentException.ParamName));
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, error);
            }
            catch (MetadataValidationException metadataValidationException)
            {
                if (metadataValidationException.NotFound)
                {
                    error = metadataValidationException.GetHttpError(string.Format(MessageStrings.Required_Field_Metadata_Not_Found_Template, metadataValidationException.FieldName));
                }
                else if (metadataValidationException.MetadataTypeNotFound)
                {
                    error = metadataValidationException.GetHttpError(string.Format(MessageStrings.Metadata_Type_Not_Found_Template, metadataValidationException.FieldName));
                }
                else
                {
                    error = metadataValidationException.GetHttpError(string.Format(MessageStrings.Metadata_Field_Value_Type_Mismatch_Template, metadataValidationException.FieldName));
                }

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, error);
            }
            catch (DataFileNotFoundException fileNotFoundException)
            {
                error = fileNotFoundException.GetHttpError(MessageStrings.FileNotFound);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, error);
            }
            catch (RepositoryNotFoundException repositoryNotFoundException)
            {
                error = repositoryNotFoundException.GetHttpError(MessageStrings.Invalid_Repository_id);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, error);
            }
            catch (AccessTokenNotFoundException accessTokenNotFoundException)
            {
                error = accessTokenNotFoundException.GetHttpError(MessageStrings.Access_Token_Is_Null);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, error);
            }
            catch (FileAlreadyPublishedException fileAlreadyPublishedException)
            {
                error = fileAlreadyPublishedException.GetHttpError(MessageStrings.File_Already_Published);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, error);
            }
        }

        /// <summary>
        /// API method to post publish message to queue.
        /// POST - api/publish?nameIdentifier=ServiceIdentity'sNameIdentifier&fileId=1&repositoryName=SkyDrive1
        /// </summary>
        /// <param name="nameIdentifier">Name Identifier.</param>
        /// <param name="fileId">File Id.</param>
        /// <param name="repositoryName">Repository Name.</param>
        /// <returns>HttpResponseMessage instance.</returns>
        [HttpPost]
        [Authorize(Roles = "Service")]
        public HttpResponseMessage Publish(string nameIdentifier, int fileId, string repositoryName)
        {
            string message = string.Empty;
            HttpError error = null;

            try
            {
                Check.IsNotEmptyOrWhiteSpace(nameIdentifier, "nameIdentifier");
                Check.IsNotEmptyOrWhiteSpace(repositoryName, "repositoryName");

                User impersonatedUser = IdentityHelper.GetUser(this.userService, nameIdentifier);

                Repository repository = this.repositoryService.GetRepositoryByName(repositoryName);
                if (repository == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.Repository_Not_Found);
                }
                else if (repository.IsImpersonating != true)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, MessageStrings.ServiceCannotPostFileToARepositoryThatIsNotImpersonated);
                }

                PublishMessage publishMessage = new PublishMessage()
                {
                    UserId = impersonatedUser.UserId,
                    FileId = fileId,
                    RepositoryId = repository.RepositoryId,
                    AuthToken = new AuthToken()
                };

                // Create file service instance
                this.fileService = this.fileServiceFactory.GetFileService(repository.BaseRepository.Name);

                File file = this.fileService.GetFileByFileId(publishMessage.FileId);
                if (null == file)
                {
                    diagnostics.WriteErrorTrace(TraceEventId.Exception, "File is null");
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.Invalid_File_Id);
                }

                // Perform validations
                this.fileService.ValidateForPublish(publishMessage);

                // Publish the message to queue
                this.publishQueueService.PostFileToQueue(publishMessage);

                file.Status = FileStatus.Inqueue.ToString();
                file.RepositoryId = repository.RepositoryId;
                file.ModifiedOn = DateTime.UtcNow;
                // update the file status and associate repository
                this.fileService.UpdateFile(file);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (ArgumentException argumentException)
            {
                error = new HttpError(string.Format(MessageStrings.Argument_Error_Message_Template, argumentException.ParamName));
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, error);
            }
            catch (MetadataValidationException metadataValidationException)
            {
                if (metadataValidationException.NotFound)
                {
                    error = metadataValidationException.GetHttpError(string.Format(MessageStrings.Required_Field_Metadata_Not_Found_Template, metadataValidationException.FieldName));
                }
                else if (metadataValidationException.MetadataTypeNotFound)
                {
                    error = metadataValidationException.GetHttpError(string.Format(MessageStrings.Metadata_Type_Not_Found_Template, metadataValidationException.FieldName));
                }
                else
                {
                    error = metadataValidationException.GetHttpError(string.Format(MessageStrings.Metadata_Field_Value_Type_Mismatch_Template, metadataValidationException.FieldName));
                }

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, error);
            }
            catch (DataFileNotFoundException fileNotFoundException)
            {
                error = fileNotFoundException.GetHttpError(MessageStrings.FileNotFound);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, error);
            }
            catch (RepositoryNotFoundException repositoryNotFoundException)
            {
                error = repositoryNotFoundException.GetHttpError(MessageStrings.InvalidRepositoryName);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, error);
            }
            catch (AccessTokenNotFoundException accessTokenNotFoundException)
            {
                error = accessTokenNotFoundException.GetHttpError(MessageStrings.Access_Token_Is_Null);
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, error);
            }
            catch (FileAlreadyPublishedException fileAlreadyPublishedException)
            {
                error = fileAlreadyPublishedException.GetHttpError(MessageStrings.File_Already_Published);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, error);
            }
        }
    }
}
