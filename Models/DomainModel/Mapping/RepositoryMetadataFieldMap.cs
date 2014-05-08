
namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
        using System.ComponentModel.DataAnnotations.Schema;
        using System.Data.Entity.ModelConfiguration;

    public class RepositoryMetadataFieldMap : EntityTypeConfiguration<RepositoryMetadataField>
    {
        public RepositoryMetadataFieldMap()
        {
            // Primary Key
            this.HasKey(t => t.RepositoryMetadataFieldId);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(50);

            this.Property(t => t.Description)
                .HasMaxLength(200);

            this.Property(t => t.Range)
                .HasMaxLength(20);

            this.Property(t => t.Mapping)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("RepositoryMetadataField");
            this.Property(t => t.RepositoryMetadataFieldId).HasColumnName("RepositoryMetadataFieldId");
            this.Property(t => t.RepositoryMetadataId).HasColumnName("RepositoryMetadataId");
            this.Property(t => t.MetadataTypeId).HasColumnName("MetadataTypeId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.IsRequired).HasColumnName("IsRequired");
            this.Property(t => t.Range).HasColumnName("Range");
            this.Property(t => t.Mapping).HasColumnName("Mapping");
            this.Property(t => t.Order).HasColumnName("Order");

            // Relationships
            //this.HasOptional(t => t.MetadataType)
            //    .WithMany(t => t.RepositoryMetadataFields)
            //    .HasForeignKey(d => d.MetadataTypeId);
            //this.HasRequired(t => t.RepositoryMetadata)
            //    .WithMany(t => t.RepositoryMetadataFields)
            //    .HasForeignKey(d => d.RepositoryMetadataId);

        }
    }
}
