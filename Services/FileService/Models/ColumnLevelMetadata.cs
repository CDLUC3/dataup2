namespace Microsoft.Research.DataOnboarding.FileService.Models
{
    /// <summary>
    /// Model class for column level metadata
    /// </summary>
    public class ColumnLevelMetadata
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets SelectedEntityName.
        /// </summary>
        public string SelectedEntityName { get; set; }

        /// <summary>
        /// Gets or sets EntityDescription.
        /// </summary>
        public string EntityDescription { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets SelectedTypeId.
        /// </summary>
        public int SelectedTypeId { get; set; }

        /// <summary>
        /// Gets or sets SelectedUnitId.
        /// </summary>
        public int? SelectedUnitId { get; set; }
    }
}
