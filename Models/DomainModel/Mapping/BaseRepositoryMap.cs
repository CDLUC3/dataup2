using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.DomainModel.Mapping
{
    public class BaseRepositoryMap : EntityTypeConfiguration<BaseRepository>
    {
        public BaseRepositoryMap()
        {
            // Primary Key
            this.HasKey(t => t.BaseRepositoryId);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("BaseRepositories");
            this.Property(t => t.BaseRepositoryId).HasColumnName("BaseRepositoryId");
            this.Property(t => t.Name).HasColumnName("Name");
        }
    }
}
