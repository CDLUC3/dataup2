
namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
        using System.ComponentModel.DataAnnotations.Schema;
        using System.Data.Entity.ModelConfiguration;

    public class FileColumnMap : EntityTypeConfiguration<FileColumn>
    {
        public FileColumnMap()
        {
            // Primary Key
            this.HasKey(t => t.FileColumnId);

            // Properties
            this.Property(t => t.EntityName)
                .HasMaxLength(100);

            this.Property(t => t.EntityDescription)
                .HasMaxLength(1000);

            this.Property(t => t.Name)
                .HasMaxLength(100);

            this.Property(t => t.Description)
                .HasMaxLength(1000);

            // Table & Column Mappings
            this.ToTable("FileColumn");
            this.Property(t => t.FileColumnId).HasColumnName("FileColumnId");
            this.Property(t => t.FileId).HasColumnName("FileId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.EntityName).HasColumnName("EntityName");
            this.Property(t => t.EntityDescription).HasColumnName("EntityDescription");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.FileColumnTypeId).HasColumnName("FileColumnTypeId");
            this.Property(t => t.FileColumnUnitId).HasColumnName("FileColumnUnitId");

            //// Relationships
            this.HasRequired(t => t.File)
                .WithMany(t => t.FileColumns)
                .HasForeignKey(d => d.FileId);
            this.HasOptional(t => t.FileColumnType)
                .WithMany()
                .HasForeignKey(d => d.FileColumnTypeId);
            this.HasOptional(t => t.FileColumnUnit)
                .WithMany()
                .HasForeignKey(d => d.FileColumnUnitId);

        }
    }
}
