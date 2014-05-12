// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.DomainModel.ConceptualModel;
using Microsoft.Research.DataOnboarding.FileService.Enums;
using Microsoft.Research.DataOnboarding.FileService.Exceptions;
using Microsoft.Research.DataOnboarding.FileService.FileProcesser;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.FileService.Resource;
using Microsoft.Research.DataOnboarding.RepositoriesService;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Extensions;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using DM = Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.FileService
{
    /// <summary>
    /// class contains file service provider methods
    /// </summary>
    public class FileServiceProvider : IFileService
    {
        private IFileProcesser fileProcessor = null;

        protected IBlobDataRepository BlobDataRepository { get; set; }

        protected IFileRepository FileDataRepository { get; set; }

        protected IRepositoryDetails RepositoryDetails { get; set; }

        protected IUnitOfWork UnitOfWork { get; set; }

        protected IRepositoryAdapter RepositoryAdapter { get; set; }

        protected IRepositoryAdapterFactory RepositoryAdapterFactory { get; set; }

        protected IRepositoryService RepositoryService { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileServiceProvider"/> class.
        /// </summary>
        /// <param name="fileDataRepository">IFileRepository instance</param>
        /// <param name="blobDataRepository">IBlobDataRepository instance</param>
        /// <param name="unitOfWork">IUnitOfWork instance</param>
        /// <param name="repositoryDetails">IRepositoryDetails instance</param>
        /// <param name="repositoryService">IRepositoryService instance</param>
        /// <param name="repositoryAdapterFactory">IRepositoryAdapterFactory instance</param>
        public FileServiceProvider(IFileRepository fileDataRepository, IBlobDataRepository blobDataRepository, IUnitOfWork unitOfWork, IRepositoryDetails repositoryDetails, IRepositoryService repositoryService, IRepositoryAdapterFactory repositoryAdapterFactory)
        {
            this.UnitOfWork = unitOfWork;
            this.FileDataRepository = fileDataRepository;
            this.BlobDataRepository = blobDataRepository;
            this.RepositoryDetails = repositoryDetails;
            this.RepositoryService = repositoryService;
            this.RepositoryAdapterFactory = repositoryAdapterFactory;
        }

        /// <summary>
        /// method to check file exists for the user id
        /// </summary>
        /// <param name="fileName">file Name</param>
        /// <param name="userId">user Id</param>
        /// <returns>returns status</returns>
        public bool CheckFileExists(string fileName, int userId)
        {
            bool checkFileExists = false;

            var file = this.FileDataRepository.GetItem(userId, fileName);

            checkFileExists = (file != null && file.FileId > 0 && file.Status.Equals(FileStatus.Uploaded.ToString(), StringComparison.Ordinal) && (file.isDeleted == null || file.isDeleted == false));

            return checkFileExists;
        }

        /// <summary>
        /// Method to upload the file to blob and save the file to File table
        /// </summary>
        /// <param name="dataDetail">data detail</param>
        /// <returns>returns data detail with updated file information</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public DataDetail UploadFile(DataDetail dataDetail)
        {
            Check.IsNotNull(dataDetail, "dataDetail");

            bool isUpdate = true;
            var file = new DomainModel.File();
            if (Constants.UploadToAzure)
            {
                file = this.FileDataRepository.GetItem(dataDetail.FileDetail.CreatedBy, dataDetail.FileDetail.Name);
                if (file == null || file.Status.Equals(FileStatus.Posted.ToString(), StringComparison.Ordinal))
                {
                    isUpdate = false;

                    // set the values from the passed object
                    file = dataDetail.FileDetail;
                    file.CreatedOn = DateTime.UtcNow;
                    file.ModifiedOn = DateTime.UtcNow;
                    file.BlobId = Guid.NewGuid().ToString();
                }
                else
                {
                    file.ModifiedBy = dataDetail.FileDetail.CreatedBy;
                    file.ModifiedOn = DateTime.UtcNow;
                    file.Size = dataDetail.FileDetail.Size;
                    dataDetail.FileDetail.BlobId = file.BlobId;
                }
                if (this.BlobDataRepository.UploadFile(dataDetail))
                {
                    if (!isUpdate)
                    {
                        file = this.FileDataRepository.AddFile(file);
                    }
                    else
                    {
                        file = this.FileDataRepository.UpdateFile(file);
                    }
                }

                this.UnitOfWork.Commit();
            }
            dataDetail.FileDetail = file;
            dataDetail.DataStream = null;
            return dataDetail;
        }

        /// <summary>
        /// Method to get all the available files for the specified user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>Files collection created by the mentioned user.</returns>
        public IEnumerable<DomainModel.File> GetAllFiles(int userId)
        {
            string postStatus = FileStatus.Posted.ToString();
            Func<DM.File, bool> filter = file => file.CreatedBy == userId && (file.Status.Equals(postStatus, StringComparison.Ordinal) || file.isDeleted == null || file.isDeleted == false);
            return this.FileDataRepository.GetFiles(filter).OrderByDescending(file => file.CreatedOn);
        }

        /// <summary>
        /// Get all the files to be purged.
        /// </summary>
        /// <param name="uploadedFilesExpirationDurationInHours">Number of days uploaded files can remain in the database.</param>
        /// <returns>Files to be purged.</returns>
        public IEnumerable<DM.File> GetFilesToBePurged(double uploadedFilesExpirationDurationInHours)
        {
            DateTime lastDate = DateTime.UtcNow.Subtract(TimeSpan.FromHours(uploadedFilesExpirationDurationInHours));
            Func<DM.File, bool> filter = file => file.isDeleted != true && (file.Status == FileStatus.Uploaded.ToString() || file.Status == FileStatus.Error.ToString()) && file.ModifiedOn.HasValue && file.ModifiedOn.Value <= lastDate;
            var filesToBePurged = this.FileDataRepository.GetFiles(filter).ToList();
            return filesToBePurged;
        }

        /// <summary>
        /// Get files in Uploaded staus.
        /// </summary>
        /// <returns>List of File objects</returns>
        public IEnumerable<DM.File> GetUploadedFiles()
        {
            Func<DM.File, bool> filter = file => file.isDeleted != true && (file.Status == FileStatus.Uploaded.ToString() || file.Status == FileStatus.Error.ToString());
            var uploadedFiles = this.FileDataRepository.GetFiles(filter).ToList();
            return uploadedFiles;
        }

        /// <summary>
        /// Method to delete the specified file store.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="fileId">file id.</param>
        /// <returns>True incase of success else false.</returns>
        public bool DeleteFile(int userId, int fileId)
        {
            bool deleteResult = false;

            var file = this.FileDataRepository.GetItem(userId, fileId);

            Check.IsNotNull<DM.File>(file, "file");
            bool isBlobDeleted = false;
            if (string.IsNullOrWhiteSpace(file.BlobId))
            {
                isBlobDeleted = true;
            }
            else
            {
                isBlobDeleted = this.BlobDataRepository.DeleteFile(file.BlobId);
            }

            if (isBlobDeleted)
            {
                if (file.Status == FileStatus.Posted.ToString())
                {
                    file.BlobId = string.Empty;
                    file.isDeleted = true;
                    this.FileDataRepository.UpdateFile(file);
                }
                else
                {
                    this.FileDataRepository.DeleteFile(file.FileId, file.Status);
                }

                if (file.CreatedBy == userId && file.FileId == fileId)
                {
                    UnitOfWork.Commit();
                    deleteResult = true;
                }
            }

            return deleteResult;
        }

        /// <summary>
        /// Method to download the file with updated metadata.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="fileId">File id.</param>
        /// <returns>DataDetail object.</returns>
        public DataDetail DownloadFile(int userId, int fileId)
        {
            DM.File fileDetails = this.FileDataRepository.GetItem(userId, fileId);
            Check.IsNotNull<DM.File>(fileDetails, "fileInformation");

            IFileProcesser fileProcesser = FileFactory.GetFileTypeInstance(Path.GetExtension(fileDetails.Name), this.BlobDataRepository, this.FileDataRepository, this.RepositoryService);
            return fileProcesser.DownloadDocument(fileDetails);
        }

        /// <summary>
        /// Method to get the file information by file id.
        /// </summary>
        /// <param name="fileId">File id.</param>
        /// <returns>File object.</returns>
        public DM.File GetFileByFileId(int fileId)
        {
            return this.FileDataRepository.GetItem(0, fileId);
        }

        /// <summary>
        /// Method to update the existing file details
        /// </summary>
        /// <param name="fileDetails">File data.</param>
        /// <returns>True incase of success else false.</returns>ns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public bool UpdateFile(DM.File fileDetails)
        {
            Check.IsNotNull<DM.File>(fileDetails, "fileToUpdate");

            bool updateResult = false;
            //file details and filemetadata fields will be updated
            var updatedFile = this.FileDataRepository.UpdateFile(fileDetails);
            //first update the file and file meta data fileds
            if (updatedFile != null && updatedFile.FileId == fileDetails.FileId)
            {
                updateResult = true;
                this.UnitOfWork.Commit();
            }

            return updateResult;
        }

        /// <summary>
        /// Method to save the file data.
        /// </summary>
        /// <param name="postFileData">Post file midel.</param>
        /// <returns>True incase of success else false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public virtual bool SaveFile(PostFileModel postFileData)
        {
            Check.IsNotNull(postFileData, "postFileData");

            // get the curent file object
            var file = this.GetFileByFileId(postFileData.FileId);

            file.ModifiedOn = DateTime.UtcNow;
            file.RepositoryId = postFileData.SelectedRepositoryId;

            // update the file
            var status = this.UpdateFile(file);

            // commit the above changes before calling the update metadata sheet
            this.UnitOfWork.Commit();

            return status;
        }

        /// <summary>
        /// Method to get document sheet list
        /// </summary>
        /// <param name="fileDetail"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public async Task<IEnumerable<ColumnLevelMetadata>> GetColumnLevelMetadataFromFile(DM.File fileDetail)
        {
            Check.IsNotNull(fileDetail, "fileDetail");

            IFileProcesser fileProcesser = FileFactory.GetFileTypeInstance(Path.GetExtension(fileDetail.Name), this.BlobDataRepository, this.FileDataRepository, this.RepositoryService);
            return await fileProcesser.GetColumnMetadataFromFile(fileDetail);
        }

        /// <summary>
        /// Method to get document sheet list
        /// </summary>
        /// <param name="fileDetail"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public async Task<IEnumerable<FileSheet>> GetDocumentSheetDetails(DM.File fileDetail)
        {
            Check.IsNotNull(fileDetail, "fileDetail");

            IFileProcesser fileProcesser = FileFactory.GetFileTypeInstance(Path.GetExtension(fileDetail.Name), this.BlobDataRepository, this.FileDataRepository, this.RepositoryService);
            return await fileProcesser.GetDocumentSheetDetails(fileDetail);
        }

        /// <summary>
        /// Method to return the collection of Column Units
        /// </summary>
        /// <returns>returns the column units</returns>
        public IEnumerable<DM.FileColumnUnit> RetrieveFileColumnUnits()
        {
            return this.FileDataRepository.RetrieveFileColumnUnits();
        }

        /// <summary>
        /// returns the collection of column types
        /// </summary>
        /// <returns>returns the column types</returns>
        public IEnumerable<DM.FileColumnType> RetrieveFileColumnTypes()
        {
            return this.FileDataRepository.RetrieveFileColumnTypes();
        }

        /// <summary>
        /// Method to return list of metadata types
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DM.MetadataType> RetrieveMetaDataTypes()
        {
            return this.FileDataRepository.RetrieveMetaDataTypes();
        }

        public virtual string PublishFile(PublishMessage postFileData)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Helper method to get the post query data.
        /// </summary>
        /// <param name="identifier">Unique identifier.</param>
        /// <param name="fileData">File information.</param>
        /// <param name="postFileData">Post file indormation.</param>
        /// <returns></returns>
        protected MerritQueryData GetPostQueryData(string identifier, DM.File fileData, Citation citation, Repository repository, PublishMessage postFileData)
        {
            Check.IsNotNull(identifier, "identifier");

            MerritQueryData queryData = new MerritQueryData();

            // TODO: Currently hard coded, needs to be replaced with specific value
            //@"<eml:eml packageId=""doi:10.5072/12345?"" system=""DCXL"" xmlns:eml=""eml://ecoinformatics.org/eml2.1.0"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""eml://ecoinformatics.org/eml-2.1.0 eml.xsd"">    <dataset id=""doi:10.5072/12345"">        <creator>            <!-- http://knb.ecoinformatics.org/software/eml/eml-2.1.0/eml-resource.html#creator -->            <!-- multiple creators allowed -->            <individualName>                <givenName></givenName>                <surName></surName>            </individualName>            <address>                <deliveryPoint></deliveryPoint>                <city></city>                <administrativeArea></administrativeArea><postalCode></postalCode><country></country></address><phone></phone><electronicMailAddress></electronicMailAddress><organizationName></organizationName></creator><title></title><pubDate></pubDate><abstract><para></para></abstract><publisher><para></para></publisher><url></url><contact><individualName><givenName></givenName><surName></surName></individualName><address><deliveryPoint></deliveryPoint><city></city><administrativeArea></administrativeArea><postalCode></postalCode><country></country></address><phone></phone><electronicMailAddress></electronicMailAddress><organizationName></organizationName></contact><keywordSet><keyword></keyword><keywordThesaurus></keywordThesaurus></keywordSet><coverage><!-- http://knb.ecoinformatics.org/software/eml/eml-2.1.0/eml-resource.html#coverage -->  <geographicCoverage><geographicDescription></geographicDescription><boundingCoordinates><westBoundingCoordinate></westBoundingCoordinate><eastBoundingCoordinate></eastBoundingCoordinate><northBoundingCoordinate></northBoundingCoordinate><southBoundingCoordinate></southBoundingCoordinate></boundingCoordinates></geographicCoverage><temporalCoverage id=""tempcov""><rangeOfDates> <beginDate><calendarDate></calendarDate></beginDate><endDate><calendarDate></calendarDate></endDate></rangeOfDates></temporalCoverage></coverage><project><!-- http://knb.ecoinformatics.org/software/eml/eml-2.1.0/eml-dataset.html#project --><title></title><abstract><para></para></abstract><funding><para></para></funding></project>        <intellectualRights>            <para></para>        </intellectualRights>        <dataTable>            <!-- http://knb.ecoinformatics.org/software/eml/eml-2.1.0/eml-dataTable.html#dataTable -->            <!-- dataTable is equivalent to a single tab in the excel spreadsheet.         One can have multiple data tables within the document. -->            <entityName></entityName>            <entityDescription></entityDescription>            <attributeList>                <!-- http://knb.ecoinformatics.org/software/eml/eml-2.1.0/eml-dataTable.html#attributeList -->                <!-- attribute list is equivalent to the parameter table from the requirements document.         One can have many attributes in a single table. -->                <attribute>                    <attributeName>                        <!-- non-empty string --></attributeName>                    <attributeDefinition>                        <!-- non-empty string --></attributeDefinition>                </attribute>            </attributeList>        </dataTable>    </dataset>    <additionalMetadata>        <describes>tempcov</describes>        <metadata>            <description>                <!-- non-empty string describing temporal coverage -->            </description>        </metadata>    </additionalMetadata>    <additionalMetadata>        <metadata>            <formattedCitation>                <!-- non-empty string -->            </formattedCitation>        </metadata>    </additionalMetadata></eml:eml>";

            // TODO: Currently hard coded, needs to be replaced with specific value
            queryData.MetadataXML = System.IO.File.ReadAllText("MerritMetadata.txt");

            List<DKeyValuePair<string, string>> content = new List<DKeyValuePair<string, string>>();

            Check.IsNotNull<DM.Repository>(repository, "selectedRepository");

            var repositoryMetadata = repository.RepositoryMetadata.FirstOrDefault();
            if (repositoryMetadata != null)
            {
                foreach (var repositoryMetaData in repositoryMetadata.RepositoryMetadataFields)
                {
                    DKeyValuePair<string, string> metadata = new DKeyValuePair<string, string>();
                    metadata.Key = repositoryMetaData.Name;
                    var metadataField = fileData.FileMetadataFields.Where(f => f.RepositoryMetadataFieldId == repositoryMetaData.RepositoryMetadataFieldId).FirstOrDefault();

                    if (metadataField != null)
                    {
                        metadata.Value = fileData.FileMetadataFields.Where(f => f.RepositoryMetadataFieldId == repositoryMetaData.RepositoryMetadataFieldId).FirstOrDefault().MetadataValue;
                    }

                    content.Add(metadata);
                }
            }

            //set the data to filemetadata fields
            //postFileData.FileMetaDataFields = fileData.FileMetadataFields;

            content.Add(new DKeyValuePair<string, string>() { Key = "Profile", Value = ConfigReader<string>.GetSetting("Profile_Post") });
            content.Add(new DKeyValuePair<string, string>() { Key = "who", Value = citation.Publisher });
            content.Add(new DKeyValuePair<string, string>() { Key = "what", Value = citation.Title });
            content.Add(new DKeyValuePair<string, string>() { Key = "when", Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) });
            content.Add(new DKeyValuePair<string, string>() { Key = "where", Value = identifier });
            content.Add(new DKeyValuePair<string, string>() { Key = "ARK", Value = identifier });

            queryData.KeyValuePair = content.ToArray();

            return queryData;
        }

        /// <summary>
        /// Helpe method to get the repository model.
        /// </summary>
        /// <param name="repository">Repository data.</param>
        /// <param name="authorization">Authorization string.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected static RepositoryModel GetRepositoryModel(DM.Repository repository, string authorization)
        {
            Check.IsNotNull(repository, "repository");
            Check.IsNotNull(authorization, "authorization");

            RepositoryModel repositoryModel = new RepositoryModel();

            repositoryModel.Authorization = authorization;
            repositoryModel.RepositoryName = MerritConstants.OneShare;
            repositoryModel.SelectedRepository = repository;
            repositoryModel.RepositoryLink = repository.HttpPostUriTemplate;

            return repositoryModel;
        }

        /// <summary>
        /// Helper method to get the DataFile object.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public DataFile GetDataFile(DM.File file)
        {
            Check.IsNotNull(file, "file");

            IFileProcesser fileProcesser = FileFactory.GetFileTypeInstance(Path.GetExtension(file.Name), this.BlobDataRepository, this.FileDataRepository, this.RepositoryService);
            DataDetail dataDetail = fileProcesser.DownloadDocument(file);

            DataFile dataFile = new DataFile();
            dataFile.ContentType = dataDetail.MimeTypeToDownLoad;
            dataFile.CreatedBy = dataDetail.FileDetail.CreatedBy;
            dataFile.FileInfo = dataDetail.FileDetail;
            dataFile.FileExtentsion = System.IO.Path.GetExtension(dataDetail.FileNameToDownLoad);
            dataFile.FileName = dataDetail.FileNameToDownLoad;
            dataFile.FileContent = dataDetail.DataStream;
            dataFile.IsCompressed = true;

            if (dataFile.FileExtentsion.Equals(Constants.XLFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                dataFile.IsCompressed = false;
            }

            return dataFile;
        }

        /// <summary>
        /// Helper method to get the identifier from merrit.
        /// </summary>
        /// <param name="postFileData"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        protected string GetIdentifier(PublishMessage postFileData, string authorization, string identifierUrl, Citation citation)
        {
            Check.IsNotNull(postFileData, "postFileData");
            Check.IsNotNull(authorization, "authorization");
            Check.IsNotNull(identifierUrl, "identifierUrl");
            var file = this.GetFileByFileId(postFileData.FileId);

            MerritQueryData queryData = new MerritQueryData();

            queryData.KeyValuePair = new List<DKeyValuePair<string, string>>()
            {
                         new DKeyValuePair<string, string> { Key = "Profile", Value = "oneshare_ark_only" },
                         new DKeyValuePair<string, string> { Key = "Who", Value = citation.Publisher },
                         new DKeyValuePair<string, string> { Key = "What", Value = citation.Title},
                         new DKeyValuePair<string, string> { Key = "When", Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) }
            }.ToArray();

            RepositoryModel repositoryModel = new RepositoryModel()
            {
                Authorization = authorization,
                RepositoryLink = identifierUrl,
                RepositoryName = MerritConstants.OneShare
            };

            return this.RepositoryAdapter.GetIdentifier(queryData, repositoryModel);
        }


        /// <summary>
        /// Downloads the File from Repository
        /// </summary>
        /// <param name="file">File object.</param>
        /// <param name="repository">Repository instance.</param>
        /// <param name="user">User instance.</param>
        /// <param name="credentials">credentials required by the repository.</param>
        /// <returns>DataFile containing the file data.</returns>
        public virtual DataFile DownLoadFileFromRepository(DM.File file, Repository repository, User user, RepositoryCredentials credentials)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///  Verifies if the file exists in the repository
        /// </summary>
        /// <param name="verifyFileMessage">Verify File Message</param>
        /// <returns>OperationStatus returns Success if the file exists</returns>
        public virtual OperationStatus CheckIfFileExistsOnExternalRepository(VerifyFileMessage verifyFileMessage)
        {
            return OperationStatus.CreateFailureStatus("not implemented");
        }

        /// <summary>
        /// Performs the necessary validations required for the file to be published
        /// </summary>
        /// <param name="message">Publish Message</param>
        /// <returns>Operation Status</returns>
        public virtual void ValidateForPublish(PublishMessage message)
        {
            Repository repository = this.RepositoryService.GetRepositoryById(message.RepositoryId);
            DM.File file = this.GetFileByFileId(message.FileId);

            if (null == file)
            {
                throw new DataFileNotFoundException()
                {
                    FileId = message.FileId
                };
            }

            FileStatus fileStatus = (FileStatus)Enum.Parse(typeof(FileStatus), file.Status);

            if (fileStatus == FileStatus.Posted || fileStatus == FileStatus.Verifying || fileStatus == FileStatus.Inqueue)
            {
                throw new FileAlreadyPublishedException()
                {
                    FileStatus = file.Status,
                    FileId = message.FileId,
                    RepositoryId = message.RepositoryId
                };
            }

            this.ValidateMetadata(message.RepositoryId, message.FileId);
        }

        /// <summary>
        /// Performs the ncessary validations for uploading the file to a blob storage.
        /// </summary>
        /// <param name="fileName">File Name.</param>
        /// <param name="userId">User Id.</param>
        public void ValidateForUpload(string fileName, int userId)
        {
            // get all the files for the user
            List<DM.File> files = this.FileDataRepository.GetFiles(file => string.Equals(file.Name, fileName, StringComparison.InvariantCultureIgnoreCase) &&
                    file.CreatedBy == userId &&
                    (
                        file.Status == FileStatus.Inqueue.ToString() ||
                        file.Status == FileStatus.Verifying.ToString()
                    )
                ).ToList();

            if (files != null && files.Count > 0)
            {
                throw new ValidationException()
                {
                    FileName = fileName,
                    ValidationExceptionType = ValidationExceptionType.UploadFile.ToString()
                };
            }
        }

        /// <summary>
        /// Method to get all the available files for the specified user.
        /// </summary>
        /// <param name="predicate">Function that is used to filter records</param>
        /// <returns>Files collection created by the mentioned user.</returns>
        public IEnumerable<DM.File> GetFiles(Func<DM.File, bool> predicate)
        {
            return this.FileDataRepository.GetFiles(predicate);
        }

        /// <summary>
        /// Gets errors in a file.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="fileId">File Id</param>
        /// <returns>File and list of sheets with errors.</returns>
        public async Task<Tuple<DM.File, IList<FileSheet>>> GetErrors(int userId, int fileId)
        {
            DM.File file = this.FileDataRepository.GetItem(userId, fileId);
            if (null == file)
            {
                throw new DataFileNotFoundException() { FileId = fileId };
            }
            else if (Path.GetExtension(file.Name) != Constants.XLFileExtension)
            {
                throw new InvalidOperationException(Messages.BestPractisesNotSupportedMessage);
            }

            IFileProcesser fileProcesser = FileFactory.GetFileTypeInstance(Path.GetExtension(file.Name), this.BlobDataRepository, this.FileDataRepository, this.RepositoryService);
            var fileSheets = await fileProcesser.GetErrors(file);
            return Tuple.Create(file, fileSheets);
        }

        /// <summary>
        /// Removes errors from a file sheet.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="fileId">File Id</param>
        /// <param name="sheetName">Sheet Name</param>
        /// <param name="errorTypeList">Error type list</param>
        /// <returns>Boolean indicating whether the removal was successful or not.</returns>
        public async Task RemoveErrors(int userId, int fileId, string sheetName, IEnumerable<ErrorType> errorTypeList)
        {
            DM.File file = this.FileDataRepository.GetItem(userId, fileId);
            if (null == file)
            {
                throw new DataFileNotFoundException() { FileId = fileId };
            }
            else if (Path.GetExtension(file.Name) != Constants.XLFileExtension)
            {
                throw new InvalidOperationException(Messages.BestPractisesNotSupportedMessage);
            }

            IFileProcesser fileProcesser = FileFactory.GetFileTypeInstance(Path.GetExtension(file.Name), this.BlobDataRepository, this.FileDataRepository, this.RepositoryService);
            using (Stream stream = GetFileStream(file.BlobId))
            {
                await fileProcesser.RemoveError(stream, sheetName, errorTypeList);

                if (Constants.UploadToAzure)
                {
                    DataFile dataFile = new DataFile()
                    {
                        FileContent = await stream.GetBytesAsync(),
                        ContentType = MimeMapping.GetMimeMapping(file.Name),
                        FileName = file.Name,
                        CreatedBy = userId,
                        FileExtentsion = Path.GetExtension(file.Name)
                    };

                    DataDetail dataDetail = new DataDetail(dataFile);
                    file.ModifiedBy = dataDetail.FileDetail.CreatedBy;
                    file.ModifiedOn = DateTime.UtcNow;
                    file.Size = dataDetail.FileDetail.Size;
                    dataDetail.FileDetail.BlobId = file.BlobId;
                    if (this.BlobDataRepository.UploadFile(dataDetail))
                    {
                        file = this.FileDataRepository.UpdateFile(file);
                    }

                    this.UnitOfWork.Commit();
                    dataDetail.DataStream = null;
                }
            }
        }

        /// <summary>
        /// Saves File Level Metadata
        /// </summary>
        /// <param name="fileId">File Id.</param>
        /// <param name="repositoryId">Repository Id.</param>
        /// <param name="saveFileLevelMetadataList">List of SaveFileLevelMetadata.</param>
        public void SaveFileLevelMetadata(int fileId, int repositoryId, IEnumerable<SaveFileLevelMetadata> saveFileLevelMetadataList)
        {
            // get the curent file object
            DM.File file = this.GetFileByFileId(fileId);
            file.RepositoryId = repositoryId;
            file.ModifiedOn = DateTime.UtcNow;

            foreach (var fileLevelMetadata in saveFileLevelMetadataList)
            {
                FileMetadataField fileMetadataField = null;
                if (fileLevelMetadata.FileMetadataFieldId != 0)
                {
                    fileMetadataField = file.FileMetadataFields.Where(f => f.FileMetadataFieldId == fileLevelMetadata.FileMetadataFieldId).FirstOrDefault();
                }

                if (fileMetadataField != null)
                {
                    fileMetadataField.MetadataValue = fileLevelMetadata.FieldValue;
                }
                else if (!string.IsNullOrEmpty(fileLevelMetadata.FieldValue))
                {
                    file.FileMetadataFields.Add(new DomainModel.FileMetadataField()
                    {
                        FileMetadataFieldId = fileLevelMetadata.FileMetadataFieldId,
                        FileId = fileId,
                        RepositoryMetadataFieldId = fileLevelMetadata.RepositoryMetadataFieldId,
                        MetadataValue = fileLevelMetadata.FieldValue
                    });
                }
            }

            this.FileDataRepository.UpdateFile(file);
            this.UnitOfWork.Commit();
        }

        public void SaveColumnLevelMetadata(int fileId, IEnumerable<ColumnLevelMetadata> columnLevelMetadataList)
        {
            DM.File file = this.GetFileByFileId(fileId);
            file.ModifiedOn = DateTime.UtcNow;

            foreach (FileColumn fileColumn in file.FileColumns)
            {
                ColumnLevelMetadata columnLevelMetadataToBeDeleted = columnLevelMetadataList.Where(clm => clm.Id == fileColumn.FileColumnId).FirstOrDefault();
                if (columnLevelMetadataToBeDeleted == null)
                {
                    fileColumn.EntityDescription = string.Empty;
                }
            }

            foreach (var columnLevelMetadata in columnLevelMetadataList)
            {
                FileColumn fileColumn = null;
                if (columnLevelMetadata.Id != 0)
                {
                    fileColumn = file.FileColumns.Where(f => f.FileColumnId == columnLevelMetadata.Id).FirstOrDefault();
                }

                if (fileColumn != null)
                {
                    fileColumn.EntityName = columnLevelMetadata.SelectedEntityName;
                    fileColumn.EntityDescription = columnLevelMetadata.EntityDescription;
                    fileColumn.Name = columnLevelMetadata.Name;
                    fileColumn.Description = columnLevelMetadata.Description;
                    fileColumn.FileColumnTypeId = columnLevelMetadata.SelectedTypeId;
                    fileColumn.FileColumnUnitId = columnLevelMetadata.SelectedUnitId;
                }
                else
                {
                    file.FileColumns.Add(new DomainModel.FileColumn()
                    {
                        FileId = fileId,
                        EntityName = columnLevelMetadata.SelectedEntityName,
                        EntityDescription = columnLevelMetadata.EntityDescription,
                        Name = columnLevelMetadata.Name,
                        Description = columnLevelMetadata.Description,
                        FileColumnTypeId = columnLevelMetadata.SelectedTypeId,
                        FileColumnUnitId = columnLevelMetadata.SelectedUnitId
                    });
                }
            }

            this.FileDataRepository.UpdateFile(file);
            this.UnitOfWork.Commit();
        }

        #region private methods

        /// <summary>
        /// Validates the metadata
        /// </summary>
        /// <param name="repositoryId">Repository Id</param>
        /// <param name="fileId">Field Id</param>
        private void ValidateMetadata(int repositoryId, int fileId)
        {
            Repository repository = this.RepositoryService.GetRepositoryById(repositoryId);

            DM.File file = this.GetFileByFileId(fileId);
            ICollection<FileMetadataField> fileMetadata = file.FileMetadataFields;

            foreach (RepositoryMetadata metadata in repository.RepositoryMetadata.ToList())
            {
                foreach (RepositoryMetadataField field in metadata.RepositoryMetadataFields.ToList())
                {
                    FileMetadataField fileMetadataField = fileMetadata.Where(m => m.FileId == fileId && m.RepositoryMetadataFieldId == field.RepositoryMetadataFieldId).FirstOrDefault();

                    // Checks if the matadata exists for required field.
                    if (field.IsRequired == true && (fileMetadataField == null || string.IsNullOrEmpty(fileMetadataField.MetadataValue)))
                    {
                        throw new MetadataValidationException()
                        {
                            FileId = fileId,
                            FieldName = field.Name,
                            RepositoryId = repositoryId,
                            FieldId = field.RepositoryMetadataFieldId,
                            IsRequired = true,
                            NotFound = true
                        };
                    }

                    // Checks if MetadataType is valid
                    MetadataType metadataType = this.RepositoryService.GetMetadataTypes().Where(mt => mt.MetadataTypeId == field.MetadataTypeId).FirstOrDefault();
                    if (metadataType == null)
                    {
                        throw new MetadataValidationException()
                        {
                            FileId = fileId,
                            RepositoryId = repositoryId,
                            IsRequired = (bool)field.IsRequired,
                            FieldId = field.RepositoryMetadataFieldId,
                            NotFound = false,
                            TypeMismatch = false,
                            MetadataTypeNotFound = true
                        };
                    }
                }
            }
        }

        private Stream GetFileStream(string fileID)
        {
            return this.BlobDataRepository.GetBlob(fileID);
        }

        #endregion
    }
}

