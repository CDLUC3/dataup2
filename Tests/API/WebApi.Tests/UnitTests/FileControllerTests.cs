using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.Enums;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.QCService.Interface;
using Microsoft.Research.DataOnboarding.QCService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.RepositoriesService;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces.Fakes;
using Microsoft.Research.DataOnboarding.Services.UserService;
using Microsoft.Research.DataOnboarding.Services.UserService.Fakes;
using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Extensions;
using Microsoft.Research.DataOnboarding.Utilities.Fakes;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.Research.DataOnboarding.WebApi.Api;
using Microsoft.Research.DataOnboarding.WebApi.Helpers.Fakes;
using Microsoft.Research.DataOnboarding.WebApi.Resources;
using Microsoft.Research.DataOnboarding.WebApi.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;
using System.Web.Fakes;
using System.Web.Http;
using System.Web.Http.Hosting;
using DM = Microsoft.Research.DataOnboarding.DomainModel;
using Memory = System.IO;

namespace Microsoft.Research.DataOnboarding.WebApi.Tests.UnitTests
{
    [TestClass]
    public class FileControllerShould
    {
        public HttpContext context;
        private IFileService fileService = null;
        private IFileServiceFactory fileServiceFactory = null;

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_File_By_FileName_And_UserId_Successfull()
        {
            DM.File fakeFile = null;

            this.fileService = new StubIFileService()
            {
                CheckFileExistsStringInt32 = (fileName, userId) =>
                {
                    return true;
                }
            };
            FilesController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Get);
            var response = controller.GetFiles(fakeFile.Name, fakeFile.CreatedBy);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Satus Code is not as expected(OK).");
            var result = response.Content.ReadAsAsync<bool>().Result;
            Assert.IsNotNull(result, "Result is Null");
            Assert.IsTrue(result, "Result is not as expected(true).");

        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_File_By_FileName_And_UserId_Unsuccessfull()
        {
            DM.File fakeFile = null;

            this.fileService = new StubIFileService()
            {
                CheckFileExistsStringInt32 = (fileName, userId) =>
                {
                    return false;
                }
            };

            FilesController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Get);
            var response = controller.GetFiles(fakeFile.Name, fakeFile.CreatedBy);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Satus is not as expected(OK)");
            var result = response.Content.ReadAsAsync<bool>().Result;
            Assert.IsNotNull(result, "Result is Null");
            Assert.IsFalse(result, "REsult is not as expected(false)");

        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_File_By_FileName_And_UserId_Error()
        {
            DM.File fakeFile = null;

            this.fileService = new StubIFileService()
            {
                CheckFileExistsStringInt32 = (fileName, userId) =>
                {
                    throw new ApplicationException("Invalid File Name");
                }
            };

            // Perform
            FilesController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Get);
            var response = controller.GetFiles(fakeFile.Name, fakeFile.CreatedBy);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Satus is not as expected(OK)");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is Null");
            Assert.IsTrue(result.Message.Contains("Invalid File Name"), "Result is not as expected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_File_By_FileName_And_UserId_Error_InnerException()
        {
            DM.File fakeFile = null;

            this.fileService = new StubIFileService()
            {
                CheckFileExistsStringInt32 = (fileName, userId) =>
                {
                    throw new Exception("Invalid FileName/UserId", new InvalidOperationException());
                }
            };

            // Perform
            FilesController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Get);
            var response = controller.GetFiles(fakeFile.Name, fakeFile.CreatedBy);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Satus is not as expected(OK)");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is Null");
            Assert.IsTrue(result.Message.Contains("Invalid FileName/UserId"), "Result is not as expected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_File_By_Id_Success()
        {
            DM.File fakeFile = null;

            this.fileService = new StubIFileService()
            {
                GetAllFilesInt32 = id =>
                {
                    return new[] { fakeFile };
                }
            };

            // Perform
            FilesController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Get);
            var response = controller.GetFiles(fakeFile.CreatedBy);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Status is not as expected(OK).");
            var result = response.Content.ReadAsAsync<List<DM.File>>().Result;

            Assert.IsNotNull(result, "Result is Null");
            Assert.AreEqual(result[0].Name, fakeFile.Name, "Expected and actual File Names are not same.");
            Assert.AreEqual(result[0].FileId, fakeFile.FileId, "Actual and Expected FileId are not same.");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_File_By_Id_When_Id_Null()
        {
            DM.File fakeFile = null;

            this.fileService = new StubIFileService()
            {
                GetAllFilesInt32 = id =>
                {
                    return null;
                }
            };

            FilesController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Get);
            // Perform
            var response = controller.GetFiles(fakeFile.CreatedBy);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Status is not as expected(OK)");
            var result = response.Content.ReadAsAsync<List<DM.File>>().Result;
            Assert.IsNull(result, "Result is not as expected(null)");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_File_By_Id_Failure_when_File_Service_Null()
        {
            DM.File fakeFile = null;

            this.fileService = null;

            FilesController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Get);
            // Perform
            var response = controller.GetFiles(fakeFile.CreatedBy);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest, "Stataus is not as expected(BadRequest)");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.AreEqual(result.Message, MessageStrings.Invalid_File_Id, "Expected and Actual Message is not same.");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_File_By_Id_Error_For_Invalid_Model_State()
        {
            DM.File fakeFile = null;

            this.fileService = null;

            FilesController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Get);
            //Setting Model State to Invalid
            controller.ModelState.AddModelError("Invalid", "Setting Model as Invalid");
            // Perform
            var response = controller.GetFiles(fakeFile.CreatedBy);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest, "Stataus is not as expected(BadRequest)");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is Null");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_File_By_Id_Error_From_Internal_Server()
        {
            DM.File fakeFile = null;

            this.fileService = new StubIFileService()
            {
                GetAllFilesInt32 = id => { throw new Exception("Exception Getting Files"); }
            };

            FilesController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Get);

            // Perform
            var response = controller.GetFiles(fakeFile.CreatedBy);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Stataus is not as expected(BadRequest)");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsTrue(result.Message.Contains("Exception Getting Files"), "Result is not as expected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_File_By_Id_Error_From_Inner_Exception()
        {
            DM.File fakeFile = null;

            this.fileService = new StubIFileService()
            {
                GetAllFilesInt32 = id => { throw new Exception("Exception Getting Files", new Exception("Inner Exception Occured")); }
            };

            FilesController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Get);

            // Perform
            var response = controller.GetFiles(fakeFile.CreatedBy);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Stataus is not as expected(BadRequest)");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsTrue(result.Message.Contains("Inner Exception Occured"), "Result is not as expected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Delete_File_Based_On_UserID_and_FileId_Success()
        {
            DM.File fakeFile = null;

            this.fileService = new StubIFileService()
            {
                DeleteFileInt32Int32 = (fileId, userId) => { return true; }
            };

            FilesController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Delete);
            // Perform
            var response = controller.Delete(fakeFile.FileId, fakeFile.CreatedBy);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Stataus is not as expected(OK)");
            var result = response.Content.ReadAsAsync<bool>().Result;
            Assert.IsTrue(result, "Expected and Actual result is not same.");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Delete_File_Based_On_UserID_and_FileId_Failure()
        {
            DM.File fakeFile = null;

            this.fileService = new StubIFileService()
            {
                DeleteFileInt32Int32 = (fileId, userId) => { return false; }
            };

            FilesController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Delete);
            // Perform
            var response = controller.Delete(fakeFile.FileId, fakeFile.CreatedBy);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Stataus is not as expected(OK)");
            var result = response.Content.ReadAsAsync<bool>().Result;
            Assert.IsFalse(result, "Result is not as ecpected(false)");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Delete_File_Based_On_UserID_When_File_Service_Null()
        {
            DM.File fakeFile = null;

            this.fileService = null;

            FilesController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Delete);
            // Perform
            var response = controller.Delete(fakeFile.FileId, fakeFile.CreatedBy);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Stataus is not as expected(OK)");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsTrue(result.Message.Contains("fileService"), "Result is not as ecpected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Delete_File_Based_On_UserID_and_Invalid_File_Model()
        {
            DM.File fakeFile = null;

            this.fileService = new StubIFileService()
            {
                DeleteFileInt32Int32 = (fileId, userId) => { return false; }
            };

            FilesController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Delete);
            controller.ModelState.AddModelError("Invalid", "Invalid File Model");

            // Perform
            var response = controller.Delete(fakeFile.FileId, fakeFile.CreatedBy);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest, "Stataus is not as expected");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is not as ecpected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Delete_File_Based_On_UserID_Server_Exception()
        {
            DM.File fakeFile = null;

            this.fileService = new StubIFileService()
            {
                DeleteFileInt32Int32 = (fileId, userId) => { throw new Exception("Cannot Delete File"); }
            };

            FilesController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Delete);

            // Perform
            var response = controller.Delete(fakeFile.FileId, fakeFile.CreatedBy);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Stataus is not as expected");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsTrue(result.Message.Contains("Cannot Delete File"), "Result is not as ecpected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Delete_File_Based_On_UserID_Error_Internal_Exception()
        {
            DM.File fakeFile = null;

            this.fileService = new StubIFileService()
            {
                DeleteFileInt32Int32 = (fileId, userId) => { throw new Exception("Cannot Delete File", new Exception("Internal Exception Occured")); }
            };

            FilesController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Delete);

            // Perform
            var response = controller.Delete(fakeFile.FileId, fakeFile.CreatedBy);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Stataus is not as expected");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsTrue(result.Message.Contains("Internal Exception Occured"), "Result is not as ecpected");
        }

        /// <summary>
        /// Setup Base for controller and Create Request
        /// </summary>
        /// <param name="fakeFile">file object</param>
        /// <param name="Method">MEthod type to pass (GET, PUT, POST or Delete)</param>
        /// <returns>Contrller object</returns>
        private FilesController SetupBaseAndRequest(out DM.File fakeFile, HttpMethod Method)
        {
            fakeFile = new DM.File()
            {
                Name = "test1",
                FileId = 100,
                RepositoryId = 1,
                CreatedBy = 1,
                FileAttributes = new[] { new FileAttribute() },
                FileColumns = new[] { new FileColumn() },
                FileMetadataFields = new[] { new FileMetadataField() },
                FileQualityChecks = new[] { new FileQualityCheck() }
            };

            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => { return this.fileService; }
            };

            IUserService userService = new Microsoft.Research.DataOnboarding.Services.UserService.Fakes.StubIUserService();
            IQCService qualityService = new StubIQCService();
            IRepositoryService repositoryService = new Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes.StubIRepositoryService();
            FilesController fileController;
            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (us, cp) => { return new User() { UserId = 1 }; };
                fileController = new FilesController(this.fileServiceFactory, repositoryService, qualityService, userService, null);
            }

            fileController.Request = new HttpRequestMessage(Method, string.Empty);
            fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

            // Perform
            return fileController;
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Publish_Should_Succeed_When_PostFileModel_Is_Valid()
        {
            DM.File fakeFile = new DM.File() { Name = "test1", FileId = 100, RepositoryId = 1, CreatedBy = 1, Status = FileStatus.None.ToString() };

            IFileService fileService = new StubIFileService()
            {
                UploadFileDataDetail = dataDetail =>
                {
                    return new DataDetail() { FileDetail = fakeFile };
                },

                GetFileByFileIdInt32 = (fileId) => { return fakeFile; },

                UpdateFileFile = updatedFile => { return true; },

                SaveFilePostFileModel = postFileModel => { return true; },
                PublishFilePublishMessage = publishMessage =>
                {
                    return "Success";
                }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };
            IRepositoryService repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => new Repository()
                {
                    RepositoryId = repositoryId,
                    BaseRepository = new BaseRepository() { Name = "Meritt", BaseRepositoryId = 1 }
                }
            };
            IQCService qualityService = new StubIQCService();
            IUserService userService = new StubIUserService()
            {
                GetUserWithRolesByNameIdentifierString = (nameIndentifier) =>
                    {
                        User user = new User()
                        {
                            UserId = 1,
                            FirstName = "test"
                        };

                        return user;
                    }
            };

            FilesController publishController = new FilesController(fileServiceFactory, repositoryService, qualityService, userService, null);
            publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
            publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

            var postModel = new PostFileModel() { SelectedRepositoryId = 3, isPublish = true };

            postModel.UserAuthToken = new AuthToken()
            {
                AccessToken = null
            };

            string postDataFile = postModel.SerializeObject<PostFileModel>("postFile");
            HttpResponseMessage response;

            using (ShimsContext.Create())
            {
                var httpRequest = new ShimHttpRequest();
                httpRequest.ItemGetString = key => key == "PostFileData" ? postDataFile.EncodeTo64() : null;

                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;

                // Perform
                response = publishController.Publish();
            }

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Stataus is not as expected(OK)");
            var result = response.Content.ReadAsAsync<OperationStatus>().Result;
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Publish_Should_Throw_InternalServerError_When_PostFileModel_Is_Invalid()
        {
            DM.File fakeFile = new DM.File() { Name = "test1", FileId = 100, RepositoryId = 1, CreatedBy = 1, Status = FileStatus.None.ToString() };

            IFileService fileService = null;
            IRepositoryService repositoryService = null;
            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory();
            IUserService userService = null;
            IQCService qualityService = new StubIQCService();
            HttpResponseMessage response;

            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User()
                {
                    UserId = 1,
                    LastName = "Test"
                };

                FilesController publishController = new FilesController(fileServiceFactory, repositoryService, qualityService, userService, null);
                publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                var httpRequest = new ShimHttpRequest();
                httpRequest.ItemGetString = key => key == "PostFileData" ? string.Empty : null;

                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;

                // Perform
                response = publishController.Publish();
            }

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Status is not as expected");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is not as expected.");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void DownloadFile_ShouldThrowInternalServerError_WhenFileDownloadException()
        {
            IFileService fileService = new StubIFileService()
            {
                GetFilesFuncOfFileBoolean = (p) => new List<File>()
                {
                    new File(){RepositoryId =1}
                },

                DownLoadFileFromRepositoryFileRepositoryUserRepositoryCredentials = (file, repositoryInstance, user, credentials) =>
                {
                    throw new FileDownloadException()
                    {
                        RepositoryId = 1,
                        FileDownloadExceptionType = FileDownloadExceptionType.DownloadUrlNotFound.ToString()
                    };
                }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };
            Repository repository = new Repository()
            {
                HttpGetUriTemplate = "SomeHttpGetUriTemplate",
                BaseRepository = new BaseRepository()
            };
            IRepositoryService repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => repository
            };
            IRepositoryAdapter repositoryAdapter = new StubIRepositoryAdapter()
            {
                DownloadFileStringStringString = (downloadURL, authorization, fileName) => new DataFile() { FileContent = new byte[0], ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" }
            };
            IRepositoryAdapterFactory repositoryAdapterFactory = new StubIRepositoryAdapterFactory()
            {
                GetRepositoryAdapterString = (instanceName) => repositoryAdapter
            };
            HttpResponseMessage httpResponseMessage = null;

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => null;
                ShimIdentityHelper.GetUserIUserServiceString = (service, nameIdentifier) =>
                {
                    return new User();
                };
                FilesController fileController = new FilesController(fileServiceFactory, repositoryService, null, null, repositoryAdapterFactory);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                httpResponseMessage = fileController.DownloadFileFromRepository("SomeNameIdentifier", 1);
            }

            Assert.AreEqual(HttpStatusCode.InternalServerError, httpResponseMessage.StatusCode);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void DownloadFile_ShouldThrowArgumentException_WhenNameIdentifierIsNullOrEmpty()
        {
            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory();

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => null;

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = fileController.DownloadFileFromRepository(string.Empty, 1);
                Assert.AreEqual(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void DownloadFile_ShouldThrowArgumentNullException_When_FileId_Is_Zero()
        {
            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory();

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => null;

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = fileController.DownloadFileFromRepository(0);
                Assert.AreEqual(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void DownloadFile_Send_NotFound_When_File_Not_Found()
        {
            IFileService fileService = new StubIFileService()
            {
                GetFilesFuncOfFileBoolean = (p) => new List<File>(),

                DownLoadFileFromRepositoryFileRepositoryUserRepositoryCredentials = (file, repositoryInstance, user, credentials) =>
                {
                    throw new FileDownloadException()
                    {
                        RepositoryId = 1,
                        FileDownloadExceptionType = FileDownloadExceptionType.DownloadUrlNotFound.ToString()
                    };
                }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };
            Repository repository = new Repository()
            {
                HttpGetUriTemplate = "SomeHttpGetUriTemplate",
                BaseRepository = new BaseRepository()
            };
            IRepositoryService repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => repository
            };
            IRepositoryAdapter repositoryAdapter = new StubIRepositoryAdapter()
            {
                DownloadFileStringStringString = (downloadURL, authorization, fileName) => new DataFile() { FileContent = new byte[0], ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" }
            };
            IRepositoryAdapterFactory repositoryAdapterFactory = new StubIRepositoryAdapterFactory()
            {
                GetRepositoryAdapterString = (instanceName) => repositoryAdapter
            };
            HttpResponseMessage httpResponseMessage = null;

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => null;
                ShimIdentityHelper.GetUserIUserServiceString = (service, nameIdentifier) =>
                {
                    return new User();
                };
                FilesController fileController = new FilesController(fileServiceFactory, repositoryService, null, null, repositoryAdapterFactory);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                httpResponseMessage = fileController.DownloadFileFromRepository(1);
            }

            Assert.AreEqual(HttpStatusCode.NotFound, httpResponseMessage.StatusCode, "Status Code is not as expected(NotFound)");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void DownloadFile_Send_NotFound_When_RepositoryId_Is_Null()
        {
            IFileService fileService = new StubIFileService()
            {
                GetFilesFuncOfFileBoolean = (p) => new List<File>() { new File() },

                DownLoadFileFromRepositoryFileRepositoryUserRepositoryCredentials = (file, repositoryInstance, user, credentials) =>
                {
                    throw new FileDownloadException()
                    {
                        RepositoryId = 1,
                        FileDownloadExceptionType = FileDownloadExceptionType.DownloadUrlNotFound.ToString()
                    };
                }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };
            Repository repository = new Repository()
            {
                HttpGetUriTemplate = "SomeHttpGetUriTemplate",
                BaseRepository = new BaseRepository()
            };
            IRepositoryService repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => repository
            };
            IRepositoryAdapter repositoryAdapter = new StubIRepositoryAdapter()
            {
                DownloadFileStringStringString = (downloadURL, authorization, fileName) => new DataFile() { FileContent = new byte[0], ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" }
            };
            IRepositoryAdapterFactory repositoryAdapterFactory = new StubIRepositoryAdapterFactory()
            {
                GetRepositoryAdapterString = (instanceName) => repositoryAdapter
            };
            HttpResponseMessage httpResponseMessage = null;

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => null;
                ShimIdentityHelper.GetUserIUserServiceString = (service, nameIdentifier) =>
                {
                    return new User();
                };
                FilesController fileController = new FilesController(fileServiceFactory, repositoryService, null, null, repositoryAdapterFactory);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                httpResponseMessage = fileController.DownloadFileFromRepository(1);
            }

            Assert.AreEqual(HttpStatusCode.NotFound, httpResponseMessage.StatusCode, "Status Code is not as expected(NotFound)");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void DownloadFile_Send_NotFound_When_Repository_DoesNotExist()
        {
            IFileService fileService = new StubIFileService()
            {
                GetFilesFuncOfFileBoolean = (p) => new List<File>() { new File() { RepositoryId = 1 } },

                DownLoadFileFromRepositoryFileRepositoryUserRepositoryCredentials = (file, repositoryInstance, user, credentials) =>
                {
                    throw new FileDownloadException()
                    {
                        RepositoryId = 1,
                        FileDownloadExceptionType = FileDownloadExceptionType.DownloadUrlNotFound.ToString()
                    };
                }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };
            Repository repository = new Repository()
            {
                HttpGetUriTemplate = "SomeHttpGetUriTemplate",
                BaseRepository = new BaseRepository()
            };
            IRepositoryService repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => null
            };
            IRepositoryAdapter repositoryAdapter = new StubIRepositoryAdapter()
            {
                DownloadFileStringStringString = (downloadURL, authorization, fileName) => new DataFile() { FileContent = new byte[0], ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" }
            };
            IRepositoryAdapterFactory repositoryAdapterFactory = new StubIRepositoryAdapterFactory()
            {
                GetRepositoryAdapterString = (instanceName) => repositoryAdapter
            };
            HttpResponseMessage httpResponseMessage = null;

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => null;
                ShimIdentityHelper.GetUserIUserServiceString = (service, nameIdentifier) =>
                {
                    return new User();
                };
                FilesController fileController = new FilesController(fileServiceFactory, repositoryService, null, null, repositoryAdapterFactory);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                httpResponseMessage = fileController.DownloadFileFromRepository(1);
            }

            Assert.AreEqual(HttpStatusCode.NotFound, httpResponseMessage.StatusCode, "Status Code is not as expected(NotFound)");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void DownloadFile_ShouldReturnOkHttpStatusCode_WhenHttpGetUriTemplateIsValid()
        {
            IFileService fileService = new StubIFileService()
            {
                GetFilesFuncOfFileBoolean = (p) => new List<File>()
                {
                    new File(){RepositoryId =1}
                },

                DownLoadFileFromRepositoryFileRepositoryUserRepositoryCredentials = (file, repositoryInstance, user, credentials) => new DataFile()
                {
                    FileContent = new byte[] { }
                }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };
            Repository repository = new Repository()
            {
                HttpGetUriTemplate = "SomeHttpGetUriTemplate",
                BaseRepository = new BaseRepository()
            };
            IRepositoryService repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => repository
            };
            IRepositoryAdapter repositoryAdapter = new StubIRepositoryAdapter()
            {
                DownloadFileStringStringString = (downloadURL, authorization, fileName) => new DataFile() { FileContent = new byte[0], ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" }
            };
            IRepositoryAdapterFactory repositoryAdapterFactory = new StubIRepositoryAdapterFactory()
            {
                GetRepositoryAdapterString = (instanceName) => repositoryAdapter
            };
            HttpResponseMessage httpResponseMessage = null;

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => null;
                ShimIdentityHelper.GetUserIUserServiceString = (service, nameIdentifier) =>
                {
                    return new User();
                };
                FilesController fileController = new FilesController(fileServiceFactory, repositoryService, null, null, repositoryAdapterFactory);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                httpResponseMessage = fileController.DownloadFileFromRepository("SomeNameIdentifier", 1);
            }

            Assert.AreEqual(HttpStatusCode.OK, httpResponseMessage.StatusCode, "Status Code is not as expected(Bad Request)");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task GetErrors_ShouldReturnHttpStatusCodeNotFound_WhenFileServiceThrowsDataFileNotFoundException()
        {
            IFileService fileService = new StubIFileService()
            {
                GetErrorsInt32Int32 = (userId, fileId) => { throw new DataFileNotFoundException(); }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = await fileController.GetErrors(0);
                Assert.AreEqual(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task GetErrors_ShouldReturnHttpStatusCodeNotImplemented_WhenFileServiceThrowsInvalidOperationException()
        {
            IFileService fileService = new StubIFileService()
            {
                GetErrorsInt32Int32 = (userId, fileId) => { throw new InvalidOperationException(); }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = await fileController.GetErrors(0);
                Assert.AreEqual(HttpStatusCode.NotImplemented, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task GetErrors_ShouldReturnHttpStatusCodeInternalServerError_WhenFileServiceThrowsFileFormatException()
        {
            IFileService fileService = new StubIFileService()
            {
                GetErrorsInt32Int32 = (userId, fileId) => { throw new System.IO.FileFormatException(); }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = await fileController.GetErrors(0);
                Assert.AreEqual(HttpStatusCode.InternalServerError, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task GetErrors_ShouldReturnHttpStatusCodeOK()
        {
            File file = new File() { Name = "File1.xlsx" };
            IList<FileSheet> fileSheets = new List<FileSheet>();
            IFileService fileService = new StubIFileService()
            {
                GetErrorsInt32Int32 = (userId, fileId) => { return Task.FromResult(Tuple.Create(file, fileSheets)); }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = await fileController.GetErrors(0);
                Assert.AreEqual(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task RemoveErrors_ShouldReturnHttpStatusCodeBadRequest_WhenRemoveErrorsViewModelIsNull()
        {
            IFileService fileService = new StubIFileService();

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = await fileController.RemoveErrors(null);
                Assert.AreEqual(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
                var message = httpResponseMessage.Content.ReadAsAsync<HttpError>().Result.Message;
                Assert.IsTrue(message.Contains("removeErrorsViewModel"));
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task RemoveErrors_ShouldReturnHttpStatusCodeBadRequest_WhenSheetNameIsNull()
        {
            IFileService fileService = new StubIFileService();

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                RemoveErrorsViewModel removeErrorsViewModel = new RemoveErrorsViewModel();
                HttpResponseMessage httpResponseMessage = await fileController.RemoveErrors(removeErrorsViewModel);
                Assert.AreEqual(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
                var message = httpResponseMessage.Content.ReadAsAsync<HttpError>().Result.Message;
                Assert.IsTrue(message.Contains("SheetName"));
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task RemoveErrors_ShouldReturnHttpStatusCodeBadRequest_WhenErrorTypesIsNull()
        {
            IFileService fileService = new StubIFileService();
            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                RemoveErrorsViewModel removeErrorsViewModel = new RemoveErrorsViewModel() { SheetName = "Sheet 1" };
                HttpResponseMessage httpResponseMessage = await fileController.RemoveErrors(removeErrorsViewModel);
                Assert.AreEqual(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
                var message = httpResponseMessage.Content.ReadAsAsync<HttpError>().Result.Message;
                Assert.IsTrue(message.Contains("ErrorTypes"));
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task RemoveErrors_ShouldReturnHttpStatusCodeBadRequest_WhenErrorTypesIsInvalid()
        {
            IFileService fileService = new StubIFileService()
            {
                RemoveErrorsInt32Int32StringIEnumerableOfErrorType = (userId, fileId, sheetName, errorTypes) => { throw new DataFileNotFoundException(); }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                RemoveErrorsViewModel removeErrorsViewModel = new RemoveErrorsViewModel() { SheetName = "Sheet 1", ErrorTypes = new List<ErrorType>() { (ErrorType)100 } };
                HttpResponseMessage httpResponseMessage = await fileController.RemoveErrors(removeErrorsViewModel);
                Assert.AreEqual(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task RemoveErrors_ShouldReturnHttpStatusCodeNotFound_WhenFileServiceThrowsDataFileNotFoundException()
        {
            IFileService fileService = new StubIFileService()
            {
                RemoveErrorsInt32Int32StringIEnumerableOfErrorType = (userId, fileId, sheetName, errorTypes) => { throw new DataFileNotFoundException(); }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                RemoveErrorsViewModel removeErrorsViewModel = new RemoveErrorsViewModel() { SheetName = "Sheet 1", ErrorTypes = new List<ErrorType>() { ErrorType.Comments } };
                HttpResponseMessage httpResponseMessage = await fileController.RemoveErrors(removeErrorsViewModel);
                Assert.AreEqual(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task RemoveErrors_ShouldReturnHttpStatusCodeNotImplemented_WhenFileServiceThrowsInvalidOperationException()
        {
            IFileService fileService = new StubIFileService()
            {
                RemoveErrorsInt32Int32StringIEnumerableOfErrorType = (userId, fileId, sheetName, errorTypes) => { throw new InvalidOperationException(); }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                RemoveErrorsViewModel removeErrorsViewModel = new RemoveErrorsViewModel() { SheetName = "Sheet 1", ErrorTypes = new List<ErrorType>() { ErrorType.Comments } };
                HttpResponseMessage httpResponseMessage = await fileController.RemoveErrors(removeErrorsViewModel);
                Assert.AreEqual(HttpStatusCode.NotImplemented, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task RemoveErrors_ShouldReturnHttpStatusCodeInternalServerError_WhenFileServiceThrowsFileFormatException()
        {
            IFileService fileService = new StubIFileService()
            {
                RemoveErrorsInt32Int32StringIEnumerableOfErrorType = (userId, fileId, sheetName, errorTypes) => { throw new System.IO.FileFormatException(); }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                RemoveErrorsViewModel removeErrorsViewModel = new RemoveErrorsViewModel() { SheetName = "Sheet 1", ErrorTypes = new List<ErrorType>() { ErrorType.Comments } };
                HttpResponseMessage httpResponseMessage = await fileController.RemoveErrors(removeErrorsViewModel);
                Assert.AreEqual(HttpStatusCode.InternalServerError, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task RemoveErrors_ShouldReturnHttpStatusCodeOK()
        {
            File file = new File() { Name = "File1.xlsx" };
            IList<FileSheet> fileSheets = new List<FileSheet>();
            IFileService fileService = new StubIFileService()
            {
                RemoveErrorsInt32Int32StringIEnumerableOfErrorType = (userId, fileId, sheetName, errorTypes) => { return Task.Run(() => { }); }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                RemoveErrorsViewModel removeErrorsViewModel = new RemoveErrorsViewModel() { SheetName = "Sheet 1", ErrorTypes = new List<ErrorType>() { ErrorType.Comments } };
                HttpResponseMessage httpResponseMessage = await fileController.RemoveErrors(removeErrorsViewModel);
                Assert.AreEqual(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void GetFileLevelMetadata_ShouldReturnHttpStatusCodeNotFound_WhenFileIsNull()
        {
            IFileService fileService = new StubIFileService()
            {
                GetFilesFuncOfFileBoolean = fileByFileIdAndUserIdFilter => Enumerable.Empty<File>()
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = fileController.GetFileLevelMetadata(0, 0);
                Assert.AreEqual(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void GetFileLevelMetadata_ShouldReturnHttpStatusCodeNotFound_WhenRepositoryIsNull()
        {
            IFileService fileService = new StubIFileService()
            {
                GetFilesFuncOfFileBoolean = fileByFileIdAndUserIdFilter => new List<File>() { new File() }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            IRepositoryService repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => null
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, repositoryService, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = fileController.GetFileLevelMetadata(0, 0);
                Assert.AreEqual(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void GetFileLevelMetadata_ShouldReturnHttpStatusCodeOk_WhenRepositoryMetadataIsEmpty()
        {
            IFileService fileService = new StubIFileService()
            {
                GetFilesFuncOfFileBoolean = fileByFileIdAndUserIdFilter => new List<File>() { new File() }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            IRepositoryService repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => new Repository() { RepositoryMetadata = new List<RepositoryMetadata>() }
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, repositoryService, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = fileController.GetFileLevelMetadata(0, 0);
                Assert.AreEqual(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void GetFileLevelMetadata_ShouldReturnHttpStatusCodeOk_WhenRepositoryMetadataFieldsIsEmpty()
        {
            IFileService fileService = new StubIFileService()
            {
                GetFilesFuncOfFileBoolean = fileByFileIdAndUserIdFilter => new List<File>() { new File() }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            IRepositoryService repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => new Repository() { RepositoryMetadata = new List<RepositoryMetadata>() { new RepositoryMetadata() { IsActive = true, RepositoryMetadataFields = new List<RepositoryMetadataField>() } } }
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, repositoryService, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = fileController.GetFileLevelMetadata(0, 0);
                Assert.AreEqual(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void GetFileLevelMetadata_ShouldReturnHttpStatusCodeOk_WhenRepositoryHasMetadata()
        {
            File file = new File();
            file.FileMetadataFields = new List<FileMetadataField>();
            file.FileMetadataFields.Add(new FileMetadataField());

            IFileService fileService = new StubIFileService()
            {
                GetFileByFileIdInt32 = fileId => { return file; }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            List<RepositoryMetadataField> repositoryMetadataFields = new List<RepositoryMetadataField>();
            repositoryMetadataFields.Add(new RepositoryMetadataField() { Range = "10to20" });

            List<RepositoryMetadata> repositoryMetadatas = new List<RepositoryMetadata>();
            repositoryMetadatas.Add(new RepositoryMetadata() { IsActive = true, RepositoryMetadataFields = repositoryMetadataFields });

            Repository repository = new Repository() { RepositoryMetadata = repositoryMetadatas };

            IRepositoryService repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => repository
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, repositoryService, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = fileController.GetFileLevelMetadata(0, 0);
                Assert.AreEqual(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void SaveFileLevelMetadata_ShouldReturnHttpStatusCodeBadRequest_WhenSaveFileLevelMetadataListlIsNUll()
        {
            IFileService fileService = new StubIFileService();
            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();


                HttpResponseMessage httpResponseMessage = fileController.SaveFileLevelMetadata(0, 0, null);
                Assert.AreEqual(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void SaveFileLevelMetadata_ShouldReturnHttpStatusCodeOK()
        {
            List<SaveFileLevelMetadata> SaveFileLevelMetadataList = new List<SaveFileLevelMetadata>();
            IFileService fileService = new StubIFileService()
            {
                SaveFileLevelMetadataInt32Int32IEnumerableOfSaveFileLevelMetadata = (fileId, repositoryId, saveFileLevelMetadataList) => { }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = fileController.SaveFileLevelMetadata(0, 0, SaveFileLevelMetadataList);
                Assert.AreEqual(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task GetColumnLevelMetadata_ShouldReturnHttpStatusCodeNotFound_WhenFileIsNull()
        {
            IFileService fileService = new StubIFileService()
            {
                GetFilesFuncOfFileBoolean = fileByFileIdAndUserIdFilter => Enumerable.Empty<File>()
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = await fileController.GetColumnLevelMetadata(0);
                Assert.AreEqual(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task GetColumnLevelMetadata_ShouldReturnHttpStatusCodeOk_WhenFileHasMetadata()
        {
            File file = new File() { Name = "abc.xlsx" };
            file.FileColumns.Add(new FileColumn() { FileColumnId = 1, FileId = 1, EntityName = "Entity Name 1", EntityDescription = "Entity Description 1", Name = "Name 1", Description = "Description 1", FileColumnTypeId = 1, FileColumnUnitId = 1 });

            IFileService fileService = new StubIFileService()
            {
                GetFileByFileIdInt32 = fileId => { return file; },
                GetDocumentSheetDetailsFile = f => Task.FromResult(Enumerable.Empty<FileSheet>())
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = await fileController.GetColumnLevelMetadata(0);
                Assert.AreEqual(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void SaveColumnLevelMetadata_ShouldReturnHttpStatusCodeBadRequest_WhenColumnLevelMetadataViewModelIsNUll()
        {
            IFileService fileService = new StubIFileService();
            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = fileController.SaveColumnLevelMetadata(0, null);
                Assert.AreEqual(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void SaveColumnLevelMetadata_ShouldReturnHttpStatusCodeOK()
        {
            List<ColumnLevelMetadata> metadataList = new List<ColumnLevelMetadata>();
            IFileService fileService = new StubIFileService()
            {
                SaveColumnLevelMetadataInt32IEnumerableOfColumnLevelMetadata = (fileId, ml) => { }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = fileController.SaveColumnLevelMetadata(0, metadataList);
                Assert.AreEqual(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void GetPostFileDetails_ShouldReturnHttpStatusCodeNotFound_WhenFileIsNotFound()
        {
            IFileService fileService = new StubIFileService()
            {
                GetFilesFuncOfFileBoolean = fileByFileIdAndUserIdFilter => Enumerable.Empty<File>()
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, null, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = fileController.GetPostFileDetails(0, 0);
                Assert.AreEqual(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void GetPostFileDetails_ShouldReturnHttpStatusCodeNotFound_WhenRepositoryIsNotFound()
        {
            IFileService fileService = new StubIFileService()
            {
                GetFilesFuncOfFileBoolean = fileByFileIdAndUserIdFilter => new List<File>() { new File() { Name = "TestFile.xlsx" } }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            IRepositoryService repositoryService = new StubIRepositoryService()
            {
                GetRepositoriesByRoleAndFileExtensionBooleanString = (isAdmin, fileExtension) => Enumerable.Empty<Repository>()
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, repositoryService, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = fileController.GetPostFileDetails(0, 0);
                Assert.AreEqual(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void GetPostFileDetails_ShouldReturnHttpStatusCodeOK()
        {
            IFileService fileService = new StubIFileService()
            {
                GetFilesFuncOfFileBoolean = fileByFileIdAndUserIdFilter => new List<File>() { new File() { Name = "TestFile.xlsx" } }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            IRepositoryService repositoryService = new StubIRepositoryService()
            {
                GetRepositoriesByRoleAndFileExtensionBooleanString = (isAdmin, fileExtension) => new List<Repository>() { new Repository() }
            };

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                FilesController fileController = new FilesController(fileServiceFactory, repositoryService, null, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = fileController.GetPostFileDetails(0, 0);
                Assert.AreEqual(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            }
        }

        /// <summary>
        /// Serialize Object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object that needs to be converted</param>
        /// <returns>byte stream</returns>
        private byte[] SerializeObject<T>(T obj)
        {
            DataContractSerializer se = new DataContractSerializer(typeof(T));

            byte[] tempArr;
            using (Memory.MemoryStream ms = new Memory.MemoryStream())
            {
                se.WriteObject(ms, obj);
                ms.Position = 0;
                tempArr = new byte[ms.Length];
                ms.Read(tempArr, 0, Convert.ToInt32(ms.Length));
            }

            return tempArr;
        }

        /// <summary>
        /// Deserialize Object
        /// </summary>
        /// <typeparam name="T">type of Object</typeparam>
        /// <param name="buffer">stream</param>
        /// <returns>object</returns>
        public object DeserializeObject<T>(byte[] buffer)
        {
            DataContractSerializer se = new DataContractSerializer(typeof(T));
            Memory.MemoryStream memStream = new Memory.MemoryStream(buffer);
            object obj = se.ReadObject(memStream);
            return obj;
        }
    }
}

