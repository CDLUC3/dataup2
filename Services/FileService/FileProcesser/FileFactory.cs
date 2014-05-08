// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;

namespace Microsoft.Research.DataOnboarding.FileService.FileProcesser
{
    public class FileFactory
    {
        public static IFileProcesser GetFileTypeInstance(string fileExtension, IBlobDataRepository blobDataRepository, IFileRepository fileDataRepository = null, IRepositoryService repositoryService = null)
        {
            switch (fileExtension)
            {
                case Constants.XLFileExtension:
                    return new ExcelFileProcesser(blobDataRepository, fileDataRepository, repositoryService);
                case Constants.CSVFileExtension:
                    return new CSVFileProcessor(blobDataRepository, fileDataRepository, repositoryService);
                default:
                    return new DefaultFileProcessor(blobDataRepository, fileDataRepository, repositoryService);
            }
        }
    }
}
