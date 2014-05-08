// -----------------------------------------------------------------------
// <copyright file="ExcelFileHelper.cs" company="Microsoft Corporation">
// Helper class for XL file related methods.
// </copyright>
// -----------------------------------------------------------------------

using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Research.DataOnboarding.FileService.FileProcesser.Extensions;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.FileService.Resource;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ss = DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Microsoft.Research.DataOnboarding.FileService.FileProcesser;
using Microsoft.Research.DataOnboarding.DomainModel;

using S = DocumentFormat.OpenXml.Spreadsheet;

using C = DocumentFormat.OpenXml.Drawing.Charts;

namespace Microsoft.Research.DataOnboarding.FileService.Helper
{
    /// <summary>
    /// Helper class for XL file related methods
    /// </summary>
    public static class ExcelFileHelper
    {
        const int ERROR_CELL_COUNT = 10;
        private static Regex regkey;
        private static Regex regForVal;
        private static Regex reg1;
        private static Regex regColumnName;
        private static Regex regRow;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static ExcelFileHelper()
        {
            regkey = new Regex(Constants.SpecialCharKeySequence, RegexOptions.Compiled);
            regForVal = new Regex(@"-?[0-9]+[^.]", RegexOptions.Compiled);
            reg1 = new Regex(@"-?[0-9]+\.[0-9]+", RegexOptions.Compiled);
            regColumnName = new Regex(@"[A-Za-z]+", RegexOptions.Compiled);
            regRow = new Regex(@"\d+", RegexOptions.Compiled);
        }

        /// <summary>
        /// Method to get headers information
        /// </summary>
        /// <param name="document">Document Id</param>
        /// <param name="sheetId">Sheet Id</param>
        /// <returns>returns the headers collection</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static IEnumerable<SheetCell> GetHeaders(SpreadsheetDocument document, string sheetId)
        {
            Check.IsNotNull(document, "document");
            Check.IsNotNull(sheetId, "sheetId");

            IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.Descendants<Sheet>();//.Where(s => s.Name == worksheetName);
            IEnumerable<SheetCell> headerCells = new List<SheetCell>(); ;
            if (!sheets.Any())
            {
                // The specified worksheet does not exist.
                return null;
            }
            WorksheetPart worksheetPart = null;

            worksheetPart = (WorksheetPart)document.WorkbookPart.GetParentParts();

            var sheet = document.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Id == sheetId);
            SheetData sheetLevelData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();
            //}

            Dictionary<int, string> sharedStringList = new Dictionary<int, string>();
            if (document.WorkbookPart.SharedStringTablePart != null)
            {
                sharedStringList = CreateSharedStringList(document.WorkbookPart.SharedStringTablePart.SharedStringTable);
            }

            if (sheetLevelData.Descendants<Row>().Any())
            {
                //string[] bounds = GetWorkSheetBounds(worksheetPart);
                //int lastRow = Convert.ToInt32(bounds[2]);
                headerCells = worksheetPart.Worksheet.Descendants<Cell>()
                                           .Select(c => new SheetCell
                                           {
                                               ColumnName = GetColumnName(c.CellReference.Value),
                                               ExcelCell = c,
                                               RowIndex = GetRowIndex(c.CellReference.Value),
                                               ColumnLocation = c.CellReference,
                                               SheetId = sheet.Id,
                                               SheetName = sheet.Name,
                                               Value = (c.DataType != null && c.DataType == CellValues.SharedString) ? sharedStringList[Convert.ToInt32(c.CellValue.Text, CultureInfo.CurrentCulture)] : c.CellValue.Text
                                           }).Where(r => r.RowIndex == 1);
            }
            return headerCells;
        }

        /// <summary>
        /// Method to get headers information
        /// </summary>
        /// <param name="document">Document Id</param>
        /// <param name="sheetId">Sheet Id</param>
        /// <returns>returns the headers collection</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static IEnumerable<ColumnLevelMetadata> GetColumnMetadataForAllSheets(SpreadsheetDocument document)
        {
            IList<ColumnLevelMetadata> columnLevelMetadataList = new List<ColumnLevelMetadata>(); ;

            Check.IsNotNull(document, "document");

            WorkbookPart wbPart = document.WorkbookPart;
            var sheets = wbPart.Workbook.Sheets;

            foreach (Sheet sheet in sheets)
            {
                WorksheetPart worksheetPart = null;
                IEnumerable<SheetCell> headerCells = new List<SheetCell>(); ;
                IEnumerable<SheetCell> dataRowCells = new List<SheetCell>(); ;
                List<SheetCell> dataRowCellsList = new List<SheetCell>(); ;
                IEnumerable<ColumnLevelMetadata> columnLevelMetadataListForSheet = new List<ColumnLevelMetadata>(); ;

                worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheet.Id);

                SheetData sheetLevelData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();

                Dictionary<int, string> sharedStringList = new Dictionary<int, string>();
                if (document.WorkbookPart.SharedStringTablePart != null)
                {
                    sharedStringList = CreateSharedStringList(document.WorkbookPart.SharedStringTablePart.SharedStringTable);
                }

                if (sheetLevelData.Descendants<Row>().Any())
                {
                    //string[] bounds = GetWorkSheetBounds(worksheetPart);
                    //int lastRow = Convert.ToInt32(bounds[2]);
                    headerCells = worksheetPart.Worksheet.Descendants<Cell>()
                                               .Select(c => new SheetCell
                                               {
                                                   ColumnName = GetColumnName(c.CellReference.Value),
                                                   ExcelCell = c,
                                                   RowIndex = GetRowIndex(c.CellReference.Value),
                                                   ColumnLocation = c.CellReference,
                                                   SheetId = sheet.Id,
                                                   SheetName = sheet.Name,
                                                   Value = (c.DataType != null && c.DataType == CellValues.SharedString) ? sharedStringList[Convert.ToInt32(c.CellValue.Text, CultureInfo.CurrentCulture)] : c.CellValue.Text
                                               }).Where(r => r.RowIndex == 1);
                }

                columnLevelMetadataListForSheet = CreateColumnLevelMetadataList(sheet.Name, headerCells);

                foreach (ColumnLevelMetadata columnLevelMetadata in columnLevelMetadataListForSheet)
                {
                    columnLevelMetadataList.Add(columnLevelMetadata);
                }
            }

