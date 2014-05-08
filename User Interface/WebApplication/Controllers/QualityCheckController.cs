// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Research.DataOnboarding.WebApplication.Infrastructure;
using Microsoft.Research.DataOnboarding.WebApplication.ViewModels;
using Microsoft.Research.DataOnboarding.WebApplication.Helpers;
using System.Web.Script.Serialization;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.WebApplication.Extensions;
using Microsoft.Research.DataOnboarding.Utilities.Extensions;
using System.Collections.Specialized;
using Microsoft.Research.DataOnboarding.WebApplication.Resource;
using Microsoft.Research.DataOnboarding.Utilities.Model;

namespace Microsoft.Research.DataOnboarding.WebApplication.Controllers
{

    /// <summary>
    /// Controller class for quality check related methods.
    /// </summary>
    public class QualityCheckController : BaseController
    {
        /// <summary>
        /// web request manager
        /// </summary>
        private HttpWebRequestManager webRequestManager = null;

        /// <summary>
        /// client request manager
        /// </summary>
        private WebClientRequestManager webClientManager = null;

        /// <summary>
        /// Action method to return the index view.
        /// </summary>
        /// <returns>Index view.</returns>
        public ActionResult Index()
        {
            return View(this.GetQualityCheckViewModel());
        }

        /// <summary>
        /// Action method to get the add edit quality check rule view.
        /// </summary>
        /// <param name="qcRuleId">Quality check rule id.</param>
        /// <returns>Add edit quality check rule view.</returns>
        public ActionResult AddEditRule(int qcRuleId)
        {
            return View(this.GetQCRuleViewModel(qcRuleId));
        }

        /// <summary>
        /// Action method to save the rule data to the database
        /// </summary>
        /// <param name="ruleModel">Rule view Model</param>
        /// <returns>Status of the operation as json result.</returns>
        public JsonResult SaveRule(QCRuleViewModel ruleModel)
        {
            string message = string.Empty;
            bool status = false;
            if (ruleModel != null)
            {
                QualityCheck qcModel = new QualityCheck();
                qcModel = ruleModel.SetValuesTo(qcModel);

                // serlize the data file before passing to API
                string qcRuleData = qcModel.SerializeObject<QualityCheck>("qcRuleModel");
                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

                webClientManager = new WebClientRequestManager();

                // Checking for the duplicate rule name
                string responseDupruleName = webClientManager.UploadValues(new RequestParams()
                {
                    RequestURL = string.Concat(BaseController.BaseWebApiQCPath + "?ruleName=" + qcModel.Name),
                    RequestMode = RequestMode.POST
                });

                int ruleResult = jsSerializer.Deserialize<int>(responseDupruleName);

                if (ruleResult == 0 || ruleModel.QCRuleId == ruleResult)
                {
                    NameValueCollection values = new NameValueCollection();
                    values.Add("qcRuleModel", qcRuleData.EncodeTo64());

                    string responseString = webClientManager.UploadValues(new RequestParams()
                    {
                        RequestURL = string.Concat(BaseController.BaseWebApiQCPath),
                        RequestMode = RequestMode.POST,
                        Values = values
                    });


                    bool postResult = jsSerializer.Deserialize<bool>(responseString);

                    if (postResult)
                    {
                        status = true;
                    }
                    else
                    {
                        ViewBag.ErrorMessage = Messages.QualityCheckErrorMsg;
                        status = false;
                        message = Messages.QualityCheckErrorMsg;
                    }
                }
                else
                {
                    status = false;
                    message = Messages.DuplicateRuleMsg;
                }
            }
            else
            {
                status = false;
                message = Messages.QualityCheckErrorMsg;
            }

            return Json(new { Status = status, Message = message });
        }

        #region private methods

        /// <summary>
        /// Method to get the quality check view model.
        /// </summary>
        /// <returns></returns>
        private QualityCheckViewModel GetQualityCheckViewModel()
        {
            QualityCheckViewModel viewModel = new QualityCheckViewModel();

            // create the object of HttpWeb Request Manager
            webRequestManager = new HttpWebRequestManager();

            // set the request details to get file list
            webRequestManager.SetRequestDetails(new RequestParams()
            {
                RequestURL = string.Concat(BaseController.BaseWebApiQCPath),
            });

            string qcRulesList = webRequestManager.RetrieveWebResponse();

            // set the request details to get repostiry list
            webRequestManager.SetRequestDetails(new RequestParams()
            {
                RequestURL = string.Concat(BaseController.BaseWebApiFilePath, "?type=QCCOLUMNTYPES"),
            });

            string columnTypeJson = webRequestManager.RetrieveWebResponse();


            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            jsSerializer.MaxJsonLength = int.MaxValue;

            var lstQCRules = jsSerializer.Deserialize<IList<QualityCheckModel>>(qcRulesList);
            lstQCRules = lstQCRules.OrderByDescending(rul => rul.QualityCheckData.CreatedOn).ToList();
            var lstQCColumnTypes = jsSerializer.Deserialize<IEnumerable<QualityCheckColumnType>>(columnTypeJson);

            foreach (var qcObj in lstQCRules)
            {
                QCRuleViewModel rule = new QCRuleViewModel();
                rule.SetValuesFrom(qcObj, lstQCColumnTypes);
                viewModel.QualityCheckRules.Add(rule);
            }

            return viewModel;
        }

        /// <summary>
        /// Helper method to get the single rule view model.
        /// </summary>
        /// <param name="id">Quality check id.</param>
        /// <returns>Quality check rule view model.</returns>
        private QCRuleViewModel GetQCRuleViewModel(int id)
        {
            QCRuleViewModel ruleModel = new QCRuleViewModel();
            // create the object of HttpWeb Request Manager
            webRequestManager = new HttpWebRequestManager();
            // set the request details to get repostiry list
            webRequestManager.SetRequestDetails(new RequestParams()
            {
                RequestURL = string.Concat(BaseController.BaseWebApiFilePath, "?type=QCCOLUMNTYPES"),
            });
            string columnTypeJson = webRequestManager.RetrieveWebResponse();

            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            jsSerializer.MaxJsonLength = int.MaxValue;
            var lstQCColumnTypes = jsSerializer.Deserialize<IEnumerable<QualityCheckColumnType>>(columnTypeJson);

            if (id > 0)
            {
                // set the request details to get file list
                webRequestManager.SetRequestDetails(new RequestParams()
                {
                    RequestURL = string.Concat(BaseController.BaseWebApiQCPath + id),
                });

                string qcRuleList = webRequestManager.RetrieveWebResponse();
                var qualityCheckData = jsSerializer.Deserialize<QualityCheckModel>(qcRuleList);
                ruleModel.SetValuesFrom(qualityCheckData, lstQCColumnTypes);
            }
            else
            {
                QCHeaderViewModel headerModel = new QCHeaderViewModel();
                headerModel.QCColumnTypes = new SelectList(lstQCColumnTypes, "QualityCheckColumnTypeId", "Name");
                headerModel.Order = 1;
                ruleModel.LstHeaderNames.Add(headerModel);
            }

            return ruleModel;
        }

        #endregion
    }
}
