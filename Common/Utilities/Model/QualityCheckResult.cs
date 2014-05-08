using System.Collections.Generic;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    public class QualityCheckResult
    {
        public QualityCheckResult()
        {
            this.Errors = new List<string>();
        }

        /// <summary>
        /// Gets or sets the Sheet ID
        /// </summary>
        public string SheetId { get; set; }

        /// <summary>
        /// Gets or sets the Sheet Name
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// Gets or sets the Error Desc
        /// </summary>
        public IList<string> Errors { get; set; }
    }
}
