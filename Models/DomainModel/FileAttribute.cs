
namespace Microsoft.Research.DataOnboarding.DomainModel
{
	using System;
	using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

	public partial class FileAttribute
	{
        public FileAttribute()
        {
            this.File = new File();
        }

		public int FileAttributeId { get; set; }
		public int FileId { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
        [ForeignKey("FileId")]
		public virtual File File { get; set; }
	}
}
