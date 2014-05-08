// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Microsoft.Research.DataOnboarding.WebApplication.ViewModels
{
    /// <summary>
    /// View model class for quality check header.
    /// </summary>
    public class QCHeaderViewModel
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="QCHeaderViewModel" /> class.
        /// </summary>
        public QCHeaderViewModel()
        {
            this.QCColumnTypes = new SelectList(Enumerable.Empty<QCColumnTypeViewModel>());
        }

        /// <summary>
        /// Gets or sets quality check column rule id.
        /// </summary>
        public int QCColumnRuleId { get; set; }

        /// <summary>
        /// Gets or sets column header name.
        /// </summary>
        public string HeaderName { get; set; }

        /// <summary>
        /// Gets or sets indicating whether this collumn is mandatory or not.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets error message.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets order of the header.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets column type id.
        /// </summary>
        public int ColumnTypeId { get; set; }

        /// <summary>
        /// Gets or sets the range start value.
        /// </summary>
        public string RangeStart { get; set; }

        /// <summary>
        /// Gets or sets the range End value.
        /// </summary>
        public string RangeEnd { get; set; }

        /// <summary>
        /// Gets or sets list of column types.
        /// </summary>
        public SelectList QCColumnTypes { get; set; }
    }
}