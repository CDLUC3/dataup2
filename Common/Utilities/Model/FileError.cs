// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.Utilities.Enums;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    /// <summary>
    /// FileError model class.
    /// </summary>
    public class FileError
    {
        /// <summary>
        /// Gets or sets the error type.
        /// </summary>
        public ErrorType ErrorType { get; set; }

        /// <summary>
        /// Gets or sets the error title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the address / unique identifier of the error.
        /// </summary>
        public string ErrorAddress { get; set; }

        /// <summary>
        /// Gets or sets the Description of the error.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the recommendation for the error.
        /// </summary>
        public string Recommendation { get; set; }

        /// <summary>
        /// Gets or seta a value indicating whether the error is fixable
        /// </summary>
        public bool CanFix { get; set; }
    }
}
