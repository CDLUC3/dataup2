using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.Services.UserService;
using Microsoft.Research.DataOnboarding.Services.UserService.Fakes;
using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.Research.DataOnboarding.Utilities.Extensions.Fakes;
using Microsoft.Research.DataOnboarding.WebApi.Api;
using Microsoft.Research.DataOnboarding.WebApi.Fakes;
using Microsoft.Research.DataOnboarding.WebApi.FileHandlers;
using Microsoft.Research.DataOnboarding.WebApi.Helpers.Fakes;
using Microsoft.Research.DataOnboarding.WebApi.FileHandlers.Fakes;
using Microsoft.Research.DataOnboarding.WebApi.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Fakes;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;
using System.Web.Fakes;
using System.Web.Http;
using System.Web.Http.Hosting;
using Memory = System.IO;

namespace Microsoft.Research.DataOnboarding.WebApi.Tests.UnitTests
{
    [TestClass]
    public class BlobControllerUnitTests
    {
        public HttpContext context;
        private IFileService fileService = null;
        private IFileServiceFactory fileServiceFactory = null;

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Allow_File_Download_For_Valid_UserID_and_FileId()
        {
            File fakeFile = null;

            byte[] byteArray = null;

            this.fileService = new StubIFileService()
            {
                DownloadFileInt32Int32 = (userId, fileId) => { return new DataDetail() { FileDetail = fakeFile, FileNameToDownLoad = fakeFile.Name, MimeTypeToDownLoad = "text/plain", DataStream = byteArray }; }

            };

            BlobController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Get);

            // Serialize the object to send as stream
            byteArray = SerializeObject(fakeFile);
            HttpResponseMessage response;

            // Perform
            response = controller.DownLoadFile(fakeFile.FileId);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Stataus is not as expected(OK)");
            var result = response.Content.ReadAsByteArrayAsync().Result;

            //Deserialize the object to identify results
            File resultFile = (File)DeserializeObject<File>(result);

            Assert.AreEqual(resultFile.Name, fakeFile.Name, "Expected and Actual result is not same.");
            Assert.AreEqual(resultFile.FileId, fakeFile.FileId, "Expected and Actual result is not same.");
            Assert.AreEqual(resultFile.RepositoryId, fakeFile.RepositoryId, "Expected and Actual result is not same.");
            Assert.AreEqual(resultFile.CreatedBy, fakeFile.CreatedBy, "Expected and Actual result is not same.");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Return_Error_On_Download_When_FileService_Is_Null()
        {
            File fakeFile = null;
            this.fileService = null;

            BlobController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Get);

            // Perform
            var response = controller.DownLoadFile(fakeFile.FileId);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest, "Stataus is not as expected");
            var result = response.Content.ReadAsAsync<HttpError>().Result;

