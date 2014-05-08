
namespace Microsoft.Research.DataOnboarding.DomainModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    public partial class File
    {
        public File()
        {
            this.FileAttributes = new List<FileAttribute>();
            this.FileColumns = new List<FileColumn>();
            this.FileQualityChecks = new List<FileQualityCheck>();
            this.FileMetadataFields = new List<FileMetadataField>();
        }

        public int FileId { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<int> RepositoryId { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Size { get; set; }
        public string Status { get; set; }
        public string MimeType { get; set; }
        public string Citation { get; set; }
        public string Identifier { get; set; }
        public string BlobId { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<System.DateTime> PublishedOn { get; set; }
        public Nullable<bool> isDeleted { get; set; }
        public short? LifelineInHours { get; set; }

        public virtual User User { get; set; }

        public virtual Repository Repository { get; set; }

        public virtual ICollection<FileAttribute> FileAttributes { get; set; }

        public virtual ICollection<FileColumn> FileColumns { get; set; }

        public virtual ICollection<FileMetadataField> FileMetadataFields { get; set; }

        public virtual ICollection<FileQualityCheck> FileQualityChecks { get; set; }
    }
}
