using Microsoft.Research.DataOnboarding.Utilities.Enums;
using System.Collections.Generic;

namespace Microsoft.Research.DataOnboarding.WebApi.ViewModels
{
    /// <summary>
    /// View model for remove errors.
    /// </summary>
    public class RemoveErrorsViewModel
    {
        /// <summary>
        /// File Id.
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Sheet name.
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// List of error types.
        /// </summary>
        public IEnumerable<ErrorType> ErrorTypes { get; set; }
    }
}