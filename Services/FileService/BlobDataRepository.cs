// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Extensions;
using Microsoft.WindowsAzure;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;

namespace Microsoft.Research.DataOnboarding
{
    /// <summary>
    /// Class representing the Blob Data Repository having methods for adding/retrieving files from
    /// Azure blob storage.
    /// </summary>
    public class BlobDataRepository : IBlobDataRepository
    {
        /// <summary>
        /// lazy client variable.
        /// </summary>
        private static Lazy<CloudBlobClient> lazyClient;

        /// <summary>
        /// Storage account variable.
        /// </summary>
        private CloudStorageAccount storageAccount;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobDataRepository"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Code does not grant its callers access to operations or resources that can be used in a destructive manner.")]
        public BlobDataRepository()
        {
            if (!CloudStorageAccount.TryParse(ConfigReader<string>.GetSetting(Constants.StorageSettingName, string.Empty), out storageAccount))
            {
                throw new ArgumentException("Storage account details are either invalid or cannot be found. Check the configuration files.", Constants.StorageSettingName);
            }
            lazyClient = new Lazy<CloudBlobClient>(() => (this.storageAccount.CreateCloudBlobClient()));
        }

        /// <summary>
        /// Gets the container URL.
        /// </summary>
        public static Uri ContainerUrl
        {
            get
            {
                return new Uri(string.Join(Constants.PathSeparator, new string[] { BlobClient.BaseUri.AbsolutePath, Constants.ContainerName }));
            }
        }

        /// <summary>
        /// Gets the CloudBlobClient instance from lazy client.
        /// </summary>
        private static CloudBlobClient BlobClient
        {
            get
            {
                CloudBlobClient blobclient = lazyClient.Value;
                blobclient.RetryPolicy = Constants.DefaultRetryPolicy;
                return blobclient;
            }
        }

        /// <summary>
        /// Method to get the url for the specified blob.
        /// </summary>
        /// <param name="blobId">Blob id.</param>
        /// <returns>URL of the specific blob.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public string GetBlobURL(string blobId)
        {
            Check.IsNotEmptyOrWhiteSpace(blobId, "blobId");

            CloudBlobContainer container = GetContainer(Constants.ContainerName);

            string blobUrl = container.Uri.ToString();

            return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", blobUrl, blobId.ToUpperInvariant());
        }

        /// <summary>
        /// Gets the blob content from azure as a stream.
        /// </summary>
        /// <param name="blobName">
        /// Name of the blob.
        /// </param>
        /// <returns>
        /// The blob details.
        /// </returns>
        public DataDetail GetBlobContent(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentNullException("blobName");
            }

            return GetBlob(blobName, Constants.ContainerName);
        }

        /// <summary>
        /// Uploads a file to azure as a blob.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the file is uploaded successfully; otherwise false.
        /// </returns>
        public bool UploadFile(DataDetail details)
        {
            details.CheckNotNull();
            return UploadBlobContent(details, Constants.ContainerName);
        }

        /// <summary>
        /// Deletes a file from azure.
        /// </summary>
        /// <param name="blobId">Azure file id.</param>
        /// <returns>True if the file is deleted successfully; otherwise false.</returns>
        public bool DeleteFile(string blobId)
        {
            return DeleteBlob(blobId, Constants.ContainerName);
        }

        /// <summary>
        /// Checks a file in azure.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be checked.
        /// </param>
        /// <returns>
        /// True if the file is found successfully; otherwise false.
        /// </returns>
        public bool CheckIfExists(DataDetail details)
        {
            details.CheckNotNull();
            return ExistsBlobContent(details, Constants.ContainerName);
        }

        /// <summary>
        /// Gets the content from azure as a stream.
        /// </summary>
        /// <param name="blobName">
        /// Name of the blob.
        /// </param>
        /// <param name="outputStream">
        /// The content is exposed as output stream.
        /// </param>
        /// <param name="container">
        /// Container where we could find the blob.
        /// </param>
        /// <returns>
        /// The blob properties.
        /// </returns>
        private static BlobProperties GetContent(string blobName, Stream outputStream, CloudBlobContainer container)
        {
            new { blobName, outputStream, container }.CheckNotNull();

            var blob = container.GetBlobReferenceFromServer(blobName.ToUpperInvariant());
            blob.DownloadToStream(outputStream, null, new BlobRequestOptions() { ServerTimeout = TimeSpan.FromMinutes(30) });
            return blob.Properties;
        }

        /// <summary>
        /// Used to retrieve the container reference identified by the container name.
        /// </summary>
        /// <param name="containerName">
        /// Name of the container.
        /// </param>
        /// <returns>
        /// Container instance.
        /// </returns>
        private static CloudBlobContainer GetContainer(string containerName)
        {
            var blobContainer = BlobClient.GetContainerReference(containerName);
            return blobContainer;
        }

        /// <summary>
        /// Gets the content of the blob from specified container.
        /// </summary>
        /// <param name="blobName">
        /// Name of the blob.
        /// </param>
        /// <param name="containerName">
        /// name of the container.
        /// </param>
        /// <returns>
        /// The blob details.
        /// </returns>
        private static DataDetail GetBlob(string blobName, string containerName)
        {
            new { blobName, containerName }.CheckNotNull();

            using (Stream outputStream = new MemoryStream())
            {
                CloudBlobContainer container = GetContainer(containerName);
                GetContent(blobName, outputStream, container);

                DataDetail dataDetail = new DataDetail()
                {
                    DataStream = outputStream.GetBytes()
                };

                return dataDetail;
            }
        }

        /// <summary>
        /// Checks if the blob content is present in azure or not.
        /// </summary>
        /// <param name="details">
        /// Details of the blob.
        /// </param>
        /// <param name="containerName">
        /// Name of the container.
        /// </param>
        /// <returns>
        /// True, if the blob is successfully found to azure;otherwise false.
        /// </returns>
        private static bool ExistsBlobContent(DataDetail details, string containerName)
        {
            var returnStatus = false;
            new { details, containerName }.CheckNotNull();

            CloudBlobContainer container = GetContainer(containerName);

            // TODO: Check if the input file type and then use either block blob or page blob.
            // For plate file we need to upload the file as page blob.
            var blob = container.GetBlobReferenceFromServer(details.FileDetail.BlobId.ToUpperInvariant());
            blob.FetchAttributes();
            returnStatus = true;

            return returnStatus;
        }

        /// <summary>
        /// Deletes the specified file pointed by the blob.
        /// </summary>
        /// <param name="details">
        /// Details of the blob.
        /// </param>
        /// <param name="containerName">
        /// Name of the container.
        /// </param>
        private static bool DeleteBlob(string blobId, string containerName)
        {
            var deleteStatus = false;

            Check.IsNotEmptyOrWhiteSpace(blobId, "blobId");
            Check.IsNotEmptyOrWhiteSpace(containerName, "containerName");

            try
            {
                CloudBlobContainer container = GetContainer(containerName);

                container.GetBlobReferenceFromServer(blobId.ToUpperInvariant()).Delete();
                deleteStatus = true;
            }
            catch (InvalidOperationException)
            {
                deleteStatus = false;
                throw;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.ExtendedErrorInformation.ErrorCode == "BlobNotFound")
                {
                    deleteStatus = true;
                }
                else
                {
                    deleteStatus = false;
                    throw;
                }
            }

            return deleteStatus;
        }

        /// <summary>
        /// Upload the blob content to azure.
        /// </summary>
        /// <param name="details">
        /// Details of the blob.
        /// </param>
        /// <param name="containerName">
        /// Name of the container.
        /// </param>
        /// <returns>
        /// True, if the blob is successfully uploaded to azure;otherwise false.
        /// </returns>
        private static bool UploadBlobContent(DataDetail details, string containerName)
        {
            var uploadStatus = false;
            try
            {
                new { details, containerName }.CheckNotNull();
                CloudBlobContainer container = GetContainer(containerName);

                using (Stream filestream = new MemoryStream(details.DataStream))
                {

                    // Seek to start.
                    filestream.Position = 0;

                    // TODO: Check if the input file type and then use either block blob or page blob.
                    // For plate file we need to upload the file as page blob.
                    var blob = container.GetBlockBlobReference(details.FileDetail.BlobId.ToUpperInvariant());
                    blob.Properties.ContentType = details.FileDetail.MimeType;
                    blob.UploadFromStream(filestream, null, new BlobRequestOptions() { ServerTimeout = TimeSpan.FromMinutes(30) });
                }

                uploadStatus = true;
            }
            catch (InvalidOperationException)
            {
                uploadStatus = false;
                throw;
            }
            return uploadStatus;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="blobId"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public Stream GetBlob(string blobId)
        {
            new { blobId }.CheckNotNull();

            Stream outputStream = null;
            outputStream = new MemoryStream();
            CloudBlobContainer container = GetContainer(Constants.ContainerName);
            GetContent(blobId, outputStream, container);

            return outputStream;
        }
    }
}