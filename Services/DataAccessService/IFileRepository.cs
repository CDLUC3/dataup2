// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using System;
using System.Collections.Generic;

namespace Microsoft.Research.DataOnboarding.DataAccessService
{
    public interface IFileRepository
    {
        /// <summary>
        /// Method to add new file to data base
        /// </summary>
        /// <param name="newFile">new file object</param>
        /// <returns>returns file object</returns>
        File AddFile(File newFile);

        /// <summary>
        /// Method to update the file
        /// </summary>
        /// <param name="modifiedFile">file object</param>
        /// <returns>returns file object</returns>
        File UpdateFile(File modifiedFile);

        /// <summary>
        /// Method to get he document store item for the specified document id and user id.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="fileId">file id.</param>
        /// <returns>Document store object.</returns>
        File GetItem(int userId, int fileId);

        /// <summary>
        /// Method to get file by name identifier and file id.
        /// </summary>
        /// <param name="nameIdentifier">Name Identifier</param>
        /// <param name="fileId">file id</param>
        /// <returns>returns file object</returns>
        File GetItem(string nameIdentifier, int fileId);

        /// <summary>
        /// Method to get the file item by user id and file name
        /// </summary>
        /// <param name="userId">user Id</param>
        /// <param name="fileName">file Name</param>
        /// <returns>returns file object</returns>
        File GetItem(int userId, string fileName);

        /// <summary>
        /// Method to get all the available document store items for the specified user.
        /// </summary>
        /// <param name="predicate">Function that is used to filter records</param>
        /// <returns>Document store list of objects.</returns>
        IEnumerable<File> GetFiles(Func<File, bool> predicate);

        ///// <summary>
        ///// Method to delete (soft delete) the specified document store.
        ///// </summary>
        ///// <param name="userId">User id.</param>
        ///// <param name="fileId">File id.</param>
        ///// <returns>Deleted file.</returns>
        //File DeleteFile(int userId, int fileId);

        /// <summary>
        /// Method to retreive file column units
        /// </summary>
        /// <returns>retrieve file column units</returns>
        List<FileColumnUnit> RetrieveFileColumnUnits();

        /// <summary>
        /// Method to retrieve file column types
        /// </summary>
        /// <returns>retreive file column types</returns>
        List<FileColumnType> RetrieveFileColumnTypes();

        /// <summary>
        /// Method to get all the files for the mentioned repository.
        /// </summary>
        /// <param name="repositoryId">Repository id.</param>
        /// <returns>File collection.</returns>
        IEnumerable<File> GetFilesByRepository(int repositoryId);

        /// <summary>
        /// Method to delete the file metadata fields data for the mentioned file id.
        /// </summary>
        /// <param name="fileId">File id.</param>
        /// <param name="isFileData">Deleting only file related data.</param>     
        /// <param name="isHardDelete">Is it hard delete.</param>
        /// <returns>Success result.</returns>
        bool DeleteFile(int fileId, string status, bool isFileData = false, bool isHardDelete = true);

        ///// <summary>
        ///// Method to delete (Hard delete) the file from the database. 
        ///// </summary>
        ///// <param name="fileId">File id.</param>
        ///// <returns>True incase of success else false.</returns>
        //bool RemoveFile(int fileId);

        /// <summary>
        /// Method to retrieve meta data types
        /// </summary>
        /// <returns>retreive file meta data types</returns>
        List<MetadataType> RetrieveMetaDataTypes();

    }
}
