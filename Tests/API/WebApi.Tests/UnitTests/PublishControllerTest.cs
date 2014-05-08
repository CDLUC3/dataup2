// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.Exceptions;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.QCService.Interface;
using Microsoft.Research.DataOnboarding.QCService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.QueueService;
using Microsoft.Research.DataOnboarding.QueueService.Fakes;
using Microsoft.Research.DataOnboarding.RepositoriesService;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.Services.UserService;
using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.Research.DataOnboarding.WebApi.Api;
using Microsoft.Research.DataOnboarding.WebApi.Helpers.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Fakes;
using System.Web.Http;
using System.Web.Http.Hosting;
using DM = Microsoft.Research.DataOnboarding.DomainModel;
using Memory = System.IO;

namespace Microsoft.Research.DataOnboarding.WebApi.Tests.UnitTests
{
    [TestClass]
    public class PublishControllerTest
    {
        public HttpContext context;
        private IFileService fileService = null;
        private IFileServiceFactory fileServiceFactory = null;
        private IPublishQueueService publishQueueService;
        private IUserService userService;
        private IRepositoryService repositoryService;

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Publish_Should_Succeed_When_PublishMessage_Is_Valid()
        {
            DM.File fakeFile = new DM.File() { Name = "test1", FileId = 100, RepositoryId = 1, CreatedBy = 1, Status = FileStatus.None.ToString() };

            this.fileService = new StubIFileService()
            {
                UploadFileDataDetail = dataDetail =>
                {
                    return new DataDetail() { FileDetail = fakeFile };
                },

                GetFileByFileIdInt32 = id => { return fakeFile; },

                UpdateFileFile = updatedFile => { return true; },

                SaveFilePostFileModel = postFileModel => { return true; },

                PublishFilePublishMessage = publishMessage =>
                {
                    return "Success";
                },
            };

            this.publishQueueService = new StubIPublishQueueService();

            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => new Repository()
                {
                    RepositoryId = repositoryId,
                    BaseRepository = new BaseRepository() { Name = "Meritt", BaseRepositoryId = 1 }
                }
            };

            PublishController publishController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User()
                {
                    UserId = 1,
                    LastName = "Test"
                };

                publishController = new PublishController(this.publishQueueService, this.userService, repositoryService, this.fileServiceFactory);
                publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;

                PublishMessage message = new PublishMessage()
                {
                    FileId = 1,
                    RepositoryId = 1,
                };

                HttpResponseMessage reponse = publishController.Publish(message);
                Assert.IsTrue(reponse.StatusCode == HttpStatusCode.OK);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Publish_Should_Send_BadRequest_When_PostFileModel_Is_Invalid()
        {
            this.fileService = new StubIFileService();
            this.publishQueueService = new StubIPublishQueueService();
            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => new Repository()
                {
                    RepositoryId = repositoryId,
                    BaseRepository = new BaseRepository() { Name = "Meritt", BaseRepositoryId = 1 }
                }
            };

            PublishController publishController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User()
                {
                    UserId = 1,
                    LastName = "Test"
                };

                publishController = new PublishController(this.publishQueueService, this.userService, repositoryService, this.fileServiceFactory);
                publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;
                PublishMessage message = null;

                HttpResponseMessage reponse = publishController.Publish(message);
                Assert.IsTrue(reponse.StatusCode == HttpStatusCode.BadRequest);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Publish_Should_Send_NotFound_When_RepositoryId_Is_Invalid()
        {
            this.fileService = new StubIFileService();
            this.publishQueueService = new StubIPublishQueueService();
            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => { return null; }
            };

            PublishController publishController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User()
                {
                    UserId = 1,
                    LastName = "Test"
                };

                publishController = new PublishController(this.publishQueueService, this.userService, repositoryService, this.fileServiceFactory);
                publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;
                PublishMessage message = new PublishMessage()
                {
                    FileId = 1,
                    RepositoryId = 1,
                };

