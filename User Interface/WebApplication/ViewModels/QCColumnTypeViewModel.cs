// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.WebApplication.ViewModels
{
    /// <summary>
    /// View model class for quality check column type.
    /// </summary>
    public class QCColumnTypeViewModel
    {
        /// <summary>
        /// Gets or sets column type id.
        /// </summary>
        public int ColumnTypeId { get; set; }

        /// <summary>
        /// Gets or sets column type name.
        /// </summary>
        public string ColumnTypeName { get; set; }

        /// <summary>
        /// Gets or sets column type Description.
        /// </summary>
        public string ColumnTypeDescription { get; set; }
    }
}