// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Web;
using System.Xml;
using System;
using System.Linq;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.Research.DataOnboarding.WebApplication.Helpers;
using Microsoft.Research.DataOnboarding.WebApplication.Infrastructure;
using Microsoft.Research.DataOnboarding.WebApplication.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.Research.DataOnboarding.WebApplication.Extensions;
using Microsoft.Research.DataOnboarding.DomainModel;
using System.Collections.Specialized;
using Microsoft.Research.DataOnboarding.WebApplication.Resource;
using Microsoft.Research.DataOnboarding.Utilities.Extensions;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using System.Globalization;
using System.Net;
using System.IO;


namespace Microsoft.Research.DataOnboarding.WebApplication.Controllers
{
    /// <summary>
    ///  Controller class for repository related methods.
    /// </summary>
    public class RepositoryController : BaseController
    {
        /// <summary>
        /// Private variable for web request manager.
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
            return View(GetRepositoryViewModel());
        }

        /// <summary>
        /// Action method to get the add edit repository view.
        /// </summary>
        /// <param name="repositoryId">Repository id.</param>
        /// <returns>Add Edit repository view.</returns>
        public ActionResult ManageRepository(int repositoryId)
        {
            return View(this.GetSelectedViewModel(repositoryId));
        }

        /// <summary>
        /// Action method to save the repository data to the database.
        /// </summary>
        /// <param name="repositoryViewModel">Repository viewmodel.</param>
        /// <returns>Json result with the status.</returns>
        public ActionResult SaveRepository(ManageReopsitoryViewModel repositoryViewModel)
        {
            string message = string.Empty;
            string status = "success";
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            try
            {
                if (repositoryViewModel != null)
                {
                    Repository repModel = new Repository();
                    repModel = repositoryViewModel.SetValuesTo(repModel);

                    // serlize the data file before passing to API
                    string repositoryData = repModel.SerializeObject<Repository>("repositoryModel");

                    // create the object of HttpWeb Request Manager
                    webRequestManager = new HttpWebRequestManager();
                    webClientManager = new WebClientRequestManager();
                    webRequestManager.SetRequestDetails(new RequestParams()
                    {
                        RequestURL = string.Concat(BaseController.BaseWebApiRepositoryPath + "?repositoryName=" + repModel.Name),
                    });

                    string repositoryIdResult = webRequestManager.RetrieveWebResponse();
                    int repositoryResult = jsSerializer.Deserialize<int>(repositoryIdResult);

                    if (repositoryResult == 0 || repModel.RepositoryId == repositoryResult)
                    {
                        NameValueCollection values = new NameValueCollection();
                        values.Add("repositoryModel", repositoryData.EncodeTo64());

                        string responseString = webClientManager.UploadValues(new RequestParams()
                        {
                            RequestURL = string.Concat(BaseController.BaseWebApiRepositoryPath),
                            RequestMode = RequestMode.POST,
                            AllowAutoRedirect = false,
                            Values = values
                        });

                        if (!string.IsNullOrEmpty(webClientManager.RedirectionURL))
                        {
                            status = "redirect";
                            message = webClientManager.RedirectionURL;
                            return Json(new { Status = status, Message = message });
                        }

                        bool postResult = jsSerializer.Deserialize<bool>(responseString);

                        if (postResult)
                        {
                            status = "success";
                            // after updating the repostory metadata field,delete the repository metadata field that are marked for deletion
                            if (repositoryViewModel.DeletedMetaDataFieldIds != null && repositoryViewModel.DeletedMetaDataFieldIds.Length > 0)
                            {
                                string deleteResponseString = webClientManager.UploadValues(new RequestParams()
                                {
                                    RequestURL = string.Concat(BaseController.BaseWebApiRepositoryPath, "?repositorId=", repositoryViewModel.RepositoryId, "&repositoryMetadataFields=", repositoryViewModel.DeletedMetaDataFieldIds),
                                    RequestMode = RequestMode.DELETE
                                });

                                jsSerializer = new JavaScriptSerializer();
                                jsSerializer.Deserialize<bool>(deleteResponseString);
                            }

                        }
                        else
                        {
                            ViewBag.ErrorMessage = Messages.RepositoryErrMsg;
                            status = "error";
                            message = Messages.RepositoryErrMsg;
                        }
                    }
                    else
                    {
                        status = "error";
                        message = Messages.DuplicateRepositoryMsg;
                    }
                }
                else
                {
                    status = "error";
                    message = Messages.RepositoryErrMsg;
                }
            }
            catch (WebException webException)
            {
                using (Stream stream = webException.Response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        status = "error";
                        message = reader.ReadToEnd();
                        jsSerializer = new JavaScriptSerializer();
                        var data = jsSerializer.Deserialize<Dictionary<string,object>>(message);
                        message = (string)data.First(m=>m.Key == "Message").Value;
                    }
                }
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }
         
