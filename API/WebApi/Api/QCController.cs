// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.DomainModel.ConceptualModel;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.QCService.Interface;
using Microsoft.Research.DataOnboarding.Services.UserService;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Extensions;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.Research.DataOnboarding.WebApi.Helpers;
using Microsoft.Research.DataOnboarding.WebApi.Resources;
using Microsoft.Research.DataOnboarding.WebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DM = Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.WebApi.Api
{
    /// <summary>
    /// Web API controller class for QC related methods
    /// </summary>
    public class QCController : ApiController
    {
        /// <summary>
        /// Private variable to hold the QC service object.
        /// </summary>
        private IQCService qcService = null;

        /// <summary>
        /// interface IFileServiceFactory 
        /// </summary>
        protected IFileServiceFactory fileServiceFactory;

        /// <summary>
        /// Statis message.
        /// </summary>
        string message = string.Empty;

        /// <summary>
        /// Status code.
        /// </summary>
        HttpStatusCode status = HttpStatusCode.OK;

        /// <summary>
        /// Holds the Reference to user object
        /// </summary>
        protected User user;

        /// <summary>
        /// Holds the reference to IUserService
        /// </summary>
        protected IUserService userService;

        /// <summary>
        /// contains the referene to DiagnosticsProvider
        /// </summary>
        private readonly DiagnosticsProvider diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="QCController" /> class.
        /// </summary>
        /// <param name="qcService">IQC Service object.</param>
        /// <param name="fileServiceFactory">File service factory</param>
        /// <param name="userService">User service</param>
        public QCController(IQCService qcService, IFileServiceFactory fileServiceFactory, IUserService userService)
        {
            this.qcService = qcService;
            this.fileServiceFactory = fileServiceFactory;
            this.userService = userService;
            this.user = IdentityHelper.GetCurrentUser(this.userService, this.User as ClaimsPrincipal);
            this.diagnostics = new DiagnosticsProvider(this.GetType());
        }

        /// <summary>
        /// Method to get the all available quality check rules.
        /// </summary>
        /// <returns>Http response.</returns>
        [HttpGet]
        public HttpResponseMessage RetrieveQualityCheckRules()
        {
            bool isAdmin = this.user.UserRoles.Any(ur => ur.Role.Name.Equals(Roles.Administrator.ToString(), StringComparison.OrdinalIgnoreCase));
            IList<QualityCheckModel> qualityCheckList = this.qcService.GetQualityCheckRules(isAdmin);
            return Request.CreateResponse<IList<QualityCheckModel>>(status, qualityCheckList);
        }

        /// <summary>
        /// Method to get data for showing quality check rules and file sheets.
        /// </summary>
        /// <param name="fileId">File Id.</param>
        /// <returns>Http response message.</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator, User")]
        public async Task<HttpResponseMessage> GetQualityCheckRulesAndFileSheets(int fileId)
        {
            var fileService = fileServiceFactory.GetFileService(BaseRepositoryEnum.Default.ToString());
            Func<DM.File, bool> fileByFileIdAndUserIdFilter = f => f.FileId == fileId && f.CreatedBy == this.user.UserId && (f.isDeleted == null || f.isDeleted == false);
            var file = fileService.GetFiles(fileByFileIdAndUserIdFilter).FirstOrDefault();

            if (file == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, MessageStrings.FileNotFound);
            }

            string fileExtension = Path.GetExtension(file.Name);
            if (!(fileExtension.Equals(Constants.XLSX, StringComparison.InvariantCultureIgnoreCase) || fileExtension.Equals(Constants.CSV, StringComparison.InvariantCultureIgnoreCase)))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, MessageStrings.QualityCheckNotSupportedErrorMessage);
            }

            bool isAdmin = this.user.UserRoles.Any(ur => ur.Role.Name.Equals(Roles.Administrator.ToString(), StringComparison.OrdinalIgnoreCase));
            QualityChecksViewModel qualityChecksViewModel = new QualityChecksViewModel();
            qualityChecksViewModel.ColumnRules = this.qcService.GetQualityCheckRules(isAdmin);
            try
            {
                qualityChecksViewModel.FileSheets = await fileService.GetDocumentSheetDetails(file);
            }
            catch (FileFormatException ex)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, qualityChecksViewModel);
        }

        /// <summary>
        /// Method to get the quality check data for the specific id.
        /// </summary>
        /// <param name="id">Quality check id.</param>
        /// <returns>Response with the quality check data.</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public HttpResponseMessage GetQualityCheckById(int id)
        {
            // Check if the model binding was successful and is in a valid state
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Empty);
            }
            try
            {
                // Check if the file service is valid
                Check.IsNotNull(this.qcService, "qcService");

                QualityCheckModel qualityCheckList = this.qcService.GetQualityCheckById(id);

                return Request.CreateResponse<QualityCheckModel>(status, qualityCheckList);
            }
            catch (ArgumentNullException ane)
            {
                if (ane.ParamName.Equals("qcService"))
                {
                    message = MessageStrings.QC_Service_Is_Null;
                    status = HttpStatusCode.InternalServerError;
                }
                else
                {
                    message = ane.Message;
                    status = HttpStatusCode.BadRequest;
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
        /// Method to add or update the quality check data.
        /// </summary>
        /// <returns>Add or update operation result.</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public HttpResponseMessage EditQualityCheck()
        {
            // Check if the model binding was successful and is in a valid state
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Empty);
            }
            try
            {
                // Check if the file service is valid
                Check.IsNotNull(this.qcService, "qcService");

                if (HttpContext.Current.Request["qcRuleModel"] != null)
                {
                    var qcModelString = HttpContext.Current.Request["qcRuleModel"];

                    if (qcModelString != null)
                    {
                        QualityCheck qualityCheck = Helper.DeSerializeObject<QualityCheck>(qcModelString.DecodeFrom64(), "qcRuleModel");
                        bool result = this.qcService.AddUpdateQualityCheck(qualityCheck);
                        return Request.CreateResponse<bool>(status, result);
                    }
                }
            }
            catch (ArgumentNullException ane)
            {
                if (ane.ParamName.Equals("qcService"))
                {
                    message = MessageStrings.QC_Service_Is_Null;
                    status = HttpStatusCode.InternalServerError;
                }
                else
                {
                    message = ane.Message;
                    status = HttpStatusCode.BadRequest;
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
                Check.IsNotNull(this.qcService, "qcService");

                bool deleteResult = this.qcService.DeleteQualityCheckRule(id);
                return Request.CreateResponse<bool>(HttpStatusCode.OK, deleteResult);
            }
            catch (ArgumentNullException ane)
            {
                if (ane.ParamName.Equals("qcService"))
                {
                    message = MessageStrings.QC_Service_Is_Null;
                    status = HttpStatusCode.InternalServerError;
                }
                else
                {
                    message = ane.Message;
                    status = HttpStatusCode.BadRequest;
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
        /// Method to check file exists for the user id
        /// </summary>
        /// <param name="ruleName">Rule name.</param>
        /// <returns>Boolean result to check the existence of the rule name.</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public HttpResponseMessage CheckRuleExists(string ruleName)
        {
            // Check if the model binding was successful and is in a valid state
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Empty);
            }
            try
            {
                var ruleId = this.qcService.CheckRuleExists(ruleName);

                return Request.CreateResponse<int>(HttpStatusCode.OK, ruleId);
            }
            catch (ArgumentNullException ane)
            {
                if (ane.ParamName.Equals("qcService"))
                {
                    message = MessageStrings.QC_Service_Is_Null;
                    status = HttpStatusCode.InternalServerError;
                }
                else
                {
                    message = ane.Message;
                    status = HttpStatusCode.BadRequest;
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
        /// Method to retrieve Quality Check issues
        /// </summary>
        /// <param name="fieId">File Id</param>
        /// <param name="ruleId">Rule Id</param>
        /// <param name="sheetIds">Sheet Ids</param>
        /// <returns>returns the collection of Quality Check issues</returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetQualityCheckIssues(int fileId, int qualityCheckId, string sheetIds)
        {
            IEnumerable<QualityCheckResult> qualityCheckList = await this.qcService.GetQualityCheckIssues(fileId, qualityCheckId, sheetIds);
            return Request.CreateResponse<IEnumerable<QualityCheckResult>>(status, qualityCheckList);
        }
    }
}
