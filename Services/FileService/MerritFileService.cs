// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.DomainModel.ConceptualModel;
using Microsoft.Research.DataOnboarding.FileService.Enums;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.RepositoriesService;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.RepositoryAdapters;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces;
using Microsoft.Research.DataOnboarding.Services.UserService;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Newtonsoft.Json;
using DM = Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.FileService
{
    public class MerritFileService : FileServiceProvider
    {
        /// <summary>
        /// Holds the reference to userService.
        /// </summary>
        private IUserService userService;

        public MerritFileService(IFileRepository fileDataRepository, IBlobDataRepository blobDataRepository, IUnitOfWork unitOfWork, IRepositoryDetails repositoryDetails, IRepositoryService repositoryService, IRepositoryAdapterFactory repositoryAdapterFactory, IUserService userService) :
            base(fileDataRepository, blobDataRepository, unitOfWork, repositoryDetails, repositoryService, repositoryAdapterFactory)
        {
            this.userService = userService;
        }

        /// <summary>
        /// Method to publish file
        /// </summary>
        /// <param name="postFileData">PublishMessage object</param>
        /// <returns>returns File Identifier</returns>
        public override string PublishFile(PublishMessage postFileData)
        {
            var file = this.GetFileByFileId(postFileData.FileId);
            IEnumerable<DM.FileColumnType> fileColumnTypes = null;
            IEnumerable<DM.FileColumnUnit> fileColumnUnits = null;
            OperationStatus status = null;

            Encoding encoding = Encoding.UTF8;
            string identifier = string.Empty;


            var repository = this.RepositoryService.GetRepositoryById(postFileData.RepositoryId);

            // TODO: Needs to check Filling repository credintials in case of system provided repository type
            if (repository.BaseRepositoryId == Constants.MerritRepositoryTypeID && repository.IsImpersonating == true)
            {
                postFileData.UserName = repository.ImpersonatingUserName;
                postFileData.Password = repository.ImpersonatingPassword;
            }

            string authorization = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(postFileData.UserName /*UserName*/ + ":" + postFileData.Password /*Password*/));

            string baseRepoName = this.RepositoryDetails.GetBaseRepositoryName(repository.BaseRepositoryId);
            this.RepositoryAdapter = new MerritAdapter();
            
            // This is a temporary fix to initialize citation for 
            // Merritt repository. 
            // TBD: move this to the base class 
            Citation citation = default(Citation);
            if (string.IsNullOrWhiteSpace(file.Citation))
            {
                User user = this.userService.GetUserById(postFileData.UserId);
                citation = new Citation();
                citation.Title = Path.GetFileNameWithoutExtension(file.Name);
                citation.Publisher = string.Join(",", user.LastName, user.FirstName);
            }
            else
            {
                citation = JsonConvert.DeserializeObject<Citation>(file.Citation);
            }
            
            identifier = this.GetIdentifier(postFileData, authorization, repository.HttpIdentifierUriTemplate, citation);

            if (identifier.StartsWith("true"))
            {
                identifier = identifier.Substring(identifier.IndexOf("ark:", StringComparison.Ordinal)).Trim();

                var postQueryData = GetPostQueryData(identifier, file, citation, repository, postFileData);
                var dataFile = this.GetDataFile(file);
                var repositoryModel = GetRepositoryModel(repository, authorization);

                if (file.FileColumns != null && file.FileColumns.Count > 0)
                {
                    // before posting the file set file column units and types
                    fileColumnTypes = this.RetrieveFileColumnTypes();
                    fileColumnUnits = this.RetrieveFileColumnUnits();
                }

                PublishMerritFileModel publishMerritFileModel = new PublishMerritFileModel()
                {
                    File = dataFile,
                    RepositoryModel = repositoryModel,
                    FileColumnTypes = fileColumnTypes,
                    FileColumnUnits = fileColumnUnits,
                    MerritQueryData = postQueryData
                };

                //post the file
                status = this.RepositoryAdapter.PostFile(publishMerritFileModel);
            }
            else
            {
                string message = identifier.Replace("false|", "");

                // Todo need to throw a custom exception
                throw new SystemException(message);
            }

            return identifier;
        }

        /// <summary>
        ///  Verifies if the file exists in the repository
        /// </summary>
        /// <param name="fileId">File Id</param>
        /// <param name="repositoryId">Repository Id</param>
        /// <param name="userId">User Id</param>
        /// <returns>OperationStatus returns Success if the file exists</returns>
        public override OperationStatus CheckIfFileExistsOnExternalRepository(VerifyFileMessage verifyFileMessage)
        {
            Repository repository = this.RepositoryService.GetRepositoryById(verifyFileMessage.RepositoryId);
            DM.File file = this.GetFileByFileId(verifyFileMessage.FileId);
            this.RepositoryAdapter = this.RepositoryAdapterFactory.GetRepositoryAdapter(repository.BaseRepository.Name);

            string userName;
            string password;
            if ((bool)repository.IsImpersonating)
            {
                userName = repository.ImpersonatingUserName;
                password = repository.ImpersonatingPassword;
            }
            else
            {
                userName = verifyFileMessage.UserName;
                password = verifyFileMessage.Password;
            }

            string authorization = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(userName /*UserName*/ + ":" + password /*Password*/));
            string downloadURL = String.Join(string.Empty, repository.HttpGetUriTemplate, HttpUtility.UrlEncode(file.Identifier));
            OperationStatus status = this.RepositoryAdapter.CheckIfFileExists(downloadURL, file.Identifier, authorization);
            return OperationStatus.CreateSuccessStatus();
        }

        /// <summary>
        /// Performs the necessary validations required for the file to be published in skydrive
        /// </summary>
        /// <param name="message">Publish Message</param>
        public override void ValidateForPublish(PublishMessage message)
        {
            base.ValidateForPublish(message);

            Repository repository = this.RepositoryService.GetRepositoryById(message.RepositoryId);

            if (!(bool)repository.IsImpersonating)
            {
                if (string.IsNullOrEmpty(message.UserName))
                {
                    throw new ArgumentException(string.Empty, "UserName");
                }

                if (string.IsNullOrEmpty(message.Password))
                {
                    throw new ArgumentException(string.Empty, "Password");
                }
            }
        }

        /// <summary>
        /// Downloads the File from Repository
        /// </summary>
        /// <param name="file">File object.</param>
        /// <param name="repository">Repository instance.</param>
        /// <param name="user">User instance.</param>
        /// <param name="credentials">credentials required by the repository.</param>
        /// <returns>DataFile containing the file data.</returns>
        public override DataFile DownLoadFileFromRepository(DomainModel.File file, Repository repository, User user, RepositoryCredentials credentials)
        {
            string userName = string.Empty;
            string password = string.Empty;
           
            if ((bool)repository.IsImpersonating)
            {
                userName = repository.ImpersonatingUserName;
                password = repository.ImpersonatingPassword;
            }
            else
            {
                userName = credentials[BasicAuthenticationCredentialKeys.UserName];
                password = credentials[BasicAuthenticationCredentialKeys.Password];

                if (string.IsNullOrEmpty(userName))
                {
                    throw new ArgumentNullException(BasicAuthenticationCredentialKeys.UserName);
                }

                if (string.IsNullOrEmpty(password))
                {
                    throw new ArgumentNullException(BasicAuthenticationCredentialKeys.Password);
                }
            }

            var authorization = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(userName /*UserName*/ + ":" + password/*Password*/));

            if (string.IsNullOrEmpty(repository.HttpGetUriTemplate))
            {
                throw new FileDownloadException()
                {
                    FileId = file.FileId,
                    RepositoryId = repository.RepositoryId,
                    FileDownloadExceptionType = FileDownloadExceptionType.DownloadUrlNotFound.ToString()
                };
            }

            var downloadURL = string.Join(string.Empty, repository.HttpGetUriTemplate, HttpUtility.UrlEncode(file.Identifier));
            IRepositoryAdapter repAdapter = this.RepositoryAdapterFactory.GetRepositoryAdapter(repository.BaseRepository.Name);
            DataFile dataFile = repAdapter.DownloadFile(downloadURL, authorization, file.Name);
            return dataFile;
        }
    }
}
