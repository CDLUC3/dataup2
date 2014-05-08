
namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;

    public class FileMap : EntityTypeConfiguration<File>
    {
        public FileMap()
        {
            // Primary Key
            this.HasKey(t => t.FileId);

            // Properties
            this.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.MimeType)
                .HasMaxLength(200);

            this.Property(t => t.Citation)
                .HasMaxLength(500);

            this.Property(t => t.Identifier)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("File");
            this.Property(t => t.FileId).HasColumnName("FileId");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RepositoryId).HasColumnName("RepositoryId");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Size).HasColumnName("Size");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.MimeType).HasColumnName("MimeType");
            this.Property(t => t.Citation).HasColumnName("Citation");
            this.Property(t => t.Identifier).HasColumnName("Identifier");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            this.Property(t => t.isDeleted).HasColumnName("isDeleted");

            // Relationships

            this.HasRequired(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.CreatedBy);
        }
    }
}
