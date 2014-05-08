
namespace Microsoft.Research.DataOnboarding.DomainModel
{
	using System;
	using System.Collections.Generic;

	public partial class RepositoryMetadata
	{
		public RepositoryMetadata()
		{
			//this.RepositoryMetadataFields = new List<RepositoryMetadataField>();
            //this.Repository = new Repository();
		}

		public int RepositoryMetadataId { get; set; }
		public int CreatedBy { get; set; }
		public Nullable<int> ModifiedBy { get; set; }
		public int RepositoryId { get; set; }
		public string Name { get; set; }
		public Nullable<bool> IsActive { get; set; }
		public Nullable<System.DateTime> CreatedOn { get; set; }
		public Nullable<System.DateTime> ModifiedOn { get; set; }
		//public virtual Repository Repository { get; set; }
        //public virtual User User { get; set; }
        //public virtual User User1 { get; set; }
		public virtual ICollection<RepositoryMetadataField> RepositoryMetadataFields { get; set; }
	}
}
