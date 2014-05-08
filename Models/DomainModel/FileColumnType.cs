
namespace Microsoft.Research.DataOnboarding.DomainModel
{
    using System;
    using System.Collections.Generic;

    public partial class FileColumnType
    {
        public FileColumnType()
        {
            //this.FileColumns = new List<FileColumn>();
            //this.FileColumnUnits = new List<FileColumnUnit>();
        }

        public int FileColumnTypeId { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        //public virtual ICollection<FileColumn> FileColumns { get; set; }
        //public virtual ICollection<FileColumnUnit> FileColumnUnits { get; set; }
    }
}
