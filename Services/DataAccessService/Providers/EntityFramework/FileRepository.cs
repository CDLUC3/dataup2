// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace Microsoft.Research.DataOnboarding.DataAccessService.Providers.EntityFramework
{
    /// <summary>
    /// Implements  file <see cref="IFileRepository"/> leveraging
    /// entity framework
    /// </summary>
    public class FileRepository : RepositoryBase, IFileRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileRepository" /> class.
        /// </summary>
        /// <param name="dataContext">IUnitOfWork context</param>
        public FileRepository(IUnitOfWork dataContext)
            : base(dataContext)
        {
        }

        /// <summary>
        /// Method to add file information to database
        /// </summary>
        /// <param name="newFile">File object</param>
        /// <returns>returns file object</returns>
        public File AddFile(File newFile)
        {
            Check.IsNotNull<File>(newFile, "newFile");
            return Context.Files.Add(newFile);
        }

        /// <summary>
        /// Method to update file information to database
        /// </summary>
        /// <param name="modifiedFile">File object</param>
        /// <returns>returns file object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Validated the modified file in Check.IsNotNull", MessageId = "0")]
        public File UpdateFile(File modifiedFile)
        {
            Check.IsNotNull<File>(modifiedFile, "modifiedFile");

            File updatedFile = null;
            if (Context.GetEntityState(modifiedFile) == EntityState.Detached)
            {
                updatedFile = Context.Files.Attach(modifiedFile);
            }
            else
            {
                updatedFile = modifiedFile;
            }

            //set metadata fields
            UpdateFileLevelMetadata(modifiedFile);

            //set file column
            UpdateColumnLevelMetadata(modifiedFile);

            Context.SetEntityState<File>(updatedFile, EntityState.Modified);
            return updatedFile;
        }

        /// <summary>
        /// Method to update file level metadata.
        /// </summary>
        /// <param name="modifiedFile">modifiedFile</param>
        private void UpdateFileLevelMetadata(File modifiedFile)
        {
            if (modifiedFile.FileMetadataFields != null)
            {
                foreach (var fileMetaData in modifiedFile.FileMetadataFields)
                {
                    //first update the existing records
                    if (fileMetaData.FileMetadataFieldId != 0 && !string.IsNullOrEmpty(fileMetaData.MetadataValue))
                    {
                        Context.SetEntityState<FileMetadataField>(fileMetaData, EntityState.Modified);
                    }
                }

                List<FileMetadataField> ToBeDeletedfileMetaDataList = new List<FileMetadataField>();
                foreach (var fileMetaData in modifiedFile.FileMetadataFields)
                {
                    if (string.IsNullOrEmpty(fileMetaData.MetadataValue))
                    {
                        ToBeDeletedfileMetaDataList.Add(fileMetaData);
                    }
                }

                foreach (var deleteFileMetaData in ToBeDeletedfileMetaDataList)
                {
                    Context.SetEntityState<FileMetadataField>(deleteFileMetaData, EntityState.Deleted);
                    Context.FileMetadataFields.Remove(deleteFileMetaData);
                }
            }
        }

        /// <summary>
        /// Method to update column level metadata.
        /// </summary>
        /// <param name="modifiedFile">File object</param>
        private void UpdateColumnLevelMetadata(File modifiedFile)
        {
            List<FileColumn> fileColumnsToBeDeleted = new List<FileColumn>();
            if (modifiedFile.FileColumns != null)
            {
                foreach (var fileColumn in modifiedFile.FileColumns)
                {
                    if (!string.IsNullOrWhiteSpace(fileColumn.EntityDescription))
                    {
                        if (fileColumn.FileColumnId != 0)
                        {
                            var updatedFileColumn = Context.FileColumns.Attach(fileColumn);
                            Context.SetEntityState<FileColumn>(fileColumn, EntityState.Modified);
                        }
                        else
                        {
                            var addedFileColumn = Context.FileColumns.Add(fileColumn);
                            Context.SetEntityState<FileColumn>(addedFileColumn, EntityState.Added);
                        }
                    }
                    else
                    {
                        fileColumnsToBeDeleted.Add(fileColumn);
                    }
                }

                foreach (var deleteFileColumn in fileColumnsToBeDeleted)
                {
                    Context.SetEntityState<FileColumn>(deleteFileColumn, EntityState.Deleted);
                    Context.FileColumns.Remove(deleteFileColumn);
                }
            }
        }

        /// <summary>
        /// Method to get file item based on the user id and filename
        /// </summary>
        /// <param name="userId">user Id</param>
        /// <param name="fileName">file Name</param>
        /// <returns>returns file object</returns>
        public File GetItem(int userId, string fileName)
        {
            return Context.Files.Where(f => f.CreatedBy == userId && fileName.Equals(f.Name) && (f.isDeleted == null || f.isDeleted == false)).FirstOrDefault();
        }

        /// <summary>
        /// Method to get file item based on the user id and file id
        /// </summary>
        /// <param name="userId">user Id</param>
        /// <param name="fileId">file id</param>
        /// <returns>returns file object</returns>
        public File GetItem(int userId, int fileId)
        {
            if (userId > 0)
            {
                return Context.Files
                    .Include(f => f.FileMetadataFields)
                    .Include(f => f.FileColumns)
                    .Where(f => f.CreatedBy == userId && f.FileId == fileId && (f.isDeleted == null || f.isDeleted == false)).FirstOrDefault();
            }
            else
            {
                return Context.Files
                    .Include(f => f.FileMetadataFields)
                    .Include(f => f.FileColumns)
                    .Where(f => f.FileId == fileId && (f.isDeleted == null || f.isDeleted == false)).FirstOrDefault();
            }
        }

        /// <summary>
        /// Method to get file by name identifier and file id.
        /// </summary>
        /// <param name="nameIdentifier">Name Identifier</param>
        /// <param name="fileId">file id</param>
        /// <returns>returns file object</returns>
        public File GetItem(string nameIdentifier, int fileId)
        {
            return Context.Files.Where(f => f.User.NameIdentifier.Equals(nameIdentifier, StringComparison.OrdinalIgnoreCase) && f.FileId == fileId).FirstOrDefault();
        }

        /// <summary>
        /// Method to get all the available files for the specified user.
        /// </summary>
        /// <param name="predicate">Function that is used to filter records</param>
        /// <returns>Files collection created by the mentioned user.</returns>
        public IEnumerable<File> GetFiles(Func<File, bool> predicate)
        {
            return Context.Files.Where(predicate);
        }

        /// <summary>
        /// Method to Retrieve Column Units
        /// </summary>
        /// <returns>returns list of File column units</returns>
        public List<FileColumnUnit> RetrieveFileColumnUnits()
        {
            var fileColumnUnits = new List<FileColumnUnit>();
            foreach (var item in Context.FileColumnUnits)
            {
                fileColumnUnits.Add(item);
            }
            return fileColumnUnits;
        }

        /// <summary>
        /// Method to Retrieve Column Types
        /// </summary>
        /// <returns>returns list of File column types</returns>
        public List<FileColumnType> RetrieveFileColumnTypes()
        {
            var fileColumnTypes = new List<FileColumnType>();
            foreach (var item in Context.FileColumnTypes)
            {
                fileColumnTypes.Add(item);
            }
            return fileColumnTypes;
        }

        /// <summary>
        /// Method to get all the files for the mentioned repository.
        /// </summary>
        /// <param name="repositoryId">Repository id.</param>
        /// <returns>File collection.</returns>
        public IEnumerable<File> GetFilesByRepository(int repositoryId)
        {
            return Context.Files.Where(file => file.RepositoryId == repositoryId);
        }

        /// <summary>
        /// Method to delete the file metadata fields data for the mentioned file id.
        /// </summary>
        /// <param name="fileId">File id.</param>
        /// <param name="isFileData">Deleting only file related data.</param>
        /// <param name="isSoftDelete">Is it soft delete.</param>
        /// <returns>Success result.</returns>
        public bool DeleteFile(int fileId, string status, bool isFileData = false, bool isHardDelete = true)
        {
            bool result = false;
            if (isFileData)
            {
                var fileMetadataFields = Context.FileMetadataFields.Where(fileMeta => fileMeta.FileId == fileId);

                foreach (var fileMetadataField in fileMetadataFields)
                {
                    Context.SetEntityState<FileMetadataField>(fileMetadataField, EntityState.Deleted);
                    Context.FileMetadataFields.Remove(fileMetadataField);
                }

                if (status.Equals(FileStatus.Posted.ToString(), StringComparison.InvariantCulture))
                {
                    var fileColumns = Context.FileColumns.Where(fileCol => fileCol.FileId == fileId);

                    foreach (var fileColumn in fileColumns)
                    {
                        Context.SetEntityState<FileColumn>(fileColumn, EntityState.Deleted);
                        Context.FileColumns.Remove(fileColumn);
                    }

                    // Deleting quality check data
                    var fileQualityChecks = Context.FileQualityChecks.Where(filQc => filQc.FileId == fileId);

                    foreach (var fileQC in fileQualityChecks)
                    {
                        Context.SetEntityState<FileQualityCheck>(fileQC, EntityState.Deleted);
                        Context.FileQualityChecks.Remove(fileQC);
                    }
                }

                result = true;
            }
            else if (isHardDelete)
            {
                var fileAttributesToDelete = Context.FileAttributes.Where(file => file.FileId == fileId).ToList();
                fileAttributesToDelete.ForEach(fa =>
                {
                    Context.SetEntityState<FileAttribute>(fa, EntityState.Deleted);
                    Context.FileAttributes.Remove(fa);
                });

                var fileColumnsToDelete = Context.FileColumns.Where(file => file.FileId == fileId).ToList();
                fileColumnsToDelete.ForEach(fc =>
                {
                    Context.SetEntityState<FileColumn>(fc, EntityState.Deleted);
                    Context.FileColumns.Remove(fc);
                });

                var fileMetadataFieldsToDelete = Context.FileMetadataFields.Where(file => file.FileId == fileId).ToList();
                fileMetadataFieldsToDelete.ForEach(fmf =>
                {
                    Context.SetEntityState<FileMetadataField>(fmf, EntityState.Deleted);
                    Context.FileMetadataFields.Remove(fmf);
                });

                var fileQualityChecksToDelete = Context.FileQualityChecks.Where(file => file.FileId == fileId).ToList();
                fileQualityChecksToDelete.ForEach(fqc =>
                {
                    Context.SetEntityState<FileQualityCheck>(fqc, EntityState.Deleted);
                    Context.FileQualityChecks.Remove(fqc);
                });

                var fileToDel = Context.Files.Where(file => file.FileId == fileId).FirstOrDefault();
                Context.SetEntityState<File>(fileToDel, EntityState.Deleted);
                Context.Files.Remove(fileToDel);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Method to get meta data types
        /// </summary>
        /// <returns>returns the meta data types</returns>
        public List<MetadataType> RetrieveMetaDataTypes()
        {
            var metaDataTypes = new List<MetadataType>();
            foreach (var item in Context.MetadataTypes)
            {
                metaDataTypes.Add(item);
            }
            return metaDataTypes;
        }
    }
}
