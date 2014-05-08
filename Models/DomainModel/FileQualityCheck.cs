
namespace Microsoft.Research.DataOnboarding.DomainModel
{
	using System;
	using System.Collections.Generic;

	public partial class FileQualityCheck
	{
        public FileQualityCheck()
        {
            this.QualityCheck = new QualityCheck();
        }

		public int FileQualityCheckId { get; set; }
		public int QualityCheckId { get; set; }
		public int FileId { get; set; }
		public bool Status { get; set; }
        public Nullable<System.DateTime> LastRunDateTime { get; set; }
		public virtual File File { get; set; }
		public virtual QualityCheck QualityCheck { get; set; }
	}
}
