// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using Microsoft.Research.DataOnboarding.FileService.Resource;

namespace Microsoft.Research.DataOnboarding.FileService.FileProcesser
{
    /// <summary>
    /// Base class for all file processors
    /// </summary>
    public abstract class FileProcessor
    {
        private IBlobDataRepository blobDataRepository;
        private IFileRepository fileDataRepository;
        private IRepositoryService repositoryService;

        public static int GetFileColumnType(string cellValue, IEnumerable<FileColumnType> fileColumnTypes)
        {
            DateTime dateValue;

            int number;
            if (Int32.TryParse(cellValue, out number))
            {
                foreach (FileColumnType fileColumnType in fileColumnTypes)
                {
                    if (string.Compare(fileColumnType.Name, "Numeric", true) == 0)
                    {
                        return fileColumnType.FileColumnTypeId;
                    }
                }
            }

            decimal decimalNumber;
            if (Decimal.TryParse(cellValue, out decimalNumber))
            {
                foreach (FileColumnType fileColumnType in fileColumnTypes)
                {
                    if (string.Compare(fileColumnType.Name, "Numeric", true) == 0)
                    {
                        return fileColumnType.FileColumnTypeId;
                    }
                }
            }

            if (DateTime.TryParse(cellValue, out dateValue))
            {
                foreach (FileColumnType fileColumnType in fileColumnTypes)
                {
                    if (string.Compare(fileColumnType.Name, "DateTime", true) == 0)
                    {
                        return fileColumnType.FileColumnTypeId;
                    }
                }
            }


            foreach (FileColumnType fileColumnType in fileColumnTypes)
            {
                if (string.Compare(fileColumnType.Name, "Text", true) == 0)
                {
                    return fileColumnType.FileColumnTypeId;
                }
            }

            return 1;
        }

        protected IBlobDataRepository BlobDataRepository
        {
            get
            {
                return this.blobDataRepository;
            }
        }

        protected IFileRepository FileDataRepository
        {
            get
            {
                return this.fileDataRepository;
            }
        }

        protected IRepositoryService RepositoryService
        {
            get
            {
                return this.repositoryService;
            }
        }


        public FileProcessor(IBlobDataRepository blobDataRepository,
                            IFileRepository fileDataRepository,
                            IRepositoryService repositoryService)
        {
            this.blobDataRepository = blobDataRepository;
            this.fileDataRepository = fileDataRepository;
            this.repositoryService = repositoryService;
        }

