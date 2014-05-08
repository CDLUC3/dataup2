// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.FileService.Models;
using System.IO;

namespace Microsoft.Research.DataOnboarding.FileService.Interface
{
    /// <summary>
    /// Interface representing the blob data repository methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface IBlobDataRepository
    {
        /// <summary>
        /// Gets the blob content from azure as a stream.
        /// </summary>
        /// <param name="blobName">
        /// Name of the blob.
        /// </param>
        /// <returns>
        /// The blob details.
        /// </returns>
        DataDetail GetBlobContent(string blobName);

        /// <summary>
        /// Uploads a file to azure as a blob.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the file is uploaded successfully; otherwise false.
        /// </returns>
        bool UploadFile(DataDetail details);

        /// <summary>
        /// Deletes a file from azure.
        /// </summary>
        /// <param name="blobId">Azure file id.</param>
        /// <returns>True if the file is deleted successfully; otherwise false.</returns>
        bool DeleteFile(string blobId);

        /// <summary>
        /// Checks a file in azure.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be checked.
        /// </param>
        /// <returns>
        /// True if the file is found successfully; otherwise false.
        /// </returns>
        bool CheckIfExists(DataDetail details);

        /// <summary>
        /// Method to get the url for the specified blob.
        /// </summary>
        /// <param name="blobId">Blob id.</param>
        /// <returns>URL of the specific blob.</returns>
        string GetBlobURL(string blobId);

        Stream GetBlob(string blobId);
    }
}
