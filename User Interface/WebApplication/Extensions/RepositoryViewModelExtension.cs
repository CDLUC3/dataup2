// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.Research.DataOnboarding.WebApplication.ViewModels;

namespace Microsoft.Research.DataOnboarding.WebApplication.Extensions
{
    /// <summary>
    /// Extension method class for repository view model to fill the data.
    /// </summary>
    public static class RepositoryViewModelExtension
    {
        /// <summary>
        /// Extension method for repository view model to fill the data from repository data model.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="model"></param>
        public static void SetValuesFrom(this RepositoryViewModel viewModel, RepositoryDataModel model)
        {
            Check.IsNotNull<RepositoryViewModel>(viewModel, "repositoryViewModel");
            Check.IsNotNull<RepositoryDataModel>(model, "repositoryDataModel");
            Check.IsNotNull<Repository>(model.RepositoryData, "repository");

            viewModel.AuthenticationMode = model.AuthenticationType;
            viewModel.CreatedUser = model.CreatedUser;
            viewModel.CreatedDate = model.RepositoryData.CreatedOn == null ? string.Empty : ((DateTime)model.RepositoryData.CreatedOn).ToClientTime().ToString();
            viewModel.FileTypes = model.RepositoryData.AllowedFileTypes;
            viewModel.IsVisibleToAll = model.RepositoryData.IsVisibleToAll;
            viewModel.RepositoryId = model.RepositoryData.RepositoryId;
            viewModel.RepositoryName = model.RepositoryData.Name;
        }
    }
}