            return columnLevelMetadataList;
        }

        private static IList<ColumnLevelMetadata> CreateColumnLevelMetadataList(string sheetName, IEnumerable<SheetCell> headerCells)
        {
            IList<ColumnLevelMetadata> columnLevelMetadaList = new List<ColumnLevelMetadata>();

            if (headerCells == null || headerCells.Count() == 0)
            {
                return columnLevelMetadaList;
            }

            for (int i = 0; i < headerCells.Count(); i++)
            {
                ColumnLevelMetadata columnLevelMetadata = new ColumnLevelMetadata();
                columnLevelMetadata.SelectedEntityName = sheetName;
                columnLevelMetadata.Name = headerCells.ElementAt(i).Value;

                columnLevelMetadaList.Add(columnLevelMetadata);
            }

            return columnLevelMetadaList;
        }

        /// <summary>
        /// Method to give header and content collection
        /// </summary>
        /// <param name="document">Spread sheet document</param>
        /// <param name="sheetId">Sheet Id</param>
        /// <returns>returns the sheetcell collection</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static IEnumerable<SheetCell> GetHeadersAndColumns(SpreadsheetDocument document, string sheetId)
        {
            Check.IsNotNull(document, "document");
            Check.IsNotNull(sheetId, "sheetId");

            IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.Descendants<Sheet>();//.Where(s => s.Name == worksheetName);
            IEnumerable<SheetCell> cells = new List<SheetCell>(); ;
            if (!sheets.Any())
            {
                // The specified worksheet does not exist.
                return null;
            }
            WorksheetPart worksheetPart = null;

            worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheetId);

            var sheet = document.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Id == sheetId);
            SheetData sheetLevelData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();
            //}

            Dictionary<int, string> sharedStringList = new Dictionary<int, string>();
            if (document.WorkbookPart.SharedStringTablePart != null)
            {
                sharedStringList = CreateSharedStringList(document.WorkbookPart.SharedStringTablePart.SharedStringTable);
            }

            if (sheetLevelData.Descendants<Row>().Any())
            {
                //string[] bounds = GetWorkSheetBounds(worksheetPart);
                //int lastRow = Convert.ToInt32(bounds[2]);
                cells = worksheetPart.Worksheet.Descendants<Cell>()
                                           .Where(c => c.CellValue != null && !string.IsNullOrWhiteSpace(c.CellValue.ToString()))
                                           .Select(c => new SheetCell
                                           {
                                               ColumnName = GetColumnName(c.CellReference.Value),
                                               ExcelCell = c,
                                               RowIndex = GetRowIndex(c.CellReference.Value),
                                               ColumnLocation = c.CellReference,
                                               SheetId = sheet.Id,
                                               SheetName = sheet.Name,
                                               Value = (c.DataType != null && c.DataType == CellValues.SharedString) ? sharedStringList[Convert.ToInt32(c.CellValue.Text, CultureInfo.CurrentCulture)] : c.CellValue.Text
                                           });
                foreach (var cell in cells)
                {
                    cell.ColumnLocationIndex = GetColumnLocationIndex(cell.ColumnLocation);
                }
            }
            return cells;
        }

        /// <summary>
        /// Method to check whether given sheet has data or not.
        /// </summary>
        /// <param name="document">SpreadSheetDocument object.</param>
        /// <param name="sheetId">Sheet id.</param>
        /// <returns>True if data is available else false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static bool CheckSheetForData(SpreadsheetDocument document, string sheetId)
        {
            Check.IsNotNull(document, "document");
            Check.IsNotNull(sheetId, "sheetId");

            bool isDataAvailable = false;
            WorksheetPart worksheetPart = null;
            worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheetId);
            SheetData sheetLevelData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();

            if (sheetLevelData.Descendants<Row>().Any())
            {
                isDataAvailable = true;
            }

            return isDataAvailable;
        }

        /// <summary>
        /// Validates the graphics.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>List of sheet with errors</returns>
        public async static Task<IList<FileSheet>> ValidateGraphics(SpreadsheetDocument document)
        {
            List<FileSheet> fileSheets = new List<FileSheet>();
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    WorkbookPart wbPart = document.WorkbookPart;

                    if (wbPart != null)
                    {
                        Dictionary<int, string> sharedStringList = new Dictionary<int, string>();
                        if (wbPart.SharedStringTablePart != null)
                        {
                            sharedStringList = CreateSharedStringList(wbPart.SharedStringTablePart.SharedStringTable);
                        }

                        Dictionary<int, CellFormat> sharedFormatList = new Dictionary<int, CellFormat>();
                        CellStyles cellStyle = null;
                        if (wbPart.WorkbookStylesPart != null)
                        {
                            sharedFormatList = CreateCellFormatsList(wbPart.WorkbookStylesPart.Stylesheet.CellFormats);
                            cellStyle = (CellStyles)wbPart.WorkbookStylesPart.Stylesheet.CellStyles;
                        }

                        string sheetId, sheetName;
                        foreach (var theSheet in wbPart.Workbook.Descendants<Sheet>())
                        {
                            sheetName = (theSheet == null) ? string.Empty : theSheet.Name.Value;
                            sheetId = (theSheet == null) ? string.Empty : theSheet.SheetId;
                            WorksheetPart sheet = (WorksheetPart)document.WorkbookPart.GetPartById(theSheet.Id);

                            if (!theSheet.IsHidden())
                            {
                                FileSheet fileSheet = new FileSheet()
                                {
                                    SheetId = theSheet.SheetId,
                                    SheetName = theSheet.Name.Value
                                };

                                var commentErrors = RetrieveCommentErrors(sheet, sheetName, sheetId);
                                fileSheet.FileErrors.AddRange(commentErrors);

                                var mergedCellErrors = RetrieveMergedCellErrors(sheet, sheetName, sheetId);
                                fileSheet.FileErrors.AddRange(mergedCellErrors);

                                if (sheet.DrawingsPart != null)
                                {
                                    var chartErrors = RetrieveChartErrors(sheet.DrawingsPart, sheetName, sheetId);
                                    fileSheet.FileErrors.AddRange(chartErrors);

                                    var picErrors = RetrievePicErrors(sheet.DrawingsPart, sheetName, sheetId);
                                    fileSheet.FileErrors.AddRange(picErrors);

                                    var shapeErrors = RetrieveShapeErrors(sheet.DrawingsPart, sheetName, sheetId);
                                    fileSheet.FileErrors.AddRange(shapeErrors);
                                }

                                var cellErrors = RetrieveCellErrors(sheet, cellStyle, sheetName, sheetId, sharedStringList, sharedFormatList);
                                fileSheet.FileErrors.AddRange(cellErrors);

                                fileSheets.Add(fileSheet);
                            }
                        }
                    }
                });
            }
            catch (AggregateException ae)
            {
                throw;
            }

            return fileSheets;
        }

        /// <summary>
        /// Removes an error from a file sheet.
        /// </summary>
        /// <param name="document">Spreadsheet document</param>
        /// <param name="sheetName">Sheet Name</param>
        /// <param name="errorType">Error type</param>
        /// <returns>Boolean indicating whether the removal was successful or not.</returns>
        public async static Task RemoveError(SpreadsheetDocument document, string sheetName, ErrorType errorType)
        {
            await Task.Run(() =>
            {
                WorkbookPart workbookPart = document.WorkbookPart;
                Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().Single(s => s.Name.ToString().Equals(sheetName, StringComparison.OrdinalIgnoreCase));
                WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheet.Id);

                switch (errorType)
                {
                    case ErrorType.Comments:
                        RemoveComments(worksheetPart);
                        break;
                    case ErrorType.Charts:
                        RemoveCharts(worksheetPart);
                        break;
                    case ErrorType.Shapes:
                        RemoveShapes(worksheetPart);
                        break;
                    case ErrorType.MergedCell:
                        RemoveMergeCells(worksheetPart);
                        break;
                    case ErrorType.ColorCoded:
                        RemoveColorCells(worksheetPart, workbookPart);
                        break;
                    case ErrorType.Pictures:
                        RemovePictures(worksheetPart);
                        break;
                }
            });
        }

        #region Private Methods

        private static Dictionary<int, CellFormat> CreateCellFormatsList(CellFormats formats)
        {
            Dictionary<int, CellFormat> sharedFormatList = new Dictionary<int, CellFormat>();
            int itemIndex = 0;
            foreach (CellFormat cell in formats.Descendants<CellFormat>())
            {
                sharedFormatList.Add(itemIndex++, cell);
            }

            return sharedFormatList;
        }

        /// <summary>
        /// Retrieves the comment errors.
        /// </summary>
        /// <param name="part">The Worksheet part.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="sheetId">The sheet id.</param>
        /// <returns>List of comment errors.</returns>
        private static IList<FileError> RetrieveCommentErrors(WorksheetPart part, string sheetName, string sheetId)
        {
            List<FileError> fileErrors = new List<FileError>();
            var commentPart = part.WorksheetCommentsPart;
            StringBuilder sbAddress = new StringBuilder();
            int count = 0;
            if (commentPart != null)
            {
                foreach (var item in commentPart.Comments.CommentList)
                {
                    if (!string.IsNullOrWhiteSpace(item.ToString()))
                    {
                        sbAddress.Append((((Comment)(item))).Reference);
                        sbAddress.Append(", ");
                    }

                    if (++count > ERROR_CELL_COUNT)
                    {
                        break;
                    }
                }

                fileErrors.Add(new FileError()
                {
                    ErrorType = ErrorType.Comments,
                    Title = Messages.EmbeddedComments,
                    ErrorAddress = sbAddress.ToString(),
                    CanFix = true,
                    Description = Messages.EmbeddedCommentsDescription,
                    Recommendation = Messages.EmbeddedCommentsRecommendation
                });

                sbAddress = null;
            }

            return fileErrors;
        }

        /// <summary>
        /// Retrieves the merged cell errors.
        /// </summary>
        /// <param name="part">The Worksheet part.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="sheetId">The sheet id.</param>
        /// <returns>List of merged cell errors</returns>
        private static IList<FileError> RetrieveMergedCellErrors(WorksheetPart part, string sheetName, string sheetId)
        {
            List<FileError> fileErrors = new List<FileError>();
            if (part.Worksheet.Descendants<MergeCell>().Count() > 0)
            {
                StringBuilder sbAddress = new StringBuilder();
                int count = 0;
                foreach (var mergedCell in part.Worksheet.Descendants<MergeCell>())
                {
                    sbAddress.Append(mergedCell.Reference);
                    sbAddress.Append(",");
                    if (++count > ERROR_CELL_COUNT)
                    {
                        break;
                    }
                }

                fileErrors.Add(new FileError()
                {
                    ErrorType = ErrorType.MergedCell,
                    Title = Messages.MergedCells,
                    ErrorAddress = sbAddress.ToString(),
                    CanFix = true,
                    Description = Messages.MergedCellsDescription,
                    Recommendation = Messages.MergedCellsRecommendation
                });

                sbAddress = null;
            }

            return fileErrors;
        }

        /// <summary>
        /// Retrieves the chart errors.
        /// </summary>
        /// <param name="part">The drawings part of Excel document.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="sheetId">The sheet id.</param>
        /// <returns>List of chart errors</returns>
        private static IList<FileError> RetrieveChartErrors(DrawingsPart part, string sheetName, string sheetId)
        {
            List<FileError> fileErrors = new List<FileError>();
            if (part.ChartParts != null)
            {
                Dictionary<string, string> address = new Dictionary<string, string>();
                part.WorksheetDrawing.Descendants<GraphicFrame>().ToList().ForEach(q =>
                {
                    var anchor = (TwoCellAnchor)q.Parent;
                    address.Add(q.Graphic.GraphicData.GetFirstChild<ChartReference>().Id, GetColumnNameFromIndex(anchor.FromMarker.ColumnId.Text) + anchor.FromMarker.RowId.Text);

                });

                StringBuilder sbAddress = new StringBuilder();
                int count = 0;
                string str = String.Empty;
                foreach (var chartpart in part.ChartParts)
                {
                    str = part.GetIdOfPart(chartpart);
                    sbAddress.Append(address[str]);
                    sbAddress.Append(",");
                    if (++count > ERROR_CELL_COUNT)
                    {
                        break;
                    }
                }

                if (count > 0)
                {
                    fileErrors.Add(new FileError()
                    {
                        ErrorType = ErrorType.Charts,
                        Title = Messages.EmbeddedCharts,
                        ErrorAddress = sbAddress.ToString(),
                        CanFix = true,
                        Description = Messages.EmbeddedChartsDescription,
                        Recommendation = Messages.EmbeddedChartsRecommendation
                    });

                    sbAddress = null;
                    address = null;
                }
            }

            return fileErrors;
        }

        /// <summary>
        /// Gets the Excel Column Name from the Column Index.
        /// </summary>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns>Excel Column Name</returns>
        private static string GetColumnNameFromIndex(string columnIndex)
        {
            int dividend = Convert.ToInt32(columnIndex) + 1;
            string columnName = String.Empty;
            int modifier;

            while (dividend > 0)
            {
                modifier = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modifier).ToString() + columnName;
                dividend = (int)((dividend - modifier) / 26);
            }

            return columnName;
        }

        /// <summary>
        /// Retrieves the pic errors.
        /// </summary>
        /// <param name="part">The drawings part of Excel document.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="sheetId">The sheet id.</param>
        /// <returns>List of pic errors</returns>
        private static IList<FileError> RetrievePicErrors(DrawingsPart part, string sheetName, string sheetId)
        {
            List<FileError> fileErrors = new List<FileError>();
            if (part.ImageParts != null)
            {
                Dictionary<string, string> address = new Dictionary<string, string>();
                part.WorksheetDrawing.Descendants<ss.Picture>().ToList().ForEach(q =>
                {
                    var anchor = (TwoCellAnchor)q.Parent;
                    address.Add(q.BlipFill.Blip.Embed, GetColumnNameFromIndex(anchor.FromMarker.ColumnId.Text) + anchor.FromMarker.RowId.Text);

                });

                StringBuilder sbAddress = new StringBuilder();
                int count = 0;
                string str = String.Empty;
                foreach (var imagePart in part.ImageParts)
                {
                    str = part.GetIdOfPart(imagePart);
                    sbAddress.Append(address[str]);
                    sbAddress.Append(",");
                    if (++count > ERROR_CELL_COUNT)
                    {
                        break;
                    }
                }

                if (count > 0)
                {
                    fileErrors.Add(new FileError()
                    {
                        ErrorType = ErrorType.Pictures,
                        Title = Messages.EmbeddedPictures,
                        ErrorAddress = sbAddress.ToString(),
                        CanFix = true,
                        Description = Messages.EmbeddedPicturesDescription,
                        Recommendation = Messages.EmbeddedPicturesRecommendation
                    });

                    sbAddress = null;
                    address = null;
                }
            }

            return fileErrors;
        }

        /// <summary>
        /// Retrieves the shape errors.
        /// </summary>
        /// <param name="part">The drawings part of Excel document.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="sheetId">The sheet id.</param>
        /// <returns>List of shape errors</returns>
        private static IList<FileError> RetrieveShapeErrors(DrawingsPart part, string sheetName, string sheetId)
        {
            List<FileError> fileErrors = new List<FileError>();
            var shapes = part.WorksheetDrawing.Descendants<ss.Shape>();
            if (shapes.Any())
            {
                Dictionary<string, string> address = new Dictionary<string, string>();
                part.WorksheetDrawing.Descendants<ss.Shape>().ToList().ForEach(q =>
                {
                    var anchor = (TwoCellAnchor)q.Parent;
                    address.Add(q.NonVisualShapeProperties.NonVisualDrawingProperties.Id,
                        GetColumnNameFromIndex(anchor.FromMarker.ColumnId.Text) + anchor.FromMarker.RowId.Text);

                });
                StringBuilder sbAddress = new StringBuilder();
                int count = 0;

                foreach (ss.Shape val in shapes)
                {
                    var id = val.NonVisualShapeProperties.NonVisualDrawingProperties.Id;
                    sbAddress.Append(address[id]);
                    sbAddress.Append(",");
                    if (++count > ERROR_CELL_COUNT)
                    {
                        break;
                    }
                }
                if (count > 0)
                {
                    fileErrors.Add(new FileError()
                    {
                        ErrorType = ErrorType.Shapes,
                        Title = Messages.EmbeddedShapes,
                        ErrorAddress = sbAddress.ToString(),
                        CanFix = true,
                        Description = Messages.EmbeddedShapesDescription,
                        Recommendation = Messages.EmbeddedShapesRecommendation
                    });
                    sbAddress = null;
                    address = null;
                }
            }

            return fileErrors;
        }

        /// <summary>
        /// Retrieves the cell errors.
        /// </summary>
        /// <param name="part">The Worksheet part.</param>
        /// <param name="cellFormats">The cell formats.</param>
        /// <param name="cellstyle">The cellstyle.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="sheetId">The sheet id.</param>
        /// <param name="sharedStringList">List of shared strings.</param>
        /// <param name="sharedFormatList">List of shared formats</param>
        /// <returns>List of cell errors</returns>
        private static IList<FileError> RetrieveCellErrors(WorksheetPart part, CellStyles cellstyle, string sheetName, string sheetId, Dictionary<int, string> sharedStringList, Dictionary<int, CellFormat> sharedFormatList)
        {
            List<FileError> fileErrors = new List<FileError>();
            SheetData sheetData = part.Worksheet.Elements<SheetData>().FirstOrDefault();

            if (sheetData.Descendants<Row>().Any())
            {
                bool checkCommaErrors = true,
                    checkSpecialCharacterErrors = true,
                    checkStyleErrors = true;
                List<string> commaErrors = new List<string>();
                List<string> specialCharactersErrors = new List<string>();
                List<string> styleErrors = new List<string>();

                IEnumerable<SheetCell> cells = part.Worksheet.Descendants<Cell>()
                                           .Select(c => new SheetCell
                                           {
                                               ColumnName = GetColumnName(c.CellReference.Value),
                                               ExcelCell = c,
                                               RowIndex = GetRowIndex(c.CellReference.Value),
                                               Value = (c.DataType != null && c.DataType == CellValues.SharedString) ?
                                                        sharedStringList[Convert.ToInt32(c.CellValue.Text)] : (c.CellValue == null) ?
                                                        string.Empty : c.CellValue.Text
                                           });

                foreach (SheetCell cell in cells)
                {
                    // Check comma errors
                    if (checkCommaErrors && CellHasCommas(cell))
                    {
                        commaErrors.Add(cell.ExcelCell.CellReference.Value);
                        if (commaErrors.Count == ERROR_CELL_COUNT)
                        {
                            checkCommaErrors = false;
                        }
                    }

                    // Check for special characters
                    if (checkSpecialCharacterErrors && CellHasSpecialCharacters(cell))
                    {
                        specialCharactersErrors.Add(cell.ExcelCell.CellReference.Value);
                        if (specialCharactersErrors.Count == ERROR_CELL_COUNT)
                        {
                            checkSpecialCharacterErrors = false;
                        }
                    }

                    // check for style errors
                    if (checkStyleErrors && CellHasStyleErrors(cell, sharedFormatList))
                    {
                        styleErrors.Add(cell.ExcelCell.CellReference.Value);
                        if (styleErrors.Count == ERROR_CELL_COUNT)
                        {
                            checkStyleErrors = false;
                        }
                    }

                    // break out of loop if the error count for each type of error has reached 
                    // the max limit.
                    if (!checkCommaErrors && !checkSpecialCharacterErrors && !checkStyleErrors)
                    {
                        break;
                    }
                }

                // add comma errors to the return list
                if (commaErrors.Count > 0)
                {
                    fileErrors.Add(new FileError()
                    {
                        ErrorType = ErrorType.Commas,
                        Title = Messages.Commas,
                        ErrorAddress = string.Join(",", commaErrors),
                        CanFix = false,
                        Description = Messages.CommasDescription,
                        Recommendation = Messages.CommasRecommendation
                    });
                }

                // add special character errors to the return list
                if (specialCharactersErrors.Count > 0)
                {
                    fileErrors.Add(new FileError()
                    {
                        ErrorType = ErrorType.SpecialCharacter,
                        Title = Messages.SpecialCharacters,
                        ErrorAddress = string.Join(",", specialCharactersErrors),
                        CanFix = false,
                        Description = Messages.SpecialCharactersDescription,
                        Recommendation = Messages.SpecialCharactersRecommendation
                    });
                }

                // add style errors to the return list
                if (styleErrors.Count > 0)
                {
                    fileErrors.Add(new FileError()
                    {
                        ErrorType = ErrorType.ColorCoded,
                        Title = Messages.ColorCodedTextOrCellShading,
                        ErrorAddress = string.Join(",", styleErrors),
                        CanFix = true,
                        Description = Messages.ColorCodedTextOrCellShadingDescription,
                        Recommendation = Messages.ColorCodedTextOrCellShadingRecommendation
                    });
                }

                /*********************************************************
                 * Commenting the mixed data type error check below. As the '
                 * implementation is cumbersome and performs poorly. The 
                 * correct solution is TBD.
                 * *******************************************************/
                //var mixedDataTypeErrors = RetrieveMixedDataTypeErrors(part, cells, sheetName, sheetId, sharedFormatList);
                //fileErrors.AddRange(mixedDataTypeErrors);

                cells = null;
            }

            return fileErrors;
        }

        private static bool CellHasCommas(SheetCell cell)
        {
            bool hasCommas = false;
            if (!string.IsNullOrWhiteSpace(cell.Value.ToString()) && cell.Value.Contains(','))
            {
                hasCommas = true;
            }
            return hasCommas;
        }

        private static bool CellHasSpecialCharacters(SheetCell cell)
        {
            bool hasSpecialCharacters = false;
            if (regkey.IsMatch(cell.Value))
            {
                hasSpecialCharacters = true;
            }
            return hasSpecialCharacters;
        }

        private static bool CellHasStyleErrors(SheetCell cell, Dictionary<int, CellFormat> sharedFormatList)
        {
            bool hasStyleError = false;
            if (cell.ExcelCell.StyleIndex == null)
            {
                return hasStyleError;
            }

            if (cell.ExcelCell.CellValue == null && cell.ExcelCell.StyleIndex == 0)
            {
                hasStyleError = true;
            }
            else if (cell.ExcelCell.CellValue != null || cell.ExcelCell.StyleIndex >= 0)
            {
                CellFormat cellformate = sharedFormatList[Convert.ToInt32(cell.ExcelCell.StyleIndex.Value)];

                if ((cellformate.FontId != null && cellformate.FontId > 0 && cell.ExcelCell.DataType != null) ||
                                            (cellformate.FillId != null && cellformate.FillId > 0))
                {
                    hasStyleError = true;
                }
            }

            return hasStyleError;
        }

        /// <summary>
        /// Retrieves the mixed data type errors.
        /// </summary>
        /// <param name="part">The Worksheet part.</param>
        /// <param name="cells">The cells.</param>
        /// <param name="cellFormats">The cell formats.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="sheetId">The sheet id.</param>
        /// <returns>List of mixed datatype errors</returns>
        private static IList<FileError> RetrieveMixedDataTypeErrors(WorksheetPart part, IEnumerable<SheetCell> cells, string sheetName, string sheetId, Dictionary<int, CellFormat> sharedFormatList)
        {
            List<FileError> fileErrors = new List<FileError>();
            string[] bounds = GetWorkSheetBounds(part);
            StringBuilder sbAddress = new StringBuilder();
            int count = 0;

            string[] columnNames;
            var columnNamesList = GetColumnNames();
            int startIndex = columnNamesList.IndexOf(bounds[1]);
            int endIndex = columnNamesList.IndexOf(bounds[3]);
            if (endIndex > startIndex)
            {
                columnNames = new string[(endIndex - startIndex) + 1];
                columnNamesList.CopyTo(startIndex, columnNames, 0, (endIndex - startIndex) + 1);
            }
            else
            {
                if (String.Compare(bounds[1], bounds[3]) != 0)
                {
                    columnNames = new string[2];
                    columnNames[0] = bounds[1];
                    columnNames[1] = bounds[3];
                }
                else
                {
                    columnNames = new string[1];
                    columnNames[0] = bounds[1];
                }
            }

            var groupedColumn = from val in cells
                                group val by val.ColumnName into columns
                                select new
                                {
                                    Key = columns.Key,
                                    Count = columns.Count(),
                                    GroupedCols = columns
                                };

            foreach (var columns in groupedColumn)
            {
                string type = string.Empty;
                string typesec = string.Empty;

                foreach (SheetCell cell in columns.GroupedCols)
                {
                    if (cell.ExcelCell.CellValue == null)
                    {
                        if (cell.ExcelCell.StyleIndex != null && cell.ExcelCell.StyleIndex.Value > 0)
                        {
                            cell.ExcelCell.StyleIndex.Value = 0;
                        }
                        continue;
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(type) && cell.ExcelCell.CellValue != null)
                        {
                            type = GetCelltype(type, cell.ExcelCell, sharedFormatList);
                        }
                        else
                        {
                            if (cell.ExcelCell.CellValue != null)
                            {
                                typesec = GetCelltype(typesec, cell.ExcelCell, sharedFormatList);

                                if (type != typesec)
                                {
                                    sbAddress.Append(cell.ExcelCell.CellReference.Value);
                                    sbAddress.Append(",");
                                    count++;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (count > ERROR_CELL_COUNT)
                {
                    break;
                }
            }

            if (count > 0)
            {
                fileErrors.Add(new FileError()
                {
                    ErrorType = ErrorType.MixedType,
                    Title = Messages.ColumnsHaveMixedDataTypes,
                    ErrorAddress = sbAddress.ToString(),
                    CanFix = false,
                    Description = Messages.ColumnsHaveMixedDataTypesDescription,
                    Recommendation = Messages.ColumnsHaveMixedDataTypesRecommendation
                });
            }

            int rows = (Convert.ToInt32(bounds[2]) - Convert.ToInt32(bounds[0]));
            rows = (rows > 0) ? rows + 1 : 1;
            count = 0;
            sbAddress = null;
            int blankCellCount = 0;
            count = columnNames.Count() - groupedColumn.Count();
            if (count > 0)
            {
                blankCellCount = blankCellCount + (rows * count);
            }

            foreach (var val in groupedColumn)
            {
                if (val.Count < rows)
                {
                    blankCellCount = blankCellCount + (rows - val.Count);

                    if (++count > ERROR_CELL_COUNT)
                    {
                        break;
                    }
                }
            }

            if (count > 0)
            {
                fileErrors.Add(new FileError()
                {
                    ErrorType = ErrorType.BlankCell,
                    Title = Messages.BlankCells,
                    ErrorAddress = Messages.TotalCount + ": " + ((blankCellCount > ERROR_CELL_COUNT) ? blankCellCount.ToString() + "+" : blankCellCount.ToString()),
                    CanFix = false,
                    Description = Messages.BlankCellsDescription,
                    Recommendation = Messages.BlankCellsRecommendation
                });
            }

            groupedColumn = null;
            var groupedRows = from val in cells
                              group val by val.RowIndex into values
                              select new
                              {
                                  Key = values.Key,
                                  Count = values.Count()
                              };

            int colCount = columnNames.Count();
            colCount = (colCount > 0) ? colCount : 1;
            foreach (var excelRow in groupedRows)
            {
                if (excelRow.Count < colCount)
                {
                    if (++count > ERROR_CELL_COUNT)
                    {
                        break;
                    }
                }
            }

            if (count > 0)
            {
                fileErrors.Add(new FileError()
                {
                    ErrorType = ErrorType.Noncontiguous,
                    Title = Messages.NonContiguousData,
                    ErrorAddress = Messages.TotalCount + ": " + ((count > ERROR_CELL_COUNT) ? count.ToString() + "+" : count.ToString()),
                    CanFix = false,
                    Description = Messages.NonContiguousDataDescription,
                    Recommendation = Messages.NonContiguousDataRecommendation
                });
            }

            return fileErrors;
        }

        /// <summary>
        /// Gets the work sheet bounds.
        /// </summary>
        /// <param name="part">The Worksheet part.</param>
        /// <returns></returns>
        private static string[] GetWorkSheetBounds(WorksheetPart part)
        {
            string bounds = part.Worksheet.SheetDimension.Reference;
            string[] sheetBounds = bounds.Split(':').ToArray();

            int firstRow = GetRowIndex(sheetBounds[0]);
            string firstCol = GetColumnName(sheetBounds[0]);
            int lastRow = 1;
            string lastCol = "A";
            if (sheetBounds.Length > 1)
            {
                lastRow = GetRowIndex(sheetBounds[1]);
                lastCol = GetColumnName(sheetBounds[1]);
            }

            return new string[] { firstRow.ToString(), firstCol, lastRow.ToString(), lastCol };
        }

        /// <summary>
        /// Gets the column names.
        /// </summary>
        /// <param name="lastColumn">The last column.</param>
        /// <returns>List of Column Names</returns>
        private static List<string> GetColumnNames(string lastColumn = "XFD")
        {
            List<string> columns = new List<string>();
            string[] columnChars = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            foreach (string column in columnChars)
            {
                columns.Add(column);
                if ((column.Equals(lastColumn, StringComparison.OrdinalIgnoreCase)))
                {
                    return columns;
                }
            }

            for (int i = 0; i <= columnChars.Length - 1; i++)
            {
                for (int j = 0; j <= columnChars.Length - 1; j++)
                {
                    columns.Add(string.Format("{0}{1}", columnChars[i], columnChars[j]));

                    if ((columns[columns.Count - 1].Equals(lastColumn, StringComparison.OrdinalIgnoreCase)))
                    {
                        return columns;
                    }
                }
            }

            for (int i = 26; i <= columns.Count - 1; i++)
            {
                for (int j = 0; j <= columnChars.Length - 1; j++)
                {
                    columns.Add(string.Format("{0}{1}", columns[i], columns[j]));

                    if ((columns[columns.Count - 1].Equals(lastColumn, StringComparison.OrdinalIgnoreCase)))
                    {
                        return columns;
                    }
                }
            }

            return columns;
        }

        /// <summary>
        /// Gets the celltype.
        /// </summary>
        /// <param name="cellFormats">The cell formats.</param>
        /// <param name="type">The type.</param>
        /// <param name="cellsInColumn">The cells in column.</param>
        /// <returns>The cell type as string</returns>
        private static string GetCelltype(string type, Cell cellsInColumn, Dictionary<int, CellFormat> sharedFormatList)
        {
            if (cellsInColumn.DataType == null)
            {
                if (cellsInColumn.StyleIndex != null)
                {
                    //CellFormat cf = cellFormats.Descendants<CellFormat>().ElementAt<CellFormat>(Convert.ToInt32(cellsInColumn.StyleIndex.Value));
                    CellFormat cf = sharedFormatList[Convert.ToInt32(cellsInColumn.StyleIndex.Value)];
                    //this is for number
                    if ((cf.NumberFormatId != null) && cf.NumberFormatId >= 0 && cf.NumberFormatId <= 13)
                    {
                        if (reg1.IsMatch(cellsInColumn.CellValue.Text))
                        {
                            decimal decimalCellValue = 0.0M;
                            Decimal.TryParse(cellsInColumn.CellValue.Text, out decimalCellValue);
                            var valtxt = decimalCellValue;
                            type = valtxt.GetType().FullName;
                        }
                        else
                        {
                            if (regForVal.IsMatch(cellsInColumn.CellValue.Text))
                            {
                                int intCellValue = 0;
                                int.TryParse(cellsInColumn.CellValue.Text, out intCellValue);
                                var valtxt = intCellValue;// int.Parse(cellsInColumn.CellValue.Text);
                                type = valtxt.GetType().FullName;

                            }
                        }
                    }
                    //this is for date
                    else if ((cf.NumberFormatId != null) && cf.NumberFormatId >= 14 && cf.NumberFormatId <= 22)
                    {
                        var valdate = DateTime.FromOADate(Convert.ToDouble(cellsInColumn.CellValue.Text));
                        type = valdate.GetType().FullName;
                    }
                    else
                    {
                        //this is for some text
                        var valtxt = cellsInColumn.CellValue.Text;
                        type = valtxt.GetType().FullName;
                    }
                }
                else
                {

                    if (reg1.IsMatch(cellsInColumn.CellValue.Text))
                    {
                        decimal decimalCellValue = 0.0M;
                        Decimal.TryParse(cellsInColumn.CellValue.Text, out decimalCellValue);
                        var valtxt = decimalCellValue;//                       Convert.ToDecimal(cellsInColumn.CellValue.Text);
                        type = valtxt.GetType().FullName;
                    }
                    else
                    {
                        if (regForVal.IsMatch(cellsInColumn.CellValue.Text))
                        {
                            int intCellValue = 0;
                            int.TryParse(cellsInColumn.CellValue.Text, out intCellValue);
                            var valtxt = intCellValue;// int.Parse(cellsInColumn.CellValue.Text);
                            type = valtxt.GetType().FullName;
                        }
                    }
                }
            }
            else
            {
                switch (cellsInColumn.DataType.Value)
                {
                    case CellValues.SharedString:
                        string sharedstring = " ";
                        type = sharedstring.GetType().FullName;
                        break;
                    case CellValues.Boolean:
                        var boolval = cellsInColumn.CellValue.Text == "1" ? true : false;
                        type = boolval.GetType().FullName;
                        break;
                    case CellValues.Date:
                        var dateval = DateTime.FromOADate(Convert.ToDouble(cellsInColumn.CellValue.Text));
                        type = dateval.GetType().FullName;
                        break;
                    case CellValues.Number:
                        decimal decimalCellValue = 0.0M;
                        Decimal.TryParse(cellsInColumn.CellValue.Text, out decimalCellValue);
                        var numval = decimalCellValue;
                        type = numval.GetType().FullName;
                        break;
                    default:
                        if (cellsInColumn.CellValue != null)
                        {
                            var somtxt = cellsInColumn.CellValue.Text;
                            if (somtxt != null)
                            {
                                type = somtxt.GetType().FullName;
                            }
                        }
                        break;
                }
            }
            return type;
        }

        /// <summary>
        /// Method to get column location index
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private static int GetColumnLocationIndex(string location)
        {
            int offset = location.Length;
            for (int i = location.Length - 1; i >= 0; i--)
            {
                char c = location[i];
                if (char.IsDigit(c))
                {
                    offset--;
                }
                else
                {
                    if (offset == location.Length)
                    {
                        // No int at the end
                        return -1;
                    }

                    return int.Parse(location.Substring(offset), CultureInfo.InvariantCulture);
                }
            }

            return int.Parse(location.Substring(offset), CultureInfo.InvariantCulture);
        }

        private static Dictionary<int, string> CreateSharedStringList(SharedStringTable table)
        {
            Dictionary<int, string> sharedStringList = new Dictionary<int, string>();
            int itemIndex = 0;
            foreach (SharedStringItem item in table)
            {
                sharedStringList.Add(itemIndex++, item.InnerText);
            }

            return sharedStringList;
        }

        /// <summary>
        /// Removes comments in a excel sheet.
        /// </summary>
        /// <param name="worksheetPart">WorksheetPart</param>
        private static void RemoveComments(WorksheetPart worksheetPart)
        {
            WorksheetCommentsPart commentsPart = worksheetPart.WorksheetCommentsPart;
            if (commentsPart != null)
            {
                worksheetPart.DeletePart(commentsPart);
            }
        }

        /// <summary>
        /// Removes charts in a excel sheet.
        /// </summary>
        /// <param name="worksheetPart">WorksheetPart</param>
        private static void RemoveCharts(WorksheetPart worksheetPart)
        {
            DrawingsPart drawingsPart = worksheetPart.DrawingsPart;
            if (drawingsPart != null)
            {
                ChartPart[] chartParts = new ChartPart[drawingsPart.ChartParts.Count()];
                drawingsPart.ChartParts.ToList().CopyTo(chartParts);
                foreach (ChartPart chartPart in chartParts)
                {
                    drawingsPart.DeletePart(chartPart);
                }

                foreach (GraphicFrame graphicFrame in drawingsPart.WorksheetDrawing.Descendants<GraphicFrame>())
                {
                    graphicFrame.Parent.Remove();
                }
            }
        }

        /// <summary>
        /// Removes shapes in a excel sheet.
        /// </summary>
        /// <param name="worksheetPart">WorksheetPart</param>
        private static void RemoveShapes(WorksheetPart worksheetPart)
        {
            var shapes = worksheetPart.DrawingsPart.WorksheetDrawing.Descendants<ss.Shape>();
            foreach (ss.Shape shape in shapes)
            {
                shape.Parent.Remove();
            }
        }

        /// <summary>
        /// Removes merge cells in a excel sheet.
        /// </summary>
        /// <param name="worksheetPart">WorksheetPart</param>
        private static void RemoveMergeCells(WorksheetPart worksheetPart)
        {
            var mergcells = worksheetPart.Worksheet.Descendants<MergeCells>();
            foreach (var mergecell in mergcells)
            {
                mergecell.RemoveAllChildren();
                mergecell.Remove();
            }
        }

        /// <summary>
        /// Removes color cells in a excel sheet.
        /// </summary>
        /// <param name="worksheetPart">WorksheetPart</param>
        /// <param name="workbookPart">WorkbookPart</param>
        private static void RemoveColorCells(WorksheetPart worksheetPart, WorkbookPart workbookPart)
        {
            CellFormats cellFormats = (CellFormats)workbookPart.WorkbookStylesPart.Stylesheet.CellFormats;
            SheetData sheetdata = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();
            foreach (var row in sheetdata.Descendants<Row>())
            {
                foreach (Cell cell in row.Descendants<Cell>())
                {
                    if (cell.StyleIndex != null)
                    {
                        if ((cell.CellValue == null && cell.StyleIndex == 0) || (cell.CellValue != null || cell.StyleIndex >= 0))
                        {
                            CellFormat cellformate = cellFormats.Descendants<CellFormat>().ElementAt<CellFormat>(Convert.ToInt32(cell.StyleIndex.Value));
                            cellformate.FontId = null;
                            cellformate.FillId = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes pictures in a excel sheet
        /// </summary>
        /// <param name="worksheetPart">WorksheetPart</param>
        private static void RemovePictures(WorksheetPart worksheetPart)
        {
            ImagePart[] imageParts = new ImagePart[worksheetPart.DrawingsPart.ImageParts.Count()];
            worksheetPart.DrawingsPart.ImageParts.ToList().CopyTo(imageParts);
            foreach (var imagePart in imageParts)
            {
                worksheetPart.DrawingsPart.DeletePart(imagePart);
            }

            foreach (var val in worksheetPart.DrawingsPart.WorksheetDrawing.Descendants<ss.Picture>())
            {
                val.Parent.Remove();
            }
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <param name="cellName">Name of the cell.</param>
        /// <returns></returns>
        private static string GetColumnName(string cellName)
        {
            if (!String.IsNullOrWhiteSpace(cellName))
            {
                Match match = regColumnName.Match(cellName);
                return match.Value;
            }
            return null;
        }

        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        /// <param name="cellName">Name of the cell.</param>
        /// <returns>row index as int</returns>
        private static int GetRowIndex(string cellName)
        {
            //Match match = Regex.Match(cellName, "\\d+");
            Match match = regRow.Match(cellName);
            int rowIndex = 0;
            return int.TryParse(match.Value, out rowIndex) ? rowIndex : -1;
        }

        #endregion

    }

    public class ExcelColumnValues
    {
        public ExcelColumnValues()
        {
            Header = string.Empty;
            cellValues = new List<CellValueWithFormat>();
        }

        public string Header { get; set; }
        public List<CellValueWithFormat> cellValues { get; set; }
    }

    public class CellValueWithFormat
    {
        public string Value { get; set; }
        public uint? Format { get; set; }
    }
}
