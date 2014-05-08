using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.DataAccessService.Fakes;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService;
using Microsoft.Research.DataOnboarding.FileService.Fakes;
using Microsoft.Research.DataOnboarding.FileService.FileProcesser;
using Microsoft.Research.DataOnboarding.FileService.FileProcesser.Fakes;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces.Fakes;
using Microsoft.Research.DataOnboarding.Services.UserService;
using Microsoft.Research.DataOnboarding.Services.UserService.Fakes;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Fakes;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;
using Fakes = Microsoft.Research.DataOnboarding.DataAccessService.Fakes;

namespace FileService.Tests
{
    /// <summary>
    /// class to test the SkyDriveFile Service methods
    /// </summary>
    [TestClass]
    public class SkyDriveFileServiceTest
    {
        private IRepositoryDetails repositoryDetails;

        private IRepositoryAdapter skyDriveAdapter;

        private IFileRepository fileRepository;

        private IUserService userService;

        private IBlobDataRepository blobDataRepository;

        private NameValueCollection skyDriveConfiguration;

        private Repository repository;

        private AuthToken userAuthToken;

        private StubIRepositoryService repositoryService;

        /// <summary>
        /// Method to Initialize the 
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
        }

        /// <summary>
        /// The method is to test repository creation
        /// </summary>
        [TestMethod]
        public void Allow_Publish_For_Impersonated_Repository()
        {
            // construct the model
            // construct the model
            AuthToken authToken = new AuthToken()
            {
                UserId = 1,
                RespositoryId = 2
            };

            File fakeFile = new File() { Name = "test1", FileId = 100, RepositoryId = 1, CreatedBy = 1, Status = FileStatus.None.ToString() };
            PublishMessage model = new PublishMessage() { RepositoryId = 1, AuthToken = authToken, FileId = fakeFile.FileId, UserId = 1 };
            this.repository = new Repository() { BaseRepositoryId = 2, Name = "test", IsImpersonating = true, AccessToken = "accessToken", RefreshToken = "refreshToken", TokenExpiresOn = DateTime.UtcNow.AddHours(1), BaseRepository = new BaseRepository { Name = "SkyDrive" } };
            SkyDriveFileService skyDriveFileService = this.GetSkyDriveFileService();
            IFileProcesser fileProcessor = new StubIFileProcesser()
            {
                DownloadDocumentFile = file => new DataDetail() { FileNameToDownLoad = "abc.xyz" }
            };

            string fileIdentifier;
            using (ShimsContext.Create())
            {
                ShimFileFactory.GetFileTypeInstanceStringIBlobDataRepositoryIFileRepositoryIRepositoryService = (fileExtension, blobDataRepository, fileDataRepository, repositoryService) => fileProcessor;
                fileIdentifier = skyDriveFileService.PublishFile(model);
            }

            Assert.IsFalse(string.IsNullOrEmpty(fileIdentifier));
        }

        [TestMethod]
        public void Allow_Publish_When_AccessToken_Is_Null()
        {
            // construct the model
            AuthToken authToken = new AuthToken()
            {
                UserId = 1,
                RespositoryId = 2
            };

            File fakeFile = new File() { Name = "test1", FileId = 100, RepositoryId = 1, CreatedBy = 1, Status = FileStatus.None.ToString() };
            PublishMessage model = new PublishMessage() { RepositoryId = 1, AuthToken = authToken, FileId = fakeFile.FileId, UserId = 1 };
            this.repository = new Repository() { BaseRepositoryId = 2, Name = "test", IsImpersonating = false, BaseRepository = new BaseRepository { Name = "SkyDrive" } };

            this.userAuthToken = new AuthToken()
            {
                AccessToken = "accessToken",
                RefreshToken = "refreshToken",
                TokenExpiresOn = DateTime.UtcNow.AddHours(1)
            };

            SkyDriveFileService skyDriveFileService = this.GetSkyDriveFileService();
            IFileProcesser fileProcessor = new StubIFileProcesser()
            {
                DownloadDocumentFile = file => new DataDetail() { FileNameToDownLoad = "abc.xyz" }
            };

            string fileIdentifier;
            using (ShimsContext.Create())
            {
                ShimFileFactory.GetFileTypeInstanceStringIBlobDataRepositoryIFileRepositoryIRepositoryService = (fileExtension, blobDataRepository, fileDataRepository, repositoryService) => fileProcessor;
                fileIdentifier = skyDriveFileService.PublishFile(model);
            }

            Assert.IsFalse(string.IsNullOrEmpty(fileIdentifier));
        }

