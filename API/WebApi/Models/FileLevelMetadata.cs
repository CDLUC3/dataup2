namespace Microsoft.Research.DataOnboarding.WebApi.Models
{
    /// <summary>
    /// Model class for file level metadata.
    /// </summary>
    public class FileLevelMetadata
    {
        /// <summary>
        /// Gets or sets the File Metadata Field Id
        /// </summary>
        public int FileMetadataFieldId { get; set; }

        /// <summary>
        /// Gets or sets the Repository Metadata Field Id
        /// </summary>
        public int RepositoryMetadataFieldId { get; set; }

        /// <summary>
        /// Gets or sets the Repository Metadata Id
        /// </summary>
        public int RepositoryMetadataId { get; set; }

        /// <summary>
        /// Gets or sets the Metadata Type Id
        /// </summary>
        public int? MetaDataTypeId { get; set; }

        /// <summary>
        /// Gets or sets the Field name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the Field Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Data type
        /// </summary>
        public string Datatype { get; set; }

        /// <summary>
        /// Gets or sets the Is required
        /// </summary>
        public bool? IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the Range Values
        /// </summary>
        public float[] RangeValues { get; set; }

        /// <summary>
        /// Gets or sets the Field Value
        /// </summary>
        public string FieldValue { get; set; }
    }
}
