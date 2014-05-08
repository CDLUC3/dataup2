
namespace Microsoft.Research.DataOnboarding.DomainModel
{
    using System;
    using System.Collections.Generic;

    public partial class QualityCheck
    {
        public QualityCheck()
        {
            //this.FileQualityChecks = new List<FileQualityCheck>();
            //this.QualityCheckColumnRules = new List<QualityCheckColumnRule>();
        }

        public int QualityCheckId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsVisibleToAll { get; set; }
        public Nullable<bool> EnforceOrder { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }


        public virtual ICollection<FileQualityCheck> FileQualityChecks { get; set; }
        public virtual ICollection<QualityCheckColumnRule> QualityCheckColumnRules { get; set; }
        //public virtual User CreatedUser { get; set; }
    }
}