            Assert.IsNotNull(result, "Result is not as Expected");
            Assert.AreEqual(result.Message, MessageStrings.Invalid_File_Id, "Result is not as expected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(AggregateException), "An empty file name was inappropriately allowed.")]
        public void Post_ShouldThrowArgumentException_WhenfileNameIsNullOrEmpty()
        {
            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                //GetFileServiceStringSkyDriveConfiguration = (instanceName, skyDriveConfiguration) => { return null; }
            };
            IUserService userService = new StubIUserService();
            IFileHandlerFactory fileHandlerFactory = new StubIFileHandlerFactory();

            try
            {
                using (ShimsContext.Create())
                {
                    ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (us, cp) => null;
                    BlobController blobController = new BlobController(fileServiceFactory, userService, fileHandlerFactory);
                    Task task = blobController.Post(string.Empty, string.Empty, string.Empty);
                    task.Wait();
                }

                Assert.Fail("Should have exceptioned above!");
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException is ArgumentException)
                    throw;
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(AggregateException), "An empty file extension was inappropriately allowed.")]
        public void Post_ShouldThrowArgumentException_WhenFileExtensionIsNullOrEmpty()
        {
            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => { return null; }
            };
            IUserService userService = new StubIUserService();
            IFileHandlerFactory fileHandlerFactory = new StubIFileHandlerFactory();

            try
            {
                using (ShimsContext.Create())
                {
                    ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (us, cp) => null;
                    BlobController blobController = new BlobController(fileServiceFactory, userService, fileHandlerFactory);
                    Task task = blobController.Post("SomeFileName", string.Empty, string.Empty);
                    task.Wait();
                }

                Assert.Fail("Should have exceptioned above!");
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException is ArgumentException)
                    throw;
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(AggregateException), "An empty content type was inappropriately allowed.")]
        public void Post_ShouldThrowArgumentException_WhenContentTypeIsNullOrEmpty()
        {
            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => { return null; }
            };
            IUserService userService = new StubIUserService();
            IFileHandlerFactory fileHandlerFactory = new StubIFileHandlerFactory();

            try
            {
                using (ShimsContext.Create())
                {
                    ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (us, cp) => null;
                    BlobController blobController = new BlobController(fileServiceFactory, userService, fileHandlerFactory);
                    Task task = blobController.Post("SomeFileName", "SomeFileExtension", string.Empty);
                    task.Wait();
                }

                Assert.Fail("Should have exceptioned above!");
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException is ArgumentException)
                    throw;
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(AggregateException), "A null bufferless input stream was inappropriately allowed.")]
        public void Post_ShouldThrowArgumentNullException_WhenBufferlessInputStreamIsNull()
        {
            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => { return null; }
            };
            IUserService userService = new StubIUserService();
            IFileHandlerFactory fileHandlerFactory = new StubIFileHandlerFactory();

            try
            {
                using (ShimsContext.Create())
                {
                    ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                    ShimHttpContext.AllInstances.RequestGet = httpContext => new ShimHttpRequest();
                    ShimHttpRequest.AllInstances.GetBufferlessInputStream = httpRequest => null;
                    ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (us, cp) => null;

                    BlobController blobController = new BlobController(fileServiceFactory, userService, fileHandlerFactory);
                    Task task = blobController.Post("SomeFileName", "SomeFileExtension", "SomeContentType");
                    task.Wait();
                }

                Assert.Fail("Should have exceptioned above!");
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException is ArgumentException)
                    throw;
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Post_ShouldReturnHttpStatusCodeOk_WhenBufferlessInputStreamIsNotNull()
        {
            HttpResponseMessage httpResponseMessage = null;
            IFileService fileService = new StubIFileService()
            {
                UploadFileDataDetail = dd => { dd.FileDetail = new File() { FileId = 1 }; return dd; }
            };
            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };
            IUserService userService = new StubIUserService();

            IFileHandler fileHandler = new StubIFileHandler()
            {
                UploadDataFile = df => new List<DataDetail>() { new DataDetail() { FileDetail = new File() { FileId = 1 } } }
            };
            IFileHandlerFactory fileHandlerFactory = new StubIFileHandlerFactory()
            {
                GetFileHandlerStringInt32 = (type, userId) => fileHandler
            };

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, string.Empty);
            httpRequestMessage.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            using (ShimsContext.Create())
            {
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = httpContext => new ShimHttpRequest();
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (us, cp) => new User();
                ShimStreamExtensions.GetBytesAsyncStream = stream => Task.FromResult(new byte[0]);
                ShimHttpRequest.AllInstances.GetBufferlessInputStream = httpRequest => new Memory.MemoryStream();

                BlobController blobController = new BlobController(fileServiceFactory, userService, fileHandlerFactory);
                blobController.Request = httpRequestMessage;
                var task = blobController.Post("SomeFileName", "SomeFileExtension", "SomeContentType");
                task.Wait();
                httpResponseMessage = task.Result;
            }

            Assert.AreEqual(HttpStatusCode.OK, httpResponseMessage.StatusCode, "Stataus is not as expected(Bad Request)");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(Exception), "File Service threw exception.")]
        public void DownLoadFile_ShouldThrowException_WhenFileServiceThrowsException()
        {
            File fakeFile = null;
            this.fileService = new StubIFileService()
            {
                DownloadFileInt32Int32 = (userId, fileId) => { throw new Exception("Cannot Download File"); }
            };

            BlobController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Get);
            controller.DownLoadFile(fakeFile.FileId);
        }

        /// <summary>
        /// Setup Base for controller and Create Request
        /// </summary>
        /// <param name="fakeFile">file object</param>
        /// <param name="Method">MEthod type to pass (GET, PUT, POST or Delete)</param>
        /// <returns>Contrller object</returns>
        private BlobController SetupBaseAndRequest(out File fakeFile, HttpMethod Method)
        {
            fakeFile = new File()
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

            IUserService userService = new Microsoft.Research.DataOnboarding.Services.UserService.Fakes.StubIUserService()
            {
            };
            IFileHandlerFactory fileHandlerFactory = new StubIFileHandlerFactory();
            BlobController blobController;

            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (us, cp) => new User();
                blobController = new BlobController(this.fileServiceFactory, userService, fileHandlerFactory);
            }

            blobController.Request = new HttpRequestMessage(Method, string.Empty);
            blobController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

            // Perform
            return blobController;
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Return_Error_On_Download_When_Model_State_Is_Invalid()
        {
            File fakeFile = null;

            byte[] byteArray = null;

            this.fileService = new StubIFileService()
            {
                DownloadFileInt32Int32 = (userId, fileId) => { return new DataDetail() { FileDetail = fakeFile, FileNameToDownLoad = fakeFile.Name, MimeTypeToDownLoad = "text/plain", DataStream = byteArray }; }
            };

            BlobController controller = SetupBaseAndRequest(out fakeFile, HttpMethod.Get);
            controller.ModelState.AddModelError("Invalid", "Invalid Model State");

            // Serialize the object to send as stream
            byteArray = SerializeObject(fakeFile);

            // Perform
            var response = controller.DownLoadFile(fakeFile.FileId);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest, "Stataus is not as expected");
            var result = response.Content.ReadAsAsync<HttpError>().Result;

            Assert.IsNotNull(result, "Result is not as Expected");
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

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void DownLoadFile_ShouldThrowArgumentException_WhenNameIdentifierIsNullOrEmpty()
        {
            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => { return null; }
            };
            IUserService userService = new StubIUserService();
            IFileHandlerFactory fileHandlerFactory = new StubIFileHandlerFactory();

            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (us, cp) => null;
                BlobController blobController = new BlobController(fileServiceFactory, userService, fileHandlerFactory);
                blobController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                blobController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = blobController.DownLoadFile(string.Empty, 0);
                Assert.AreEqual(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void DownLoadFile_ShouldReturnBadRequestHttpStatusCode_WhenModelStateIsInvalid()
        {
            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => { return null; }
            };
            IUserService userService = new StubIUserService();
            IFileHandlerFactory fileHandlerFactory = new StubIFileHandlerFactory();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, string.Empty);
            httpRequestMessage.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            HttpResponseMessage httpResponseMessage = null;

            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (us, cp) => null;
                BlobController blobController = new BlobController(fileServiceFactory, userService, fileHandlerFactory);
                blobController.ModelState.AddModelError("Invalid", "Invalid Model State");
                blobController.Request = httpRequestMessage;
                httpResponseMessage = blobController.DownLoadFile("SomeNameIdentifier", 0);
            }

            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode, "Status Code is not as expected(Bad Request)");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void DownLoadFile_ShouldReturnOktHttpStatusCode_WhenUserIdAndFileIdAreValid()
        {
            DataDetail dataDetail = new DataDetail() { FileNameToDownLoad = "abc.csv", DataStream = new byte[0] };
            StubIFileService fileService = new StubIFileService()
            {
                DownloadFileInt32Int32 = (userId, fileId) => dataDetail
            };
            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => fileService
            };
            IUserService userService = new StubIUserService();
            IFileHandlerFactory fileHandlerFactory = new StubIFileHandlerFactory();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, string.Empty);
            httpRequestMessage.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            HttpResponseMessage httpResponseMessage = null;

            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (us, cp) => null;
                ShimIdentityHelper.GetUserIUserServiceString = (us, ni) => new User();
                BlobController blobController = new BlobController(fileServiceFactory, userService, fileHandlerFactory);
                blobController.Request = httpRequestMessage;
                httpResponseMessage = blobController.DownLoadFile("SomeNameIdentifier", 0);
            }

            Assert.AreEqual(HttpStatusCode.OK, httpResponseMessage.StatusCode, "Status Code is not as expected(Bad Request)");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(AggregateException), "An empty name identifier was inappropriately allowed.")]
        public void Post_ShouldThrowArgumentException_WhenNameIdentifierIsNullOrEmpty()
        {
            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => { return null; }
            };
            IUserService userService = new StubIUserService();
            IFileHandlerFactory fileHandlerFactory = new StubIFileHandlerFactory();

            try
            {
                using (ShimsContext.Create())
                {
                    ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (us, cp) => null;
                    BlobController blobController = new BlobController(fileServiceFactory, userService, fileHandlerFactory);
                    Task task = blobController.Post(string.Empty);
                    task.Wait();
                }

                Assert.Fail("Should have exceptioned above!");
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException is ArgumentException)
                    throw;
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(AggregateException), "An empty name identifier was inappropriately allowed.")]
        public void Post_ShouldThrowHttpResponseException_WhenRequestContentIsNotMimeMultipart()
        {
            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => { return null; }
            };
            IUserService userService = new StubIUserService();
            IFileHandlerFactory fileHandlerFactory = new StubIFileHandlerFactory();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, string.Empty);
            httpRequestMessage.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            httpRequestMessage.Content = new StubHttpContent();

            try
            {
                using (ShimsContext.Create())
                {
                    ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (us, cp) => null;
                    BlobController blobController = new BlobController(fileServiceFactory, userService, fileHandlerFactory);
                    blobController.Request = httpRequestMessage;
                    Task task = blobController.Post("SomeNameIdentifier");
                    task.Wait();

                    Assert.Fail("Should have exceptioned above!");
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException is HttpResponseException)
                    throw;
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Post_ShouldReturnOktHttpStatusCode_WhenRequestContentIsMimeMultipart()
        {
            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = (instanceName) => { return null; }
            };
            IUserService userService = new StubIUserService();
            IFileHandlerFactory fileHandlerFactory = new StubIFileHandlerFactory();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, string.Empty);
            httpRequestMessage.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            HttpContent httpContent = new StubMultipartFormDataContent();
            httpRequestMessage.Content = httpContent;

            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (us, cp) => null;
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.ServerGet = httpContext => new ShimHttpServerUtility();
                ShimHttpServerUtility.AllInstances.MapPathString = (hsu, s) => @"C:\";

                BlobController blobController = new BlobController(fileServiceFactory, userService, fileHandlerFactory);
                blobController.Request = httpRequestMessage;
                Task task = blobController.Post("SomeNameIdentifier");
                task.Wait();
            }
        }
    }
}
