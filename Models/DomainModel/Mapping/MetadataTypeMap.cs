
namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
        using System.ComponentModel.DataAnnotations.Schema;
        using System.Data.Entity.ModelConfiguration;

    public class MetadataTypeMap : EntityTypeConfiguration<MetadataType>
    {
        public MetadataTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.MetadataTypeId);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("MetadataTypes");
            this.Property(t => t.MetadataTypeId).HasColumnName("MetadataTypeId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Status).HasColumnName("Status");
        }
    }
}
