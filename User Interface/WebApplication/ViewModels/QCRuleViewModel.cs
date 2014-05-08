// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.WebApplication.Helpers;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Microsoft.Research.DataOnboarding.WebApplication.ViewModels
{
    /// <summary>
    /// View model class for quality check rule.
    /// </summary>
    public class QCRuleViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QCRuleViewModel" /> class.
        /// </summary>
        public QCRuleViewModel()
        {
            this.LstHeaderNames = new List<QCHeaderViewModel>();
            this.VisibilityOptions = Helper.GetVisibilityList();
        }

        /// <summary>
        /// Gets or sets quality check rule id.
        /// </summary>
        public int QCRuleId { get; set; }

        /// <summary>
        /// Gets or sets quality check rule name.
        /// </summary>
        public string QCRuleName { get; set; }

        /// <summary>
        /// Gets or sets quality check rule description.
        /// </summary>
        public string QCRuleDescription { get; set; }

        /// <summary>
        /// Gets or sets indicating whether headers order is required or not.
        /// </summary>
        public bool IsOrderRequired { get; set; }

        /// <summary>
        /// Gets or sets indicating whether the rule is visible to all or not.
        /// </summary>
        public bool IsVisibleToAll { get; set; }

        /// <summary>
        /// Gets or sets rule created date time.
        /// </summary>
        public string CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets rule created user id.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets created user name.
        /// </summary>
        public string CreatedUser { get; set; }

        /// <summary>
        /// Gets or sets header names.
        /// </summary>
        public string HeaderNames { get; set; }

        /// <summary>
        /// Gets or sets the collection of header information for this rule.
        /// </summary>
        public IList<QCHeaderViewModel> LstHeaderNames { get; set; }

        /// <summary>
        /// Gets or sets selected visibility option.
        /// </summary>
        public int VisibilityOption { get; set; }

        /// <summary>
        /// Gets or sets list of visibility types.
        /// </summary>
        public SelectList VisibilityOptions { get; set; }
    }
}