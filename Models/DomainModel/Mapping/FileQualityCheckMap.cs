
namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
        using System.ComponentModel.DataAnnotations.Schema;
        using System.Data.Entity.ModelConfiguration;

    public class FileQualityCheckMap : EntityTypeConfiguration<FileQualityCheck>
    {
        public FileQualityCheckMap()
        {
            // Primary Key
            this.HasKey(t => t.FileQualityCheckId);

            // Properties
            // Table & Column Mappings
            this.ToTable("FileQualityCheck");
            this.Property(t => t.FileQualityCheckId).HasColumnName("FileQualityCheckId");
            this.Property(t => t.QualityCheckId).HasColumnName("QualityCheckId");
            this.Property(t => t.FileId).HasColumnName("FileId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.LastRunDateTime).HasColumnName("LastRunDateTime");

            // Relationships
            //this.HasRequired(t => t.File)
            //    .WithMany(t => t.FileQualityChecks)
            //    .HasForeignKey(d => d.FileId);
            //this.HasRequired(t => t.QualityCheck)
            //    .WithMany(t => t.FileQualityChecks)
            //    .HasForeignKey(d => d.QualityCheckId);

        }
    }
}