        protected string GetMetadata(DomainModel.File fileDetails)
        {
            if (fileDetails.Status == FileStatus.Uploaded.ToString() || fileDetails.RepositoryId == null || fileDetails.RepositoryId <= 0)
            {
                return null;
            }

            var repository = this.RepositoryService.GetRepositoryById(Convert.ToInt32(fileDetails.RepositoryId, CultureInfo.InvariantCulture));
            if (repository == null)
            {
                return null;
            }

            StringBuilder metaDataBuilder = new StringBuilder();
            var repositoryMetadata = repository.RepositoryMetadata.FirstOrDefault();
            if (repositoryMetadata != null)
            {
                var repositoryMetaDataFields = repositoryMetadata.RepositoryMetadataFields;
                var fileMetaDataFields = fileDetails.FileMetadataFields.ToList();

                metaDataBuilder.Append("Name,Value");
                metaDataBuilder.Append(Environment.NewLine);
                fileMetaDataFields.ForEach(q =>
                {
                    metaDataBuilder.Append(repositoryMetaDataFields.Where(r => r.RepositoryMetadataFieldId == q.RepositoryMetadataFieldId).FirstOrDefault().Name + "," + q.MetadataValue);
                    metaDataBuilder.Append(Environment.NewLine);
                });
            }

            if (fileDetails.FileColumns != null && fileDetails.FileColumns.Any())
            {
                // get the file column type for gettig the column type
                var fileColumnTypes = this.FileDataRepository.RetrieveFileColumnTypes();

                // get file column units for getting the unit name
                var fileColumnUnits = this.FileDataRepository.RetrieveFileColumnUnits();

                if (metaDataBuilder.Length > 0)
                {
                    metaDataBuilder.Append(Environment.NewLine);
                }

                metaDataBuilder.Append(string.Join(",", Statics.TableName, Statics.TableDescription, Statics.FieldName, Statics.FieldDescription, Statics.DataType, Statics.Units));
                metaDataBuilder.Append(Environment.NewLine);
                string columnTypeName = string.Empty;
                string columnUnitName = string.Empty;
                fileDetails.FileColumns.ToList().ForEach(q =>
                {
                    columnTypeName = string.Empty;
                    columnUnitName = string.Empty;
                    if (q.FileColumnTypeId != null && q.FileColumnTypeId != 0)
                    {
                        columnTypeName = fileColumnTypes.Where(fc => fc.FileColumnTypeId == q.FileColumnTypeId).FirstOrDefault().Name;
                    }

                    if (q.FileColumnUnitId != null && q.FileColumnUnitId != 0)
                    {
                        columnUnitName = fileColumnUnits.Where(fc => fc.FileColumnUnitId == q.FileColumnUnitId).FirstOrDefault().Name;
                    }

                    metaDataBuilder.Append(q.EntityName + "," + q.EntityDescription + "," + q.Name + "," + q.Description + "," + columnTypeName + "," + columnUnitName);
                    metaDataBuilder.Append(Environment.NewLine);
                });
            }

            return metaDataBuilder.ToString();
        }

        protected byte[] GetFileContentsAsByteArray(string fileID)
        {
            byte[] dataStream = null;

            if (string.IsNullOrWhiteSpace(fileID))
            {
                return dataStream;
            }

            var blobDetails = this.BlobDataRepository.GetBlobContent(fileID);
            dataStream = blobDetails.DataStream;
            return dataStream;
        }

        protected Models.DataDetail DownloadFileWithMetadataAsZip(DomainModel.File fileDetails)
        {
            // Validate parameters
            Check.IsNotNull<DomainModel.File>(fileDetails, "filetoDownload");
            Check.IsNotNull<ICollection<FileMetadataField>>(fileDetails.FileMetadataFields, "fileMetadata");
            string fileName = default(string);
            try
            {
                // initialize data detail 
                DataDetail dataDetail = new DataDetail();
                dataDetail.FileDetail = fileDetails;
                dataDetail.MimeTypeToDownLoad = MediaTypeNames.Application.Zip;
                dataDetail.FileNameToDownLoad = string.Join(string.Empty, Path.GetFileNameWithoutExtension(fileDetails.Name), Constants.ZipFileExtension);

                // create work directory
                fileName = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
                DirectoryInfo dir = Directory.CreateDirectory(fileName);

                // download data file
                using (Stream dataStream = BlobDataRepository.GetBlob(fileDetails.BlobId))
                {
                    string dataFileName = Path.Combine(fileName, fileDetails.Name);
                    using (Stream dataFile = System.IO.File.OpenWrite(dataFileName))
                    {
                        dataStream.Seek(0, SeekOrigin.Begin);
                        dataStream.CopyToStream(dataFile);
                        dataFile.Flush();
                    }
                }

                // download metadata
                string metadata = GetMetadata(fileDetails);
                if (!string.IsNullOrEmpty(metadata))
                {
                    string metadataFileName = Path.Combine(fileName, string.Concat("File-metadata", Constants.CSVFileExtension));
                    using (StreamWriter metadataStream = new StreamWriter(metadataFileName))
                    {
                        metadataStream.WriteLine(metadata);
                    }
                }

                // archive
                dataDetail.DataStream = ZipFileHelper.ZipFiles(dir.FullName).GetBytes();

                return dataDetail;
            }
            finally
            {
                // delete the directory
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    Directory.Delete(fileName, true);
                }
            }

        }
    }
}
