using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System.Collections.Generic;

namespace Microsoft.Research.DataOnboarding.WebApi.ViewModels
{
    public class ColumnLevelMetadataViewModel
    {
        public ColumnLevelMetadataViewModel()
        {
            this.MetadataList = new List<ColumnLevelMetadata>();
            this.SheetList = new List<FileSheet>();
            this.TypeList = new List<FileColumnType>();
            this.UnitList = new List<FileColumnUnit>();
        }

        public IEnumerable<ColumnLevelMetadata> MetadataList { get; set; }

        public IEnumerable<FileSheet> SheetList { get; set; }

        public IEnumerable<FileColumnType> TypeList { get; set; }

        public IEnumerable<FileColumnUnit> UnitList { get; set; }
    }
}
