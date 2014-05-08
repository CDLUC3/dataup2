
namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
        using System.ComponentModel.DataAnnotations.Schema;
        using System.Data.Entity.ModelConfiguration;

    public class FileColumnTypeMap : EntityTypeConfiguration<FileColumnType>
    {
        public FileColumnTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.FileColumnTypeId);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FileColumnTypes");
            this.Property(t => t.FileColumnTypeId).HasColumnName("FileColumnTypeId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Status).HasColumnName("Status");
        }
    }
}