                HttpResponseMessage reponse = publishController.Publish(message);
                Assert.IsTrue(reponse.StatusCode == HttpStatusCode.NotFound);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Publish_Should_Send_InternalServerError_When_Metadata_Is_Invalid()
        {
            this.fileService = new StubIFileService()
            {

                ValidateForPublishPublishMessage = publishMessage =>
                    {
                        throw new MetadataValidationException()
                        {
                            RepositoryId = 1,
                            FileId = 1,
                            NotFound = true,
                            FieldName = "RequiredField"
                        };
                    }
            };

            this.publishQueueService = new StubIPublishQueueService();
            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => new Repository()
                {
                    RepositoryId = repositoryId,
                    BaseRepository = new BaseRepository() { Name = "Meritt", BaseRepositoryId = 1 }
                }
            };

            PublishController publishController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User()
                {
                    UserId = 1,
                    LastName = "Test"
                };

                publishController = new PublishController(this.publishQueueService, this.userService, repositoryService, this.fileServiceFactory);
                publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;
                PublishMessage message = new PublishMessage()
                {
                    FileId = 1,
                    RepositoryId = 1,
                };

                HttpResponseMessage response = publishController.Publish(message);
                Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);

                HttpError error = response.Content.ReadAsAsync<HttpError>().Result;
                Assert.AreEqual(error[MetadataValidationException.FieldNameKey], "RequiredField");
                Assert.IsTrue((bool)error[MetadataValidationException.NotFoundKey]);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Publish_Should_Send_InternalServerError_When_Metadata_Type_Is_Invalid()
        {
            this.fileService = new StubIFileService()
            {

                ValidateForPublishPublishMessage = publishMessage =>
                {
                    throw new MetadataValidationException()
                    {
                        RepositoryId = 1,
                        FileId = 1,
                        MetadataTypeNotFound = true,
                        FieldName = "RequiredField",
                    };
                }
            };

            this.publishQueueService = new StubIPublishQueueService();
            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => new Repository()
                {
                    RepositoryId = repositoryId,
                    BaseRepository = new BaseRepository() { Name = "Meritt", BaseRepositoryId = 1 }
                }
            };

            PublishController publishController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User()
                {
                    UserId = 1,
                    LastName = "Test"
                };

                publishController = new PublishController(this.publishQueueService, this.userService, repositoryService, this.fileServiceFactory);
                publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;
                PublishMessage message = new PublishMessage()
                {
                    FileId = 1,
                    RepositoryId = 1,
                };

