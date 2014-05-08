// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.DomainModel.ConceptualModel;
using Microsoft.Research.DataOnboarding.FileService.Enums;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.QCService.Interface;
using Microsoft.Research.DataOnboarding.RepositoriesService;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces;
using Microsoft.Research.DataOnboarding.Services.UserService;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Extensions;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.Research.DataOnboarding.WebApi.Helpers;
using Microsoft.Research.DataOnboarding.WebApi.Models;
using Microsoft.Research.DataOnboarding.WebApi.Resources;
using Microsoft.Research.DataOnboarding.WebApi.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using DM = Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.WebApi.Api
{
    /// <summary>
    /// class to manage the file related API methods.
    /// </summary>
    public class FilesController : ApiController
    {

        /// <summary>
        /// interface IFileServiceFactory 
        /// </summary>
        protected IFileServiceFactory fileServiceFactory;

        /// <summary>
        /// interface IFileService 
        /// </summary>
        protected IFileService fileService;

        /// <summary>
        /// interface IQCService 
        /// </summary>
        protected IQCService qcService;

        ///// <summary>
        ///// Interface IRepositoryService variable.
        ///// </summary>
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
        /// Statis message.
        /// </summary>
        string message = string.Empty;

        /// <summary>
        /// contains the RepositoryAdapterFactory
        /// </summary>
        private IRepositoryAdapterFactory repositoryAdapterFactory;

        /// <summary>
        /// Status code.
        /// </summary>
        HttpStatusCode status = HttpStatusCode.OK;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesController" /> class.
        /// </summary>
        /// <param name="fileServiceFactory">IFileServiceFactory</param>
        /// <param name="repositoryService">IRepositoryService</param>
        /// <param name="qcService">IQCService</param>
        /// <param name="userService">IUserService</param>
        /// <param name="repositoryAdapterFactory">IRepositoryAdapterFactory</param>
        public FilesController(IFileServiceFactory fileServiceFactory, IRepositoryService repositoryService, IQCService qcService, IUserService userService, IRepositoryAdapterFactory repositoryAdapterFactory)
        {
            this.repositoryService = repositoryService;
            this.qcService = qcService;
            this.fileServiceFactory = fileServiceFactory;
            this.userService = userService;
            this.fileService = fileServiceFactory.GetFileService(BaseRepositoryEnum.Default.ToString());
            this.diagnostics = new DiagnosticsProvider(this.GetType());
            this.repositoryAdapterFactory = repositoryAdapterFactory;
            this.user = IdentityHelper.GetCurrentUser(this.userService, this.User as ClaimsPrincipal);
        }

        /// <summary>
        /// Method to check file exists for the user id
        /// </summary>
        /// <param name="fileName">file Name</param>
        /// <param name="userId">user Id</param>
        /// <returns>returns http response</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator, User")]
        public HttpResponseMessage GetFiles(string fileName, int userId)
        {
            try
            {
                var fileExists = this.fileService.CheckFileExists(fileName, userId);
                return Request.CreateResponse<bool>(HttpStatusCode.OK, fileExists);
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
        /// API method to get all the uploaded files by the specified user.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <returns>Response with the list of uploaded files.</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator, User")]
        public HttpResponseMessage GetFiles(int id)
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

                IEnumerable<DM.File> filesList = this.fileService.GetAllFiles(this.user.UserId);
                foreach (DM.File file in filesList)
                {
                    if (file.Status.Equals(FileStatus.Posted.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        Citation citation = new Citation();
                        if (!string.IsNullOrWhiteSpace(file.Citation))
                        {
                            citation = JsonConvert.DeserializeObject<Citation>(file.Citation);
                        }

                        file.Citation = string.Format(Constants.CitationStringFormat, citation.Publisher ?? string.Empty, citation.PublicationYear ?? string.Empty, citation.Title ?? string.Empty, citation.Version ?? string.Empty, file.Identifier ?? string.Empty);
                    }
                }

                return Request.CreateResponse<IEnumerable<DM.File>>(status, filesList);
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
        /// API method to get file details by fileid.
        /// </summary>
        /// <param name="fileId">File id.</param>
        /// <param name="repositoryId">Repository Id.</param>
        /// <returns>Response with the of uploaded file.</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator, User")]
        public HttpResponseMessage GetPostFileDetails(int fileId, int repositoryId)
        {
            Func<DM.File, bool> fileByFileIdAndUserIdFilter = f => f.FileId == fileId && f.CreatedBy == this.user.UserId && (f.isDeleted == null || f.isDeleted == false);
            DM.File file = fileService.GetFiles(fileByFileIdAndUserIdFilter).FirstOrDefault();
            if (file == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.FileNotFound);
            }

            bool isAdmin = this.user.UserRoles.Any(ur => ur.Role.Name.Equals(Roles.Administrator.ToString(), StringComparison.OrdinalIgnoreCase));
            string fileExtension = Path.GetExtension(file.Name).Replace(".", string.Empty);
            IEnumerable<DM.Repository> repositoryList = this.repositoryService.GetRepositoriesByRoleAndFileExtension(isAdmin, fileExtension);
            if (repositoryList.FirstOrDefault(r => r.RepositoryId == repositoryId) == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.Repository_Not_Found);
            }

            PostPageViewModel postPageViewModel = new PostPageViewModel();
            postPageViewModel.FileId = fileId;
            postPageViewModel.FileName = file.Name;
            postPageViewModel.RepositoryId = repositoryId;
            foreach (DM.Repository dmRepository in repositoryList)
            {
                Models.Repository repository = new Models.Repository()
                {
                    Id = dmRepository.RepositoryId,
                    Name = dmRepository.Name,
                    BaseRepositoryId = dmRepository.BaseRepositoryId,
                    IsImpersonating = dmRepository.IsImpersonating.HasValue && dmRepository.IsImpersonating.Value == true,
                    UserAgreement = dmRepository.UserAgreement
                };

                postPageViewModel.RepositoryList.Add(repository);
            }

            return Request.CreateResponse<PostPageViewModel>(HttpStatusCode.OK, postPageViewModel);
        }

        /// <summary>
        /// Gets file level metadata for a file by file id and repository id.
        /// api/files/{121}/filelevelmetadata?repositoryId=5
        /// </summary>
        /// <param name="fileId">File Id</param>
        /// <param name="repositoryId">Repository Id</param>
        /// <returns>Http response message</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator, User")]
        public HttpResponseMessage GetFileLevelMetadata(int fileId, int repositoryId)
        {
            List<FileLevelMetadata> fileLevelMetadataList = new List<FileLevelMetadata>();

            DM.File file = fileService.GetFileByFileId(fileId);
            if (file == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.FileNotFound);
            }

            DM.Repository repository = this.repositoryService.GetRepositoryById(repositoryId);
            if (repository == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.Repository_Not_Found);
            }

            if (repository.RepositoryMetadata == null || !repository.RepositoryMetadata.Any())
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.NoMetadataFould);
            }

            RepositoryMetadata repositoryMetadata = repository.RepositoryMetadata.FirstOrDefault(m => m.IsActive == true);
            if (repositoryMetadata == null || repositoryMetadata.RepositoryMetadataFields == null || !repositoryMetadata.RepositoryMetadataFields.Any())
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.NoMetadataFould);
            }

            file.RepositoryId = repositoryId;
            foreach (RepositoryMetadataField repositoryMetadataField in repositoryMetadata.RepositoryMetadataFields)
            {
                FileLevelMetadata fileLevelMetadata = new FileLevelMetadata()
                {
                    RepositoryMetadataId = repositoryMetadataField.RepositoryMetadataId,
                    RepositoryMetadataFieldId = repositoryMetadataField.RepositoryMetadataFieldId,
                    MetaDataTypeId = repositoryMetadataField.MetadataTypeId,
                    FieldName = repositoryMetadataField.Name,
                    Description = repositoryMetadataField.Description,
                    IsRequired = repositoryMetadataField.IsRequired,
                    Datatype = repositoryMetadataField.MetadataTypeId.ToString()
                };

                if (!string.IsNullOrWhiteSpace(repositoryMetadataField.Range))
                {
                    float[] range = repositoryMetadataField.Range.Split(new string[] { Constants.RangeSeparator }, StringSplitOptions.RemoveEmptyEntries).Select(v => Convert.ToSingle(v)).ToArray();
                    fileLevelMetadata.RangeValues = range;
                }

                FileMetadataField fileMetadataField = null;
                // first try to get the values from the database
                if (file.FileMetadataFields != null && file.FileMetadataFields.Any())
                {
                    fileMetadataField = file.FileMetadataFields.Where(p => p.RepositoryMetadataFieldId == repositoryMetadataField.RepositoryMetadataFieldId).FirstOrDefault();
                }

                if (fileMetadataField != null)
                {
                    fileLevelMetadata.FileMetadataFieldId = fileMetadataField.FileMetadataFieldId;
                    fileLevelMetadata.FieldValue = fileMetadataField.MetadataValue;
                }

                fileLevelMetadataList.Add(fileLevelMetadata);
            }

            return Request.CreateResponse(HttpStatusCode.OK, fileLevelMetadataList);
        }

        /// <summary>
        /// Save file level metadata for a file.
        /// api/files/{121}/savefilelevelmetadata?repositoryId=5
        /// </summary>
        /// <param name="saveFileLevelMetadataViewModel">saveFileLevelMetadataViewModel</param>
        /// <returns>Http response message</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        public HttpResponseMessage SaveFileLevelMetadata(int fileId, int repositoryId, IEnumerable<SaveFileLevelMetadata> saveFileLevelMetadataList)
        {
            HttpResponseMessage httpResponseMessage;

            try
            {
                Check.IsNotNull(saveFileLevelMetadataList, "saveFileLevelMetadataList");

                fileService.SaveFileLevelMetadata(fileId, repositoryId, saveFileLevelMetadataList);
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (ArgumentException ex)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ex);
                message = string.Format(CultureInfo.CurrentCulture, MessageStrings.Argument_Error_Message_Template, ex.ParamName);
                httpResponseMessage = Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }

            return httpResponseMessage;
        }

        /// <summary>
        /// Gets file level metadata for a file by file id and repository id.
        /// api/files/{121}/columnlevelmetadata
        /// </summary>
        /// <param name="fileId">File Id</param>
        /// <returns>Http response message</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator, User")]
        public async Task<HttpResponseMessage> GetColumnLevelMetadataFromFile(int fileId)
        {
            DM.File file = fileService.GetFileByFileId(fileId);
            if (file == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.FileNotFound);
            }

            ColumnLevelMetadataViewModel columnLevelMetadataViewModel = new ColumnLevelMetadataViewModel();
            try
            {
                columnLevelMetadataViewModel.SheetList = await this.fileService.GetDocumentSheetDetails(file);
            }
            catch (FileFormatException ex)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            columnLevelMetadataViewModel.TypeList = this.fileService.RetrieveFileColumnTypes();
            columnLevelMetadataViewModel.UnitList = this.fileService.RetrieveFileColumnUnits();

            try
            {
                columnLevelMetadataViewModel.MetadataList = await this.fileService.GetColumnLevelMetadataFromFile(file);
            }
            catch (FileFormatException ex)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, columnLevelMetadataViewModel);
        }

        /// <summary>
        /// Gets file level metadata for a file by file id and repository id.
        /// api/files/{121}/columnlevelmetadata
        /// </summary>
        /// <param name="fileId">File Id</param>
        /// <returns>Http response message</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator, User")]
        public async Task<HttpResponseMessage> GetColumnLevelMetadata(int fileId)
        {
            ////HttpResponseMessage response = await GetColumnLevelMetadataFromFile(fileId);
            ////return response;

            DM.File file = fileService.GetFileByFileId(fileId);
            if (file == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.FileNotFound);
            }

            ////List<ColumnLevelMetadata> columnLevelMetadataList = await response.Content.ReadAsAsync<List<ColumnLevelMetadata>>();

            List<ColumnLevelMetadata> columnLevelMetadataList = new List<ColumnLevelMetadata>();

            if (file.FileColumns != null && file.FileColumns.Any())
            {
                foreach (FileColumn fileColumn in file.FileColumns)
                {
                    ColumnLevelMetadata columnLevelMetadata = new ColumnLevelMetadata()
                    {
                        Id = fileColumn.FileColumnId,
                        SelectedEntityName = fileColumn.EntityName,
                        EntityDescription = fileColumn.EntityDescription,
                        Name = fileColumn.Name,
                        Description = fileColumn.Description,
                        SelectedTypeId = fileColumn.FileColumnTypeId.Value,
                        SelectedUnitId = fileColumn.FileColumnUnitId
                    };

                    columnLevelMetadataList.Add(columnLevelMetadata);
                }
            }

            ColumnLevelMetadataViewModel columnLevelMetadataViewModel = new ColumnLevelMetadataViewModel();
            columnLevelMetadataViewModel.MetadataList = columnLevelMetadataList;
            try
            {
                columnLevelMetadataViewModel.SheetList = await this.fileService.GetDocumentSheetDetails(file);
            }
            catch (FileFormatException ex)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            columnLevelMetadataViewModel.TypeList = this.fileService.RetrieveFileColumnTypes();
            columnLevelMetadataViewModel.UnitList = this.fileService.RetrieveFileColumnUnits();

            return Request.CreateResponse(HttpStatusCode.OK, columnLevelMetadataViewModel);
        }

        /// <summary>
        /// Save column level metadata for a file.
        /// api/files/{121}/savecolumnlevelmetadata
        /// </summary>
        /// <param name="columnLevelMetadataViewModel">columnLevelMetadataViewModel</param>
        /// <returns>Http response message</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        public HttpResponseMessage SaveColumnLevelMetadata(int fileId, IEnumerable<ColumnLevelMetadata> metadataList)
        {
            HttpResponseMessage httpResponseMessage;

            try
            {
                Check.IsNotNull(metadataList, "metadataList");

                fileService.SaveColumnLevelMetadata(fileId, metadataList);
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (ArgumentException ex)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ex);
                message = string.Format(CultureInfo.CurrentCulture, MessageStrings.Argument_Error_Message_Template, ex.ParamName);
                httpResponseMessage = Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }

            return httpResponseMessage;
        }

        /// <summary>
        /// Gets citation for a file by file id.
        /// api/files/{121}/citation
        /// </summary>
        /// <param name="fileId">File Id</param>
        /// <param name="repositoryId">Repository Id</param>
        /// <returns>Http response message</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator, User")]
        public HttpResponseMessage GetCitation(int fileId, int repositoryId)
        {
            DM.File file = fileService.GetFileByFileId(fileId);
            if (file == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.FileNotFound);
            }

            DM.Repository repository = this.repositoryService.GetRepositoryById(repositoryId);
            if (repository == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.Repository_Not_Found);
            }
            else if (repository.BaseRepository.BaseRepositoryId != 1)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, string.Format(MessageStrings.CitationNotSupportedMessage, repository.BaseRepository.Name));
            }

            Citation citation = new Citation()
            {
                PublicationYear = DateTime.Now.Year.ToString(),
                Title = file.Title,
                Publisher = string.Concat(this.user.LastName, ", ", this.user.FirstName)
            };
            if (!string.IsNullOrWhiteSpace(file.Citation))
            {
                citation = JsonConvert.DeserializeObject<Citation>(file.Citation);
            }

            return Request.CreateResponse(HttpStatusCode.OK, citation);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        public HttpResponseMessage SaveCitation(int fileId, Citation citation)
        {
            HttpResponseMessage httpResponseMessage;

            try
            {
                Check.IsNotNull(citation, "citation");

                DM.File file = this.fileService.GetFileByFileId(fileId);
                if (null == file)
                {
                    diagnostics.WriteErrorTrace(TraceEventId.Exception, "File is null");
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.Invalid_File_Id);
                }

                file.Citation = JsonConvert.SerializeObject(citation);
                file.ModifiedOn = DateTime.UtcNow;
                fileService.UpdateFile(file);
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (ArgumentException ex)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ex);
                message = string.Format(CultureInfo.CurrentCulture, MessageStrings.Argument_Error_Message_Template, ex.ParamName);
                httpResponseMessage = Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }

            return httpResponseMessage;
        }

        /// <summary>
        /// API method to delete the specified file details.
        /// </summary>
        /// <param name="fileId">File id.</param>
        /// <param name="userId">User id.</param>
        /// <returns>Delete result response.</returns>      
        [HttpDelete]
        [Authorize(Roles = "Administrator, User")]
        public HttpResponseMessage Delete(int fileId, int userId)
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

                bool deleteResult = this.fileService.DeleteFile(this.user.UserId, fileId);
                return Request.CreateResponse<bool>(HttpStatusCode.OK, deleteResult);
            }
            catch (ArgumentNullException ex)
            {
                if (ex.ParamName.Equals("file", StringComparison.OrdinalIgnoreCase))
                {
                    message = MessageStrings.FileDoesntExist;
                    status = HttpStatusCode.NotFound;
                }
                else
                {
                    message = ex.Message;
                    status = HttpStatusCode.InternalServerError;
                }
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
        /// API method to get all the uploaded files by the specified user.
        /// Pass "FILECOLUMNTYPE" for getting filecolumn types and "FILECOLUMNUNIT" for getting file column units
        /// </summary>
        /// <param name="id">User id.</param>
        /// <returns>Response with the list of uploaded files.</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator, User")]
        public HttpResponseMessage GetLookUpData(string type)
        {
            HttpResponseMessage responseMessage = null;
            // Check if the model binding was successful and is in a valid state
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "type is empty");
            }
            try
            {
                // Check if the file service is valid
                Check.IsNotNull(this.fileService, "fileService");
                Check.IsNotNull(this.qcService, "qcService");

                switch (type.ToUpper())
                {
                    case Constants.FILECOLUMNTYPE:
                        var filesColumnTypes = this.fileService.RetrieveFileColumnTypes();
                        responseMessage = Request.CreateResponse<IEnumerable<DM.FileColumnType>>(status, filesColumnTypes);
                        break;
                    case Constants.FILECOLUMNUNIT:
                        var filesColumnUnits = this.fileService.RetrieveFileColumnUnits();
                        responseMessage = Request.CreateResponse<IEnumerable<DM.FileColumnUnit>>(status, filesColumnUnits);
                        break;
                    case Constants.QCCOLUMNTYPES:
                        var qcColumnTypes = this.qcService.RetrieveQCColumnTypes();
                        responseMessage = Request.CreateResponse<IEnumerable<DM.QualityCheckColumnType>>(HttpStatusCode.OK, qcColumnTypes);
                        break;
                    case Constants.METADATATYPES:
                        var metadataTypes = this.fileService.RetrieveMetaDataTypes();
                        responseMessage = Request.CreateResponse<IEnumerable<DM.MetadataType>>(HttpStatusCode.OK, metadataTypes);
                        break;
                    default:
                        responseMessage = Request.CreateResponse<string>(status, "Invalid Input");
                        break;
                }
            }
            catch (ArgumentNullException ane)
            {
                if (ane.ParamName.Equals("fileService"))
                {
                    message = MessageStrings.File_Service_Is_Null;
                    status = HttpStatusCode.InternalServerError;
                }
                else if (ane.ParamName.Equals("qcService"))
                {
                    message = MessageStrings.QC_Service_Is_Null;
                    status = HttpStatusCode.InternalServerError;
                }

                message = MessageStrings.Invalid_File_Id;
                status = HttpStatusCode.BadRequest;
                responseMessage = Request.CreateErrorResponse(status, message);
            }
            catch (Exception ex)
            {
                message = ex.Message + ex.StackTrace + ex.GetType().ToString();
                if (null != ex.InnerException)
                {
                    message += ex.InnerException.Message + ex.InnerException.StackTrace + ex.InnerException.GetType().ToString();
                }
                status = HttpStatusCode.InternalServerError;
                responseMessage = Request.CreateErrorResponse(status, message);
            }

            return responseMessage;
        }

        /// <summary>
        /// API method to publish the file to specific repository.
        /// </summary>
        /// <returns>Publish result.</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        public HttpResponseMessage Publish()
        {
            string message = string.Empty;
            HttpStatusCode status = HttpStatusCode.OK;
            PostFileModel dataDetail = new PostFileModel();

            try
            {
                diagnostics.WriteInformationTrace(TraceEventId.Flow, "Into publish");

                if (HttpContext.Current.Request["PostFileData"] != null)
                {
                    var postFileString = HttpContext.Current.Request["PostFileData"];

                    if (postFileString != null)
                    {
                        //diagnostics.WriteInformationTrace(TraceEventId.Flow, "Into publishing3");

                        /****************************************************
                         * TODO: Try catch block below is required to handle the case where the 
                         * clients send post request with JSON payload as plain text.
                         * The API needs to be refactored to take the model as input
                         * and have the MVC framework resolve/deserialize the payload 
                         * into model object.
                         * **************************************************/
                        PostFileModel postFileData = default(PostFileModel);
                        try
                        {
                            postFileData = Helper.DeSerializeObject<PostFileModel>(postFileString.DecodeFrom64(), "postFile");
                        }
                        catch (Exception)
                        {
                            // If the data is not base 64 encoded
                            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(postFileString)))
                            {
                                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(PostFileModel));
                                postFileData = (PostFileModel)jsonSerializer.ReadObject(stream);
                            }
                        }

                        DM.Repository repository = repositoryService.GetRepositoryById(postFileData.SelectedRepositoryId);
                        this.fileService = this.fileServiceFactory.GetFileService(repository.BaseRepository.Name);

                        // Get the AuthToken from the request. TODO need to move below code to SkyDriveFileController.
                        AuthToken userAuthToken = postFileData.UserAuthToken;
                        userAuthToken.UserId = this.user.UserId;
                        userAuthToken.RespositoryId = repository.RepositoryId;

                        // Save the file details to db and publish logic
                        this.fileService.SaveFile(postFileData);

                        return Request.CreateResponse<OperationStatus>(HttpStatusCode.OK, OperationStatus.CreateSuccessStatus());
                    }
                }
            }
            catch (ArgumentNullException ane)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ane);
                status = HttpStatusCode.BadRequest;

                if (ane.ParamName.Equals("fileService"))
                {
                    message = MessageStrings.File_Service_Is_Null;
                }
                else
                {
                    message = ane.Message + ane.StackTrace + ane.GetType().ToString();
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

        [HttpGet]
        [Authorize(Roles = "Administrator, User")]
        public HttpResponseMessage DownloadFileFromRepository(int fileId)
        {
            return this.DownloadFileFromRepository(fileId, this.user);
        }

        /// <summary>
        /// Download a file from Merritt repository.
        /// GET - api/files?nameIdentifier=ServiceIdentity'sNameIdentifier&fileId=1
        /// </summary>
        /// <param name="nameIdentifier">Name Identifier</param>
        /// <param name="fileId">File Id</param>
        /// <param name="userName">User Name</param>
        /// <param name="password">Password</param>
        /// <returns>Returns HttpResponseMessage</returns>
        [HttpGet]
        [Authorize(Roles = "Service")]
        public HttpResponseMessage DownloadFileFromRepository(string nameIdentifier, int fileId)
        {
            HttpError error;
            try
            {
                Check.IsNotEmptyOrWhiteSpace(nameIdentifier, "nameIdentifier");
                User user = IdentityHelper.GetUser(this.userService, nameIdentifier);

                if (user == null)
                {
                    error = new HttpError(MessageStrings.User_Not_Found)
                    {
                        {
                            "nameIdentifier",
                            nameIdentifier
                        }
                    };

                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, error);
                }

                return this.DownloadFileFromRepository(fileId, user);
            }
            catch (ArgumentException ex)
            {
                message = string.Format(CultureInfo.CurrentCulture, MessageStrings.Argument_Error_Message_Template, ex.ParamName);
                status = HttpStatusCode.BadRequest;
            }

            return Request.CreateErrorResponse(status, message);
        }

        /// <summary>
        /// Gets errors in a file.
        /// GET - api/files/1/errors
        /// </summary>
        /// <param name="fileId">File Id</param>
        /// <returns>List of errors.</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator, User")]
        public async Task<HttpResponseMessage> GetErrors(int fileId)
        {
            HttpResponseMessage httpResponseMessage;

            try
            {
                var fileAndFileSheets = await fileService.GetErrors(this.user.UserId, fileId);
                BestPractisesViewModel bestPractisesViewModel = new BestPractisesViewModel()
                {
                    FileId = fileId,
                    FileName = fileAndFileSheets.Item1.Name,
                    MimeType = MimeMapping.GetMimeMapping(fileAndFileSheets.Item1.Name),
                    CanDeleteErrors = Path.GetExtension(fileAndFileSheets.Item1.Name).Equals(Constants.XLSX, StringComparison.OrdinalIgnoreCase),
                    FileSheets = (List<FileSheet>)fileAndFileSheets.Item2
                };

                httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, bestPractisesViewModel);
            }
            catch (DataFileNotFoundException ex)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ex);
                httpResponseMessage = Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.FileNotFound);
            }
            catch (InvalidOperationException ex)
            {
                diagnostics.WriteInformationTrace(TraceEventId.Exception, ex.Message);
                httpResponseMessage = Request.CreateErrorResponse(HttpStatusCode.NotImplemented, ex.Message);
            }
            catch (FileFormatException ex)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ex);
                httpResponseMessage = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return httpResponseMessage;
        }

        /// <summary>
        /// Removes selected errors in a file.
        /// DELETE - api/files/1/removeerrors?sheetName=sheet1&errorTypes=Pictures
        /// DELETE - api/files/1/removeerrors?sheetName=sheet1&errorTypes=13
        /// </summary>
        /// <param name="removeErrorsViewModel">RemoveErrorsViewModel</param>
        /// <returns>Http response message</returns>
        [HttpDelete]
        [Authorize(Roles = "Administrator, User")]
        public async Task<HttpResponseMessage> RemoveErrors(RemoveErrorsViewModel removeErrorsViewModel)
        {
            HttpResponseMessage httpResponseMessage;

            try
            {
                Check.IsNotNull(removeErrorsViewModel, "removeErrorsViewModel");
                Check.IsNotEmptyOrWhiteSpace(removeErrorsViewModel.SheetName, "SheetName");
                Check.IsNotNull(removeErrorsViewModel.ErrorTypes, "ErrorTypes");

                foreach (var errorType in removeErrorsViewModel.ErrorTypes)
                {
                    if (!Enum.IsDefined(typeof(ErrorType), errorType))
                    {
                        throw new InvalidEnumArgumentException();
                    }
                }

                await fileService.RemoveErrors(this.user.UserId, removeErrorsViewModel.FileId, removeErrorsViewModel.SheetName, removeErrorsViewModel.ErrorTypes);
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (InvalidEnumArgumentException ex)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ex);
                message = string.Format(CultureInfo.CurrentCulture, MessageStrings.InvalidErrorTypes, removeErrorsViewModel.ErrorTypes);
                httpResponseMessage = Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }
            catch (ArgumentException ex)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ex);
                message = string.Format(CultureInfo.CurrentCulture, MessageStrings.Argument_Error_Message_Template, ex.ParamName);
                httpResponseMessage = Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }
            catch (DataFileNotFoundException ex)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ex);
                httpResponseMessage = Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.FileNotFound);
            }
            catch (InvalidOperationException ex)
            {
                diagnostics.WriteInformationTrace(TraceEventId.Exception, ex.Message);
                httpResponseMessage = Request.CreateErrorResponse(HttpStatusCode.NotImplemented, ex.Message);
            }
            catch (FileFormatException ex)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ex);
                httpResponseMessage = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return httpResponseMessage;
        }

        /// <summary>
        /// Downloads the file from Repository.
        /// </summary>
        /// <param name="fileId">File Id.</param>
        /// <param name="user">User instance.</param>
        /// <returns>File stream.</returns>
        private HttpResponseMessage DownloadFileFromRepository(int fileId, User user)
        {
            HttpError error;
            try
            {
                if (fileId <= 0)
                {
                    error = new HttpError(string.Format(MessageStrings.Argument_Error_Message_Template, "fileId"));
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, error);
                }

                var file = this.fileService.GetFiles(p => p.FileId == fileId && p.CreatedBy == user.UserId).FirstOrDefault();

                if (file == null)
                {
                    error = new HttpError(MessageStrings.FileDoesntExist)
                    {
                        {
                            "FileId",
                            fileId
                        }
                    };

                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, error);
                }

                if (file.RepositoryId == null)
                {
                    error = new HttpError(MessageStrings.File_Repository_Is_Null)
                    {
                        {
                            "FileId",
                            fileId
                        }
                    };

                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, error);
                }

                DM.Repository repository = this.repositoryService.GetRepositoryById((int)file.RepositoryId);

                if (repository == null)
                {
                    error = new HttpError(MessageStrings.Repository_Not_Found)
                    {
                        {
                            "RepositoryId",
                            (int)file.RepositoryId
                        }
                    };

                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, error);
                }

                RepositoryCredentials repositoryCredentials = GetRepsitoryCredentials();

                this.fileService = this.fileServiceFactory.GetFileService(repository.BaseRepository.Name);
                DataFile dataFile = this.fileService.DownLoadFileFromRepository(file, repository, user, repositoryCredentials);

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(new MemoryStream(dataFile.FileContent));
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(Constants.APPLICATION_X_ZIP);
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = dataFile.FileName;
                return result;
            }
            catch (ArgumentNullException ane)
            {
                message = string.Format(MessageStrings.Argument_Error_Message_Template, ane.ParamName);
                status = HttpStatusCode.BadRequest;
            }
            catch (FileDownloadException downloadException)
            {
                if (downloadException.FileDownloadExceptionType == FileDownloadExceptionType.DownloadUrlNotFound.ToString())
                {
                    error = downloadException.GetHttpError(MessageStrings.Download_URL_Empty);
                }
                else
                {
                    error = downloadException.GetHttpError(string.Empty);
                }

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, error);
            }
            catch (WebException ex)
            {
                // If status code is 404 then send the custom message indicating file does not exist in repository.
                // else read the message and send it to client as text.
                HttpResponseMessage response;
                if (ex.Status == WebExceptionStatus.ProtocolError && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    error = new HttpError(MessageStrings.FileDoesNotExistInRepository);
                    response = Request.CreateErrorResponse(HttpStatusCode.NotFound, error);
                }
                else
                {
                    string errorText = string.Empty;
                    using (Stream st = ((System.Net.WebException)(ex)).Response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(st))
                    {
                        errorText = reader.ReadToEnd();
                    }
                    error = new HttpError(errorText);
                    response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, error);
                }

                return response;
            }

            return Request.CreateErrorResponse(status, message);
        }

        /// <summary>
        /// Retreives the Repository Credentials from Header
        /// </summary>
        /// <returns>string header value</returns>
        private RepositoryCredentials GetRepsitoryCredentials()
        {

            IEnumerable<string> header = null;
            if (!this.Request.Headers.TryGetValues(Constants.RepositoryCredentialHeaderName, out header))
            {
                return null;
            }

            var credentials = header.FirstOrDefault();
            RepositoryCredentials repositoryCredentials = new RepositoryCredentials();
            //Dictionary<string, string> attributes = JsonConvert.DeserializeObject<Dictionary<string, string>>(credentials);
            repositoryCredentials.Attributes = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(credentials);
            //  attributes = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(credentials);
            //repositoryCredentials = new RepositoryCredentials();

            //JArray myObjects = JsonConvert.DeserializeObject<JArray>(credentials);

            //foreach (string v in myObjects)
            //{
            //    JToken  token = JObject.Parse(v);
            //    repositoryCredentials[token.SelectToken("key").Value<string>()] = token.SelectToken("value").Value<string>();
            //}

            return repositoryCredentials;
        }
    }
}
