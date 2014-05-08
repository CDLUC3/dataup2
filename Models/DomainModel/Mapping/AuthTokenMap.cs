// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Data.Entity.ModelConfiguration;

namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
    public class AuthTokenMap : EntityTypeConfiguration<AuthToken>
    {
        public AuthTokenMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Table & Column Mappings
            this.ToTable("AuthToken");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.RespositoryId).HasColumnName("RepositoryId");
            this.Property(t => t.RefreshToken).HasColumnName("RefreshToken");
            this.Property(t => t.AccessToken).HasColumnName("AccessToken");
            this.Property(t => t.TokenExpiresOn).HasColumnName("TokenExpiresOn");
        }
    }
}
