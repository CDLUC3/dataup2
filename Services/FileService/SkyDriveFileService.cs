// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.Exceptions;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces;
using Microsoft.Research.DataOnboarding.Services.UserService;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using DM = Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.FileService
{
    public class SkyDriveFileService:FileServiceProvider
    {
        /// <summary>
        /// Holds the reference to diagnostics provider
        /// </summary>
        private readonly DiagnosticsProvider diagnostics;

        /// <summary>
        /// Holds the reference to userService.
        /// </summary>
        private IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkyDriveFileService"/> class.
        /// </summary>
        /// <param name="fileDataRepository">IFileRepository instance.</param>
        /// <param name="blobDataRepository">IBlobDataRepository instance.</param>
        /// <param name="unitOfWork">IUnitOfWork instance.</param>
        /// <param name="repositoryDetails">IRepositoryDetails instance.</param>
        /// <param name="repositoryService">IRepositoryService instance.</param>
        /// <param name="userService">IUserService instance</param>
        /// <param name="repositoryAdapterFactory">IRepositoryAdapterFactory instance.</param>
        public SkyDriveFileService(IFileRepository fileDataRepository, IBlobDataRepository blobDataRepository, IUnitOfWork unitOfWork, IRepositoryDetails repositoryDetails,IRepositoryService repositoryService, IUserService userService, IRepositoryAdapterFactory repositoryAdapterFactory)
            : base(fileDataRepository, blobDataRepository, unitOfWork, repositoryDetails, repositoryService, repositoryAdapterFactory)
        {
            this.userService = userService;
            diagnostics = new DiagnosticsProvider(this.GetType());
        }

        /// <summary>
        /// Method to publish file
        /// </summary>
        /// <param name="postFileData">PostFileModel object</param>
        /// <returns>returns File Identifier</returns>
        public override string PublishFile(PublishMessage publishModel)
        {
            string fileIndentifier = null;

            var file = this.GetFileByFileId(publishModel.FileId);
            Check.IsNotNull<DM.File>(file, "fileToPublish");
            
            IEnumerable<DM.FileColumnType> fileColumnTypes = null;
            IEnumerable<DM.FileColumnUnit> fileColumnUnits = null;
            OperationStatus status = null;
            Encoding encoding = Encoding.UTF8;
            string identifier = string.Empty;
           
            Repository repository = this.RepositoryService.GetRepositoryById(publishModel.RepositoryId);
            string baseRepoName = repository.BaseRepository.Name;

            this.RepositoryAdapter = this.RepositoryAdapterFactory.GetRepositoryAdapter(baseRepoName);
                
            DataFile dataFile = this.GetDataFile(file);
               
            if (file.FileColumns != null && file.FileColumns.Count > 0)
            {
                // before posting the file set file column units and types
                fileColumnTypes = this.RetrieveFileColumnTypes();
                fileColumnUnits = this.RetrieveFileColumnUnits();
            }

            // Set the user Id on AuthToken
            publishModel.AuthToken.UserId = publishModel.UserId;
            publishModel.AuthToken.RespositoryId = repository.RepositoryId;
            AuthToken authToken = this.GetOrUpdateAuthTokens(repository, publishModel.AuthToken);

            PublishSkyDriveFileModel publishSkyDriveFileModel = new PublishSkyDriveFileModel()
            {
                File = dataFile,
                Repository = repository,
                FileColumnTypes = fileColumnTypes,
                FileColumnUnits = fileColumnUnits,
                AuthToken = authToken
            };

            //post the file
            status = this.RepositoryAdapter.PostFile(publishSkyDriveFileModel);

            if (status.Succeeded)
            {
                fileIndentifier = (string)status.CustomReturnValues;
            }

            return fileIndentifier;
        }

        /// <summary>
        /// Downloads the File from Repository
        /// </summary>
        /// <param name="file">File object.</param>
        /// <param name="repository">Repository instance.</param>
        /// <param name="user">User instance.</param>
        /// <param name="credentials">credentials required by the repository.</param>
        /// <returns>DataFile containing the file data.</returns>
        public override DataFile DownLoadFileFromRepository(File file, Repository repository, User user, RepositoryCredentials credentials)
        {
            // construct the AuthToken object
            AuthToken authToken = new AuthToken()
            {
                UserId = user.UserId,
                RespositoryId = repository.RepositoryId
            };

            if (credentials != null)
            {
                authToken.AccessToken = credentials[WindowsLiveAuthenticationCredentialKeys.AccessToken];
                authToken.RefreshToken = credentials[WindowsLiveAuthenticationCredentialKeys.RefreshToken];
                
                if (credentials[WindowsLiveAuthenticationCredentialKeys.TokenExpiresOn] != null)
                {
                    DateTime tokenExpiryDate;
                    if (DateTime.TryParse(credentials[WindowsLiveAuthenticationCredentialKeys.TokenExpiresOn], out tokenExpiryDate))
                    {
                        authToken.TokenExpiresOn = tokenExpiryDate;
                    };
                }
            }
            
            this.RepositoryAdapter = this.RepositoryAdapterFactory.GetRepositoryAdapter(repository.BaseRepository.Name);

            // Retreive the AuthToken from database or save the token to database if access token is present in the credentials.
            authToken = this.GetOrUpdateAuthTokens(repository, authToken);
           
            DataFile dataFile = this.RepositoryAdapter.DownloadFile(file.Identifier, authToken.AccessToken, file.Name);
            return dataFile;           
        }

        /// <summary>
        ///  Verifies if the file exists in the repository
        /// </summary>
        /// <param name="verifyFileMessage">Verify File Message</param>
        /// <returns>OperationStatus returns Success if the file exists</returns>
        public override OperationStatus CheckIfFileExistsOnExternalRepository(VerifyFileMessage verifyFileMessage)
        {
            Repository repository = this.RepositoryDetails.GetRepositoryById(verifyFileMessage.RepositoryId);
            DM.File file = this.GetFileByFileId(verifyFileMessage.FileId);
            this.RepositoryAdapter = this.RepositoryAdapterFactory.GetRepositoryAdapter(repository.BaseRepository.Name);
            AuthToken token = this.GetAuthTokens(repository, verifyFileMessage.UserId);
            OperationStatus status = this.RepositoryAdapter.CheckIfFileExists(string.Empty, file.Identifier, token.AccessToken);
            return status;
        }

        /// <summary>
        /// Performs the necessary validations required for the file to be published in skydrive
        /// </summary>
        /// <param name="message">Publish Message</param>
        public override void ValidateForPublish(PublishMessage message)
        {
            base.ValidateForPublish(message);

            Repository repository = this.RepositoryService.GetRepositoryById(message.RepositoryId);
            
            if ((bool)repository.IsImpersonating)
            {
                return;
            }

            if (string.IsNullOrEmpty(message.AuthToken.AccessToken))
            {
                AuthToken userAuthToken = this.userService.GetUserAuthToken(message.UserId, message.RepositoryId);
                if (null == userAuthToken)
                {
                    throw new AccessTokenNotFoundException()
                    {
                        RepositoryId = message.RepositoryId,
                        UserId = message.RepositoryId
                    };
                }
            }
        }

        /// <summary>
        /// Method to save the file data.
        /// </summary>
        /// <param name="postFileData">Post file midel.</param>
        /// <returns>True incase of success else false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public override bool SaveFile(PostFileModel postFileData)
        {
            bool result = base.SaveFile(postFileData);
            AuthToken userAuthToken = postFileData.UserAuthToken;

            // Add the user oAuthToken to the database.
            if (!string.IsNullOrEmpty(userAuthToken.AccessToken))
            {
                postFileData.UserAuthToken = this.userService.AddUpdateAuthToken(postFileData.UserAuthToken);
            }

            return result;
        }

        #region private methods

        /// <summary>
        /// Gets or updates the AuthTokens
        /// </summary>
        /// <param name="repository">Repository</param>
        /// <param name="userAuthToken">AuthToken</param>
        /// <returns>AuthToken</returns>
        private AuthToken GetOrUpdateAuthTokens(Repository repository, AuthToken userAuthToken)
        {
            AuthToken token = userAuthToken;
            
            // if Impersonating then get the token from repository
            if ((bool)repository.IsImpersonating)
            {
                diagnostics.WriteVerboseTrace(TraceEventId.Flow, "Impersonation is ON");
                token = new AuthToken()
                {
                    AccessToken = repository.AccessToken,
                    RefreshToken = repository.RefreshToken,
                    TokenExpiresOn = (DateTime)repository.TokenExpiresOn
                };
            }
            // if accessToken is null then get the authtoken from the database
            else
            {
                if (string.IsNullOrEmpty(userAuthToken.AccessToken))
                {
                    diagnostics.WriteVerboseTrace(TraceEventId.Flow, "Get the token from the database");
                    token = this.userService.GetUserAuthToken(userAuthToken.UserId, repository.RepositoryId);
                }
                else if(userAuthToken.Id <= 0)
                {
                    diagnostics.WriteVerboseTrace(TraceEventId.Flow, "Request has the token so add the token to database");
                    token = this.userService.AddUpdateAuthToken(userAuthToken);
                }
            }

            if (null == token)
            {
                throw new AccessTokenNotFoundException()
                {
                    RepositoryId = repository.RepositoryId,
                    UserId = userAuthToken.UserId
                };
            }

            // check if the AuthToken expired if yes then get new access token from refresh token
            return RefreshAccessToken(repository, token);
        }

        /// <summary>
        /// Returns the AuthToken for the Repository and User
        /// </summary>
        /// <param name="repository">Repository instance.</param>
        /// <param name="userId">User Id.</param>
        /// <returns>AuthToken object.</returns>
        private AuthToken GetAuthTokens(Repository repository, int userId)
        {
            AuthToken token;

            // if Impersonating then get the token from repository
            if ((bool)repository.IsImpersonating)
            {
                diagnostics.WriteVerboseTrace(TraceEventId.Flow, "Impersonation is ON");
                token = new AuthToken()
                {
                    AccessToken = repository.AccessToken,
                    RefreshToken = repository.RefreshToken,
                    TokenExpiresOn = (DateTime)repository.TokenExpiresOn
                };
            }
            else
            {
                diagnostics.WriteVerboseTrace(TraceEventId.Flow, "Get the token from the database");
                token = this.userService.GetUserAuthToken(userId, repository.RepositoryId);
            }

            if (null == token)
            {
                throw new AccessTokenNotFoundException()
                {
                    RepositoryId = repository.RepositoryId,
                    UserId = userId
                };
            }

            // check if the AuthToken expired if yes then get new access token from refresh token
            return RefreshAccessToken(repository, token);
        }
        
        /// <summary>
        /// validates if the token is expired
        /// </summary>
        /// <param name="token">AuthToken</param>
        /// <returns>Boolean</returns>
        private AuthToken RefreshAccessToken(Repository repository, AuthToken token)
        {
            if (DateTime.UtcNow < token.TokenExpiresOn)
            {
                return token;
            }

            AuthToken freshToken = this.RepositoryAdapter.RefreshToken(token.RefreshToken);

            freshToken.RespositoryId = token.RespositoryId;
            freshToken.UserId = token.UserId;

            if ((bool)repository.IsImpersonating)
            {
                repository.AccessToken = freshToken.AccessToken;
                repository.RefreshToken = freshToken.RefreshToken;
                repository.TokenExpiresOn = freshToken.TokenExpiresOn;
                this.RepositoryDetails.UpdateRepository(repository);
            }
            else
            {
                this.userService.AddUpdateAuthToken(freshToken);
            }

            return freshToken;
        }

        #endregion
    }
}
