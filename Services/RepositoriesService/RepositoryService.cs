// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Research.DataOnboarding.RepositoriesService
{
    /// <summary>
    /// Repository service class.
    /// </summary>
    public class RepositoryService : IRepositoryService
    {
        /// <summary>
        /// Variable to hold the user repository object.
        /// </summary>
        private IUserRepository userRepository = null;

        /// <summary>
        /// Variable to hold the repository details object.
        /// </summary>
        private IRepositoryDetails repositoryDetails = null;

        /// <summary>
        /// Variable to hold the file repository.
        /// </summary>
        private IFileRepository fileRepository = null;

        /// <summary>
        /// Variable to hold the unit of work object.
        /// </summary>
        private IUnitOfWork unitOfWork = null;

        /// <summary>
        /// Initalizes the Respository Service
        /// </summary>
        /// <param name="repositoryDetails">Repository Details</param>
        /// <param name="unitOfWork">object of IUnitOfWork</param>
        public RepositoryService(IRepositoryDetails repositoryDetails, IUnitOfWork unitOfWork, IUserRepository userRepository, IFileRepository fileRepository)
        {
            this.repositoryDetails = repositoryDetails;
            this.unitOfWork = unitOfWork;
            this.userRepository = userRepository;
            this.fileRepository = fileRepository;
        }

        /// <summary>
        ///  Method to retrieve all the available repositories.
        /// </summary>
        /// <param name="includeAdminRepositories">bool indicates if repositories marked as AdminOnly should be returned or not</param>
        /// <returns>RepositoryCollection</returns>
        public IEnumerable<RepositoryDataModel> RetrieveRepositories(bool includeAdminRepositories)
        {
            IList<RepositoryDataModel> lstRepository = new List<RepositoryDataModel>();
            var lstRep = this.repositoryDetails.RetrieveRepositories();
            foreach (var rep in lstRep)
            {
                if (includeAdminRepositories || rep.IsVisibleToAll)
                {
                    RepositoryDataModel model = new RepositoryDataModel();

                    model.RepositoryData = rep;
                    var user = this.userRepository.GetUserbyUserId(rep.CreatedBy);
                    model.CreatedUser = user.FirstName + " " + user.LastName;
                    //var repType = this.repositoryDetails.GetRepositoryTypeById(rep.RepositoryTypeId);
                    model.AuthenticationType = string.Empty;
                    lstRepository.Add(model);
                }
            }

            return lstRepository;
        }

        /// <summary>
        /// Method to get all the available repository types.
        /// </summary>
        /// <returns>Repository type collection.</returns>
        public IEnumerable<BaseRepository> RetrieveRepositoryTypes()
        {
            return this.repositoryDetails.RetrieveRepositoryTypes();
        }

        /// <summary>
        /// Method to get the selected repository details.
        /// </summary>
        /// <param name="repositoryId">Repository id.</param>
        /// <returns>Selected fepository.</returns>
        public Repository GetRepositoryById(int repositoryId)
        {
            return this.repositoryDetails.GetRepositoryById(repositoryId);
        }

        /// <summary>
        /// Method to get the selected repository details.
        /// </summary>
        /// <param name="name">Repository name.</param>
        /// <returns>Selected fepository.</returns>
        public Repository GetRepositoryByName(string name)
        {
            Repository repository = this.repositoryDetails.GetRepositoryByName(name);
            if (null == repository)
            {
                throw new RepositoryNotFoundException() { Name = name };
            }

            return repository;
        }

        /// <summary>
        /// Method to  add\update the specified repository.
        /// </summary>
        /// <param name="repositoryData">Repository details.</param>
        /// <returns>True if success else false.</returns>
        public bool AddUpdateRepository(Repository repositoryData)
        {
            bool updateResult = false;

            if (repositoryData.BaseRepositoryId == 2 && (bool)repositoryData.IsImpersonating)
            {
                if (string.IsNullOrEmpty(repositoryData.AccessToken))
                {
                    throw new ArgumentNullException("accessToken");
                }

                if (string.IsNullOrEmpty(repositoryData.RefreshToken))
                {
                    throw new ArgumentNullException("refreshToken");
                }

                if (repositoryData.TokenExpiresOn == null)
                {
                    throw new ArgumentNullException("tokenExpiresOn");
                }
            }

            if (repositoryData.RepositoryId <= 0)
            {
                // First save the repository and get the repository id and set to repository metadata
                // And then save the repository metadata and get the repository metadata id and then save the repository metadata fields
                var repositry = SetRepositoryValues(repositoryData);

                // adding new repository
                var addedRepository = this.repositoryDetails.AddRepository(repositry);

                if (addedRepository != null)
                {
                    updateResult = true;
                    this.unitOfWork.Commit();
                }

                if (repositoryData.RepositoryMetadata != null && repositoryData.RepositoryMetadata.Count > 0
                        && repositoryData.RepositoryMetadata.FirstOrDefault().RepositoryMetadataFields != null && repositoryData.RepositoryMetadata.FirstOrDefault().RepositoryMetadataFields.Count > 0)
                {
                    var actualRepositoryMetaData = repositoryData.RepositoryMetadata.FirstOrDefault();

                    var addedRepositoryMetaData = SetRepositoryMetaData(repositoryData, addedRepository, actualRepositoryMetaData);

                    if (addedRepositoryMetaData != null)
                    {
                        this.unitOfWork.Commit();
                    }
                }
            }
            else
            {
                // get only repository values and update only repository data first
                var repositry = SetRepositoryValues(repositoryData);

                // Updating existing repository
                var updatedRepositoryData = this.repositoryDetails.UpdateRepository(repositry);

                if (updatedRepositoryData != null && updatedRepositoryData.RepositoryId == repositoryData.RepositoryId)
                {
                    updateResult = true;
                    this.unitOfWork.Commit();
                }

                var savedRepositoryMetaData = repositoryData.RepositoryMetadata.FirstOrDefault();

                // Add or Update the Repository MetaData
                if (repositoryData.RepositoryMetadata != null && repositoryData.RepositoryMetadata.FirstOrDefault() != null
                     && repositoryData.RepositoryMetadata.FirstOrDefault().RepositoryMetadataFields != null &&
                     repositoryData.RepositoryMetadata.FirstOrDefault().RepositoryMetadataFields.Count > 0)
                {
                    if (repositoryData.RepositoryMetadata.FirstOrDefault().RepositoryMetadataId > 0)
                    {
                        repositoryData.RepositoryMetadata.FirstOrDefault().ModifiedBy = repositoryData.CreatedBy;
                        repositoryData.RepositoryMetadata.FirstOrDefault().ModifiedOn = DateTime.UtcNow;
                    }
                    else
                    {
                        repositoryData.RepositoryMetadata.FirstOrDefault().CreatedBy = repositoryData.CreatedBy;
                        repositoryData.RepositoryMetadata.FirstOrDefault().CreatedOn = repositoryData.CreatedOn;
                    }

                    savedRepositoryMetaData = this.repositoryDetails.SaveRepositoryMetaData(repositoryData.RepositoryMetadata.FirstOrDefault());
                    if (savedRepositoryMetaData != null)
                    {
                        this.unitOfWork.Commit();
                    }
                }
            }
            return updateResult;
        }

        private RepositoryMetadata SetRepositoryMetaData(Repository repositoryData, Repository updatedRepositoryData, RepositoryMetadata savedRepositoryMetaData)
        {
            var actualRepositoryMetaData = repositoryData.RepositoryMetadata.FirstOrDefault();

            actualRepositoryMetaData.RepositoryId = updatedRepositoryData.RepositoryId;
            if (actualRepositoryMetaData.RepositoryMetadataId > 0)
            {
                actualRepositoryMetaData.ModifiedBy = updatedRepositoryData.CreatedBy;
                actualRepositoryMetaData.ModifiedOn = DateTime.UtcNow;
            }
            else
            {
                actualRepositoryMetaData.CreatedBy = updatedRepositoryData.CreatedBy;
                actualRepositoryMetaData.CreatedOn = DateTime.UtcNow;
            }

            savedRepositoryMetaData = this.repositoryDetails.SaveRepositoryMetaData(actualRepositoryMetaData);
            return savedRepositoryMetaData;
        }

        private static Repository SetRepositoryValues(Repository repositoryDetails)
        {
            var repositry = new Repository();
            repositry.RepositoryId = repositoryDetails.RepositoryId;
            repositry.AllowedFileTypes = repositoryDetails.AllowedFileTypes;
            repositry.BaseRepositoryId = repositoryDetails.BaseRepositoryId;
            repositry.CreatedBy = repositoryDetails.CreatedBy;
            repositry.CreatedOn = repositoryDetails.CreatedOn;
            repositry.HttpDeleteUriTemplate = repositoryDetails.HttpDeleteUriTemplate;
            repositry.HttpGetUriTemplate = repositoryDetails.HttpGetUriTemplate;
            repositry.HttpIdentifierUriTemplate = repositoryDetails.HttpIdentifierUriTemplate;
            repositry.HttpPostUriTemplate = repositoryDetails.HttpPostUriTemplate;
            repositry.ImpersonatingPassword = repositoryDetails.ImpersonatingPassword;
            repositry.ImpersonatingUserName = repositoryDetails.ImpersonatingUserName;
            repositry.IsActive = repositoryDetails.IsActive;
            repositry.IsImpersonating = repositoryDetails.IsImpersonating;
            repositry.IsVisibleToAll = repositoryDetails.IsVisibleToAll;
            repositry.ModifiedBy = repositoryDetails.ModifiedBy;
            repositry.ModifiedOn = repositoryDetails.ModifiedOn;
            repositry.Name = repositoryDetails.Name;
            repositry.UserAgreement = repositoryDetails.UserAgreement;
            repositry.AccessToken = repositoryDetails.AccessToken;
            repositry.RefreshToken = repositoryDetails.RefreshToken;
            repositry.TokenExpiresOn = repositoryDetails.TokenExpiresOn;

            return repositry;
        }

        /// <summary>
        /// Method to delete the repository.
        /// </summary>
        /// <param name="repositoryDetails">Repository details.</param>
        /// <returns>True if success else false.</returns>
        public bool DeleteRepository(int repositoryId)
        {
            bool deleteResult = false;

            // Deleting existing files mapped for this repository
            var fileList = this.fileRepository.GetFilesByRepository(repositoryId);

            foreach (var file in fileList)
            {
                this.fileRepository.DeleteFile(file.FileId, file.Status, true, false);

                if (file.Status.Equals(FileStatus.Posted.ToString(), StringComparison.InvariantCulture))
                {
                    this.fileRepository.DeleteFile(file.FileId, file.Status);
                }
                else
                {
                    file.RepositoryId = null;
                    this.fileRepository.UpdateFile(file);
                }
            }
            //before deleting reposiotry ,delete the file information
            this.unitOfWork.Commit();

            var deleteRepository = this.repositoryDetails.DeleteRepository(repositoryId);

            if (deleteRepository != null && deleteRepository.RepositoryId == repositoryId)
            {
                deleteResult = true;
                this.unitOfWork.Commit();
            }

            return deleteResult;
        }

        /// <summary>
        /// Method to check the duplicate repository name.
        /// </summary>
        /// <param name="repositoryName">Repository name.</param>
        /// <returns>Repository id if exists else 0.</returns>
        public int CheckRepositoryExists(string repositoryName)
        {
            int result = 0;
            var repository = this.repositoryDetails.GetRepositoryByName(repositoryName);

            if (repository != null && repository.Name.Equals(repositoryName, StringComparison.InvariantCultureIgnoreCase))
            {
                result = repository.RepositoryId;
            }

            return result;
        }

        private Repository GetRepositoryWithOutChildDetails(Repository repsoitory)
        {
            var plainRepositroty = new Repository();
            return plainRepositroty;
        }

        /// <summary>
        /// Method to delete repository metadata feidls
        /// </summary>
        /// <param name="repositoryId"></param>
        /// <param name="repositoryMetaDataFields"></param>
        /// <returns></returns>
        public bool DeleteRepositoryMetaDataFields(int repositoryId, string repositoryMetaDataFields)
        {
            bool deleteResult = false;
            char[] commaSeparator = new char[] { ',' };
            if (repositoryMetaDataFields.Contains(","))
            {
                var repositoryMetaDataFieldlists = repositoryMetaDataFields.Split(commaSeparator);
                foreach (var repositoryMetaDataFieldId in repositoryMetaDataFieldlists)
                {
                    if (repositoryMetaDataFieldId.Length > 0)
                    {
                        var deleteRepositoryMetaDataField = this.repositoryDetails.DeleteRepositoryMetaDataField(Convert.ToInt32(repositoryMetaDataFieldId));
                        if (deleteRepositoryMetaDataField != null && deleteRepositoryMetaDataField.RepositoryMetadataFieldId == Convert.ToInt32(repositoryMetaDataFieldId))
                        {
                            deleteResult = true;
                            this.unitOfWork.Commit();
                        }
                    }
                }
                // After deleting check is any remaining repository metadata field,if no repository metadata found delete the repository metadata from table
                var repostiry = this.repositoryDetails.GetRepositoryById(repositoryId);
                if (repostiry != null && repostiry.RepositoryMetadata != null && (repostiry.RepositoryMetadata.FirstOrDefault().RepositoryMetadataFields == null || repostiry.RepositoryMetadata.FirstOrDefault().RepositoryMetadataFields.Count == 0))
                {
                    var repositoryMetaData = this.repositoryDetails.DeleteRepositoryMetaData(repostiry.RepositoryMetadata.FirstOrDefault().RepositoryMetadataId);
                    if (repositoryMetaData != null)
                    {
                        this.unitOfWork.Commit();
                    }
                }
            }
            return deleteResult;
        }

        /// <summary>
        /// Returns the Metadata Types.
        /// </summary>
        /// <returns>Metadata Types.</returns>
        public IEnumerable<MetadataType> GetMetadataTypes()
        {
            return this.repositoryDetails.GetMetadataTypes();
        }

        /// <summary>
        /// Gets all the repositories that allows a file extension.
        /// </summary>
        /// <param name="isAdmin">Is administrator</param>
        /// <param name="fileExtension">File extension.</param>
        /// <returns></returns>
        public IEnumerable<Repository> GetRepositoriesByRoleAndFileExtension(bool isAdmin, string fileExtension)
        {
            List<Repository> filteredRepositories = new List<Repository>();
            IEnumerable<RepositoryDataModel> allRepositories = this.RetrieveRepositories(isAdmin);
            foreach (RepositoryDataModel repositoryDataModel in allRepositories)
            {
                bool allowsFileExtension = Utilities.Helper.CheckFileTypeExists(fileExtension, repositoryDataModel.RepositoryData.AllowedFileTypes, ";");
                if (allowsFileExtension)
                {
                    filteredRepositories.Add(repositoryDataModel.RepositoryData);
                }
            }

            return filteredRepositories;
        }
    }
}
