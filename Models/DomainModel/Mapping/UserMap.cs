// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            // Primary Key
            this.HasKey(t => t.UserId);

            // Properties
            this.Property(t => t.NameIdentifier)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.IdentityProvider)
                .HasMaxLength(128);

            this.Property(t => t.FirstName)
                .HasMaxLength(64);

            this.Property(t => t.MiddleName)
                .HasMaxLength(64);

            this.Property(t => t.LastName)
                .HasMaxLength(64);

            this.Property(t => t.Organization)
                .HasMaxLength(128);

            this.Property(t => t.EmailId)
                .HasMaxLength(128);

            // Table & Column Mappings
            this.ToTable("User");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.NameIdentifier).HasColumnName("NameIdentifier");
            this.Property(t => t.IdentityProvider).HasColumnName("IdentityProvider");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.MiddleName).HasColumnName("MiddleName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.Organization).HasColumnName("Organization");
            this.Property(t => t.EmailId).HasColumnName("EmailId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
        }
    }
}
