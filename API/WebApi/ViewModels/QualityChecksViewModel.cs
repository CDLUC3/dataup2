using Microsoft.Research.DataOnboarding.Utilities.Model;
using System.Collections.Generic;

namespace Microsoft.Research.DataOnboarding.WebApi.ViewModels
{
    /// <summary>
    /// Quality checks viewmodel for post page.
    /// </summary>
    public class QualityChecksViewModel
    {
        /// <summary>
        /// Qyality check rules that need to be displayed to the user.
        /// </summary>
        public IEnumerable<QualityCheckModel> ColumnRules { get; set; }

        /// <summary>
        /// Sheets in the file.
        /// </summary>
        public IEnumerable<FileSheet> FileSheets { get; set; }
    }
}
