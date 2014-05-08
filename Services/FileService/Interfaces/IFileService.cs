// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.FileService.Interface
{
    /// <summary>
    /// Interface methods for file service class
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Method to check file exists or not
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <param name="userId">user id</param>
        /// <returns>returns boolean value of file exists</returns>
        bool CheckFileExists(string fileName, int userId);

        /// <summary>
        /// method to upload file to azure and save the file
        /// </summary>
        /// <param name="dataDetail">data detail object</param>
        /// <returns>returns data detail object</returns>
        DataDetail UploadFile(DataDetail dataDetail);

        /// <summary>
        /// Method to get all the available files for the specified user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>Files collection created by the mentioned user.</returns>
        IEnumerable<File> GetAllFiles(int userId);

        /// <summary>
        /// Get all the files to be purged.
        /// </summary>
        /// <param name="uploadedFilesExpirationDurationInHours">Number of days uploaded files can remain in the database.</param>
        /// <returns>Files to be purged.</returns>
        IEnumerable<File> GetFilesToBePurged(double uploadedFilesExpirationDurationInHours);

        /// <summary>
        /// Get files in Uploaded staus.
        /// </summary>
        /// <returns>Files to be updated.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IEnumerable<File> GetUploadedFiles();

        /// <summary>
        /// Method to get the file information by file id.
        /// </summary>
        /// <param name="fileId">File id.</param>
        /// <returns>File object.</returns>
        File GetFileByFileId(int fileId);

        /// <summary>
        /// Method to delete the specified document store.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="fileId">file id.</param>
        /// <returns>True incase of success else false.</returns>
        bool DeleteFile(int userId, int fileId);

        /// <summary>
        /// Method to download the file with updated metadata.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="fileId">File id.</param>
        /// <returns>DataDetail object.</returns>
        DataDetail DownloadFile(int userId, int fileId);

        /// <summary>
        /// Method to update the existing file details
        /// </summary>
        /// <param name="fileDetails">File data.</param>
        /// <returns>True incase of success else false.</returns>
        bool UpdateFile(File fileDetails);

        bool SaveFile(PostFileModel postFileData);

        DataFile GetDataFile(File file);

        /// <summary>
        /// Publishes the file to external repository
        /// </summary>
        /// <param name="postFileData">Publish Model</param>
        /// <returns>File Identifier</returns>
        string PublishFile(PublishMessage postFileData);

        /// <summary>
        /// Method to get document sheet details
        /// </summary>
        /// <param name="fileDetail"></param>
        /// <returns></returns>
        Task<IEnumerable<FileSheet>> GetDocumentSheetDetails(File fileDetail);

        /// <summary>
        /// Method to get document sheet details
        /// </summary>
        /// <param name="fileDetail"></param>
        /// <returns></returns>
        Task<IEnumerable<ColumnLevelMetadata>> GetColumnLevelMetadataFromFile(File fileDetail);

        /// <summary>
        /// Method to retreive file column units
        /// </summary>
        /// <returns>retrieve file column units</returns>
        IEnumerable<FileColumnUnit> RetrieveFileColumnUnits();

        /// <summary>
        /// Method to retrieve file column types
        /// </summary>
        /// <returns>retreive file column types</returns>
        IEnumerable<FileColumnType> RetrieveFileColumnTypes();

        /// <summary>
        /// Method to retrieve file meta data tyes
        /// </summary>
        /// <returns>retreive meta data types</returns>
        IEnumerable<MetadataType> RetrieveMetaDataTypes();

        /// <summary>
        /// Downloads the File from Repository
        /// </summary>
        /// <param name="file">File object.</param>
        /// <param name="repository">Repsoitory instance.</param>
        /// <param name="User">User object.</param>
        /// <param name="credentials">Creadentials required by the repository.</param>
        /// <returns>DataFile containing file data.</returns>
        DataFile DownLoadFileFromRepository(File file, Repository repository, User user, RepositoryCredentials credentials);

        /// <summary>
        ///  Verifies if the file exists in the repository
        /// </summary>
        /// <param name="verifyFileMessage">Verify Message</param>
        /// <returns>OperationStatus returns Success if the file exists</returns>
        OperationStatus CheckIfFileExistsOnExternalRepository(VerifyFileMessage verifyFileMessage);

        /// <summary>
        /// Performs the necessary validations required for the file to be published
        /// </summary>
        /// <param name="message">Publish Message</param>
        void ValidateForPublish(PublishMessage message);

        /// <summary>
        /// Performs the ncessary validations for uploading the file to a blob storage.
        /// </summary>
        /// <param name="fileName">File Name.</param>
        /// <param name="userId">User Id.</param>
        void ValidateForUpload(string fileName, int userId);

        /// <summary>
        /// Method to get all the available files for the specified user.
        /// </summary>
        /// <param name="predicate">Function that is used to filter records</param>
        /// <returns>Files collection created by the mentioned user.</returns>
        IEnumerable<File> GetFiles(Func<File, bool> predicate);

        /// <summary>
        /// Gets errors in a file.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="fileId">File Id</param>
        /// <returns>File and list of sheets with errors.</returns>
        Task<Tuple<File, IList<FileSheet>>> GetErrors(int userId, int fileId);

        /// <summary>
        /// Removes errors from a file sheet.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="fileId">File Id</param>
        /// <param name="sheetName">Sheet Name</param>
        /// <param name="errorTypeList">Error type list</param>
        /// <returns>Task object.</returns>
        Task RemoveErrors(int userId, int fileId, string sheetName, IEnumerable<ErrorType> errorTypeList);

        /// <summary>
        /// Saves File Level Metadata
        /// </summary>
        /// <param name="fileId">File Id.</param>
        /// <param name="repositoryId">Repository Id.</param>
        /// <param name="saveFileLevelMetadataList">List of SaveFileLevelMetadata.</param>
        void SaveFileLevelMetadata(int fileId, int repositoryId, IEnumerable<SaveFileLevelMetadata> saveFileLevelMetadataList);

        /// <summary>
        /// Saves column level metadata.
        /// </summary>
        /// <param name="fileId">File Id.</param>
        /// <param name="columnLevelMetadataList"></param>
        void SaveColumnLevelMetadata(int fileId, IEnumerable<ColumnLevelMetadata> columnLevelMetadataList);
    }
}