        [TestMethod]
        public void Allow_Publish_When_UserAuthToken_Exist_In_Model()
        {
            //TODO Ram 
            // Construct the auth token object
            AuthToken authToken = new AuthToken()
            {
                AccessToken = "accessToken",
                RefreshToken = "refreshToken",
                TokenExpiresOn = DateTime.UtcNow.AddHours(1)
            };

            File fakeFile = new File() { Name = "test1", FileId = 100, RepositoryId = 1, CreatedBy = 1, Status = FileStatus.None.ToString() };
            PublishMessage model = new PublishMessage() { RepositoryId = 1, AuthToken = authToken, FileId = fakeFile.FileId, UserId = 1 };
            this.repository = new Repository() { BaseRepositoryId = 2, Name = "test", IsImpersonating = false, BaseRepository = new BaseRepository { Name = "SkyDrive" } };

            SkyDriveFileService skyDriveFileService = this.GetSkyDriveFileService();
            IFileProcesser fileProcessor = new StubIFileProcesser()
            {
                DownloadDocumentFile = file => new DataDetail() { FileNameToDownLoad = "abc.xyz" }
            };

            string fileIdentifier;
            using (ShimsContext.Create())
            {
                ShimFileFactory.GetFileTypeInstanceStringIBlobDataRepositoryIFileRepositoryIRepositoryService = (fileExtension, blobDataRepository, fileDataRepository, repositoryService) => fileProcessor;
                fileIdentifier = skyDriveFileService.PublishFile(model);
            }

            Assert.IsFalse(string.IsNullOrEmpty(fileIdentifier));
        }

        [TestMethod]
        public void Allow_Publish_When_Token_Is_Expired_For_Non_Impersonating_Repository()
        {
            //TODO Ram 
            // construct the model
            AuthToken authToken = new AuthToken()
            {
                AccessToken = "accessToken",
                RefreshToken = "refreshToken",
                TokenExpiresOn = DateTime.UtcNow.AddHours(-1)
            };

            File fakeFile = new File() { Name = "test1", FileId = 100, RepositoryId = 1, CreatedBy = 1, Status = FileStatus.None.ToString() };
            PublishMessage model = new PublishMessage() { RepositoryId = 1, AuthToken = authToken, FileId = fakeFile.FileId, UserId = 1 };
            this.repository = new Repository() { BaseRepositoryId = 2, Name = "test", IsImpersonating = false, BaseRepository = new BaseRepository { Name = "SkyDrive" } };
            var fileProvider = this.GetSkyDriveFileService();
            IFileProcesser fileProcessor = new StubIFileProcesser()
            {
                DownloadDocumentFile = file => new DataDetail() { FileNameToDownLoad = "abc.xyz" }
            };

            string fileIdentifier;
            using (ShimsContext.Create())
            {
                ShimFileFactory.GetFileTypeInstanceStringIBlobDataRepositoryIFileRepositoryIRepositoryService = (fileExtension, blobDataRepository, fileDataRepository, repositoryService) => fileProcessor;
                fileIdentifier = fileProvider.PublishFile(model);
            }

            Assert.IsFalse(string.IsNullOrEmpty(fileIdentifier));
        }

        [TestMethod]
        public void Allow_Publish_When_Token_Is_Expired_For_Impersonating_Repository()
        {
            // construct the model
            AuthToken authToken = new AuthToken()
            {
                AccessToken = "accessToken",
                RefreshToken = "refreshToken",
                TokenExpiresOn = DateTime.UtcNow.AddHours(-1)
            };

            File fakeFile = new File() { Name = "test1", FileId = 100, RepositoryId = 1, CreatedBy = 1, Status = FileStatus.None.ToString() };
            PublishMessage model = new PublishMessage() { RepositoryId = 3, AuthToken = authToken, FileId = fakeFile.FileId, UserId = 1 };
            this.repository = new Repository() { RepositoryId = 1, Name = "test", IsImpersonating = false, AccessToken = "accessToken", RefreshToken = "refreshToken", TokenExpiresOn = DateTime.UtcNow.AddHours(1), BaseRepository = new BaseRepository { Name = "SkyDrive" } };
            var fileProvider = this.GetSkyDriveFileService();
            IFileProcesser fileProcessor = new StubIFileProcesser()
            {
                DownloadDocumentFile = file => new DataDetail() { FileNameToDownLoad = "abc.xyz" }
            };

            string fileIdentifier;
            using (ShimsContext.Create())
            {
                ShimFileFactory.GetFileTypeInstanceStringIBlobDataRepositoryIFileRepositoryIRepositoryService = (fileExtension, blobDataRepository, fileDataRepository, repositoryService) => fileProcessor;
                fileIdentifier = fileProvider.PublishFile(model);
            }

            Assert.IsFalse(string.IsNullOrEmpty(fileIdentifier));
        }

