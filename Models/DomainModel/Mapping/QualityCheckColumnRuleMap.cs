
namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;

    public class QualityCheckColumnRuleMap : EntityTypeConfiguration<QualityCheckColumnRule>
    {
        public QualityCheckColumnRuleMap()
        {
            // Primary Key
            this.HasKey(t => t.QualityCheckColumnRuleId);

            // Properties
            this.Property(t => t.Description)
                .HasMaxLength(250);

            this.Property(t => t.HeaderName)
                .HasMaxLength(250);

            this.Property(t => t.ErrorMessage)
                .HasMaxLength(500);

            this.Property(t => t.Range)
                .HasMaxLength(25);


            // Table & Column Mappings
            this.ToTable("QualityCheckColumnRule");
            this.Property(t => t.QualityCheckColumnRuleId).HasColumnName("QualityCheckColumnRuleId");
            this.Property(t => t.QualityCheckId).HasColumnName("QualityCheckId");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.HeaderName).HasColumnName("HeaderName");
            this.Property(t => t.Order).HasColumnName("Order");
            this.Property(t => t.QualityCheckColumnTypeId).HasColumnName("QualityCheckColumnTypeId");
            this.Property(t => t.IsRequired).HasColumnName("IsRequired");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.Range).HasColumnName("Range");

            // Relationships
            this.HasRequired(t => t.QualityCheck)
                .WithMany(t => t.QualityCheckColumnRules)
                .HasForeignKey(d => d.QualityCheckId);

        }
    }
}
