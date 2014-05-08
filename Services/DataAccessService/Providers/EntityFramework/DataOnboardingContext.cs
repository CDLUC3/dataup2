// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.DomainModel.Mapping;
using Microsoft.Research.DataOnboarding.Utilities;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace Microsoft.Research.DataOnboarding.DataAccessService.Providers.EntityFramework
{
    /// <summary>
    /// Adds unit of work implementation to the partial implementation of 
    /// the data onboarding context. 
    /// </summary>
    public class DataOnboardingContext : DbContext, IDataOnboardingContext
    {
        #region Constructors

        public DataOnboardingContext()
        {
            Database.SetInitializer<DataOnboardingContext>(null);
        }

        public DataOnboardingContext(string name)
            : base(name)
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        #endregion

        #region IDataOnboardingContext implementation

        /// <summary>
        /// Gets or sets the Roles
        /// </summary>
        public IDbSet<Role> Roles
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Users
        /// </summary>
        public IDbSet<User> Users
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the UserAttributes
        /// </summary>
        public IDbSet<UserAttribute> UserAttributes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the UserRoles
        /// </summary>
        public IDbSet<UserRole> UserRoles
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Roles
        /// </summary>
        public IDbSet<File> Files
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the FileAttributes
        /// </summary>
        public IDbSet<FileAttribute> FileAttributes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the FileColumns
        /// </summary>
        public IDbSet<FileColumn> FileColumns
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the FileMetadataFields
        /// </summary>
        public IDbSet<FileMetadataField> FileMetadataFields
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the FileColumnTypes
        /// </summary>
        public IDbSet<FileColumnType> FileColumnTypes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the FileColumnUnits
        /// </summary>
        public IDbSet<FileColumnUnit> FileColumnUnits
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Repositories
        /// </summary>
        public IDbSet<Repository> Repositories
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the BaseRepositories
        /// </summary>
        public IDbSet<BaseRepository> BaseRepositories
        {
            get;
            set;
        }

        ///// <summary>
        ///// Gets or sets the RepositoryTypes
        ///// </summary>
        //public IDbSet<RepositoryType> RepositoryTypes
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Gets or sets the Repository Metadata
        /// </summary>
        public IDbSet<RepositoryMetadata> RepositoryMetadata
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Repository Metadata fields
        /// </summary>
        public IDbSet<RepositoryMetadataField> RepositoryMetadataFields
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the Metadata types
        /// </summary>
        public IDbSet<MetadataType> MetadataTypes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the QualityChecks
        /// </summary>
        public IDbSet<QualityCheck> QualityChecks
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Quality Check Column Rules
        /// </summary>
        public IDbSet<QualityCheckColumnRule> QualityCheckColumnRules
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the FileQualityChecks
        /// </summary>
        public IDbSet<FileQualityCheck> FileQualityChecks
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Quality Check Column Type
        /// </summary>
        public IDbSet<QualityCheckColumnType> QualityCheckColumnTypes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the UserAuthTokens
        /// </summary>
        public IDbSet<AuthToken> AuthTokens
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the Entity State Generic method
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <param name="entity">Entity Instance</param>
        /// <param name="state">State of the entity</param>
        public void SetEntityState<TEntity>(TEntity entity, EntityState state) where TEntity : class
        {
            Check.IsNotNull<TEntity>(entity, "entity");
            this.Entry<TEntity>(entity).State = state;
        }

        /// <summary>
        /// Sets the Entity State
        /// </summary>
        /// <param name="entity">Entity Instance</param>
        /// <param name="state">State of the entity</param>
        public void SetEntityState(object entry, EntityState state)
        {
            Check.IsNotNull(entry, "entry");
            this.Entry(entry).State = state;
        }

        /// <summary>
        /// Returns the entity state.Generic Method
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <param name="entity">Entity Instance</param>
        /// <returns>Entity State</returns>
        public EntityState GetEntityState<TEntity>(TEntity entity) where TEntity : class
        {
            Check.IsNotNull<TEntity>(entity, "entity");
            return this.Entry<TEntity>(entity).State;
        }

        /// <summary>
        /// Returns the entity state
        /// </summary>
        /// <param name="entity">Entity Instance</param>
        /// <returns>Entity State</returns>
        public EntityState GetEntityState(object entry)
        {
            Check.IsNotNull(entry, "entry");
            return this.Entry(entry).State;
        }

        /// <summary>
        /// Returns the Entry
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <param name="entity">Entity Instance</param>
        /// <returns>Entry from the context</returns>
        public DbEntityEntry<TEntity> GetEntry<TEntity>(TEntity entity) where TEntity : class
        {
            Check.IsNotNull<TEntity>(entity, "entity");
            return this.Entry<TEntity>(entity);
        }

        /// <summary>
        /// This method wraps up the exceptions thrown while saving changes to the 
        /// data context in <see cref="UnitOfWorkException"/>
        /// </summary>
        public void Commit()
        {
            try
            {
                this.SaveChanges();
            }
            catch (DbEntityValidationException dbeve)
            {
                throw new UnitOfWorkCommitException("Entity validation failed", dbeve);
            }
            catch (DbUnexpectedValidationException dbuve)
            {
                throw new UnitOfWorkCommitException("Entity validation failed", dbuve);
            }
            catch (DBConcurrencyException dbce)
            {
                throw new UnitOfWorkCommitException("Concurrency exception while updating data context", dbce);
            }
            catch (EntityException ee)
            {
                throw new UnitOfWorkCommitException("Concurrent transactions are not allowed", ee);
            }
        }

        #endregion

        #region DbContext Overrides to Build Model

        /// <summary>
        /// This override hooks up the onboarding data model into the Entity framework
        /// model builder configuration
        /// </summary>
        /// <param name="modelBuilder">Entity framework model builder</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validating with Check helper class")]
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Check.IsNotNull<DbModelBuilder>(modelBuilder, "modelBuilder");
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new UserAttributeMap());
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new UserRoleMap());
            modelBuilder.Configurations.Add(new FileMap());
            modelBuilder.Configurations.Add(new FileAttributeMap());
            modelBuilder.Configurations.Add(new FileColumnMap());
            modelBuilder.Configurations.Add(new FileMetadataFieldMap());
            modelBuilder.Configurations.Add(new FileColumnUnitMap());
            modelBuilder.Configurations.Add(new FileColumnTypeMap());
            modelBuilder.Configurations.Add(new RepositoryMap());
            modelBuilder.Configurations.Add(new RepositoryMetadataFieldMap());
            modelBuilder.Configurations.Add(new RepositoryMetadataMap());
           // modelBuilder.Configurations.Add(new RepositoryTypeMap());
            modelBuilder.Configurations.Add(new MetadataTypeMap());
            modelBuilder.Configurations.Add(new BaseRepositoryMap());
            modelBuilder.Configurations.Add(new QualityCheckMap());
            modelBuilder.Configurations.Add(new QualityCheckColumnRuleMap());
            modelBuilder.Configurations.Add(new FileQualityCheckMap());
            modelBuilder.Configurations.Add(new QualityCheckColumnTypeMap());
            modelBuilder.Configurations.Add(new AuthTokenMap());
        }

        #endregion
    }
}