                HttpResponseMessage response = publishController.Publish(message);
                Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);

                HttpError error = response.Content.ReadAsAsync<HttpError>().Result;
                Assert.AreEqual(error[MetadataValidationException.FieldNameKey], "RequiredField");
                Assert.IsTrue((bool)error[MetadataValidationException.MetadataTypeNotFoundKey]);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Publish_Should_Send_NotFound_FileId_Is_Invalid()
        {
            this.fileService = new StubIFileService()
            {

                ValidateForPublishPublishMessage = publishMessage =>
                {
                }
            };

            this.publishQueueService = new StubIPublishQueueService()
            {
                PostFileToQueueBaseMessage = message =>
                    {
                        throw new DataFileNotFoundException()
                        {
                            FileId = 100
                        };
                    }
            };

            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => new Repository()
                {
                    RepositoryId = repositoryId,
                    BaseRepository = new BaseRepository() { Name = "Meritt", BaseRepositoryId = 1 }
                }
            };

            PublishController publishController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User()
                {
                    UserId = 1,
                    LastName = "Test"
                };

                publishController = new PublishController(this.publishQueueService, this.userService, repositoryService, this.fileServiceFactory);
                publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;
                PublishMessage message = new PublishMessage()
                {
                    FileId = 1,
                    RepositoryId = 1,
                };

                HttpResponseMessage response = publishController.Publish(message);
                Assert.IsTrue(response.StatusCode == HttpStatusCode.NotFound);

                HttpError error = response.Content.ReadAsAsync<HttpError>().Result;
                Assert.AreEqual(error[DataFileNotFoundException.FileIdKey], 100);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Publish_Should_Send_InternalServerError_When_AccessToken_NotFound()
        {
            this.fileService = new StubIFileService()
            {

                ValidateForPublishPublishMessage = publishMessage =>
                {
                }
            };

            this.publishQueueService = new StubIPublishQueueService()
            {
                PostFileToQueueBaseMessage = message =>
                {
                    throw new AccessTokenNotFoundException()
                    {
                        RepositoryId = 100,
                        UserId = 200
                    };
                }
            };

            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => new Repository()
                {
                    RepositoryId = repositoryId,
                    BaseRepository = new BaseRepository() { Name = "Meritt", BaseRepositoryId = 1 }
                }
            };

            PublishController publishController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User()
                {
                    UserId = 1,
                    LastName = "Test"
                };

                publishController = new PublishController(this.publishQueueService, this.userService, repositoryService, this.fileServiceFactory);
                publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;
                PublishMessage message = new PublishMessage()
                {
                    FileId = 1,
                    RepositoryId = 100,
                };

                HttpResponseMessage response = publishController.Publish(message);
                Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);

                HttpError error = response.Content.ReadAsAsync<HttpError>().Result;
                Assert.AreEqual(error[AccessTokenNotFoundException.RepositoryIdKeyName], 100);
                Assert.AreEqual(error[AccessTokenNotFoundException.UserIdKeyName], 200);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Publish_Should_Send_InternalServerError_When_FileAlready_Published()
        {
            this.fileService = new StubIFileService()
            {

                ValidateForPublishPublishMessage = publishMessage =>
                {
                }
            };

            this.publishQueueService = new StubIPublishQueueService()
            {
                PostFileToQueueBaseMessage = message =>
                {
                    throw new FileAlreadyPublishedException()
                    {
                        FileStatus = "Verifying",
                        FileId = 100,
                        RepositoryId = 200,
                    };
                }
            };

            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = repositoryId => new Repository()
                {
                    RepositoryId = repositoryId,
                    BaseRepository = new BaseRepository() { Name = "Meritt", BaseRepositoryId = 1 }
                }
            };

            PublishController publishController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User()
                {
                    UserId = 1,
                    LastName = "Test"
                };

                publishController = new PublishController(this.publishQueueService, this.userService, repositoryService, this.fileServiceFactory);
                publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;
                PublishMessage message = new PublishMessage()
                {
                    FileId = 100,
                    RepositoryId = 200,
                };

                HttpResponseMessage response = publishController.Publish(message);
                Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);

                HttpError error = response.Content.ReadAsAsync<HttpError>().Result;
                Assert.AreEqual(error[FileAlreadyPublishedException.FileIdKey], 100);
                Assert.AreEqual(error[FileAlreadyPublishedException.FileStatusKey], "Verifying");
                Assert.AreEqual(error[FileAlreadyPublishedException.RepositoryIdKey], 200);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void PublishByService_Should_Succeed_When_PublishMessage_Is_Valid()
        {
            DM.File fakeFile = new DM.File() { Name = "test1", FileId = 100, RepositoryId = 1, CreatedBy = 1, Status = FileStatus.None.ToString() };

            this.fileService = new StubIFileService()
            {
                UploadFileDataDetail = dataDetail =>
                {
                    return new DataDetail() { FileDetail = fakeFile };
                },

                GetFileByFileIdInt32 = id => { return fakeFile; },

                UpdateFileFile = updatedFile => { return true; },

                SaveFilePostFileModel = postFileModel => { return true; },

                PublishFilePublishMessage = publishMessage =>
                {
                    return "Success";
                },
            };

            this.publishQueueService = new StubIPublishQueueService();

            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByNameString = name => new Repository()
                {
                    IsImpersonating = true,
                    BaseRepository = new BaseRepository()
                }
            };

            PublishController publishController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User()
                {
                    UserId = 1,
                    LastName = "Test"
                };

                ShimIdentityHelper.GetUserIUserServiceString = (us, ni) => new User();

                publishController = new PublishController(this.publishQueueService, this.userService, repositoryService, this.fileServiceFactory);
                publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;

                HttpResponseMessage reponse = publishController.Publish("SomeNameIdentifier", 1, "SomeRepositoryName");
                Assert.IsTrue(reponse.StatusCode == HttpStatusCode.OK);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void PublishByService_Should_Send_HttpStatusCodeNotImplemented_When_Repository_Is_NonImpersonating()
        {
            this.fileService = new StubIFileService();
            this.publishQueueService = new StubIPublishQueueService();
            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByNameString = name => new Repository()
                {
                    BaseRepository = new BaseRepository()
                }
            };

            PublishController publishController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User()
                {
                    UserId = 1,
                    LastName = "Test"
                };

                ShimIdentityHelper.GetUserIUserServiceString = (us, ni) => new User();

                publishController = new PublishController(this.publishQueueService, this.userService, repositoryService, this.fileServiceFactory);
                publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;

                HttpResponseMessage reponse = publishController.Publish("SomeNameIdentifier", 1, "SomeRepositoryName");
                Assert.IsTrue(reponse.StatusCode == HttpStatusCode.NotImplemented);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void PublishByService_Should_Send_NotFound_When_RepositoryName_Is_Invalid()
        {
            this.fileService = new StubIFileService();
            this.publishQueueService = new StubIPublishQueueService();
            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByNameString = name => { throw new RepositoryNotFoundException() { Name = name }; }
            };

            PublishController publishController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User()
                {
                    UserId = 1,
                    LastName = "Test"
                };

                ShimIdentityHelper.GetUserIUserServiceString = (us, ni) => new User();

                publishController = new PublishController(this.publishQueueService, this.userService, repositoryService, this.fileServiceFactory);
                publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;

                HttpResponseMessage reponse = publishController.Publish("SomeNameIdentifier", 1, "SomeRepositoryName");
                Assert.IsTrue(reponse.StatusCode == HttpStatusCode.NotFound);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void PublishByService_Should_Send_BadRequest_When_Metadata_Is_Invalid()
        {
            this.fileService = new StubIFileService()
            {

                ValidateForPublishPublishMessage = publishMessage =>
                {
                    throw new MetadataValidationException()
                    {
                        RepositoryId = 1,
                        FileId = 1,
                        NotFound = true,
                        FieldName = "RequiredField"
                    };
                }
            };

            this.publishQueueService = new StubIPublishQueueService();
            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByNameString = name => new Repository()
                {
                    IsImpersonating = true,
                    BaseRepository = new BaseRepository()
                }
            };

            PublishController publishController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User()
                {
                    UserId = 1,
                    LastName = "Test"
                };

                ShimIdentityHelper.GetUserIUserServiceString = (us, ni) => new User();

                publishController = new PublishController(this.publishQueueService, this.userService, repositoryService, this.fileServiceFactory);
                publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;

                HttpResponseMessage response = publishController.Publish("SomeNameIdentifier", 1, "SomeRepositoryName");
                Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);

                HttpError error = response.Content.ReadAsAsync<HttpError>().Result;
                Assert.AreEqual(error[MetadataValidationException.FieldNameKey], "RequiredField");
                Assert.IsTrue((bool)error[MetadataValidationException.NotFoundKey]);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void PublishByService_Should_Send_BadRequest_When_Metadata_Type_Is_Invalid()
        {
            this.fileService = new StubIFileService()
            {

                ValidateForPublishPublishMessage = publishMessage =>
                {
                    throw new MetadataValidationException()
                    {
                        RepositoryId = 1,
                        FileId = 1,
                        MetadataTypeNotFound = true,
                        FieldName = "RequiredField",
                    };
                }
            };

            this.publishQueueService = new StubIPublishQueueService();
            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByNameString = name => new Repository()
                {
                    IsImpersonating = true,
                    BaseRepository = new BaseRepository()
                }
            };

            PublishController publishController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User()
                {
                    UserId = 1,
                    LastName = "Test"
                };

                ShimIdentityHelper.GetUserIUserServiceString = (us, ni) => new User();

                publishController = new PublishController(this.publishQueueService, this.userService, repositoryService, this.fileServiceFactory);
                publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;

                HttpResponseMessage response = publishController.Publish("SomeNameIdentifier", 1, "SomeRepositoryName");
                Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);

                HttpError error = response.Content.ReadAsAsync<HttpError>().Result;
                Assert.AreEqual(error[MetadataValidationException.FieldNameKey], "RequiredField");
                Assert.IsTrue((bool)error[MetadataValidationException.MetadataTypeNotFoundKey]);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void PublishByService_Should_Send_NotFound_FileId_Is_Invalid()
        {
            this.fileService = new StubIFileService()
            {

                ValidateForPublishPublishMessage = publishMessage =>
                {
                }
            };

            this.publishQueueService = new StubIPublishQueueService()
            {
                PostFileToQueueBaseMessage = message =>
                {
                    throw new DataFileNotFoundException()
                    {
                        FileId = 100
                    };
                }
            };

            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByNameString = name => new Repository()
                {
                    IsImpersonating = true,
                    BaseRepository = new BaseRepository()
                }
            };

            PublishController publishController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User()
                {
                    UserId = 1,
                    LastName = "Test"
                };

                ShimIdentityHelper.GetUserIUserServiceString = (us, ni) => new User();

                publishController = new PublishController(this.publishQueueService, this.userService, repositoryService, this.fileServiceFactory);
                publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;

                HttpResponseMessage response = publishController.Publish("SomeNameIdentifier", 1, "SomeRepositoryName");
                Assert.IsTrue(response.StatusCode == HttpStatusCode.NotFound);

                HttpError error = response.Content.ReadAsAsync<HttpError>().Result;
                Assert.AreEqual(error[DataFileNotFoundException.FileIdKey], 100);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void PublishByService_Should_Send_Unauthorized_When_AccessToken_NotFound()
        {
            this.fileService = new StubIFileService()
            {

                ValidateForPublishPublishMessage = publishMessage =>
                {
                }
            };

            this.publishQueueService = new StubIPublishQueueService()
            {
                PostFileToQueueBaseMessage = message =>
                {
                    throw new AccessTokenNotFoundException()
                    {
                        RepositoryId = 100,
                        UserId = 200
                    };
                }
            };

            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByNameString = name => new Repository()
                {
                    IsImpersonating = true,
                    BaseRepository = new BaseRepository()
                }
            };

            PublishController publishController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User()
                {
                    UserId = 1,
                    LastName = "Test"
                };

                ShimIdentityHelper.GetUserIUserServiceString = (us, ni) => new User();

                publishController = new PublishController(this.publishQueueService, this.userService, repositoryService, this.fileServiceFactory);
                publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;

                HttpResponseMessage response = publishController.Publish("SomeNameIdentifier", 1, "SomeRepositoryName");
                Assert.IsTrue(response.StatusCode == HttpStatusCode.Unauthorized);

                HttpError error = response.Content.ReadAsAsync<HttpError>().Result;
                Assert.AreEqual(error[AccessTokenNotFoundException.RepositoryIdKeyName], 100);
                Assert.AreEqual(error[AccessTokenNotFoundException.UserIdKeyName], 200);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void PublishByService_Should_Send_BadRequest_When_FileAlready_Published()
        {
            this.fileService = new StubIFileService()
            {

                ValidateForPublishPublishMessage = publishMessage =>
                {
                }
            };

            this.publishQueueService = new StubIPublishQueueService()
            {
                PostFileToQueueBaseMessage = message =>
                {
                    throw new FileAlreadyPublishedException()
                    {
                        FileStatus = "Verifying",
                        FileId = 100,
                        RepositoryId = 200,
                    };
                }
            };

            this.fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };

            this.repositoryService = new StubIRepositoryService()
            {
                GetRepositoryByNameString = name => new Repository()
                {
                    IsImpersonating = true,
                    BaseRepository = new BaseRepository()
                }
            };

            PublishController publishController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User()
                {
                    UserId = 1,
                    LastName = "Test"
                };

                ShimIdentityHelper.GetUserIUserServiceString = (us, ni) => new User();

                publishController = new PublishController(this.publishQueueService, this.userService, repositoryService, this.fileServiceFactory);
                publishController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                publishController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (e) => httpRequest;

                HttpResponseMessage response = publishController.Publish("SomeNameIdentifier", 1, "SomeRepositoryName");
                Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);

                HttpError error = response.Content.ReadAsAsync<HttpError>().Result;
                Assert.AreEqual(error[FileAlreadyPublishedException.FileIdKey], 100);
                Assert.AreEqual(error[FileAlreadyPublishedException.FileStatusKey], "Verifying");
                Assert.AreEqual(error[FileAlreadyPublishedException.RepositoryIdKey], 200);
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

            //TODO: needs to be check
            FilesController fileController = new FilesController(this.fileServiceFactory, repositoryService, qualityService, userService, null);//TODO
            fileController.Request = new HttpRequestMessage(Method, string.Empty);
            fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

            // Perform
            return fileController;
        }

    }
}

