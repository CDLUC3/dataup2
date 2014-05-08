// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.Exceptions;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.Services.UserService;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Extensions;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.Research.DataOnboarding.WebApi.FileHandlers;
using Microsoft.Research.DataOnboarding.WebApi.Helpers;
using Microsoft.Research.DataOnboarding.WebApi.Resources;

namespace Microsoft.Research.DataOnboarding.WebApi.Api
{
    /// <summary>
    /// Class to manage the publish related API methods.
    /// </summary>
    public class BlobController : ApiController
    {
        /// <summary>
        /// interface IFileService 
        /// </summary>
        private IFileService fileService;
        private IUserService userService;
        private User user;
        private IFileHandlerFactory fileHandlerFactory;

        /// <summary>
        /// Statis message.
        /// </summary>
        string message = string.Empty;

        /// <summary>
        /// Status code.
        /// </summary>
        HttpStatusCode status = HttpStatusCode.OK;

        /// <summary>
        /// Holds the reference to DiagnosticsProvider
        /// </summary>
        private readonly DiagnosticsProvider diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobController" /> class.
        /// </summary>
        /// <param name="fileService">IFileService instance</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public BlobController(IFileServiceFactory fileServiceFactory, IUserService userService, IFileHandlerFactory fileHandlerFactory)
        {
            Check.IsNotNull(fileServiceFactory, "fileServiceFactory");
            Check.IsNotNull(userService, "userService");
            Check.IsNotNull(fileHandlerFactory, "fileHandlerFactory");
            diagnostics = new DiagnosticsProvider(this.GetType());

            this.userService = userService;
            this.fileService = fileServiceFactory.GetFileService(Constants.Default);

            // get the current/signed in user
            this.user = IdentityHelper.GetCurrentUser(this.userService, this.User as ClaimsPrincipal);
            this.fileHandlerFactory = fileHandlerFactory;

        }

        /// <summary>
        /// Post method to upload the data to blob and save the data to data base
        /// </summary>
        /// <param name="fileName">File Name</param>
        /// <param name="fileExtension">File Extension</param>
        /// <param name="contentType">Content Type</param>
        /// <returns>returns HttpResponseMessage</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        public async Task<HttpResponseMessage> Post(string fileName, string fileExtension, string contentType)
        {
            try
            {
                Check.IsNotEmptyOrWhiteSpace(fileName, "fileName");
                Check.IsNotEmptyOrWhiteSpace(fileExtension, "fileExtension");
                Check.IsNotEmptyOrWhiteSpace(contentType, "contentType");

                byte[] fileContent = default(byte[]);
                if (Request.Content.IsMimeMultipartContent("form-data"))
                {
                    string root = Path.GetTempPath();
                    MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider(root);
                    await Request.Content.ReadAsMultipartAsync(streamProvider);
                    MultipartFileData multipartFileData = streamProvider.FileData.First();

                    /*********************************************************
                     * This API under stress can cause IO exception, for File read
                     * or Delete operation. More details @https://aspnetwebstack.codeplex.com/workitem/176 
                     * Following logic retries file operations at most 3 times
                     * inducing the recommended delay in case of failure. 
                     */ 
                    int executionDelay = 50, 
                        readRetryCount = 3,
                        deleteRetryCount = 3;

                    // Retry mechanics for file read operation
                    while (readRetryCount-- > 0) 
                    {
                        try
                        {
                            using (FileStream fileSource = new FileStream(multipartFileData.LocalFileName, FileMode.Open, FileAccess.Read))
                            {
                                fileContent = await fileSource.GetBytesAsync();
                            }
                            break;
                        }
                        catch (IOException ioe)
                        {
                            diagnostics.WriteErrorTrace(TraceEventId.Exception, ioe.ToString());
                        }
                        await Task.Delay(executionDelay);
                    }

                    // Retry mechanics for file delete operation
                    while (deleteRetryCount-- > 0)
                    {
                        try
                        {
                            await Task.Factory.StartNew(() => System.IO.File.Delete(multipartFileData.LocalFileName));
                            break;
                        }
                        catch (IOException ioe)
                        {
                            diagnostics.WriteErrorTrace(TraceEventId.Exception, ioe.ToString());
                        }
                        await Task.Delay(executionDelay);
                    }

                }
                else
                {
                    var bufferlessInputStream = HttpContext.Current.Request.GetBufferlessInputStream();
                    Check.IsNotNull(bufferlessInputStream, "HttpContext.Current.Request.GetBufferlessInputStream()");
                    fileContent = await bufferlessInputStream.GetBytesAsync();
                }

                this.fileService.ValidateForUpload(fileName, this.user.UserId);

                DataFile dataFile = new DataFile
                {
                    FileContent = fileContent,
                    ContentType = contentType,
                    FileName = fileName,
                    CreatedBy = this.user.UserId,
                    FileExtentsion = fileExtension
                };

                IFileHandler fileHandler = this.fileHandlerFactory.GetFileHandler(contentType, this.user.UserId);
                diagnostics.WriteInformationTrace(Utilities.Enums.TraceEventId.Flow, "blob-before file upload");
                var uploadedDataDetails = fileHandler.Upload(dataFile);
                diagnostics.WriteInformationTrace(Utilities.Enums.TraceEventId.Flow, "blob-after file upload");
                if (!uploadedDataDetails.Any() || uploadedDataDetails.Any(dd => dd.FileDetail.FileId == 0))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, MessageStrings.UploadFailed);
                }

