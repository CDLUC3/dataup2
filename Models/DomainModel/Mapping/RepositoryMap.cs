
namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;

    public class RepositoryMap : EntityTypeConfiguration<Repository>
    {
        public RepositoryMap()
        {
            // Primary Key
            this.HasKey(t => t.RepositoryId);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.ImpersonatingUserName)
                .HasMaxLength(100);

            this.Property(t => t.ImpersonatingPassword)
                .HasMaxLength(100);

            this.Property(t => t.UserAgreement)
                .HasMaxLength(2048);

            this.Property(t => t.HttpGetUriTemplate)
                .HasMaxLength(250);

            this.Property(t => t.HttpPostUriTemplate)
                .HasMaxLength(250);

            this.Property(t => t.HttpDeleteUriTemplate)
                .HasMaxLength(250);

            this.Property(t => t.HttpIdentifierUriTemplate)
                .HasMaxLength(250);

            this.Property(t => t.AllowedFileTypes)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Repository");
            this.Property(t => t.RepositoryId).HasColumnName("RepositoryId");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.IsImpersonating).HasColumnName("IsImpersonating");
            this.Property(t => t.ImpersonatingUserName).HasColumnName("ImpersonatingUserName");
            this.Property(t => t.ImpersonatingPassword).HasColumnName("ImpersonatingPassword");
            this.Property(t => t.UserAgreement).HasColumnName("UserAgreement");
            this.Property(t => t.HttpGetUriTemplate).HasColumnName("HttpGetUriTemplate");
            this.Property(t => t.HttpPostUriTemplate).HasColumnName("HttpPostUriTemplate");
            this.Property(t => t.HttpDeleteUriTemplate).HasColumnName("HttpDeleteUriTemplate");
            this.Property(t => t.HttpIdentifierUriTemplate).HasColumnName("HttpIdentifierUriTemplate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.AllowedFileTypes).HasColumnName("AllowedFileTypes");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            this.Property(t => t.BaseRepositoryId).HasColumnName("BaseRepositoryId");
            this.Property(t => t.IsVisibleToAll).HasColumnName("IsVisibleToAll");
            this.Property(t => t.AccessToken).HasColumnName("AccessToken");
            this.Property(t => t.RefreshToken).HasColumnName("RefreshToken");
            this.Property(t => t.TokenExpiresOn).HasColumnName("TokenExpiresOn");

            // Relationships    
            //this.HasRequired(t => t.RepositoryType)
            //    .WithMany(t => t.Repositories)
            //    .HasForeignKey(d => d.RepositoryTypeId);
            //this.HasRequired(t => t.BaseRepository)
            //  .WithMany(t => t.Repositories)
            //  .HasForeignKey(d => d.BaseRepositoryId);

            //this.HasRequired(t => t.User)
            //    .WithMany(t => t.Repositories)
            //    .HasForeignKey(d => d.CreatedBy);
        }
    }
}
