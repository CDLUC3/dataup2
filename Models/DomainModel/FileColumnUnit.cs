
namespace Microsoft.Research.DataOnboarding.DomainModel
{
    using System;
    using System.Collections.Generic;

    public partial class FileColumnUnit
    {
        public FileColumnUnit()
        {
            //this.FileColumns = new List<FileColumn>();
            // this.FileColumnType = new FileColumnType();
        }

        public int FileColumnUnitId { get; set; }
        public string Name { get; set; }
        public Nullable<int> FileColumnTypeId { get; set; }
        public bool Status { get; set; }
        //public virtual ICollection<FileColumn> FileColumns { get; set; }
        public virtual FileColumnType FileColumnType { get; set; }
    }
}
