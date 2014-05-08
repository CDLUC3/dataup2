// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.Utilities.Model;

namespace Microsoft.Research.DataOnboarding.RepositoriesService.Interface
{
    public interface IRepositoryService
    {
        /// <summary>
        ///  Method to retrieve all the available repositories.
        /// </summary>
        /// <param name="includeAdminRepositories">bool indicates if repositories marked as AdminOnly should be returned or not</param>
        /// <returns></returns>
        IEnumerable<RepositoryDataModel> RetrieveRepositories(bool includeAdminRepositories);

        /// <summary>
        /// Method to retrieve all the repository types.
        /// </summary>
        /// <returns>list of repository types.</returns>
        IEnumerable<BaseRepository> RetrieveRepositoryTypes();

        /// <summary>
        /// Method to get the repository for the specific repository id.
        /// </summary>
        /// <param name="repositoryId">Repository id.</param>
        /// <returns>Repository object.</returns>
        Repository GetRepositoryById(int repositoryId);

        /// <summary>
        /// Method to get the repository for the specific repository name.
        /// </summary>
        /// <param name="name">Repository name.</param>
        /// <returns>Repository object.</returns>
        Repository GetRepositoryByName(string name);

        /// <summary>
        /// Method to  add\update the repository details.
        /// </summary>
        /// <param name="repositoryDetails">Repository details.</param>
        /// <returns>Returns true if success else false.</returns>
        bool AddUpdateRepository(Repository repositoryDetails);

        /// <summary>
        /// Method to delete the specific repository.
        /// </summary>
        /// <param name="repositoryId">Repository id.</param>
        /// <returns>Returns true if success else false.</returns>
        bool DeleteRepository(int repositoryId);

        /// <summary>
        /// Method to delete the specific repository metadata fields.
        /// </summary>
        /// <param name="repositoryId">Repository id.</param>
        /// <param name="repositoryMetaDataFields">repositoryMetaDataFields seperated  by commas</param>
        /// <returns>Returns true if success else false.</returns>
        bool DeleteRepositoryMetaDataFields(int repositoryId, string repositoryMetaDataFields);

        /// <summary>
        /// Method to check the duplicate repository name.
        /// </summary>
        /// <param name="repositoryName">Repository name.</param>
        /// <returns>Repository id if exists else 0.</returns>
        int CheckRepositoryExists(string repositoryName);

        /// <summary>
        /// Returns the Metadata Types.
        /// </summary>
        /// <returns>Metadata Types.</returns>
        IEnumerable<MetadataType> GetMetadataTypes();

        /// <summary>
        /// Gets all the repositories that allows a file extension.
        /// </summary>
        /// <param name="isAdmin">Is administrator</param>
        /// <param name="fileExtension">File extension.</param>
        /// <returns></returns>
        IEnumerable<Repository> GetRepositoriesByRoleAndFileExtension(bool isAdmin, string fileExtension);
    }
}
