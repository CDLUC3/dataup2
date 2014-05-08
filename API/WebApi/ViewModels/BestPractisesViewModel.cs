using Microsoft.Research.DataOnboarding.Utilities.Model;
using System.Collections.Generic;

namespace Microsoft.Research.DataOnboarding.WebApi.ViewModels
{
    /// <summary>
    /// Best practises viewModel
    /// </summary>
    public class BestPractisesViewModel
    {
        /// <summary>
        /// Id of the file.
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Name of the file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Mime type of the file.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Can the errors be deleted.
        /// </summary>
        public bool CanDeleteErrors { get; set; }

        /// <summary>
        /// List of file sheets
        /// </summary>
        public List<FileSheet> FileSheets { get; set; }
    }
}