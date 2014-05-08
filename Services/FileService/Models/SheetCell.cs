// -----------------------------------------------------------------------
// <copyright file="SheetCell.cs" company="Microsoft Corporation">
// Model class for XL sheet cell.
// </copyright>
// -----------------------------------------------------------------------

using DocumentFormat.OpenXml.Spreadsheet;

namespace Microsoft.Research.DataOnboarding.FileService.Models
{
    /// <summary>
    /// Model class for XL sheet cell
    /// </summary>
    public class SheetCell
    {
        /// <summary>
        /// Gets or sets column name.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets row index.
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// Gets or sets Xl cell instance.
        /// </summary>
        public Cell ExcelCell { get; set; }

        /// <summary>
        /// Gets or sets value of the cell.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the value of Column location
        /// </summary>
        public string ColumnLocation { get; set; }

        /// <summary>
        /// Gets or sets the Column Order Index
        /// </summary>
        public int ColumnOrderIndex { get; set; }

        /// <summary>
        /// Gets or sets te Column Location Index
        /// </summary>
        public int ColumnLocationIndex { get; set; }

        /// <summary>
        /// Gets or sets the Sheet Id
        /// </summary>
        public string SheetId { get; set; }

        /// <summary>
        /// Gets or sets the Sheet Name
        /// </summary>
        public string SheetName { get; set; }
    }
}
