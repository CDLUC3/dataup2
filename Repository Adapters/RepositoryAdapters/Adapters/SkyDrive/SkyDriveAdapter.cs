// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace Microsoft.Research.DataOnboarding.RepositoryAdapters.SkyDrive
{
    public class SkyDriveAdapter : BaseAdapter, IRepositoryAdapter
    {
        private const string skydriveBaseUrl = "https://apis.live.net/v5.0";
        private const string rootFolderUrl = skydriveBaseUrl + "/me/skydrive";
        private const string skydriveUserFileUploadUrlTemplate = skydriveBaseUrl + "/{0}/files?access_token={1}&overwrite=true";
        public const string skyDriveMultiPartRequestBoundary = "A300x";
        private readonly DiagnosticsProvider diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkyDriveAdapter"/> class.
        /// </summary>
        public SkyDriveAdapter()
        {
            diagnostics = new DiagnosticsProvider(this.GetType());
        }
        
        #region public methods

        /// <summary>
        /// Method to get the identifier 
        /// </summary>
        /// <param name="queryData">query data</param>
        /// <param name="repositoryModel">repository model</param>
        /// <returns>returns string</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Repository link is passed from the client application and we are not doign the cast to URI")]
        public string GetIdentifier(MerritQueryData queryData, RepositoryModel repositoryModel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// method to post the file to repository
        /// </summary>
        /// <param name="request">request object</param>
        /// <param name="repositoryModel">repository model</param>
        /// <param name="file">file</param>
        /// <returns>returns File Identifier.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Repository link is passed from the client application and we are not doign the cast to URI")]
        public OperationStatus PostFile(PublishFileModel publishFileModel)
        {
            PublishSkyDriveFileModel publishSkyDriveFileModel = (PublishSkyDriveFileModel)publishFileModel;
            Check.IsNotNull(publishSkyDriveFileModel.File, "File");
            Check.IsNotEmptyOrWhiteSpace(publishSkyDriveFileModel.File.FileName, "File name");
            Check.IsNotNull(publishSkyDriveFileModel.File.FileContent, "File content");
            if (publishSkyDriveFileModel.File.FileContent.Length == 0)
            {
                throw new ArgumentException("File content is of zero length.");
            }

            Check.IsNotNull(publishSkyDriveFileModel.AuthToken, "AuthToken");

            string fileId = this.UploadFile(publishSkyDriveFileModel);
            OperationStatus status = OperationStatus.CreateSuccessStatus();
            status.CustomReturnValues = fileId;
            return status;
        }

        /// <summary>
        /// Retrives the Access token by passing refresh token
        /// </summary>
        /// <param name="refreshToken">refresh Token</param>
        /// <returns></returns>
        public AuthToken RefreshToken(string refreshToken)
        {
            string baseRepositoryName = BaseRepositoryEnum.SkyDrive.ToString();
            string url = ConfigReader<string>.GetRepositoryConfigValues(baseRepositoryName, SkyDriveConstants.OAuthUrl);
            string clientId = ConfigReader<string>.GetRepositoryConfigValues(baseRepositoryName, SkyDriveConstants.ClientId);
            string redirectionUrl = ConfigReader<string>.GetRepositoryConfigValues(baseRepositoryName, SkyDriveConstants.RedicrectionUrl);
            string clientSecret = ConfigReader<string>.GetRepositoryConfigValues(baseRepositoryName, SkyDriveConstants.ClientSecret);

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";

            string content = String.Format("client_id={0}&redirect_uri={1}&client_secret={2}&refresh_token={3}&grant_type=refresh_token",
                HttpUtility.UrlEncode(clientId),
                HttpUtility.UrlEncode(redirectionUrl),
                HttpUtility.UrlEncode(clientSecret),
                HttpUtility.UrlEncode(refreshToken));

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(content);
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(OAuthToken));
            OAuthToken oAuthToken = serializer.ReadObject(response.GetResponseStream()) as OAuthToken;
            int expiresInSeconds = Convert.ToInt32(oAuthToken.ExpiresIn);

            AuthToken token = new AuthToken()
            {
                AccessToken = oAuthToken.AccessToken,
                RefreshToken = oAuthToken.RefreshToken,
                TokenExpiresOn = DateTime.UtcNow.AddSeconds(expiresInSeconds)
            };

            return token;
        }

        /// <summary>
        /// Method to download the file from the repository.
        /// </summary>
        /// <param name="downloadInput">Input data required to download the file from the repository.</param>
        /// <returns>Downloaded file data.</returns>
        public DataFile DownloadFile(string downloadUrl, string authorization, string fileName)
        {
            DataFile dataFile = new DataFile();

            AuthToken token = new AuthToken()
            {
                AccessToken = authorization
            };

            dataFile.FileContent = this.DownloadFile(downloadUrl, token);
            string extension = Path.GetExtension(fileName);

            if (extension.ToUpperInvariant().Equals(Constants.XLSX, StringComparison.OrdinalIgnoreCase))
            {
                dataFile.FileExtentsion = extension;
            }
            else
            {
                dataFile.FileExtentsion = ".zip";
            }

            dataFile.FileName = string.Format("{0}{1}", Path.GetFileNameWithoutExtension(fileName), dataFile.FileExtentsion);
            return dataFile;
        }

        /// <summary>
        /// Verifies if the file exists in the repository
        /// </summary>
        /// <param name="fileIdentifier">File Identifier</param>
        /// <param name="authorization">AccessToken in case of skydrive and username password for Merrit</param>
        /// <returns>OperationStatus returns Success if the file exists </returns>
        public OperationStatus CheckIfFileExists(string downloadURL, string fileIdentifier, string authorization)
        {
            HttpWebRequest request = WebRequest.Create(string.Format("{0}/{1}/content?access_token={2}", skydriveBaseUrl, fileIdentifier, authorization)) as HttpWebRequest;
            request.Method = "Head";
            OperationStatus status = base.SendHttpRequest(request);
            return status;
        }

        #endregion
        
        #region private methods

        /// <summary>
        /// uploads the file to skydrive
        /// </summary>
        /// <param name="publishSkyDriveFileModel">PublishSkyDriveFileModel </param>
        /// <returns>File Id</returns>
        private string UploadFile(PublishSkyDriveFileModel publishSkyDriveFileModel)
        {
            Repository repository = publishSkyDriveFileModel.Repository;
            DataFile dataFile = publishSkyDriveFileModel.File;
            int fileId = publishSkyDriveFileModel.File.FileInfo.FileId;
            string fileName = Path.GetFileNameWithoutExtension(dataFile.FileName);

            string folderId = CreateFolderForFile(repository.Name, fileId.ToString(), fileName, publishSkyDriveFileModel.AuthToken);
            string url = string.Format(skydriveUserFileUploadUrlTemplate, folderId, publishSkyDriveFileModel.AuthToken.AccessToken);

            //Build Request Body
            Encoding encoding = Encoding.UTF8;
            string boundary = "--" + skyDriveMultiPartRequestBoundary;
            Stream formData = new MemoryStream();
            StringBuilder sbRequest = new StringBuilder();
            sbRequest.AppendLine(boundary);
            sbRequest.AppendLine(String.Format("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"", dataFile.FileName));
            sbRequest.AppendLine("Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            sbRequest.AppendLine();

            byte[] bodyStart = encoding.GetBytes(sbRequest.ToString());
            formData.Write(bodyStart, 0, bodyStart.Length);

            byte[] fileContents = dataFile.FileContent;
            formData.Write(fileContents, 0, fileContents.Length);

            sbRequest.Clear();
            sbRequest.AppendLine();
            sbRequest.AppendLine(boundary + "--");
            byte[] bodyEnd = encoding.GetBytes(sbRequest.ToString());
            formData.Write(bodyEnd, 0, bodyEnd.Length);

            formData.Position = 0;

            byte[] outboundBytes = new byte[formData.Length];
            formData.Read(outboundBytes, 0, outboundBytes.Length);

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = String.Format("multipart/form-data; boundary={0}", skyDriveMultiPartRequestBoundary);
            request.ContentLength = outboundBytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(outboundBytes, 0, outboundBytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Content));
                Content content = serializer.ReadObject(response.GetResponseStream()) as Content;
                return content.Id;
            }
        }

        /// <summary>
        /// The method creates the folder structure to upload the file
        /// </summary>
        /// <param name="repositoryName">Repository Name</param>
        /// <param name="fileId">File Id</param>
        /// <param name="authToken">Auth Token</param>
        /// <returns>Folder Id</returns>
        private string CreateFolderForFile(string repositoryName, string fileId, string fileName, AuthToken authToken)
        {
            string parentFolderId = GetRootFolder(authToken);

            diagnostics.WriteVerboseTrace(TraceEventId.Flow, string.Format("Root Folder {0}", parentFolderId));
           
            // Check if the Folder with name as Repostory exists if not then create a folder with repository name
            string repositoryFolderId = this.GetOrCreateFolder(repositoryName, parentFolderId, authToken);
            
            //Check if the folder for current date exists.
            string datefolderName = DateTime.UtcNow.ToString("yyyy-MM-dd");
            string dateFolderId = this.GetOrCreateFolder(datefolderName, repositoryFolderId, authToken);
          

            //Check if the folder with the name as File Id exists if not then create 
            string fileFolderName = string.Format("{0}_{1}", fileName, fileId);
            string fileFolderId = this.GetOrCreateFolder(fileFolderName, dateFolderId, authToken);

            // return the folder Id. the file will be uploaded to this folder.
            return fileFolderId;
        }

        /// <summary>
        /// Checks if the folder exists and if not then creates
        /// </summary>
        /// <param name="folderName">Folder to be created</param>
        /// <param name="parentFolderId">Parent folder Id</param>
        /// <param name="authToken">Auth Token</param>
        /// <returns>Folder Id</returns>
        private string GetOrCreateFolder(string folderName, string parentFolderId, AuthToken authToken)
        {
            string folderId = GetFolder(folderName, parentFolderId, authToken);

            if (string.IsNullOrEmpty(folderId))
            {
                folderId = CreateFolder(folderName, parentFolderId, authToken);
                diagnostics.WriteVerboseTrace(TraceEventId.Flow, string.Format("Folder {0} created", folderName));
            }

            return folderId;
        }

        /// <summary>
        /// Creates the folder.
        /// </summary>
        /// <param name="folderName">Folder Name.</param>
        /// <param name="parentFolderId">Parent Folder Id.</param>
        /// <param name="token">AuthToken instance.</param>
        /// <returns>Folder Id.</returns>
        private string CreateFolder(string folderName, string parentFolderId, AuthToken token)
        {
            HttpWebRequest request = WebRequest.Create(string.Format("{0}/{1}", skydriveBaseUrl, parentFolderId)) as HttpWebRequest;
            request.Headers.Add("Authorization", string.Format("Bearer {0}",HttpUtility.UrlEncode(token.AccessToken)));
            request.Method = "POST";
            request.ContentType = "application/json;boundary=" + skyDriveMultiPartRequestBoundary;
            
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("name", folderName);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string postData = serializer.Serialize(data);

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(postData);
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                DataContractJsonSerializer dataContractSerializer = new DataContractJsonSerializer(typeof(Content));
                Content content = dataContractSerializer.ReadObject(response.GetResponseStream()) as Content;

                if (content != null)
                {
                    return content.Id;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the Folder Id.
        /// </summary>
        /// <param name="folderName">Folder Id.</param>
        /// <param name="parentFolderId">Parent Folder Id.</param>
        /// <param name="token">AuthToken instance.</param>
        /// <returns>Folder Id.</returns>
        private string GetFolder(string folderName, string parentFolderId, AuthToken token )
        {
            Structure structure = this.GetChildFolders(parentFolderId, token);
            Content folder = structure.Items.Where(i => i.Name.ToLowerInvariant() == folderName.ToLowerInvariant()).FirstOrDefault();

            if (folder != null)
            {
                return folder.Id;
            }

            return null;
        }

        /// <summary>
        /// Returns the list of child folder.
        /// </summary>
        /// <param name="parentFolderId">Parent Folder Id.</param>
        /// <param name="token">AuthToken instance.</param>
        /// <returns>Strucure Instance.</returns>
        private Structure GetChildFolders(string parentFolderId, AuthToken token)
        {
            HttpWebRequest request = WebRequest.Create(string.Format("{0}/{1}/files?access_token={2}", skydriveBaseUrl, parentFolderId, token.AccessToken)) as HttpWebRequest;
            request.Method = "GET";
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Structure));
                Structure structure = serializer.ReadObject(response.GetResponseStream()) as Structure;
                return structure;
            }
        }

        /// <summary>
        /// Gets the Root Folder.
        /// </summary>
        /// <param name="token">AuthToken Instance.</param>
        /// <returns>Folder Id.</returns>
        private string GetRootFolder(AuthToken token)
        {
            HttpWebRequest request = WebRequest.Create(string.Format("{0}?access_token={1}", rootFolderUrl, token.AccessToken)) as HttpWebRequest;
            request.Method = "GET";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Content));
                Content rootFolder = serializer.ReadObject(response.GetResponseStream()) as Content;
                return rootFolder.Id;
            }
        }
        
        /// <summary>
        /// Downloads the file to a temp location
        /// </summary>
        /// <param name="fileId">File Id</param>
        /// <param name="fileName">File Name</param>
        /// <param name="tempFolderPath">Temp folder path</param>
        /// <param name="token">AuthToken</param>
        /// <returns></returns>
        private OperationStatus DownloadFile(string fileId,string fileName, string tempFolderPath, AuthToken token)
        {
            OperationStatus status;
            try
            {
                HttpWebRequest request = WebRequest.Create(string.Format("{0}/{1}/content?access_token={2}", skydriveBaseUrl, fileId, token.AccessToken)) as HttpWebRequest;
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    string filePath = string.Format("{0}/{1}", tempFolderPath, fileName);
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        using (Stream s = response.GetResponseStream())
                        {
                            byte[] read = new byte[256];
                            int count = s.Read(read, 0, read.Length);
                            
                            while (count > 0)
                            {
                                fs.Write(read, 0, count);
                                count = s.Read(read, 0, read.Length);
                            }
                        }
                    }
                }

                status = OperationStatus.CreateSuccessStatus();
            }
            catch (Exception exception)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, exception);
                status = OperationStatus.CreateFailureStatus(exception);
            }

            return status;
        }

        /// <summary>
        /// Downloads the file and returns the byte array
        /// </summary>
        /// <param name="fileId">Skydrive File Id</param>
        /// <param name="token">AuthToken</param>
        /// <returns>Byte array</returns>
        private byte[] DownloadFile(string fileId, AuthToken token)
        {
            byte[] fileContent ;
            HttpWebRequest request = WebRequest.Create(string.Format("{0}/{1}/content?access_token={2}", skydriveBaseUrl, fileId, token.AccessToken)) as HttpWebRequest;
            request.Method = "GET";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        byte[] read = new byte[256];
                        int count = stream.Read(read, 0, read.Length);
                        while (count > 0)
                        {
                            memoryStream.Write(read, 0, count);
                            count = stream.Read(read, 0, read.Length);
                        }
                    }

                    fileContent = memoryStream.ToArray();
                }
            }

            return fileContent;
        }
        
        #endregion
    }
}
