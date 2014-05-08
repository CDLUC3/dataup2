// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    /// <summary>
    /// Model class for quality check model
    /// </summary>
    public class QualityCheckModel
    {
        /// <summary>
        /// Gets or sets quality check object.
        /// </summary>
        public QualityCheck QualityCheckData { get; set; }

        /// <summary>
        /// Gets or sets the created user of the quality check rule.
        /// </summary>
        public string CreatedUser { get; set; }
    }
}
