// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.DataAccessService
{
    /// <summary>
    /// Defines contracts for repositories object
    /// </summary>
    public interface IRepositoryDetails
    {
        /// <summary>
        /// Retrieve all the available repositories.
        /// </summary>
        /// <returns>Repositories list.</returns>
        IEnumerable<Repository> RetrieveRepositories();

        /// <summary>
        /// Method to get the repository by id.
        /// </summary>
        /// <param name="repositoryId">Repository id.</param>
        /// <returns>Repository object.</returns>
        Repository GetRepositoryById(int repositoryId);

        /// <summary>
        /// Retrieve all the available repository types.
        /// </summary>
        /// <returns>Repository types list.</returns>
        IEnumerable<BaseRepository> RetrieveRepositoryTypes();

        /// <summary>
        /// Method to add new repository record.
        /// </summary>
        /// <param name="repository">Repository details.</param>
        /// <returns>Newly created repository.</returns>
        /// <exception cref="ArgumentException">When repository is null</exception>
        Repository AddRepository(Repository repository);

        /// <summary>
        /// Method to add repository metadata fields
        /// </summary>
        /// <param name="repositoryMetadata">repositry metadata data</param>
        /// <returns>returns repository metadata </returns>
        RepositoryMetadata SaveRepositoryMetaData(RepositoryMetadata repositoryMetadata);

        /// <summary>
        /// Method to add repository metadata fields
        /// </summary>
        /// <param name="RepositoryMetadataField">repositry metadata fields</param>
        /// <returns>returns repository metadata fields</returns>
        RepositoryMetadataField SaveRepositoryMetaDataField(RepositoryMetadataField repositoryMetadataField);

        /// <summary>
        /// Method to update the repository.
        /// </summary>
        /// <param name="repository">Repository details.</param>
        /// <returns>Updated repository.</returns>
        /// <exception cref="ArgumentException">When repository is null</exception>
        Repository UpdateRepository(Repository repository);

        /// <summary>
        /// Method to get the base repository name for the specific 
        /// </summary>
        /// <param name="baseRepositoryId"></param>
        /// <returns></returns>
        string GetBaseRepositoryName(int baseRepositoryId);

        /// <summary>
        /// Method to get the repository by its name.
        /// </summary>
        /// <param name="repositoryName">Repository name.</param>
        /// <returns>Selected repository object.</returns>
        Repository GetRepositoryByName(string repositoryName);

        /// <summary>
        /// Method to delete the selected repository.
        /// </summary>
        /// <param name="repositoryId">Repository id.</param>
        /// <returns>Deleted repository.</returns>
        Repository DeleteRepository(int repositoryId);

        /// <summary>
        /// Method to delete the selected repositoryMetaDataField.
        /// </summary>
        /// <param name="repositoryMetaDataField">repositoryMetaDataField</param>
        /// <returns>Deleted repository metadata field.</returns>
        RepositoryMetadataField DeleteRepositoryMetaDataField(int repositoryMetaDataField);

        /// <summary>
        /// Method to delete repostiroy metadata
        /// </summary>
        /// <param name="repositoryMetaDataId">Reposiotory metadata</param>
        /// <returns>Deleted repository metadata</returns>
        RepositoryMetadata DeleteRepositoryMetaData(int repositoryMetaDataId);
        
        ///// <summary>
        ///// Method to get the repository type for the given type id.
        ///// </summary>
        ///// <param name="repositoryTypeId">Repository type id.</param>
        ///// <returns>Selected repository type.</returns>
        //RepositoryType GetRepositoryTypeById(int repositoryTypeId);

        /// <summary>
        /// Returns the Metadata Types.
        /// </summary>
        /// <returns>Metadata Types.</returns>
        IEnumerable<MetadataType> GetMetadataTypes();
    }
}
