// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.DataAccessService.Providers.EntityFramework
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using Microsoft.Research.DataOnboarding.DataAccessService;
    using Microsoft.Research.DataOnboarding.DomainModel;

    /// <summary>
    /// Defines the entity framework context that should be 
    /// implemented by the repositories
    /// </summary>
    public interface IDataOnboardingContext : IUnitOfWork
    {
        IDbSet<User> Users { get; }
        IDbSet<UserAttribute> UserAttributes { get; }
        IDbSet<Role> Roles { get; }
        IDbSet<UserRole> UserRoles { get; }
        IDbSet<File> Files { get; }
        IDbSet<FileAttribute> FileAttributes { get; }
        IDbSet<FileColumn> FileColumns { get; }      
        IDbSet<FileMetadataField> FileMetadataFields { get; }
        IDbSet<FileColumnType> FileColumnTypes { get; }
        IDbSet<FileColumnUnit> FileColumnUnits { get; }
        // IDbSet<FileRepository> FileRepositories { get; }
        IDbSet<Repository> Repositories { get; }
        IDbSet<RepositoryMetadata> RepositoryMetadata { get; }
        IDbSet<RepositoryMetadataField> RepositoryMetadataFields { get; }
        IDbSet<MetadataType> MetadataTypes { get; }
     
        //IDbSet<RepositoryType> RepositoryTypes { get; }
        IDbSet<BaseRepository> BaseRepositories { get; }

        IDbSet<QualityCheck> QualityChecks { get; }
        IDbSet<QualityCheckColumnRule> QualityCheckColumnRules { get; }
        IDbSet<FileQualityCheck> FileQualityChecks { get; }
        IDbSet<QualityCheckColumnType> QualityCheckColumnTypes { get; }

        IDbSet<AuthToken> AuthTokens { get; set; }
        /// <summary>
        /// Generic method definition to set <see cref="EntityState"/> of the entity object
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity  whose state should be set</param>
        /// <param name="state">Sate value</param>
        /// <exception cref="ArgumentNullException">When entity is null</exception>
        /// <remarks>
        /// This method is intended to hide the <see cref="DbEntityEntry"/> implementation
        /// from the clients. This also helps in writing Fake implementation of the interface
        /// for unit tests. In future may expose the underpinnings by changing the method definition
        /// to:
        /// <![CDATA[DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;]]>
        /// </remarks>
        void SetEntityState<TEntity>(TEntity entity, EntityState state) where TEntity : class;

        /// <summary>
        /// Sets the state of an entry being tracked in the data context
        /// </summary>
        /// <param name="entry">Object entry</param>
        /// <param name="state">State value to set</param>
        /// <exception cref="ArgumentNullException">When entry is null</exception>
        /// <remarks>
        /// This method is intended to hide the <see cref="DbEntityEntry"/> from clients. This also 
        /// helps in writing Fake implementation of the interface for unit tests.
        /// </remarks>
        void SetEntityState(object entry, EntityState state);

        /// <summary>
        /// Generic method definition to get entity state of a tracked object in the data context
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity instance</param>
        /// <returns>State of the entity in the data context as <see cref="EntityState"/></returns>
        /// <exception cref="ArgumentNullException">When entity is null</exception>
        /// <remarks>
        /// This method is intended to hide the <see cref="DbEntityEntry"/> from clients. This also 
        /// helps in writing Fake implementation of the interface for unit tests.
        /// </remarks>
        EntityState GetEntityState<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Gets the state of an entry in the data context
        /// </summary>
        /// <param name="entry">Entry in data context</param>
        /// <returns>State of the entity <see cref="EntityState"/></returns>
        /// <exception cref="ArgumentNullException">When entry is null</exception>
        /// <remarks>
        /// This method is intended to hide the <see cref="DbEntityEntry"/> from clients. This also 
        /// helps in writing Fake implementation of the interface for unit tests.
        /// </remarks>
        /// <exception cref="DbEntityValidationException">
        /// When entity validation failes for one more entities in the tracked list in data context
        /// </exception>
        EntityState GetEntityState(object entry);

        /// <summary>
        /// Gets the entry in the data context
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <param name="entity">Entity instance</param>
        /// <returns>Entry in datacontext</returns>
        /// <exception cref="ArgumentNullException">When entity is null</exception>
        /// <remarks>
        /// This method is intended to hide the <see cref="DbEntityEntry"/> from clients. This also 
        /// helps in writing Fake implementation of the interface for unit tests.
        /// </remarks>
        DbEntityEntry<TEntity> GetEntry<TEntity>(TEntity entity) where TEntity : class;
    }
}
