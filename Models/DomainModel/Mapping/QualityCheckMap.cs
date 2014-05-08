
namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;

    public class QualityCheckMap : EntityTypeConfiguration<QualityCheck>
    {
        public QualityCheckMap()
        {
            // Primary Key
            this.HasKey(t => t.QualityCheckId);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(100);

            this.Property(t => t.Description)
                .HasMaxLength(1000);

            // Table & Column Mappings
            this.ToTable("QualityCheck");
            this.Property(t => t.QualityCheckId).HasColumnName("QualityCheckId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            this.Property(t => t.IsVisibleToAll).HasColumnName("IsVisibleToAll");
            this.Property(t => t.EnforceOrder).HasColumnName("EnforceOrder");

            // Relationships
            //this.HasRequired(t => t.CreatedUser)
            //    .WithMany(t => t.QualityChecks)
            //    .HasForeignKey(d => d.CreatedBy);
        }
    }
}
