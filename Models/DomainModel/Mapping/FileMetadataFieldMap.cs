using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
    public class FileMetadataFieldMap : EntityTypeConfiguration<FileMetadataField>
    {
        public FileMetadataFieldMap()
        {
            // Primary Key
            this.HasKey(t => t.FileMetadataFieldId);

            // Properties
            this.Property(t => t.MetadataValue)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("FileMetadataField");
            this.Property(t => t.FileMetadataFieldId).HasColumnName("FileMetadataFieldId");
            this.Property(t => t.FileId).HasColumnName("FileId");
            this.Property(t => t.RepositoryMetadataFieldId).HasColumnName("RepositoryMetadataFieldId");
            this.Property(t => t.MetadataValue).HasColumnName("MetadataValue");

            // Relationships
            //this.HasRequired(t => t.File)
            //    .WithMany(t => t.FileMetadataFields)
            //    .HasForeignKey(d => d.FileId);
            //this.HasRequired(t => t.RepositoryMetadataField)
            //    .WithMany(t => t.FileMetadataFields)
            //    .HasForeignKey(d => d.RepositoryMetadataFieldId);

        }
    }
}
