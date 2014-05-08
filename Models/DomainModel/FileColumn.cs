
namespace Microsoft.Research.DataOnboarding.DomainModel
{
    using System;
    using System.Collections.Generic;

    public partial class FileColumn
    {
        public FileColumn()
        {
            //this.File = new File();
            //this.FileColumnType = new FileColumnType();
            //this.FileColumnUnit = new FileColumnUnit();
            //FileColumnTypeId = 0;
            //FileColumnUnitId = 0;
        }

        public int FileColumnId { get; set; }
        public int FileId { get; set; }
        public bool Status { get; set; }
        public string EntityName { get; set; }
        public string EntityDescription { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> FileColumnTypeId { get; set; }
        public Nullable<int> FileColumnUnitId { get; set; }
        public virtual File File { get; set; }
        public virtual FileColumnType FileColumnType { get; set; }
        public virtual FileColumnUnit FileColumnUnit { get; set; }
    }
}
