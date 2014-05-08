// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.FileProcesser.Extensions;
using Microsoft.Research.DataOnboarding.FileService.Helper;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Extensions;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DM = Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.FileService.FileProcesser
{
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public class ExcelFileProcesser : FileProcessor, IFileProcesser
    {
        private const bool ErrorStatus = false;
        private const bool SuccessStatus = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelFileProcesser" /> class.
        /// </summary>
        /// <param name="blobDataRepository">Blob repository object.</param>
        public ExcelFileProcesser(IBlobDataRepository blobDataRepository,
                            IFileRepository fileDataRepository,
                            IRepositoryService repositoryService)
            : base(blobDataRepository, fileDataRepository, repositoryService)
        {
        }

        /// <summary>
        /// Method to Download document
        /// </summary>
        /// <param name="fileDetails">File object</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public DataDetail DownloadDocument(DM.File fileDetails)
        {
            Check.IsNotNull<DM.File>(fileDetails, "filetoDownload");
            Check.IsNotNull<ICollection<FileMetadataField>>(fileDetails.FileMetadataFields, "fileMetadata");

            DataDetail dataDetail = new DataDetail();
            dataDetail.FileDetail = fileDetails;

            // Update the metadata to stream, create one more metasheet tab in the file
            dataDetail = this.UpdateMetaDataSheetForExcel(fileDetails);
            dataDetail.MimeTypeToDownLoad = fileDetails.MimeType;
            dataDetail.FileNameToDownLoad = fileDetails.Name;

            return dataDetail;
        }

        public async Task<IEnumerable<ColumnLevelMetadata>> GetColumnMetadataFromFile(DomainModel.File fileDetail)
        {
            Check.IsNotNull<DomainModel.File>(fileDetail, "fileDetail");
            IEnumerable<ColumnLevelMetadata> columnLevelMetadataList = new List<ColumnLevelMetadata>();

            await Task.Factory.StartNew(() =>
            {
                var fileArray = base.GetFileContentsAsByteArray(fileDetail.BlobId);
                SpreadsheetDocument excelDocument;
                using (var documentStream = new MemoryStream(fileArray))
                {
                    excelDocument = SpreadsheetDocument.Open(documentStream, false);
                    columnLevelMetadataList = ExcelFileHelper.GetColumnMetadataForAllSheets(excelDocument);
                }
            });

            return columnLevelMetadataList;
        }

        /// <summary>
        /// Method to get the list of document sheet information
        /// </summary>
        /// <param name="fileDetail">fileDetail object</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public async Task<IEnumerable<FileSheet>> GetDocumentSheetDetails(DM.File fileDetail)
        {
            Check.IsNotNull<DM.File>(fileDetail, "fileDetail");
            List<FileSheet> fileSheets = new List<FileSheet>();

            await Task.Factory.StartNew(() =>
            {
                var fileArray = base.GetFileContentsAsByteArray(fileDetail.BlobId);
                SpreadsheetDocument excelDocument;
                using (var documentStream = new MemoryStream(fileArray))
                {
                    excelDocument = SpreadsheetDocument.Open(documentStream, false);
                    IEnumerable<Sheet> sheets = excelDocument.WorkbookPart.Workbook.Descendants<Sheet>();
                    foreach (Sheet item in sheets)
                    {
                        if (ExcelFileHelper.CheckSheetForData(excelDocument, item.Id))
                        {
                            fileSheets.Add(new FileSheet()
                            {
                                SheetName = item.Name,
                                SheetId = ((Path.GetExtension(fileDetail.Name) == Constants.CSVFileExtension) ? item.Name : item.Id)
                            });
                        }
                    }
                }
            });

            return fileSheets;
        }

        /// <summary>
        /// Method to retrieve quality check issues
        /// </summary>
        /// <param name="fileDetail">File Detail</param>
        /// <param name="qualityCheck">Quality check </param>
        /// <param name="sheetIds">Sheet Ids</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public async Task<IEnumerable<QualityCheckResult>> GetQualityCheckIssues(DM.File fileDetail, DM.QualityCheck qualityCheck, IEnumerable<QualityCheckColumnType> qualityCheckTypes, string sheetIds)
        {
            IEnumerable<QualityCheckResult> qualityCheckResults = new List<QualityCheckResult>();

            await Task.Factory.StartNew(() =>
            {
                // Get the stream
                var byteArray = base.GetFileContentsAsByteArray(fileDetail.BlobId);

                using (var documentStream = new MemoryStream(byteArray))
                {
                    //If it is EXCEL file ,convert the data to excel stream
                    if (Path.GetExtension(fileDetail.Name) == Constants.XLFileExtension)
                    {
                        // Check and set the quality check condistions
                        qualityCheckResults = GetQualityCheckRulesForExcel(documentStream, sheetIds, qualityCheck, qualityCheckTypes);
                    }
                }
            });

            return qualityCheckResults;
        }

        /// <summary>
        /// Method to update metadata in file
        /// </summary>
        /// <param name="fileDetails">file Detail object</param>
        /// <returns>returns boolean status</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times"), SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling"), SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public DataDetail UpdateMetaDataSheetForExcel(DM.File fileDetails)
        {
            Check.IsNotNull(fileDetails, "fileDetails");

            DataDetail dataDetail = new DataDetail();
            dataDetail.FileDetail = fileDetails;
            Stream stream = null;
            try
            {
                stream = GetFileStream(fileDetails.BlobId);

                // do not processor further as the file is not associates with repository.
                if (fileDetails.Status == FileStatus.Uploaded.ToString() || fileDetails.RepositoryId == null || fileDetails.RepositoryId <= 0)
                {
                    dataDetail.DataStream = stream.GetBytes();
                    return dataDetail;
                }

                //FOR CSV, AT THE TIME OF PUBLISH ,ONE MORE CSV SHEET WILL BE CRATED AND SENT AS ZIP
                using (SpreadsheetDocument excelDocument = SpreadsheetDocument.Open(stream, true))
                {
                    Sheet metadataSheet = null;

                    var repositoryMetadata = base.RepositoryService.GetRepositoryById((int)fileDetails.RepositoryId).RepositoryMetadata.FirstOrDefault();
                    ICollection<RepositoryMetadataField> repositoryMetadataFields = null;
                    if (repositoryMetadata != null)
                    {
                        repositoryMetadataFields = repositoryMetadata.RepositoryMetadataFields;
                    }

                    if ((repositoryMetadata != null && repositoryMetadataFields != null) || (fileDetails.FileColumns != null && fileDetails.FileColumns.Any()))
                    {
                        //create the sheet in any case if filecolumns exists from db or it was already existing in the file
                        metadataSheet = excelDocument.InsertMetadataWorksheet(fileDetails.FileMetadataFields.Any(), fileDetails.FileColumns.Any());
                    }

                    if (metadataSheet != null)
                    {
                        var fileColumnUnits = base.FileDataRepository.RetrieveFileColumnUnits();
                        var fileColumnTypes = base.FileDataRepository.RetrieveFileColumnTypes();
                        char column = 'A';

                        int fileMetaDataIndexer = 0;
                        if (repositoryMetadataFields != null && repositoryMetadataFields.Any() && fileDetails.FileMetadataFields != null && fileDetails.FileMetadataFields.Any())
                        {
                            foreach (var repositoryMetaData in repositoryMetadataFields)
                            {
                                // first write the repository metadata
                                excelDocument.SetCellValue(metadataSheet, string.Format(CultureInfo.InvariantCulture, "A{0}", fileMetaDataIndexer + 2), repositoryMetaData.Name);

                                // write teh metadata value if it exists
                                var fileMetaDataField = fileDetails.FileMetadataFields.Where(fm => fm.RepositoryMetadataFieldId == repositoryMetaData.RepositoryMetadataFieldId).FirstOrDefault();

                                if (fileMetaDataField != null)
                                {
                                    excelDocument.SetCellValue(metadataSheet, string.Format(CultureInfo.InvariantCulture, "B{0}", fileMetaDataIndexer + 2), fileMetaDataField.MetadataValue);
                                }

                                fileMetaDataIndexer = fileMetaDataIndexer + 1;
                            }

                            column = 'D';
                        }

                        int cellCount = 2;
                        if (fileDetails.FileColumns != null && fileDetails.FileColumns.Any())
                        {
                            char entityName = column, entityDescription = (char)(column + 1), name = (char)(column + 2), description = (char)(column + 3), fileType = (char)(column + 4), fileUnit = (char)(column + 5);

                            foreach (var val in fileDetails.FileColumns)
                            {
                                excelDocument.SetCellValue(metadataSheet, string.Format(CultureInfo.InvariantCulture, "{0}{1}", entityName, cellCount), val.EntityName);
                                excelDocument.SetCellValue(metadataSheet, string.Format(CultureInfo.InvariantCulture, "{0}{1}", entityDescription, cellCount), val.EntityDescription);
                                excelDocument.SetCellValue(metadataSheet, string.Format(CultureInfo.InvariantCulture, "{0}{1}", name, cellCount), val.Name);
                                excelDocument.SetCellValue(metadataSheet, string.Format(CultureInfo.InvariantCulture, "{0}{1}", description, cellCount), val.Description);
                                if (val.FileColumnTypeId != null && val.FileColumnUnitId != 0 && fileColumnTypes != null)
                                {
                                    excelDocument.SetCellValue(metadataSheet, string.Format(CultureInfo.InvariantCulture, "{0}{1}", fileType, cellCount), fileColumnTypes.Where(fc => fc.FileColumnTypeId == val.FileColumnTypeId).FirstOrDefault().Name);
                                }
                                else
                                {
                                    excelDocument.SetCellValue(metadataSheet, string.Format(CultureInfo.InvariantCulture, "{0}{1}", fileType, cellCount), string.Empty);
                                }
                                if (val.FileColumnUnitId != null && val.FileColumnUnitId != 0 && fileColumnUnits != null)
                                {
                                    excelDocument.SetCellValue(metadataSheet, string.Format(CultureInfo.InvariantCulture, "{0}{1}", fileUnit, cellCount), fileColumnUnits.Where(fu => fu.FileColumnUnitId == val.FileColumnUnitId).FirstOrDefault().Name);
                                }
                                else
                                {
                                    excelDocument.SetCellValue(metadataSheet, string.Format(CultureInfo.InvariantCulture, "{0}{1}", fileUnit, cellCount), string.Empty);
                                }
                                cellCount++;
                            }
                        }

                        WorksheetPart worksheetPart = excelDocument.WorkbookPart.GetPartById(metadataSheet.Id) as WorksheetPart;
                        if (worksheetPart != null)
                        {
                            TableDefinitionPart metadataTable = null;
                            TableDefinitionPart parameterTable = null;

                            // get the tables in Excel
                            var tableParts = worksheetPart.GetPartsOfType<TableDefinitionPart>();
                            if (tableParts != null)
                            {
                                foreach (var item in tableParts)
                                {
                                    if (item.Table.Name == Constants.MetadataRangeName)
                                    {
                                        metadataTable = item;
                                        if (fileDetails.FileMetadataFields != null && fileDetails.FileMetadataFields.Any())
                                        {
                                            metadataTable.Table.Reference = "A1:B" + (fileDetails.FileMetadataFields.Count + 1);
                                        }

                                        metadataTable.Table.Save();
                                    }

                                    if (item.Table.Name == Constants.ParaMetadataRangeName && fileDetails.FileColumns.Any())
                                    {
                                        parameterTable = item;
                                        if (fileDetails.FileColumns != null && fileDetails.FileColumns.Any() && fileDetails.FileMetadataFields.Any())
                                        {
                                            parameterTable.Table.Reference = string.Format(CultureInfo.InvariantCulture, "{0}1:{1}{2}", column, (char)(column + 5), fileDetails.FileColumns.Count + 1);
                                        }

                                        parameterTable.Table.Save();
                                    }
                                }
                            }
                        }

                        excelDocument.WorkbookPart.Workbook.Save();
                    }
                }

                dataDetail.DataStream = stream.GetBytes();
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
            return dataDetail;
        }

        /// <summary>
        /// Method to get the errors available on the input excel file.
        /// </summary>
        /// <param name="file">File</param>
        /// <returns>List of sheet with errors.</returns>
        public async Task<IList<FileSheet>> GetErrors(DM.File file)
        {
            var fileStream = GetFileStream(file.BlobId);
            using (SpreadsheetDocument excelDocument = SpreadsheetDocument.Open(fileStream, true))
            {
                return await ExcelFileHelper.ValidateGraphics(excelDocument);
            }
        }

        /// <summary>
        /// Removes an error from a file sheet.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="sheetName">Sheet Name</param>
        /// <param name="errorTypes">Error types</param>
        /// <returns>Stream of the file.</returns>
        public async Task RemoveError(Stream stream, string sheetName, IEnumerable<ErrorType> errorTypes)
        {
            using (SpreadsheetDocument excelDocument = SpreadsheetDocument.Open(stream, true))
            {
                foreach (var errorType in errorTypes)
                {
                    await ExcelFileHelper.RemoveError(excelDocument, sheetName, errorType);
                    excelDocument.WorkbookPart.Workbook.Save();
                }
            }
        }

        #region private Methods

        /// <summary>
        /// Method to get quality check validation conditions for excel object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="sheetIds">Sheet Ids</param>
        /// <param name="qualityCheck">Quality Check</param>
        /// <param name="qualityCheckTypes">Quality Check type</param>
        /// <returns>List of QualityCheckResult</returns>
        private IEnumerable<QualityCheckResult> GetQualityCheckRulesForExcel(Stream stream, string sheetIds, DM.QualityCheck qualityCheck, IEnumerable<QualityCheckColumnType> qualityCheckTypes)
        {
            List<QualityCheckResult> qualityCheckResults = new List<QualityCheckResult>();

            using (SpreadsheetDocument excelDocument = SpreadsheetDocument.Open(stream, false))
            {
                foreach (var sheetId in GetSheetIds(sheetIds))
                {
                    var currentSheet = excelDocument.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Id == sheetId);

                    IEnumerable<SheetCell> headersAndColumns = new List<SheetCell>();
                    if (excelDocument.WorkbookPart != null)
                    {
                        headersAndColumns = ExcelFileHelper.GetHeadersAndColumns(excelDocument, sheetId);
                    }

                    var headers = headersAndColumns.Where(c => c.RowIndex == 1).ToList();
                    var columns = headersAndColumns.Where(c => c.RowIndex != 1).ToList();

                    QualityCheckResult qualityCheckResult = new QualityCheckResult()
                    {
                        SheetId = currentSheet.Id,
                        SheetName = currentSheet.Name
                    };
                    qualityCheckResults.Add(qualityCheckResult);

                    GetHeaderIssues(qualityCheck, headers, qualityCheckResult);
                    GetColumnIssues(qualityCheck, qualityCheckTypes, headers, columns, qualityCheckResult);
                }
            }

            return qualityCheckResults;
        }

        /// <summary>
        /// Method to check header rules or matching
        /// </summary>
        /// <param name="qualityCheck">Quality check</param>
        /// <param name="headers">Headers</param>
        /// <param name="qualityCheckResult">Quality check result</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void GetHeaderIssues(DM.QualityCheck qualityCheck, IEnumerable<SheetCell> headers, QualityCheckResult qualityCheckResult)
        {
            var isValidFile = true;
            var headerIssueExists = false;

            //Check the headers are there or not
            if (headers != null && headers.Any())
            {
                // CHECK THE ITEMS ARE STARTING FROM THE A1
                if (headers.FirstOrDefault().ColumnLocation != "A1")
                {
                    isValidFile = false;
                    headerIssueExists = true;
                    qualityCheckResult.Errors.Add(string.Concat("Invalid file, headers should start from A1 location"));
                }
            }
            else
            {
                isValidFile = false;
                headerIssueExists = true;
                foreach (var qcName in qualityCheck.QualityCheckColumnRules)
                {
                    qualityCheckResult.Errors.Add(string.Format(CultureInfo.CurrentCulture, "Header '{0}' is missing", qcName.HeaderName));
                }
            }

            if (!isValidFile)//If it is valid file ,then only check the other conditions
            {
                return;
            }

            SheetCell header;
            //first check all the name exists or not and if it is required show the missing message
            foreach (var qcName in qualityCheck.QualityCheckColumnRules)
            {
                header = headers.Where(h => h.Value.Trim().Equals(qcName.HeaderName.Trim(), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (header == null && qcName.IsRequired == true)
                {
                    headerIssueExists = true;
                    qualityCheckResult.Errors.Add(string.Format(CultureInfo.CurrentCulture, "Header '{0}' is missing", qcName.HeaderName));
                }
            }

            // If enforcecheck order is true ,then append the message
            if (headerIssueExists || qualityCheck.EnforceOrder == false)
            {
                return;
            }

            var qualityCheckColumnRules = qualityCheck.QualityCheckColumnRules.OrderBy(col => col.Order);
            List<int> columnIndices = new List<int>();
            foreach (var qcName in qualityCheckColumnRules)
            {
                header = headers.Where(h => string.Compare(h.Value.Trim(), qcName.HeaderName.Trim(), StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();
                if (header != null)
                {
                    var columnLocationIndex = GetColumnLocationIndex(header.Value, headers);
                    columnIndices.Add(columnLocationIndex);
                }
            }

            // Cheking hte order of the columns
            if (!Utilities.Helper.CheckIntListAscOrder(columnIndices))
            {
                headerIssueExists = true;
                qualityCheckResult.Errors.Add(string.Format(CultureInfo.CurrentCulture, "Headers are not in order"));
            }
        }

        /// <summary>
        /// Method to get column issues
        /// </summary>
        /// <param name="qualityCheck">Qulity check</param>
        /// <param name="qualityCheckTypes">Quality check types</param>
        /// <param name="headers">Headers</param>
        /// <param name="columns">Columns</param>
        /// <param name="qualityCheckResult">Quality check result</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void GetColumnIssues(DM.QualityCheck qualityCheck, IEnumerable<QualityCheckColumnType> qualityCheckTypes, IEnumerable<SheetCell> headers, IEnumerable<SheetCell> columns, QualityCheckResult qualityCheckResult)
        {
            if (columns == null || !columns.Any())
            {
                qualityCheckResult.Errors.Add(string.Format(CultureInfo.CurrentCulture, "No data except Headers"));
                return;
            }

            foreach (var headerColumn in headers)
            {
                //Logic for aliases
                QualityCheckColumnRule qualityCheckItem = null;
                foreach (var item in qualityCheck.QualityCheckColumnRules)
                {
                    // get the quality check item   
                    if (item.HeaderName.Trim().Equals(headerColumn.Value.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        qualityCheckItem = item;
                        break;
                    }
                }

                if (qualityCheckItem == null)
                {
                    continue;
                }

                // get the column index of the header row
                GetColumnIndex(headerColumn.Value);

                // get all the child rows of this specfic header
                var columnRowDataCollection = columns.Where(c => c.ColumnName.Trim().Equals(headerColumn.ColumnName.Trim(), StringComparison.OrdinalIgnoreCase));

                // Checking for numerics and range on numarics
                if (qualityCheckTypes.Where(q => q.QualityCheckColumnTypeId == qualityCheckItem.QualityCheckColumnTypeId).FirstOrDefault().Name == "Numeric")// Numeric
                {
                    bool isTypevalid = true;
                    foreach (var columnData in columnRowDataCollection)
                    {
                        if (!columnData.Value.IsNumeric())
                        {
                            isTypevalid = false;
                            qualityCheckResult.Errors.Add(string.Format(CultureInfo.CurrentCulture, "Data under header '{0}' should be of Numeric type", headerColumn.Value));
                            break;
                        }
                    }

                    // Checking for range validation
                    if (isTypevalid && !string.IsNullOrEmpty(qualityCheckItem.Range))
                    {
                        string[] rangeValues = qualityCheckItem.Range.Split(new string[] { Utilities.Constants.RangeSeparator }, StringSplitOptions.None);
                        foreach (var columnData in columnRowDataCollection)
                        {
                            string message = CheckForRange(columnData.Value, rangeValues, headerColumn.Value);
                            if (!string.IsNullOrEmpty(message))
                            {
                                qualityCheckResult.Errors.Add(message);
                                break;
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Helper method to check the value of a column for the specified range.
        /// </summary>
        /// <param name="valueToCheck">Column value to check.</param>
        /// <param name="rangeValues">Range values.</param>
        /// <returns>Error message.</returns>
        private static string CheckForRange(string valueToCheck, string[] rangeValues, string headerName)
        {
            string errorMessage = string.Empty;

            if (rangeValues != null && rangeValues.Length > 1)
            {
                string rangeStart = rangeValues[0].Trim();
                string rangeEnd = rangeValues[1].Trim();

                // Both start and end values are available
                if (!string.IsNullOrWhiteSpace(rangeStart) && !string.IsNullOrWhiteSpace(rangeEnd))
                {
                    if (valueToCheck.ToDouble() < rangeStart.ToDouble() || valueToCheck.ToDouble() > rangeEnd.ToDouble())
                    {
                        if (rangeStart.ToDouble().Equals(rangeEnd.ToDouble()))
                        {
                            errorMessage = string.Format(CultureInfo.CurrentCulture, "Data under header '{0}'should be {1}", headerName, rangeStart);
                        }
                        else
                        {
                            errorMessage = string.Format(CultureInfo.CurrentCulture, "Data under header '{0}' is not in the specified range {1} to {2}", headerName, rangeStart, rangeEnd);
                        }
                    }
                }
                else if (string.IsNullOrWhiteSpace(rangeStart)) // If only end value is available
                {
                    if (valueToCheck.ToDouble() > rangeEnd.ToDouble())
                    {
                        errorMessage = string.Format(CultureInfo.CurrentCulture, "Data under header '{0}' should be lesser than {1}", headerName, rangeEnd);
                    }
                }
                else // Only start value is available.
                {
                    if (valueToCheck.ToDouble() < rangeStart.ToDouble())
                    {
                        errorMessage = string.Format(CultureInfo.CurrentCulture, "Data under header '{0}' should be more than {1}", headerName, rangeStart);
                    }
                }
            }

            return errorMessage;
        }

        /// <summary>
        /// Method to Get Column Location Index
        /// </summary>
        /// <param name="headerName">Header Name</param>
        /// <param name="headersList">Header List</param>
        /// <returns>returns the column location index</returns>
        private static int GetColumnLocationIndex(string headerName, IEnumerable<SheetCell> headersList)
        {
            var index = 0;
            foreach (var header in headersList)
            {
                index = index + 1;
                if (string.Equals(headerName, header.Value, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

            }
            return index;
        }

        /// <summary>
        /// Method to get column index
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>returns index</returns>
        private static int GetColumnIndex(string value)
        {
            string columnIndex = "0";
            foreach (char c in value)
            {
                if (char.IsDigit(c))
                {
                    if (columnIndex != "0")
                    {
                        columnIndex = string.Concat(columnIndex, c);
                    }
                    else
                    {
                        columnIndex = c.ToString();
                    }
                }
            }
            return Convert.ToInt32(columnIndex, CultureInfo.InvariantCulture);
        }

        private Stream GetFileStream(string fileID)
        {
            return base.BlobDataRepository.GetBlob(fileID);
        }

        /// <summary>
        /// Returns the next Column Name as it exists in Excel Sheet
        /// </summary>
        /// <param name="lastRef">The last ref.</param>
        /// <returns>Column reference.</returns>
        private static string IncrementColRef(string lastRef)
        {
            char[] characters = lastRef.ToUpperInvariant().ToCharArray();
            int sum = 0;
            for (int i = 0; i < characters.Length; i++)
            {
                sum *= 26;
                sum += (characters[i] - 'A' + 1);
            }
            sum++;
            string columnName = string.Empty;
            int modulo;
            while (sum > 0)
            {
                modulo = (sum - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                sum = (int)((sum - modulo) / 26);
            }
            return columnName;
        }

        /// <summary>
        /// Method to get sheet ids
        /// </summary>
        /// <param name="sheetIds">sheet ids with comma separated values</param>
        /// <returns>returns the list of sheet ids</returns>
        private static List<string> GetSheetIds(string sheetIds)
        {
            if (sheetIds.Contains(","))
            {
                return sheetIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            }

            return new List<string>() { sheetIds };
        }

        #endregion
    }

    public class MetaDataInfo
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class ColumnMetaDataInfo
    {
        public string EntityName { get; set; }

        public string EntityDescription { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ColumnType { get; set; }

        public string ColumnUnit { get; set; }
    }
}
