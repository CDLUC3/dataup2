// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Research.DataOnboarding.Utilities;
using System.Linq;
using System;
using Microsoft.Research.DataOnboarding.FileService.Resource;

namespace Microsoft.Research.DataOnboarding.FileService.FileProcesser.Extensions
{
    /// <summary>
    /// Extension methods for a Excel SpreadSheet.
    /// </summary>
    public static class SpreadsheetDocumentExtension
    {

        // Given a workbook document, and a sheet name, return the Worksheet
        // corresponding to the supplied name. 
        public static Sheet GetSheet(this SpreadsheetDocument document, string sheetName)
        {
            WorkbookPart wbPart = document.WorkbookPart;

            // Find the sheet with the supplied name, and then use that Sheet object
            // to retrieve a reference to the appropriate worksheet.
            Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().Where(s => string.Compare(s.Name, sheetName, true) == 0).FirstOrDefault();

            return theSheet;
        }

        public static Sheet InsertMetadataWorksheet(this SpreadsheetDocument document, bool hasFileLevelMetadata, bool hasColumnLevelMetaData)
        {
            // Add a blank WorksheetPart.
            WorksheetPart newWorksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());

            string fileLevelMetadataTableId = string.Empty;
            if (hasFileLevelMetadata)
            {
                TableDefinitionPart tableDefinitionPart = newWorksheetPart.AddNewPart<TableDefinitionPart>();
                GenerateTableDefinitionPartContent(tableDefinitionPart);
                fileLevelMetadataTableId = newWorksheetPart.GetIdOfPart(tableDefinitionPart);
            }

            string columnLevelMetadataTableId = string.Empty;
            if (hasColumnLevelMetaData)
            {
                // Generate ParameterMetaData.
                TableDefinitionPart tableParameterPart = newWorksheetPart.AddNewPart<TableDefinitionPart>();
                GenerateParameterTablePartContent(tableParameterPart, hasFileLevelMetadata);
                columnLevelMetadataTableId = newWorksheetPart.GetIdOfPart(tableParameterPart);
            }

            // Generate Content.
            GeneratePartContent(newWorksheetPart, new string[] { fileLevelMetadataTableId, columnLevelMetadataTableId });

            Sheets sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            string relationshipId = document.WorkbookPart.GetIdOfPart(newWorksheetPart);

            // Get a unique ID for the new worksheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            // Append the new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = string.Concat(Constants.MetadataSheetNamePrefix, DateTime.UtcNow.ToString("MMddyyyyhhmmss")) };
            sheets.Append(sheet);

            return sheet;
        }

        public static bool SetCellValue(this SpreadsheetDocument document, Sheet metadataSheet, string addressName, string value)
        {
            // If the string exists in the shared string table, get its index.
            // If the string doesn't exist in the shared string table, add it and get the next index.
            // Assume failure.
            bool returnValue = false;

            // Open the document for editing.
            WorkbookPart wbPart = document.WorkbookPart;

            if (metadataSheet != null)
            {
                Worksheet ws = ((WorksheetPart)(wbPart.GetPartById(metadataSheet.Id))).Worksheet;

                Cell theCell = ws.InsertCellInWorksheet(addressName);

                // Either retrieve the index of an existing string,
                // or insert the string into the shared string table
                // and get the index of the new item.
                int stringIndex = wbPart.InsertSharedStringItem(value);

                theCell.CellValue = new CellValue(stringIndex.ToString());
                theCell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                // Save the worksheet.
                ws.Save();
                returnValue = true;
            }
            return returnValue;
        }

        public static bool IsHidden(this Sheet sheet)
        {
            return sheet.State != null && (sheet.State == SheetStateValues.Hidden || sheet.State == SheetStateValues.VeryHidden);
        }

        #region private methods

