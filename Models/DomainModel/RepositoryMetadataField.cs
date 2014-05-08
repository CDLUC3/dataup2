
namespace Microsoft.Research.DataOnboarding.DomainModel
{
	using System;
	using System.Collections.Generic;

	public partial class RepositoryMetadataField
	{
        public RepositoryMetadataField()
        {
            //this.MetadataType = new MetadataType();
           // this.RepositoryMetadata = new RepositoryMetadata();
        }

		public int RepositoryMetadataFieldId { get; set; }
		public int RepositoryMetadataId { get; set; }
		public Nullable<int> MetadataTypeId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public Nullable<bool> IsRequired { get; set; }
		public string Range { get; set; }
		public string Mapping { get; set; }
		public Nullable<int> Order { get; set; }
        //public virtual MetadataType MetadataType { get; set; }
        //public virtual RepositoryMetadata RepositoryMetadata { get; set; }
	}
}
