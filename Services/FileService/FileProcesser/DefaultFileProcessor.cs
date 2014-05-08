// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.FileService.FileProcesser
{
    public class DefaultFileProcessor : FileProcessor, IFileProcesser
    {

        public DefaultFileProcessor(IBlobDataRepository blobDataRepository,
                            IFileRepository fileDataRepository,
                            IRepositoryService repositoryService)
            : base(blobDataRepository, fileDataRepository, repositoryService)
        {
        }

        public Models.DataDetail DownloadDocument(DomainModel.File fileDetails)
        {
            return base.DownloadFileWithMetadataAsZip(fileDetails);
        }

        public async Task<IEnumerable<Utilities.Model.FileSheet>> GetDocumentSheetDetails(DomainModel.File fileDetail)
        {
            //throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Cannot get sheet details for file {0}", fileDetail.Name));
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

        public async Task<IEnumerable<ColumnLevelMetadata>> GetColumnMetadataFromFile(DomainModel.File fileDetail)
        {
            //throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Cannot get sheet details for file {0}", fileDetail.Name));
            Check.IsNotNull<DomainModel.File>(fileDetail, "fileDetail");
            List<ColumnLevelMetadata> columnLevelMetadataList = new List<ColumnLevelMetadata>();

            await Task.Factory.StartNew(() =>
            {
                columnLevelMetadataList = new List<ColumnLevelMetadata>();
            });

            return columnLevelMetadataList;
        }

        public Task<IEnumerable<Utilities.Model.QualityCheckResult>> GetQualityCheckIssues(DomainModel.File fileDetail, DomainModel.QualityCheck qualityCheck, IEnumerable<DomainModel.QualityCheckColumnType> qualityCheckTypes, string sheetIds)
        {
            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Cannot run quality checks on file {0}", fileDetail.Name));
        }

        public Task<IList<Utilities.Model.FileSheet>> GetErrors(DomainModel.File file)
        {
            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Cannot get errors for file {0}", file.Name));
        }

        public Task RemoveError(System.IO.Stream stream, string sheetName, IEnumerable<Utilities.Enums.ErrorType> errorTypes)
        {
            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Cannot remove errors in sheet {0}", sheetName));
        }
    }
}
