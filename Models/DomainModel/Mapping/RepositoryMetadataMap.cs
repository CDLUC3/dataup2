
namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
        using System.ComponentModel.DataAnnotations.Schema;
        using System.Data.Entity.ModelConfiguration;

    public class RepositoryMetadataMap : EntityTypeConfiguration<RepositoryMetadata>
    {
        public RepositoryMetadataMap()
        {
            // Primary Key
            this.HasKey(t => t.RepositoryMetadataId);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("RepositoryMetadata");
            this.Property(t => t.RepositoryMetadataId).HasColumnName("RepositoryMetadataId");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RepositoryId).HasColumnName("RepositoryId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");

            // Relationships
            //this.HasRequired(t => t.Repository)
            //    .WithMany(t => t.RepositoryMetadata)
            //    .HasForeignKey(d => d.RepositoryId);
            //this.HasRequired(t => t.User)
            //    .WithMany(t => t.RepositoryMetadataCollection)
            //    .HasForeignKey(d => d.CreatedBy);
        }
    }
}