            return Json(new { Status = status, Message = message });
        }
        
        /// <summary>
        /// Adds the posted file.
        /// </summary>
        /// <param name="addContent">Content of the uploaded File</param>
        [HttpPost]
        public JsonResult UploadMetaDataFile()
        {
            var fileUploadStatus = "success";
            try
            {
                HttpPostedFileBase addContent = null;
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    addContent = Request.Files[i] as HttpPostedFileBase;
                }
                var updatedRepositoryViewModel = (ManageReopsitoryViewModel)Session[Constants.REPOSITORYVIEWMODEL];
                var fileContent = string.Empty;
                if (addContent != null && !string.IsNullOrEmpty(addContent.FileName))
                {
                    var fileStream = addContent.InputStream;
                    byte[] xmlContent = new byte[addContent.ContentLength];
                    fileStream.Read(xmlContent, 0, addContent.ContentLength);
                    fileContent = System.Text.UTF8Encoding.UTF8.GetString(xmlContent);
                    
                    fileContent = fileContent.Replace("\n", "").Replace("\r", "").Replace("\t", "");//.Replace("\"",""");

                    updatedRepositoryViewModel = GetMetaDataModelFromXML(fileContent);
                    Session[Constants.REPOSITORYVIEWMODEL] = updatedRepositoryViewModel;

                }
                else
                {
                    fileUploadStatus = "Error in reading the file";
                }
            }
            catch
            {
                fileUploadStatus = "Invalid file,Please try to upload the valid file";
            }
            //var resultString = this.RenderViewToString("ManageFileMetaData", updatedRepositoryViewModel);

            return Json(new { Message = fileUploadStatus });// Json(new { ViewString = resultString }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get the uploaded xml content used in admin metadata uploading
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUploadedXMLDataViewModel()
        {
            ManageReopsitoryViewModel updatedRepositoryViewModel = (ManageReopsitoryViewModel)Session[Constants.REPOSITORYVIEWMODEL];
            return View("ManageFileMetaData", updatedRepositoryViewModel);
        }

        #region private methods

        /// <summary>
        /// Private method to get the repository view model list.
        /// </summary>
        /// <returns>Repository view model collection.</returns>
        private IList<RepositoryViewModel> GetRepositoryViewModel()
        {
            string repositoryList = string.Empty;
            IList<RepositoryViewModel> lstRepViewModel = new List<RepositoryViewModel>();

            try
            {
                // create the object of HttpWeb Request Manager
                webRequestManager = new HttpWebRequestManager();

                // set the request details to get repostiry list
                webRequestManager.SetRequestDetails(new RequestParams()
                {
                    RequestURL = string.Concat(BaseController.BaseWebApiRepositoryPath),
                });

                repositoryList = webRequestManager.RetrieveWebResponse();

                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                jsSerializer.MaxJsonLength = int.MaxValue;

                var lstRepository = jsSerializer.Deserialize<IList<RepositoryDataModel>>(repositoryList);

                foreach (var repositoryModel in lstRepository)
                {
                    RepositoryViewModel repModel = new RepositoryViewModel();
                    repModel.SetValuesFrom(repositoryModel);
                    lstRepViewModel.Add(repModel);
                }
            }
            catch
            {
                lstRepViewModel = null;
            }

            return lstRepViewModel;
        }

        private ManageReopsitoryViewModel GetMetaDataModelFromXML(string metaDatafileContent)
        {
            ManageReopsitoryViewModel currentRepoModel = (ManageReopsitoryViewModel)Session[Constants.REPOSITORYVIEWMODEL];
            XmlDocument mappingXmlDocument = new XmlDocument();
            if (metaDatafileContent != null && metaDatafileContent != null && !string.IsNullOrEmpty(metaDatafileContent))
            {
                //metaDatafileContent = "<xml><data></data><first></first><second></second><data1></data1></xml>";
                //metaDatafileContent = "<xml><data><firstChild></firstChild></data><data1></data1><data2></data2><data3></data3><first><firstChild111></firstChild111></first><second></second><data1></data1></xml>";

                //metaDatafileContent = HttpUtility.HtmlDecode(metaDatafileContent);

                mappingXmlDocument.LoadXml(metaDatafileContent);


                // first clear the existing repositry metadata fields
                if (currentRepoModel.RepositoryMetaDataFieldList != null)
                {
                    currentRepoModel.RepositoryMetaDataFieldList.Clear();
                }

                foreach (XmlNode node in mappingXmlDocument.ChildNodes)
                {
                    if (node.NodeType != XmlNodeType.Comment && node.NodeType != XmlNodeType.Attribute)
                    {
                        if (node.HasChildNodes && node.ChildNodes.Count > 0)
                        {
                            foreach (XmlNode childNode in node.ChildNodes)
                            {
                                if (childNode.HasChildNodes && childNode.ChildNodes.Count > 0)
                                {
                                    TraverseNodes(childNode, currentRepoModel);
                                }
                                else
                                {
                                    AddDataRow(childNode, currentRepoModel);
                                }
                            }
                        }
                        else
                        {
                            AddDataRow(node, currentRepoModel);
                        }
                    }
                }
                currentRepoModel.RepositoryMetaDataFieldList = currentRepoModel.RepositoryMetaDataFieldList.Where(cr => cr.RowType == MetaDataRowType.DataRow).ToList();
            }
            else
            {
                throw new ArgumentNullException("Invalid file");
            }
            Session[Constants.REPOSITORYVIEWMODEL] = currentRepoModel;
            return currentRepoModel;
        }

        /// <summary>
        /// Action method to delete the selected repository.
        /// </summary>
        /// <param name="repositoryId">Repository id.</param>
        /// <returns>Status of the opeartion as json result.</returns>
        public JsonResult DeleteRepository(int repositoryId)
        {
            string message = string.Empty;
            bool status = false;

            if (repositoryId > 0)
            {
                webClientManager = new WebClientRequestManager();


                string responseString = webClientManager.UploadValues(new RequestParams()
                {
                    RequestURL = string.Concat(BaseController.BaseWebApiRepositoryPath + repositoryId),
                    RequestMode = RequestMode.DELETE
                });

                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                bool postResult = jsSerializer.Deserialize<bool>(responseString);

                if (postResult)
                {
                    status = true;
                }
                else
                {
                    ViewBag.ErrorMessage = Messages.RepositoryErrMsg;
                    status = false;
                    message = Messages.RepositoryErrMsg;
                }
            }
            else
            {
                status = false;
                message = Messages.RepositoryErrMsg;
            }
            return Json(new { Status = status, Message = message });
        }

        private ManageReopsitoryViewModel GetSelectedViewModel(int repositoryId)
        {
            ManageReopsitoryViewModel viewModel = new ManageReopsitoryViewModel();

            try
            {
                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                jsSerializer.MaxJsonLength = int.MaxValue;

                // create the object of HttpWeb Request Manager
                webRequestManager = new HttpWebRequestManager();

                // set the request details to get repostiry list
                webRequestManager.SetRequestDetails(new RequestParams()
                {
                    RequestURL = string.Concat(BaseController.BaseWebApiRepositoryTypesPath),
                });

                string repositoryTypeList = webRequestManager.RetrieveWebResponse();

                var lstRepositoryType = jsSerializer.Deserialize<IList<BaseRepository>>(repositoryTypeList);

                if (repositoryId > 0)
                {
                    // set the request details to get file list
                    webRequestManager.SetRequestDetails(new RequestParams()
                    {
                        RequestURL = string.Concat(BaseController.BaseWebApiRepositoryPath + repositoryId),
                    });

                    string selRepositoryStr = webRequestManager.RetrieveWebResponse();
                    var selRepository = jsSerializer.Deserialize<Repository>(selRepositoryStr);
                    viewModel.SetValuesFrom(selRepository);                    
                }
                else
                {
                    // Add default metadata
                    viewModel.AddDefaultMetaData();
                }
                Session[Constants.REPOSITORYVIEWMODEL] = viewModel;
                viewModel.RepositoryTypes = new SelectList(lstRepositoryType, Constants.BASEREPOSITORYID, Constants.NAMESTRING);

            }
            catch
            {
                viewModel = null;
            }

            return viewModel;
        }

        /// <summary>
        /// Method to traverse nodes of MetadataTypeViewModel collection
        /// </summary>
        /// <param name="node">XmlNode node</param>
        /// <param name="metadataType">MetadataTypeViewModel object</param>
        private static void TraverseNodes(XmlNode node, ManageReopsitoryViewModel metadataType)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.NodeType != XmlNodeType.Comment && childNode.NodeType != XmlNodeType.Attribute)
                {
                    if (childNode.ChildNodes.Count > 0)
                    {
                        if (childNode.ChildNodes.Count == 1 && (childNode.ChildNodes[0].NodeType == XmlNodeType.Comment || childNode.ChildNodes[0].NodeType == XmlNodeType.Text || childNode.ChildNodes[0].Name.ToUpper(CultureInfo.InvariantCulture) == "PARA"))
                        {
                            AddDataRow(childNode, metadataType);
                        }
                        else
                        {
                            TraverseNodes(childNode, metadataType);
                        }
                    }
                    else
                    {
                        AddDataRow(childNode, metadataType);
                    }
                }
            }
        }

        /// <summary>
        /// Method adds the xml node to MetadataType List collection
        /// </summary>
        /// <param name="childNode">xml child node</param>
        /// <param name="metadataType">MetadataTypeViewModel object</param>
        /// <returns>returns currentHeaderRow Index</returns>
        private static int AddDataRow(XmlNode childNode, ManageReopsitoryViewModel metadataType)
        {
            var metaDataTypes = GetMetaDataTypes();

            var currentHeaderRowIndex = -1;
            var metaDataDetailRow = new RepositoryMetadataFieldViewModel();
            metaDataDetailRow.MappedLocation = GetParentNodePath(childNode);
            metaDataDetailRow.MetaDataTypes = new SelectList(metaDataTypes, Helpers.Constants.ID, Helpers.Constants.NAME, metaDataTypes.FirstOrDefault().Id.ToString());
            metaDataDetailRow.IsBlankRow = false;
            metaDataDetailRow.MetaDataTypeId = metaDataTypes.FirstOrDefault().Id.ToString();
            //metaDataDetailRow.MetadataNodeName = childNode.Name;
            ////before adding the data row,If the header row is not there add it else get the index of the header row to add this item next to the header index
            AddHeaderRow(metaDataDetailRow.MappedLocation, ref currentHeaderRowIndex, metadataType);
            metaDataDetailRow.MappedLocation = string.Concat(metaDataDetailRow.MappedLocation, ".", childNode.Name);
            //parents tag and body tag should not be added
            if (metaDataDetailRow.MappedLocation != "xml" && metaDataDetailRow.MappedLocation != ".xml" && !metaDataDetailRow.MappedLocation.Contains(".body"))
            {
                metaDataDetailRow.MappedLocation = metaDataDetailRow.MappedLocation.Replace(".#text", string.Empty);
                if (currentHeaderRowIndex > -1)
                {
                    metadataType.RepositoryMetaDataFieldList.Insert(currentHeaderRowIndex + 1, metaDataDetailRow);
                }
                else
                {
                    metadataType.RepositoryMetaDataFieldList.Add(metaDataDetailRow);
                }
            }

            return currentHeaderRowIndex;
        }

        /// <summary>
        /// Method gets the parent node of given xml Node
        /// </summary>
        /// <param name="node">XmlNode object</param>
        /// <returns>returns path</returns>
        private static string GetParentNodePath(XmlNode node)
        {
            var nodeFullPath = string.Empty;

            var parentNodes = new List<string>();
            while (node.ParentNode != null)
            {
                parentNodes.Add(node.Name);
                node = node.ParentNode;
            }

            int iterator = 0;
            for (iterator = parentNodes.Count - 1; iterator > 0; iterator--)
            {
                if (nodeFullPath.Length == 0)
                {
                    nodeFullPath = parentNodes[iterator].ToString();
                }
                else
                {
                    nodeFullPath = string.Concat(nodeFullPath, ".", parentNodes[iterator].ToString());
                }
            }

            return nodeFullPath.Replace("eml:eml", "eml");
        }

        /// <summary>
        /// Method to add header row
        /// </summary>
        /// <param name="headerName">headerName string</param>
        /// <param name="currentHeaderRowIndex">currentHeaderRow index</param>
        /// <param name="metadataType">MetadataType object</param>
        private static void AddHeaderRow(string headerName, ref int currentHeaderRowIndex, ManageReopsitoryViewModel viewModel)
        {
            var metaDataTypes = GetMetaDataTypes();

            var headeRow = viewModel.RepositoryMetaDataFieldList.FirstOrDefault(m => (string.Compare(m.MetadataNodeName, headerName, StringComparison.Ordinal) == 0));
            if (headeRow == null)
            {
                var metaDataHeaderRow = new RepositoryMetadataFieldViewModel();
                metaDataHeaderRow.RowType = MetaDataRowType.HeaderRow;
                metaDataHeaderRow.MetadataNodeName = headerName;
                //metaDataHeaderRow.MetaDataTypes = new SelectList(metaDataTypes, Helpers.Constants.ID, Helpers.Constants.NAME, metaDataTypes.FirstOrDefault().Id.ToString());
                viewModel.RepositoryMetaDataFieldList.Add(metaDataHeaderRow);
            }
            else
            {
                currentHeaderRowIndex = viewModel.RepositoryMetaDataFieldList.FindIndex(m => (string.Compare(m.MetadataNodeName, headerName, StringComparison.Ordinal) == 0));
            }
        }

        /// <summary>
        /// Method reurns list of document meta data types
        /// </summary>
        /// <returns></returns>
        private static List<MetaDataTypeViewModel> GetMetaDataTypes()
        {

            List<MetaDataTypeViewModel> documentMetadataTypes = new List<MetaDataTypeViewModel>();
            HttpWebRequestManager webRequestManager = new HttpWebRequestManager();
            webRequestManager.SetRequestDetails(new RequestParams()
            {
                RequestURL = string.Concat(BaseController.BaseWebApiFilePath, "?type=METADATATYPES"),
            });

            string columnTypeJson = webRequestManager.RetrieveWebResponse();

            var metaDataTypes = new JavaScriptSerializer().Deserialize<IEnumerable<MetadataType>>(columnTypeJson);
            if (metaDataTypes != null)
            {
                foreach (var item in metaDataTypes)
                {
                    documentMetadataTypes.Add(new MetaDataTypeViewModel() { Id = item.MetadataTypeId.ToString(), Name = item.Name });
                }
            }
            return documentMetadataTypes;
        }
        #endregion

    }
}
