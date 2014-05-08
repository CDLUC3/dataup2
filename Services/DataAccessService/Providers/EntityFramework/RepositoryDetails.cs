// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.DomainModel;
using System;

namespace Microsoft.Research.DataOnboarding.DataAccessService.Providers.EntityFramework
{
    /// <summary>
    /// Implements repository details <see cref="IRepositoryDetails"/> leveraging
    /// entity framework
    /// </summary>
    public class RepositoryDetails : RepositoryBase, IRepositoryDetails
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryDetails"/> class.
        /// </summary>
        /// <param name="dataContext">Data context.</param>
        public RepositoryDetails(IUnitOfWork dataContext)
            : base(dataContext)
        {
        }

        #endregion

        #region IRepositoryDetails implementation

        /// <summary>
        /// Retrieve all the available repositories.
        /// </summary>
        /// <returns>Repositories list.</returns>
        public IEnumerable<Repository> RetrieveRepositories()
        {
            return Context.Repositories.OrderByDescending(rep => rep.CreatedOn);
        }

        /// <summary>
        /// Retrieve all the available repository types.
        /// </summary>
        /// <returns>List of repository types.</returns>
        public IEnumerable<BaseRepository> RetrieveRepositoryTypes()
        {
            return Context.BaseRepositories;
        }

        /// <summary>
        /// Method to add new repository record.
        /// </summary>
        /// <param name="repository">Repository details.</param>
        /// <returns>Newly created repository.</returns>
        /// <exception cref="ArgumentException">When repository is null</exception>
        public Repository AddRepository(Repository repository)
        {
            Check.IsNotNull<Repository>(repository, "newRepository");

            return Context.Repositories.Add(repository);
        }

        public RepositoryMetadata SaveRepositoryMetaData(RepositoryMetadata repositoryMetadata)
        {
            if (repositoryMetadata.RepositoryMetadataId > 0)
            {
                foreach (var repositoryMetaDataField in repositoryMetadata.RepositoryMetadataFields)
                {
                    SaveRepositoryMetaDataField(repositoryMetaDataField);
                }

                RepositoryMetadata updatedRepositoryMetaData = Context.RepositoryMetadata.Attach(repositoryMetadata);

                Context.SetEntityState<RepositoryMetadata>(updatedRepositoryMetaData, EntityState.Modified);

                return updatedRepositoryMetaData;
            }
            else
            {
                return Context.RepositoryMetadata.Add(repositoryMetadata);
            }
        }

        public RepositoryMetadataField SaveRepositoryMetaDataField(RepositoryMetadataField repositoryMetadataField)
        {
            if (repositoryMetadataField.RepositoryMetadataFieldId > 0)
            {
                var updatedRepositoryMetaDataField = Context.RepositoryMetadataFields.Attach(repositoryMetadataField);

                Context.SetEntityState<RepositoryMetadataField>(updatedRepositoryMetaDataField, EntityState.Modified);

                return updatedRepositoryMetaDataField;
            }
            else
            {
                return Context.RepositoryMetadataFields.Add(repositoryMetadataField);
            }
        }

        /// <summary>
        /// Method to update the repository.
        /// </summary>
        /// <param name="repository">Repository details.</param>
        /// <returns>Updated repository.</returns>
        /// <exception cref="ArgumentException">When repository is null</exception>
        public Repository UpdateRepository(Repository repository)
        {
            Check.IsNotNull<Repository>(repository, "modifiedRepository");
            Repository updatedRepository = Context.Repositories.Attach(repository);

            
            Context.SetEntityState<Repository>(updatedRepository, EntityState.Modified);


            return updatedRepository;
        }

        /// <summary>
        /// Method to get the repository by id.
        /// </summary>
        /// <param name="repositoryId">Repository id.</param>
        /// <returns>Repository object.</returns>
        public Repository GetRepositoryById(int repositoryId)
        {
            return Context.Repositories
                .Include(r => r.BaseRepository)
                .Include(r=> r.RepositoryMetadata.Select(m => m.RepositoryMetadataFields))
                .Where(rep => rep.RepositoryId == repositoryId).FirstOrDefault();
        }

        /// <summary>
        /// Method to get the base repository name for the specific 
        /// </summary>
        /// <param name="baseRepositoryId"></param>
        /// <returns></returns>
        public string GetBaseRepositoryName(int baseRepositoryId)
        {
            string baseRepositoryName = string.Empty;
            var selBaseRepository = Context.BaseRepositories.Where(baseRep => baseRep.BaseRepositoryId == baseRepositoryId).FirstOrDefault();

            if (selBaseRepository != null)
            {
                baseRepositoryName = selBaseRepository.Name;
            }

            return baseRepositoryName;
        }


