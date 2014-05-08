// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.WebApplication.Helpers;
using Microsoft.Research.DataOnboarding.WebApplication.Infrastructure;
using Microsoft.Research.DataOnboarding.WebApplication.ViewModels;
using System.Web;

namespace Microsoft.Research.DataOnboarding.WebApplication.Extensions
{
    /// <summary>
    /// Extension method class for add edit repository viewmodel.
    /// </summary>
    public static class ManageReopsitoryViewModelExtension
    {
        /// <summary>
        /// Extension method to fill the data to the viewmodel from the repository model class object.
        /// </summary>
        /// <param name="viewModel">Repository view model.</param>
        /// <param name="model">Repository model.</param>
        public static void SetValuesFrom(this ManageReopsitoryViewModel viewModel, Repository model)
        {
            Check.IsNotNull<ManageReopsitoryViewModel>(viewModel, "repositoryViewModel");
            Check.IsNotNull<Repository>(model, "repositoryModel");

            viewModel.AllowedFileTypes = model.AllowedFileTypes;
            viewModel.BaseRepositoryId = model.BaseRepositoryId;
            viewModel.DeleteFielURL = model.HttpDeleteUriTemplate;
            viewModel.DownloadFileURL = model.HttpGetUriTemplate;
            viewModel.GetIdentifierURL = model.HttpIdentifierUriTemplate;
            viewModel.ImpersonatePassword = model.ImpersonatingPassword;
            viewModel.ImpersonateUserName = model.ImpersonatingUserName;
            viewModel.IsImpersonate = model.IsImpersonating == null ? false : (bool)model.IsImpersonating;
            viewModel.IsVisibleToAll = model.IsVisibleToAll;
            viewModel.VisibilityOption = model.IsVisibleToAll ? 1 : 2;
            viewModel.PostFileURL = model.HttpPostUriTemplate;
            viewModel.RepositoryId = model.RepositoryId;
            viewModel.RepositoryName = model.Name;
            viewModel.UserAgreement = model.UserAgreement;
            viewModel.CreatedBy = model.CreatedBy;
            viewModel.CreatedDate = model.CreatedOn;
            viewModel.AccessToken = model.AccessToken;
            viewModel.RefreshToken = model.RefreshToken;
            viewModel.TokenExpiresOn = model.TokenExpiresOn;

            // set repository metadata
            SetRepositoryMetaData(viewModel, model);
        }

