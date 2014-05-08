
namespace Microsoft.Research.DataOnboarding.DomainModel
{
    using System;
    using System.Collections.Generic;

    public partial class MetadataType
    {
        public MetadataType()
        {
            //this.RepositoryMetadataFields = new List<RepositoryMetadataField>();
        }

        public int MetadataTypeId { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        //public virtual ICollection<RepositoryMetadataField> RepositoryMetadataFields { get; set; }
    }
}