        /// <summary>
        ///  Generates content of tableDefinitionPart1.
        /// </summary>
        /// <param name="tableDefinitionPart">Table definition.</param>
        private static void GenerateTableDefinitionPartContent(TableDefinitionPart tableDefinitionPart)
        {
            Table table1 = new Table() { Id = (UInt32Value)5U, Name = Constants.MetadataRangeName, DisplayName = Constants.MetadataRangeName, Reference = "A1:B44", TotalsRowShown = false };
            AutoFilter autoFilter1 = new AutoFilter() { Reference = "A1:B44" };

            TableColumns tableColumns1 = new TableColumns() { Count = (UInt32Value)2U };
            TableColumn tableColumn1 = new TableColumn() { Id = (UInt32Value)1U, Name = "Name" };
            TableColumn tableColumn2 = new TableColumn() { Id = (UInt32Value)2U, Name = "Value" };

            tableColumns1.Append(tableColumn1);
            tableColumns1.Append(tableColumn2);
            TableStyleInfo tableStyleInfo1 = new TableStyleInfo() { Name = Constants.MetadataTableStyleName, ShowFirstColumn = false, ShowLastColumn = false, ShowRowStripes = true, ShowColumnStripes = false };

            table1.Append(autoFilter1);
            table1.Append(tableColumns1);
            table1.Append(tableStyleInfo1);

            tableDefinitionPart.Table = table1;
        }

        /// <summary>
        /// Generates content of tableDefinitionPart1.
        /// </summary>
        /// <param name="tableDefinitionPart">Table definition part.</param>
        /// <param name="hasFileLevelMetadata">Has file level metadata.</param>
        private static void GenerateParameterTablePartContent(TableDefinitionPart tableDefinitionPart, bool hasFileLevelMetadata)
        {
            string reference = hasFileLevelMetadata ? "D1:I44" : "A1:F44";
            Table table1 = new Table() { Id = (UInt32Value)6U, Name = Constants.ParaMetadataRangeName, DisplayName = Constants.ParaMetadataRangeName, Reference = reference, TotalsRowShown = false };
            AutoFilter autoFilter1 = new AutoFilter() { Reference = reference };

            TableColumns tableColumns1 = new TableColumns() { Count = (UInt32Value)6U };

            tableColumns1.Append(new TableColumn() { Id = (UInt32Value)1U, Name = Statics.TableName });
            tableColumns1.Append(new TableColumn() { Id = (UInt32Value)2U, Name = Statics.TableDescription });
            tableColumns1.Append(new TableColumn() { Id = (UInt32Value)3U, Name = Statics.FieldName });
            tableColumns1.Append(new TableColumn() { Id = (UInt32Value)4U, Name = Statics.FieldDescription });
            tableColumns1.Append(new TableColumn() { Id = (UInt32Value)5U, Name = Statics.DataType });
            tableColumns1.Append(new TableColumn() { Id = (UInt32Value)6U, Name = Statics.Units });

            TableStyleInfo tableStyleInfo1 = new TableStyleInfo() { Name = Constants.MetadataTableStyleName, ShowFirstColumn = false, ShowLastColumn = false, ShowRowStripes = true, ShowColumnStripes = false };

            table1.Append(autoFilter1);
            table1.Append(tableColumns1);
            table1.Append(tableStyleInfo1);

            tableDefinitionPart.Table = table1;
        }