        public static void AddDefaultMetaData(this ManageReopsitoryViewModel viewModel)
        {
            var metaDataTypes = GetMetaDataTypes();

            viewModel.RepositoryMetaDataName = "DefaultMetaData";

            viewModel.RepositoryMetaDataFieldList.Add(new RepositoryMetadataFieldViewModel()
            {
                MetaDataTypeId = metaDataTypes.FirstOrDefault().Id.ToString(),
                MetaDataTypes = new SelectList(metaDataTypes, Helpers.Constants.ID, Helpers.Constants.NAME, metaDataTypes.FirstOrDefault().Id.ToString()),
                IsBlankRow = true
            });
        }
        /// <summary>
        ///  Extension method to fill the data from the viewmodel to the repository model
        ///  </summary>
        /// <param name="viewModel">Repository view model.</param>
        /// <param name="model">Repository model.</param>
        /// <returns>Updated repository model.</returns>
        public static Repository SetValuesTo(this ManageReopsitoryViewModel viewModel, Repository model)
        {
            Check.IsNotNull<ManageReopsitoryViewModel>(viewModel, "repositoryViewModel");
            Check.IsNotNull<Repository>(model, "repositoryModel");

            if (viewModel.RepositoryId == 0)
            {
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = BaseController.UserId;
            }
            else
            {
                model.CreatedOn = viewModel.CreatedDate;
                model.CreatedBy = viewModel.CreatedBy;
                model.ModifiedOn = DateTime.UtcNow;
                model.ModifiedBy = BaseController.UserId;
            }

            model.AllowedFileTypes = viewModel.AllowedFileTypes;
            model.BaseRepositoryId = viewModel.BaseRepositoryId;
            model.HttpDeleteUriTemplate = viewModel.DeleteFielURL;
            model.HttpGetUriTemplate = viewModel.DownloadFileURL;
            model.HttpIdentifierUriTemplate = viewModel.GetIdentifierURL;
            model.ImpersonatingPassword = viewModel.ImpersonatePassword;
            model.ImpersonatingUserName = viewModel.ImpersonateUserName;
            model.IsImpersonating = viewModel.IsImpersonate;
            model.IsVisibleToAll = viewModel.VisibilityOption == 1 ? true : false;
            model.HttpPostUriTemplate = viewModel.PostFileURL;
            model.RepositoryId = viewModel.RepositoryId;
            model.Name = viewModel.RepositoryName;
            model.UserAgreement = viewModel.UserAgreement;
            model.IsActive = true;
            model.AccessToken = viewModel.AccessToken;
            model.RefreshToken = viewModel.RefreshToken;
            model.TokenExpiresOn = viewModel.TokenExpiresOn;
            if (model.TokenExpiresOn == null || model.TokenExpiresOn == DateTime.MinValue)
            {
                model.TokenExpiresOn = DateTime.UtcNow;
            }
            // Set repository metadata            

            model.RepositoryMetadata = new List<RepositoryMetadata>();
            model.RepositoryMetadata.Add(new RepositoryMetadata()
                {
                    RepositoryId = viewModel.RepositoryId,
                    RepositoryMetadataId = viewModel.RepositoryMetaDataId,
                    Name = viewModel.RepositoryMetaDataName,
                    RepositoryMetadataFields = new List<RepositoryMetadataField>(),
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = viewModel.CreatedBy
                });

            foreach (var repostiryMetaDataField in viewModel.RepositoryMetaDataFieldList)
            {
                if (!string.IsNullOrEmpty(repostiryMetaDataField.Field) && !(string.IsNullOrEmpty(repostiryMetaDataField.Description)))
                {
                    model.RepositoryMetadata.FirstOrDefault().RepositoryMetadataFields.Add(new RepositoryMetadataField()
                       {
                           RepositoryMetadataFieldId = repostiryMetaDataField.RepositoryMetaDataFieldId,
                           RepositoryMetadataId = viewModel.RepositoryMetaDataId,
                           Description = repostiryMetaDataField.Description,
                           Mapping = repostiryMetaDataField.MappedLocation,
                           IsRequired = repostiryMetaDataField.IsRequired,
                           MetadataTypeId = Convert.ToInt32(repostiryMetaDataField.MetaDataTypeId),
                           Name = repostiryMetaDataField.Field,
                           Range = repostiryMetaDataField.RangeValues
                       });
                }
            }
            return model;
        }
        /// <summary>
        /// Method to set Reporsitry meta data
        /// </summary>        
        /// <param name="viewModel">AddEditReopsitoryViewModel object</param>
        /// <param name="repositoryMetaData">RepositoryMetadata object</param>
        private static void SetRepositoryMetaData(ManageReopsitoryViewModel viewModel, Repository model)
        {
            var metaDataTypes = GetMetaDataTypes();
            try
            {
                // first set the repository metadata
                if (model.RepositoryMetadata == null || model.RepositoryMetadata.Count == 0)
                {
                    viewModel.RepositoryMetaDataName = "DefaultMetaData";
                }
                else
                {
                    viewModel.RepositoryMetaDataId = model.RepositoryMetadata.FirstOrDefault().RepositoryMetadataId;
                    viewModel.RepositoryMetaDataName = model.RepositoryMetadata.FirstOrDefault().Name;
                }

                if (model.RepositoryMetadata != null || model.RepositoryMetadata.Count > 0)
                {
                    if (model.RepositoryMetadata.FirstOrDefault() == null || model.RepositoryMetadata.FirstOrDefault().RepositoryMetadataFields == null || model.RepositoryMetadata.FirstOrDefault().RepositoryMetadataFields.Count == 0)
                    {
                        viewModel.RepositoryMetaDataFieldList.Add(new RepositoryMetadataFieldViewModel()
                        {
                            MetaDataTypeId = metaDataTypes.FirstOrDefault().Id.ToString(),
                            MetaDataTypes = new SelectList(metaDataTypes, Helpers.Constants.ID, Helpers.Constants.NAME, metaDataTypes.FirstOrDefault().Id.ToString()),
                            IsBlankRow = true
                        });
                    }
                    else
                    {
                        foreach (var repositoryMetaDataField in model.RepositoryMetadata.FirstOrDefault().RepositoryMetadataFields)
                        {
                            viewModel.RepositoryMetaDataName = model.RepositoryMetadata.FirstOrDefault().Name;
                            viewModel.RepositoryMetaDataId = model.RepositoryMetadata.FirstOrDefault().RepositoryMetadataId;

                            viewModel.RepositoryMetaDataFieldList.Add(new RepositoryMetadataFieldViewModel()
                            {
                                Description = repositoryMetaDataField.Description,
                                Field = repositoryMetaDataField.Name,
                                MappedLocation = repositoryMetaDataField.Mapping,
                                RangeValues = repositoryMetaDataField.Range,
                                IsRequired = repositoryMetaDataField.IsRequired != null ? Convert.ToBoolean(repositoryMetaDataField.IsRequired) : false,
                                MetaDataTypeId = repositoryMetaDataField.MetadataTypeId.ToString(),
                                MetaDataTypes = new SelectList(metaDataTypes, Helpers.Constants.ID, Helpers.Constants.NAME, repositoryMetaDataField.MetadataTypeId.ToString()),
                                RepositoryMetaDataFieldId = repositoryMetaDataField.RepositoryMetadataFieldId,
                                RepositoryMetaDataId = repositoryMetaDataField.RepositoryMetadataId
                            });
                        }
                    }
                }
                else//Add a blank Row
                {
                    viewModel.RepositoryMetaDataName = viewModel.RepositoryName + "DefaultMetaData";

                    viewModel.RepositoryMetaDataFieldList.Add(new RepositoryMetadataFieldViewModel()
                    {
                        MetaDataTypeId = metaDataTypes.FirstOrDefault().Id.ToString(),
                        MetaDataTypes = new SelectList(metaDataTypes, Helpers.Constants.ID, Helpers.Constants.NAME, metaDataTypes.FirstOrDefault().Id.ToString()),
                        IsBlankRow = true
                    });

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method reurns list of document meta data types
        /// </summary>
        /// <returns></returns>
        private static List<MetaDataTypeViewModel> GetMetaDataTypes()        
        {
            var documentMetadataTypes = new List<MetaDataTypeViewModel>();
            var existingMetaDataTypes = HttpContext.Current.Session["MetaDataType"];
            if (existingMetaDataTypes == null)
            {
                var webRequestManager = new HttpWebRequestManager();
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
                HttpContext.Current.Session["MetaDataType"] = documentMetadataTypes;
            }
            else
            {
                documentMetadataTypes = (List<MetaDataTypeViewModel>)HttpContext.Current.Session["MetaDataType"];
            }
            return documentMetadataTypes;
        }
    }
}