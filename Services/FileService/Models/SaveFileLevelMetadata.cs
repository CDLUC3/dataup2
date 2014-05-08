namespace Microsoft.Research.DataOnboarding.FileService.Models
{
    public class SaveFileLevelMetadata
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
        /// Gets or sets the Field Value
        /// </summary>
        public string FieldValue { get; set; }
    }
}
