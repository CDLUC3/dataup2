
namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
        using System.ComponentModel.DataAnnotations.Schema;
        using System.Data.Entity.ModelConfiguration;

    public class FileAttributeMap : EntityTypeConfiguration<FileAttribute>
    {
        public FileAttributeMap()
        {
            // Primary Key
            this.HasKey(t => t.FileAttributeId);

            // Properties
            this.Property(t => t.Key)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Value)
                .IsRequired()
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("FileAttributes");
            this.Property(t => t.FileAttributeId).HasColumnName("FileAttributeId");
            this.Property(t => t.FileId).HasColumnName("FileId");
            this.Property(t => t.Key).HasColumnName("Key");
            this.Property(t => t.Value).HasColumnName("Value");

            // Relationships
            //this.HasRequired(t => t.File)
            //    .WithMany(t => t.FileAttributes)
            //    .HasForeignKey(d => d.FileId);

        }
    }
}