                return Request.CreateResponse<IEnumerable<DataDetail>>(HttpStatusCode.OK, uploadedDataDetails);
            }
            catch (ValidationException validationException)
            {
                HttpError error = validationException.GetHttpError(string.Format(MessageStrings.Upload_Validation_Error_Template, validationException.FileName));
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, error);
            }
            catch (ArgumentException ex)
            {
                message = string.Format(CultureInfo.CurrentCulture, MessageStrings.Argument_Error_Message_Template, ex.ParamName);
                status = HttpStatusCode.BadRequest;
            }

            return Request.CreateErrorResponse(status, message);
        }

        /// <summary>
        /// Post method to upload the data to blob and save the data to data base
        /// POST - api/blob?nameIdentifier=ServiceIdentity'sNameIdentifier
        /// </summary>
        /// <param name="nameIdentifier">Name Identifier</param>
        /// <returns>returns HttpResponseMessage</returns>
        [HttpPost]
        [Authorize(Roles = "Service")]
        public async Task<HttpResponseMessage> Post(string nameIdentifier)
        {
            try
            {
                Check.IsNotEmptyOrWhiteSpace(nameIdentifier, "nameIdentifier");
                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                string root = Path.GetTempPath();
                MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider(root);
                await Request.Content.ReadAsMultipartAsync(streamProvider);
                MultipartFileData multipartFileData = streamProvider.FileData.First();
                byte[] fileContent;
                using (FileStream fileSource = new FileStream(multipartFileData.LocalFileName, FileMode.Open, FileAccess.Read))
                {
                    fileContent = await fileSource.GetBytesAsync();
                }

                string fileName = multipartFileData.Headers.ContentDisposition.FileName.Trim(new char[] { '\"' });
                string fileExtension = Path.GetExtension(fileName);
                string contentType = MimeMapping.GetMimeMapping(fileName);
                Task.Factory.StartNew(() => System.IO.File.Delete(multipartFileData.LocalFileName));
                User impersonatedUser = IdentityHelper.GetUser(this.userService, nameIdentifier);

                this.fileService.ValidateForUpload(fileName, impersonatedUser.UserId);

                DataFile dataFile = new DataFile
                {
                    FileContent = fileContent,
                    ContentType = contentType,
                    FileName = fileName,
                    CreatedBy = impersonatedUser.UserId,
                    FileExtentsion = fileExtension
                };

                IFileHandler fileHandler = this.fileHandlerFactory.GetFileHandler(contentType, impersonatedUser.UserId);
                var uploadedDataDetails = fileHandler.Upload(dataFile);
                if (!uploadedDataDetails.Any() || uploadedDataDetails.Any(dd => dd.FileDetail.FileId == 0))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, MessageStrings.UploadFailed);
                }

                return Request.CreateResponse<IEnumerable<DataDetail>>(HttpStatusCode.OK, uploadedDataDetails);
            }
            catch (ArgumentException ex)
            {
                message = string.Format(CultureInfo.CurrentCulture, MessageStrings.Argument_Error_Message_Template, ex.ParamName);
                status = HttpStatusCode.BadRequest;
            }

            return Request.CreateErrorResponse(status, message);
        }

        /// <summary>
        /// API method to to download the specified file.
        /// </summary>
        /// <param name="fileId">File id.</param>
        /// <returns>Response with the file stream to download.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        [HttpGet]
        [Authorize(Roles = "Administrator, User")]
        public HttpResponseMessage DownLoadFile(int fileId)
        {
            // Check if the model binding was successful and is in a valid state
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Empty);
            }
            try
            {
                // Check if the file service is valid
                Check.IsNotNull(this.fileService, "fileService");

                DataDetail dataDetail = this.fileService.DownloadFile(this.user.UserId, fileId);
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                MemoryStream memoryStream = new MemoryStream(dataDetail.DataStream);
                result.Content = new StreamContent(memoryStream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(Constants.APPLICATION_X_ZIP);
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = dataDetail.FileNameToDownLoad;
                return result;
            }
            catch (ArgumentNullException ane)
            {
                if (ane.ParamName.Equals("fileService"))
                {
                    message = MessageStrings.File_Service_Is_Null;
                    status = HttpStatusCode.InternalServerError;
                }
                message = MessageStrings.Invalid_File_Id;
                status = HttpStatusCode.BadRequest;
            }
            catch (FileFormatException ex)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ex);
                message = ex.Message;
                status = HttpStatusCode.InternalServerError;
            }

            return Request.CreateErrorResponse(status, message);
        }

        /// <summary>
        /// API method to download the specified file.
        /// GET - api/blob?nameIdentifier=ServiceIdentity'sNameIdentifier&fileId=1
        /// </summary>
        /// <param name="nameIdentifier">Name Identifier</param>
        /// <param name="fileId">File id.</param>
        /// <returns>Response with the file stream to download.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        [HttpGet]
        [Authorize(Roles = "Service")]
        public HttpResponseMessage DownLoadFile(string nameIdentifier, int fileId)
        {
            try
            {
                Check.IsNotEmptyOrWhiteSpace(nameIdentifier, "nameIdentifier");

                // Check if the model binding was successful and is in a valid state
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Empty);
                }

                User impersonatedUser = IdentityHelper.GetUser(this.userService, nameIdentifier);
                DataDetail dataDetail = this.fileService.DownloadFile(impersonatedUser.UserId, fileId);
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                MemoryStream memoryStream = new MemoryStream(dataDetail.DataStream);
                result.Content = new StreamContent(memoryStream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(Constants.APPLICATION_X_ZIP);
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                string fileName = dataDetail.FileNameToDownLoad;
                if (Path.GetExtension(fileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = fileName.Replace(Path.GetExtension(fileName), ".zip");
                }

                result.Content.Headers.ContentDisposition.FileName = fileName;
                return result;
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
