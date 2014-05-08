using Microsoft.Research.DataOnboarding.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.Research.DataOnboarding.WebApplication.ViewModels
{
    public class RepositoryMetadataFieldViewModel
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="RepositoryMetadataFieldViewModel" /> class.
        /// </summary>
        public RepositoryMetadataFieldViewModel()
        {
            this.RowType = MetaDataRowType.DataRow;
        }

        /// <summary>
        /// Gets or sets the Repository meta data field id
        /// </summary>
        public int RepositoryMetaDataFieldId { get; set; }

        /// <summary>
        /// Gets or sets the Repository meta data id
        /// </summary>
        public int RepositoryMetaDataId { get; set; }

        /// <summary>
        /// Gets or sets the value of 
        /// </summary>
        public string MetadataNodeName { get; set; }

        /// <summary>
        /// Gets or sets the value of Field
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets the value of Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value of MappedLocation
        /// </summary>
        public string MappedLocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsRequired is true or false
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the value of MetaDataType
        /// </summary>
        public string MetaDataTypeId { get; set; }

        /// <summary>
        /// Gets or sets the value of RangeValues
        /// </summary>
        public string RangeValues { get; set; }

        /// <summary>
        /// Gets or sets the value of RowType
        /// </summary>
        public MetaDataRowType RowType { get; set; }

        /// <summary>
        /// Gets or sets the Meta data types
        /// </summary>
        public SelectList MetaDataTypes { get; set; }

        /// <summary>
        /// Gets or sets the blank Row
        /// </summary>
        public bool IsBlankRow { get; set; }
    }
}