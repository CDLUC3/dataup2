using System.Data.Entity.ModelConfiguration;

namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
    public class QualityCheckColumnTypeMap : EntityTypeConfiguration<QualityCheckColumnType>
    {
        public QualityCheckColumnTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.QualityCheckColumnTypeId);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(50);

            this.Property(t => t.Description)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("QualityCheckColumnTypes");
            this.Property(t => t.QualityCheckColumnTypeId).HasColumnName("QualityCheckColumnTypeId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
        }
    }
}
