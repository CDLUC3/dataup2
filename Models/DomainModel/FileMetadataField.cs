using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.DomainModel
{
    public partial class FileMetadataField
    {
        public FileMetadataField()
        {
            //this.RepositoryMetadataField = new RepositoryMetadataField();
        }

        public int FileMetadataFieldId { get; set; }
        public int FileId { get; set; }
        public int RepositoryMetadataFieldId { get; set; }
        public string MetadataValue { get; set; }
       // public virtual File File { get; set; }
        //public virtual RepositoryMetadataField RepositoryMetadataField { get; set; }
    }
}