        /// <summary>
        /// Generates the content of the WorkSheet part.
        /// </summary>
        /// <param name="part">The part.</param>
        private static void GeneratePartContent(WorksheetPart part, string[] partIds)
        {
            char sheetDimensionColumn = 'A';
            if (!string.IsNullOrEmpty(partIds[0]))
            {
                // If file level metadata is present, sheet dimension will be from column A to column B.
                sheetDimensionColumn = (char)(sheetDimensionColumn + 1);
            }

            if (!string.IsNullOrEmpty(partIds[1]))
            {
                // If column level metadata is present, sheet dimension will be from column D to column I if there is file level metadata. Otherwise from column A to column F.
                sheetDimensionColumn = !string.IsNullOrEmpty(partIds[0]) ? (char)(sheetDimensionColumn + 7) : (char)(sheetDimensionColumn + 5);
            }

            Worksheet worksheet1 = new Worksheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            worksheet1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            worksheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            worksheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            SheetDimension sheetDimension1 = new SheetDimension() { Reference = "A1:" + sheetDimensionColumn + "30" };

            SheetViews sheetViews1 = new SheetViews();

            SheetView sheetView1 = new SheetView() { TabSelected = true, WorkbookViewId = (UInt32Value)0U };
            // Add 2 extra columns for selection.
            char activeColumn = (char)(sheetDimensionColumn + 2);
            Selection selection1 = new Selection() { ActiveCell = activeColumn + "9", SequenceOfReferences = new ListValue<StringValue>() { InnerText = activeColumn + "9" } };

            sheetView1.Append(selection1);

            sheetViews1.Append(sheetView1);
            SheetFormatProperties sheetFormatProperties1 = new SheetFormatProperties() { DefaultRowHeight = 15D, DyDescent = 0.25D };

            Columns columns1 = new Columns();
            Column column1 = new Column() { Min = (UInt32Value)1U, Max = (UInt32Value)2U, Width = 11D, CustomWidth = true };

            columns1.Append(column1);

            SheetData sheetData1 = new SheetData();

            Row row1 = new Row() { RowIndex = (UInt32Value)1U, Spans = new ListValue<StringValue>() { InnerText = (string.IsNullOrEmpty(partIds[1])) ? "1:2" : "1:9" }, DyDescent = 0.25D };
            char column = 'A';

            if (!string.IsNullOrEmpty(partIds[0]))
            {
                // Add file level metadata name to column A.
                Cell cell1 = new Cell() { CellReference = "A1", DataType = CellValues.String };
                CellValue cellValue1 = new CellValue() { Text = "Name" };
                cell1.Append(cellValue1);
                row1.Append(cell1);

                // Add file level metadata value to column B.
                Cell cell2 = new Cell() { CellReference = "B1", DataType = CellValues.String };
                CellValue cellValue2 = new CellValue() { Text = "Value" };
                cell2.Append(cellValue2);
                row1.Append(cell2);

                // Add column level metadata from column D if file level metadata is present.
                column = 'D';
            }

            if (!string.IsNullOrEmpty(partIds[1]))
            {
                Cell cell3 = new Cell() { CellReference = (column++) + "1", DataType = CellValues.String };
                CellValue cellValue3 = new CellValue() { Text = Statics.TableName };

                cell3.Append(cellValue3);
                row1.Append(cell3);

                Cell cell4 = new Cell() { CellReference = (column++) + "1", DataType = CellValues.String };
                CellValue cellValue4 = new CellValue() { Text = Statics.TableDescription };
                cell4.Append(cellValue4);
                row1.Append(cell4);

                Cell cell5 = new Cell() { CellReference = (column++) + "1", DataType = CellValues.String };
                CellValue cellValue5 = new CellValue() { Text = Statics.FieldName };
                cell5.Append(cellValue5);
                row1.Append(cell5);

                Cell cell6 = new Cell() { CellReference = (column++) + "1", DataType = CellValues.String };
                CellValue cellValue6 = new CellValue() { Text = Statics.FieldDescription };
                cell6.Append(cellValue6);
                row1.Append(cell6);

                Cell cell7 = new Cell() { CellReference = (column++) + "1", DataType = CellValues.String };
                CellValue cellValue7 = new CellValue() { Text = Statics.DataType };
                cell7.Append(cellValue7);
                row1.Append(cell7);

                Cell cell8 = new Cell() { CellReference = (column++) + "1", DataType = CellValues.String };
                CellValue cellValue8 = new CellValue() { Text = Statics.Units };
                cell8.Append(cellValue8);
                row1.Append(cell8);
            }

            sheetData1.Append(row1);
            PageMargins pageMargins1 = new PageMargins() { Left = 0.7D, Right = 0.7D, Top = 0.75D, Bottom = 0.75D, Header = 0.3D, Footer = 0.3D };

            TableParts tableParts1 = new TableParts() { Count = new UInt32Value((uint)partIds.Count(id => !string.IsNullOrEmpty(id))) };
            if (!string.IsNullOrEmpty(partIds[0]))
            {
                tableParts1.Append(new TablePart() { Id = partIds[0] });
            }

            if (!string.IsNullOrEmpty(partIds[1]))
            {
                tableParts1.Append(new TablePart() { Id = partIds[1] });
            }

            worksheet1.Append(sheetDimension1);
            worksheet1.Append(sheetViews1);
            worksheet1.Append(sheetFormatProperties1);
            worksheet1.Append(columns1);
            worksheet1.Append(sheetData1);
            worksheet1.Append(pageMargins1);
            worksheet1.Append(tableParts1);

            part.Worksheet = worksheet1;
        }

        #endregion
    }
}
