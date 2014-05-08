
using System.Collections.Generic;

namespace Microsoft.Research.DataOnboarding.DomainModel
{
    public partial class QualityCheckColumnType
    {
        public QualityCheckColumnType()
        {
           // this.QualityCheckColumnRules = new List<QualityCheckColumnRule>();
        }

        public int QualityCheckColumnTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<QualityCheckColumnRule> QualityCheckColumnRules { get; set; }
    }
}
