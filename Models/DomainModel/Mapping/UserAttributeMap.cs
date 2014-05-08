// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    public class UserAttributeMap : EntityTypeConfiguration<UserAttribute>
    {
        public UserAttributeMap()
        {
            // Primary Key
            this.HasKey(t => t.UserAttributeId);

            // Properties
            this.Property(t => t.Key)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Value)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("UserAttributes");
            this.Property(t => t.UserAttributeId).HasColumnName("UserAttributeId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.Key).HasColumnName("Key");
            this.Property(t => t.Value).HasColumnName("Value");

            // Relationships
            this.HasRequired(t => t.User)
                .WithMany(t => t.UserAttributes)
                .HasForeignKey(d => d.UserId);
        }
    }
}
