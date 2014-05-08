// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Microsoft.Research.DataOnboarding.FileService.FileProcesser.Extensions
{
    public static class WorksheetExtension
    {

        // Given a Worksheet and an address (like "AZ254"), either return a cell reference, or 
        // create the cell reference and return it.
        public static Cell InsertCellInWorksheet(this Worksheet ws, string addressName)
        {
            // Use regular expressions to get the row number and column name.
            // If the parameter wasn't well formed, this code
            // will fail:
            Regex rx = new Regex("^(?<col>\\D+)(?<row>\\d+)");
            Match m = rx.Match(addressName);
            uint rowNumber = uint.Parse(m.Result("${row}"));
            string colName = m.Result("${col}");

            SheetData sheetData = ws.GetFirstChild<SheetData>();
            string cellReference = (colName + rowNumber.ToString());
            Cell theCell = null;

            // If the worksheet does not contain a row with the specified row index, insert one.
            var theRow = sheetData.Elements<Row>().Where(r => r.RowIndex.Value == rowNumber).FirstOrDefault();

            if (theRow == null)
            {
                theRow = new Row();
                theRow.RowIndex = rowNumber;
                sheetData.Append(theRow);
            }

            // If the cell you need already exists, return it.
            // If there is not a cell with the specified column name, insert one.  
            Cell refCell = theRow.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).FirstOrDefault();
            if (refCell != null)
            {
                theCell = refCell;
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                foreach (Cell cell in theRow.Elements<Cell>())
                {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                theCell = new Cell();
                theCell.CellReference = cellReference;

                theRow.InsertBefore(theCell, refCell);
            }

            return theCell;
        }
    }
}
