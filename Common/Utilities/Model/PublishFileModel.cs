using Microsoft.Research.DataOnboarding.DomainModel;
using System.Collections.Generic;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    public class PublishFileModel
    {
        public DataFile File {get;set;}
        public IEnumerable<FileColumnType> FileColumnTypes { get; set; }
        public IEnumerable<FileColumnUnit> FileColumnUnits { get; set; } 
    }
}