
namespace Microsoft.Research.DataOnboarding.DomainModel
{
    using System;
    using System.Collections.Generic;

    public partial class QualityCheckColumnRule
    {
        public QualityCheckColumnRule()
        {
            // this.QualityCheck = new QualityCheck();
        }

        public int QualityCheckColumnRuleId { get; set; }
        public int QualityCheckId { get; set; }
        public string Description { get; set; }
        public string HeaderName { get; set; }
        public Nullable<bool> IsRequired { get; set; }
        public string ErrorMessage { get; set; }
        public int QualityCheckColumnTypeId { get; set; }
        public int Order { get; set; }
        public string Range { get; set; }
        public Nullable<bool> IsActive { get; set; }

        public virtual QualityCheck QualityCheck { get; set; }
    }
}
