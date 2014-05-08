// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.Research.DataOnboarding.WebApplication.ViewModels
{
    /// <summary>
    /// View model class for quality check rules list.
    /// </summary>
    public class QualityCheckViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QualityCheckViewModel" /> class.
        /// </summary>
        public QualityCheckViewModel()
        {
            this.QualityCheckRules = new List<QCRuleViewModel>();
        }

        /// <summary>
        /// Gets or sets the collection of quality check rules.
        /// </summary>
        public IList<QCRuleViewModel> QualityCheckRules { get; set; }
    }
}