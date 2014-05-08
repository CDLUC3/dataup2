
namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
        using System.ComponentModel.DataAnnotations.Schema;
        using System.Data.Entity.ModelConfiguration;

    public class FileColumnUnitMap : EntityTypeConfiguration<FileColumnUnit>
    {
        public FileColumnUnitMap()
        {
            // Primary Key
            this.HasKey(t => t.FileColumnUnitId);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FileColumnUnits");
            this.Property(t => t.FileColumnUnitId).HasColumnName("FileColumnUnitId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.FileColumnTypeId).HasColumnName("FileColumnTypeId");
            this.Property(t => t.Status).HasColumnName("Status");

            // Relationships
            this.HasOptional(t => t.FileColumnType)
                .WithMany()
                .HasForeignKey(d => d.FileColumnTypeId);

        }
    }
}