        [TestMethod]
        public void Return_ErrorCode_When_Adapter_Returns_Failure()
        {
            // construct the model
            AuthToken authToken = new AuthToken()
            {
                AccessToken = "accessToken",
                RefreshToken = "refreshToken",
                TokenExpiresOn = DateTime.UtcNow.AddHours(-1),
                RespositoryId = 1
            };

            File fakeFile = new File() { Name = "test1", FileId = 100, RepositoryId = 1, CreatedBy = 1, Status = FileStatus.None.ToString() };
            PublishMessage model = new PublishMessage() { RepositoryId = 3, AuthToken = authToken, FileId = fakeFile.FileId, UserId = 1 };
            this.repository = new Repository() { BaseRepositoryId = 2, Name = "test", IsImpersonating = false, BaseRepository = new BaseRepository { Name = "SkyDrive" } };
            this.userAuthToken = null;
            var fileProvider = this.GetSkyDriveFileService();
            this.skyDriveAdapter = new Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces.Fakes.StubIRepositoryAdapter()
            {
                PostFilePublishFileModel = (publishFileModel) =>
                {
                    OperationStatus status = OperationStatus.CreateFailureStatus("error");
                    return status;
                },
            };
            IFileProcesser fileProcessor = new StubIFileProcesser()
            {
                DownloadDocumentFile = file => new DataDetail() { FileNameToDownLoad = "abc.xyz" }
            };

            string fileIdentifier;
            using (ShimsContext.Create())
            {
                ShimFileFactory.GetFileTypeInstanceStringIBlobDataRepositoryIFileRepositoryIRepositoryService = (fileExtension, blobDataRepository, fileDataRepository, repositoryService) => fileProcessor;
                ShimSkyDriveFileService.AllInstances.GetOrUpdateAuthTokensRepositoryAuthToken = (skyDriveFileService, repository, at) => new AuthToken();
                fileIdentifier = fileProvider.PublishFile(model);
            }

            Assert.IsTrue(string.IsNullOrEmpty(fileIdentifier));
        }

        [TestMethod]
        public void Allow_Download()
        {
            BaseRepository baseRepository = new BaseRepository()
            {
                BaseRepositoryId = 2,
                Name = "SkyDrive"
            };

            this.repository = new Repository() { BaseRepositoryId = 2, Name = "test", IsImpersonating = false, BaseRepository = baseRepository };
            RepositoryCredentials repositoryCredentials = new RepositoryCredentials();
            File file = new File()
            {
                FileId = 1
            };

            User user = new User()
            {
                UserId = 1
            };
          
            var fileProvider = this.GetSkyDriveFileService();
            this.skyDriveAdapter = new Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces.Fakes.StubIRepositoryAdapter()
            {
                PostFilePublishFileModel = (publishFileModel) =>
                    {
                        OperationStatus status = OperationStatus.CreateFailureStatus("error");
                        return status;
                    },
                DownloadFileStringStringString = (identifier, accessToken, fileName) =>
                    {
                        return new DataFile();
                    }
            };

            DataFile result;
            using (ShimsContext.Create())
            {
                ShimSkyDriveFileService.AllInstances.GetOrUpdateAuthTokensRepositoryAuthToken = (skyDriveFileService, repository, authToken) => new AuthToken();
                result = fileProvider.DownLoadFileFromRepository(file, this.repository, user, repositoryCredentials);
            }

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Returns the instance of SkyDriveFile Service
        /// </summary>
        /// <returns>SkyDriveFileService</returns>
        private SkyDriveFileService GetSkyDriveFileService()
        {

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = (repositoryId) =>
                {
                    return this.repository;
                },
            };

            this.userService = new StubIUserService()
             {
                 GetUserAuthTokenInt32Int32 = (userId, RepositoryId) =>
                 {
                     return this.userAuthToken;
                 },

                 AddUpdateAuthTokenAuthToken = authToken =>
                 {
                     return authToken;
                 }
             };

            this.fileRepository = new StubIFileRepository()
            {
                GetItemInt32Int32 = (userId, fileName) =>
                {
                    return new File() { Name = "test", FileId = 100, Status = "Uploaded" };
                },

                UpdateFileFile = (file) =>
                {
                    return file;
                }
            };

            IUnitOfWork unitOfWork =
                new Fakes.StubIUnitOfWork()
                {
                    Commit = () => { return; }
                };

            this.blobDataRepository = new StubIBlobDataRepository()
            {
                GetBlobContentString = (name) =>
                {
                    return new DataDetail();
                },

                DeleteFileString = (fileName) =>
                {
                    return true;
                }
            };


            this.skyDriveAdapter = new StubIRepositoryAdapter()
            {
                PostFilePublishFileModel = (publishFileModel) =>
                    {
                        OperationStatus status = OperationStatus.CreateSuccessStatus();
                        status.CustomReturnValues = "x1234";
                        return status;
                    },

                RefreshTokenString = (accessToken) =>
                    {
                        AuthToken authToken = new AuthToken()
                        {
                            AccessToken = "accessToken",
                            RefreshToken = "refreshToken",
                            TokenExpiresOn = DateTime.UtcNow.AddHours(1)
                        };

                        return authToken;
                    }
            };

            IRepositoryAdapterFactory adapterFactory = new StubIRepositoryAdapterFactory()
            {
                GetRepositoryAdapterString = (baseRepositoryName) =>
                {
                    return skyDriveAdapter;
                }
            };

            this.repositoryDetails = new StubIRepositoryDetails()
            {
                GetRepositoryByIdInt32 = id => new Repository() { BaseRepository = new BaseRepository() { Name = "Repository Type 1" } }
            };

            SkyDriveFileService skyDriveFileService;
            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                skyDriveFileService = new SkyDriveFileService(fileRepository, blobDataRepository, unitOfWork, repositoryDetails, repositoryService, userService, adapterFactory);
            }

            return skyDriveFileService;
        }
    }
}
