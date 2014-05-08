// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.FileService.Resource;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Extensions;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.FileService.FileProcesser
{
    public class CSVFileProcessor : FileProcessor, IFileProcesser
    {

        public CSVFileProcessor(IBlobDataRepository blobDataRepository,
                            IFileRepository fileDataRepository,
                            IRepositoryService repositoryService)
            : base(blobDataRepository, fileDataRepository, repositoryService)
        {
        }

        public Models.DataDetail DownloadDocument(DomainModel.File fileDetails)
        {
            return base.DownloadFileWithMetadataAsZip(fileDetails);
        }

        public async Task<IEnumerable<ColumnLevelMetadata>> GetColumnMetadataFromFile(DomainModel.File fileDetail)
        {
            Check.IsNotNull<DomainModel.File>(fileDetail, "fileDetail");
            IEnumerable<ColumnLevelMetadata> columnLevelMetadataList = new List<ColumnLevelMetadata>();

            await Task.Factory.StartNew(() =>
            {
                using (Stream dataStream = base.BlobDataRepository.GetBlob(fileDetail.BlobId))
                {
                    using (StreamReader reader = new StreamReader(new MemoryStream()))
                    {
                        // copy stream to input stream on the reader to parse content
                        dataStream.Seek(0, SeekOrigin.Begin);
                        dataStream.CopyToStream(reader.BaseStream);

                        // seek the begening of content
                        reader.BaseStream.Seek(0, SeekOrigin.Begin);
                        string fileName = Path.GetFileNameWithoutExtension(fileDetail.Name);

                        List<string> headers = new List<string>();
                        List<string> dataElements = new List<string>();

                        // iterate over each record in the data file
                        if (reader.Peek() >= 0)
                        {
                            // read the current line
                            string dataRow = reader.ReadLine();
                            headers = dataRow.Split(',').Select(e => e.Trim()).ToList();

                            ////if (reader.Peek() >= 0)
                            ////{
                            ////    dataRow = reader.ReadLine();
                            ////    dataElements = dataRow.Split(',').Select(e => e.Trim()).ToList();
                            ////}
                        }

                        columnLevelMetadataList = CreateColumnLevelMetadataList(fileName, headers);
                    }
                }
            });


            return columnLevelMetadataList;
        }

        private static IList<ColumnLevelMetadata> CreateColumnLevelMetadataList(string fileName, List<string> headers)
        {
            IList<ColumnLevelMetadata> columnLevelMetadaList = new List<ColumnLevelMetadata>();

            if (headers == null || headers.Count == 0)
            {
                return columnLevelMetadaList;
            }

            for (int i = 0; i < headers.Count; i++)
            {
                ColumnLevelMetadata columnLevelMetadata = new ColumnLevelMetadata();
                columnLevelMetadata.SelectedEntityName = fileName;
                columnLevelMetadata.Name = headers.ElementAt(i);

                columnLevelMetadaList.Add(columnLevelMetadata);
            }

            return columnLevelMetadaList;
        }

        public async Task<IEnumerable<Utilities.Model.FileSheet>> GetDocumentSheetDetails(DomainModel.File fileDetail)
        {
            Check.IsNotNull<DomainModel.File>(fileDetail, "fileDetail");
            List<FileSheet> fileSheets = new List<FileSheet>();
            await Task.Factory.StartNew(() =>
            {
                string fileName = Path.GetFileNameWithoutExtension(fileDetail.Name);
                fileSheets.Add(new FileSheet()
                {
                    SheetName = fileName,
                    SheetId = fileName
                });
            });

            return fileSheets;
        }

        public async Task<IEnumerable<Utilities.Model.QualityCheckResult>> GetQualityCheckIssues(DomainModel.File fileDetail, DomainModel.QualityCheck qualityCheck, IEnumerable<DomainModel.QualityCheckColumnType> qualityCheckTypes, string sheetIds)
        {
            Check.IsNotNull(fileDetail, "fileDetail");
            Check.IsNotNull(qualityCheck, "qualityCheck");
            Check.IsNotNull(qualityCheckTypes, "qualityCheckTypes");
            List<QualityCheckResult> qualityCheckResults = new List<QualityCheckResult>();
            await Task.Factory.StartNew(() =>
            {
                using (Stream dataStream = base.BlobDataRepository.GetBlob(fileDetail.BlobId))
                {
                    using (StreamReader reader = new StreamReader(new MemoryStream()))
                    {
                        // copy stream to input stream on the reader to parse content
                        dataStream.Seek(0, SeekOrigin.Begin);
                        dataStream.CopyToStream(reader.BaseStream);

                        // seek the begening of content
                        reader.BaseStream.Seek(0, SeekOrigin.Begin);
                        string fileName = Path.GetFileNameWithoutExtension(fileDetail.Name);
                        QualityCheckResult errorResult = new QualityCheckResult()
                        {
                            SheetId = fileName,
                            SheetName = fileName
                        };

                        bool isHeaderRow = true;
                        Dictionary<int, string> dataErrorsMap = new Dictionary<int, string>();
                        Dictionary<int, Tuple<QualityCheckColumnRule, QualityCheckColumnType>> validationIndex = new Dictionary<int, Tuple<QualityCheckColumnRule, QualityCheckColumnType>>();
                        // iterate over each record in the data file
                        while (reader.Peek() >= 0)
                        {
                            // read the current line
                            string dataRow = reader.ReadLine();
                            List<string> elements = dataRow.Split(',').Select(e => e.Trim()).ToList();

                            // check if this is the header row and run header validations 
                            if (isHeaderRow)
                            {
                                List<string> headerIssues = GetHeaderIssues(elements, qualityCheck);

                                // if errors exist append to the error result
                                if (headerIssues.Count > 0)
                                {
                                    errorResult.Errors = errorResult.Errors.Concat(headerIssues.AsEnumerable()).ToList();
                                }

                                // create an inverted index of header position to column rule and data type map. 
                                // this cached map will be used to validate each data record. 
                                validationIndex = BuildValidationIndex(elements, qualityCheck, qualityCheckTypes);
                                isHeaderRow = !isHeaderRow;
                                continue;
                            }

                            Dictionary<int, string> dataIssues = new Dictionary<int, string>();
                            try
                            {
                                // get the data issues in the current record
                                dataIssues = GetDataIssues(elements, validationIndex);
                            }
                            catch (IndexOutOfRangeException)
                            {
                                errorResult.Errors.Add(Messages.QualityCheck_ExecutionFailure);
                                break;
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                errorResult.Errors.Add(Messages.QualityCheck_ExecutionFailure);
                                break;
                            }

                            // select errors in columns which are not already in error map
                            IEnumerable<KeyValuePair<int, string>> dataErrors = from i in dataIssues
                                                                                where !dataErrorsMap.ContainsKey(i.Key)
                                                                                select i;

                            // if there new columns in error, then augment the errors to 
                            // the error map
                            if (dataErrors.Any())
                            {
                                dataErrorsMap = dataErrorsMap.Concat(dataErrors)
                                                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                            }

                            // check if the count of data errors is equal to the configured quality check 
                            // column rules. atmost 1 error per data column is recorded, so if all columns
                            // are in error, then no need to inspect rest of the records. 
                            if (dataErrorsMap.Count.Equals(qualityCheck.QualityCheckColumnRules.Count))
                            {
                                break;
                            }
                        }

                        // merge data errors with main error collection
                        errorResult.Errors = errorResult.Errors.Concat(dataErrorsMap.Values.AsEnumerable()).ToList();

                        // add error result to results collection
                        qualityCheckResults.Add(errorResult);
                    }
                }
            });

            return qualityCheckResults;
        }

        public Task<IList<Utilities.Model.FileSheet>> GetErrors(DomainModel.File file)
        {
            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Cannot get errors for file {0}", file.Name));
        }

        public Task RemoveError(System.IO.Stream stream, string sheetName, IEnumerable<Utilities.Enums.ErrorType> errorTypes)
        {
            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Cannot remove errors in sheet {0}", sheetName));
        }

        private Dictionary<int, string> GetDataIssues(List<string> data, Dictionary<int, Tuple<QualityCheckColumnRule, QualityCheckColumnType>> validationIndex)
        {
            Dictionary<int, string> errors = new Dictionary<int, string>();
            foreach (KeyValuePair<int, Tuple<QualityCheckColumnRule, QualityCheckColumnType>> validation in validationIndex)
            {
                int position = validation.Key;
                QualityCheckColumnRule rule = validation.Value.Item1;
                QualityCheckColumnType dataType = validation.Value.Item2;
                if (!dataType.Name.Equals("Numeric"))
                {
                    continue;
                }
                string elementValue = data[position];
                double value;
                if (!Double.TryParse(elementValue, out value))
                {
                    errors.Add(position, string.Format(CultureInfo.CurrentCulture, Messages.QualityCheck_NumericTypeErrorMessageTemplate, rule.HeaderName));
                    continue;
                }
                if (string.IsNullOrWhiteSpace(rule.Range))
                {
                    continue;
                }
                string[] rangeValues = rule.Range.Split(new string[] { Utilities.Constants.RangeSeparator }, StringSplitOptions.None);
                double min, max;
                if (!Double.TryParse(rangeValues[0], out min))
                {
                    min = Double.NegativeInfinity;
                }
                if (!Double.TryParse(rangeValues[1], out max))
                {
                    max = Double.PositiveInfinity;
                }
                if (!(value >= min && value <= max))
                {
                    errors.Add(position, string.Format(CultureInfo.CurrentCulture, Messages.QualityCheck_RangeErrorMessageTemplate, rule.HeaderName, rangeValues[0], rangeValues[1]));
                }
            }

            return errors;
        }

        private Dictionary<int, Tuple<QualityCheckColumnRule, QualityCheckColumnType>> BuildValidationIndex(List<string> headers, DomainModel.QualityCheck qualityCheck, IEnumerable<DomainModel.QualityCheckColumnType> qualityCheckTypes)
        {
            Dictionary<int, Tuple<QualityCheckColumnRule, QualityCheckColumnType>> index = new Dictionary<int, Tuple<QualityCheckColumnRule, QualityCheckColumnType>>();

            foreach (QualityCheckColumnRule rule in qualityCheck.QualityCheckColumnRules)
            {
                // find the position of the header
                int position = headers.FindIndex(h => h.Equals(rule.HeaderName, StringComparison.OrdinalIgnoreCase));

                // if header not found, then go to the next rule. 
                if (position < 0)
                {
                    continue;
                }

                // get data type of the header
                QualityCheckColumnType dataType = qualityCheckTypes
                                                    .Where(t => t.QualityCheckColumnTypeId.Equals(rule.QualityCheckColumnTypeId))
                                                    .FirstOrDefault();

                // build index
                index.Add(position, Tuple.Create<QualityCheckColumnRule, QualityCheckColumnType>(rule, dataType));
            }

            return index;
        }

        private List<string> GetHeaderIssues(List<string> headers, DomainModel.QualityCheck qualityCheck)
        {
            List<string> errors = new List<string>();

            // Check if the headers list is valid
            if (!headers.Any())
            {
                foreach (QualityCheckColumnRule rule in qualityCheck.QualityCheckColumnRules)
                {
                    errors.Add(string.Format(CultureInfo.CurrentCulture, Messages.QualityCheck_HeaderMissingMessageTemplate, rule.HeaderName));
                }
                return errors;
            }

            // check if required headers exist
            IEnumerable<QualityCheckColumnRule> requiredHeaders = qualityCheck
                                                                    .QualityCheckColumnRules
                                                                    .Where(r => r.IsRequired == true).AsEnumerable();
            foreach (QualityCheckColumnRule rule in requiredHeaders)
            {
                int count = headers.Count(r => r.Equals(rule.HeaderName, StringComparison.OrdinalIgnoreCase));
                if (count <= 0)
                {
                    errors.Add(string.Format(CultureInfo.CurrentCulture, Messages.QualityCheck_HeaderMissingMessageTemplate, rule.HeaderName));
                }
            }

            // if there are missing headers (or) header order need not be checked, then return errors
            if (errors.Count > 0 || qualityCheck.EnforceOrder == false)
            {
                return errors;
            }

            QualityCheckColumnRule[] orderedRules = qualityCheck.QualityCheckColumnRules.OrderBy(r => r.Order).ToArray();
            for (int i = 0; i < orderedRules.Length; i++)
            {
                string headerAtIndex = headers.ElementAtOrDefault(i);
                if (headerAtIndex.Equals(default(string))
                    || !headerAtIndex.Equals(orderedRules[i].HeaderName, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add(string.Format(CultureInfo.CurrentCulture, Messages.QualityCheck_HeaderNotInOrderMessageTemplate));
                    return errors;
                }
            }

            return errors;
        }


    }
}