        /// <summary>
        /// Method to delete the selected repository.
        /// </summary>
        /// <param name="repositoryId">Repository id.</param>
        /// <returns>Deleted repository.</returns>
        public Repository DeleteRepository(int repositoryId)
        {
            var repositoryToDelete = Context.Repositories.Where(rep => rep.RepositoryId == repositoryId).FirstOrDefault();

            var repMetadataToDelList = Context.RepositoryMetadata
                .Include(rm => rm.RepositoryMetadataFields)
                .Where(repMet => repMet.RepositoryId == repositoryId).ToList();
            List<AuthToken> authTokensToDelete = Context.AuthTokens.Where(authToken => authToken.RespositoryId == repositoryId).ToList();

            var repositoryMetadataFieldDeleteList = new List<RepositoryMetadataField>();
            var repositoryMetadataDeleteList = new List<RepositoryMetadata>();
            var fileMetadataFieldDeleteList = new List<FileMetadataField>();

            // add the objects that are required for deletion
            foreach (var repositoryMetaData in repMetadataToDelList)
            {
                foreach (var repositoryMetaDataField in repositoryMetaData.RepositoryMetadataFields)
                {
                    var existingFileMetaDatafields = Context.FileMetadataFields.Where(fmf => fmf.RepositoryMetadataFieldId == repositoryMetaDataField.RepositoryMetadataFieldId).ToList();
                    foreach (var fileMetaData in existingFileMetaDatafields)
                    {
                        fileMetadataFieldDeleteList.Add(fileMetaData);
                    }
                    repositoryMetadataFieldDeleteList.Add(repositoryMetaDataField);
                }
                repositoryMetadataDeleteList.Add(repositoryMetaData);
            }

            // delete the file metadata fields associated to repository metadata field
            foreach (var fileMetaDataField in fileMetadataFieldDeleteList)
            {
                Context.SetEntityState<FileMetadataField>(fileMetaDataField, EntityState.Deleted);
                Context.FileMetadataFields.Remove(fileMetaDataField);
            }
            
            // delete the repository metadata field list
            foreach (var repositoryMetaDataField in repositoryMetadataFieldDeleteList)
            {
                Context.SetEntityState<RepositoryMetadataField>(repositoryMetaDataField, EntityState.Deleted);
                Context.RepositoryMetadataFields.Remove(repositoryMetaDataField);
            }

            // delete the repository metadata
            foreach (var repositoryMetaData in repositoryMetadataDeleteList)
            {
                Context.SetEntityState<RepositoryMetadata>(repositoryMetaData, EntityState.Deleted);
                Context.RepositoryMetadata.Remove(repositoryMetaData);
            }

            // delete AuthTokens related to Repository
            foreach (AuthToken authToken in authTokensToDelete)
            {
                Context.AuthTokens.Remove(authToken);
            }

            Context.SetEntityState<Repository>(repositoryToDelete, EntityState.Deleted);
            return Context.Repositories.Remove(repositoryToDelete);
        }

        ///// <summary>
        ///// Method to get the repository type for the given type id.
        ///// </summary>
        ///// <param name="repositoryTypeId">Repository type id.</param>
        ///// <returns>Selected repository type.</returns>
        //public RepositoryType GetRepositoryTypeById(int repositoryTypeId)
        //{
        //    return Context.RepositoryTypes.Where(rep => rep.RepositoryTypeId == repositoryTypeId).FirstOrDefault();
        //}

        /// <summary>
        /// Method to get the repository by its name.
        /// </summary>
        /// <param name="repositoryName">Repository name.</param>
        /// <returns>Selected repository object.</returns>
        public Repository GetRepositoryByName(string repositoryName)
        {
            Check.IsNotNull<string>(repositoryName, "repositoryName");
            return Context.Repositories
                    .Include(r => r.BaseRepository)
                    .Include(r => r.RepositoryMetadata.Select(m => m.RepositoryMetadataFields))
                    .Where(rep => rep.Name.Equals(repositoryName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }

        /// <summary>
        /// Method to delete repository meta data fields
        /// </summary>
        /// <param name="repositoryMetaDataField">repositoryMetaData Field</param>
        /// <returns>returns the deleted repository metadata field</returns>
        public RepositoryMetadataField DeleteRepositoryMetaDataField(int repositoryMetaDataField)
        {
            //before deleting the repository metadata field ,remove the file metadata field
            var fileMetaDataFields = Context.FileMetadataFields.Where(fmd => fmd.RepositoryMetadataFieldId == repositoryMetaDataField).ToList();
            foreach (var fileMetaDataField in fileMetaDataFields)
            {
                Context.SetEntityState<FileMetadataField>(fileMetaDataField, EntityState.Deleted);
                Context.FileMetadataFields.Remove(fileMetaDataField);
            }
            var delRepMetadataField = Context.RepositoryMetadataFields.Where(repMetFields => repMetFields.RepositoryMetadataFieldId == repositoryMetaDataField).FirstOrDefault();
            if (delRepMetadataField != null && delRepMetadataField.RepositoryMetadataFieldId > 0)
            {
                Context.SetEntityState<RepositoryMetadataField>(delRepMetadataField, EntityState.Deleted);
                return Context.RepositoryMetadataFields.Remove(delRepMetadataField);
            }
            //dummy return 
            return new RepositoryMetadataField();
        }

        /// <summary>
        /// Method to delete repository metadata
        /// </summary>
        /// <param name="repositoryMetaDataId">repository MetaDataId</param>
        /// <returns>returns the repository metadata</returns>
        public RepositoryMetadata DeleteRepositoryMetaData(int repositoryMetaDataId)
        {
            var repositoryMetaData = Context.RepositoryMetadata.Where(rm => rm.RepositoryMetadataId == repositoryMetaDataId).First();
            Context.SetEntityState<RepositoryMetadata>(repositoryMetaData, EntityState.Deleted);
            return Context.RepositoryMetadata.Remove(repositoryMetaData);
        }

        /// <summary>
        /// Returns trhe MetadataTypes.
        /// </summary>
        /// <returns>Metadata Types</returns>
        public IEnumerable<MetadataType> GetMetadataTypes()
        {
            return Context.MetadataTypes;
        }

        #endregion
    }
}
