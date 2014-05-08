using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.QCService.Interface;
using Microsoft.Research.DataOnboarding.QCService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.Research.DataOnboarding.WebApi.Api;
using Microsoft.Research.DataOnboarding.WebApi.Helpers.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace Microsoft.Research.DataOnboarding.WebApi.Tests.UnitTests
{
    [TestClass]
    public class QCControllerTests
    {
        private IQCService qcService = null;

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_All_Available_Quality_Check_Rules_Success()
        {
            List<QualityCheckColumnRule> lstCol = new List<QualityCheckColumnRule>();
            QualityCheckColumnRule colRule = new QualityCheckColumnRule() { Description = "Column 1 desc", ErrorMessage = "error msg", HeaderName = "Column 1", IsActive = true, IsRequired = true, Order = 1, QualityCheck = null, QualityCheckColumnRuleId = 1, QualityCheckColumnTypeId = 1, QualityCheckId = 1, Range = "" };
            QualityCheckColumnRule colRule1 = new QualityCheckColumnRule() { Description = "Column 2 desc", ErrorMessage = "error msg1", HeaderName = "Column 2", IsActive = true, IsRequired = true, Order = 2, QualityCheck = null, QualityCheckColumnRuleId = 2, QualityCheckColumnTypeId = 2, QualityCheckId = 1, Range = "" };
            lstCol.Add(colRule);
            lstCol.Add(colRule1);

            List<QualityCheckModel> lstModel = new List<QualityCheckModel>()
            {
                new QualityCheckModel(){ CreatedUser="Test User", QualityCheckData= new QualityCheck(){CreatedBy = 1, CreatedOn = DateTime.UtcNow, Description = "Test Rule", EnforceOrder = true, FileQualityChecks = null, IsActive = true, IsVisibleToAll = true, ModifiedBy = 1, ModifiedOn = DateTime.UtcNow, Name = "Test Rule", QualityCheckColumnRules = lstCol, QualityCheckId = 1}}          
            };

            this.qcService = new StubIQCService()
            {
                GetQualityCheckRulesBoolean = (includeAdmin) => { return lstModel; }
            };

            HttpResponseMessage response = null;
          
            QCController qcController = CreateRequest(HttpMethod.Get);

            // Perform
            response = qcController.RetrieveQualityCheckRules();
            
         
            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Status Code is not as Expected");
            var result = response.Content.ReadAsAsync<List<QualityCheckModel>>().Result;

            Assert.IsTrue(result.Count >= 1, "There is no Quality check rule in the List");
            Assert.AreEqual(result.Count, lstModel.Count, "Expected and Actula Repository Counts are not equal");

            Assert.AreEqual(result[0].CreatedUser, "Test User");
            Assert.AreEqual(result[0].QualityCheckData.CreatedBy, 1);
            Assert.AreEqual(result[0].QualityCheckData.Description, "Test Rule");
            Assert.AreEqual(result[0].QualityCheckData.EnforceOrder, true);
            Assert.AreEqual(result[0].QualityCheckData.IsActive, true);
            Assert.AreEqual(result[0].QualityCheckData.IsVisibleToAll, true);
            Assert.AreEqual(result[0].QualityCheckData.EnforceOrder, true);
            Assert.AreEqual(result[0].QualityCheckData.ModifiedBy, 1);
            Assert.AreEqual(result[0].QualityCheckData.Name, "Test Rule");
            Assert.AreEqual(result[0].QualityCheckData.QualityCheckId, 1);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_All_Available_QC_Rules_When_No_QC_Rule_Available()
        {
            this.qcService = new StubIQCService()
            {
                GetQualityCheckRulesBoolean = (includeAdmin) => { return null; }
            };

            QCController qcController = CreateRequest(HttpMethod.Get);

            // Perform
            var response = qcController.RetrieveQualityCheckRules();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            var result = response.Content.ReadAsAsync<List<Repository>>().Result;
            Assert.IsNull(result, "Result is not null");

        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_Quality_Check_By_Id_Success()
        {
            List<QualityCheckColumnRule> lstCol = new List<QualityCheckColumnRule>();
            QualityCheckColumnRule colRule = new QualityCheckColumnRule() { Description = "Column 1 desc", ErrorMessage = "error msg", HeaderName = "Column 1", IsActive = true, IsRequired = true, Order = 1, QualityCheck = null, QualityCheckColumnRuleId = 1, QualityCheckColumnTypeId = 1, QualityCheckId = 1, Range = "" };
            QualityCheckColumnRule colRule1 = new QualityCheckColumnRule() { Description = "Column 2 desc", ErrorMessage = "error msg1", HeaderName = "Column 2", IsActive = true, IsRequired = true, Order = 2, QualityCheck = null, QualityCheckColumnRuleId = 2, QualityCheckColumnTypeId = 2, QualityCheckId = 1, Range = "" };

            lstCol.Add(colRule);
            lstCol.Add(colRule1);

            QualityCheckModel model = new QualityCheckModel() { CreatedUser = "Test User", QualityCheckData = new QualityCheck() { CreatedBy = 1, CreatedOn = DateTime.UtcNow, Description = "Test Rule", EnforceOrder = true, FileQualityChecks = null, IsActive = true, IsVisibleToAll = true, ModifiedBy = 1, ModifiedOn = DateTime.UtcNow, Name = "Test Rule", QualityCheckColumnRules = lstCol, QualityCheckId = 1 } };

            this.qcService = new StubIQCService()
            {
                GetQualityCheckByIdInt32 = (id) => { return model; }
            };

            QCController qcController = CreateRequest(HttpMethod.Get);

            // Perform
            var response = qcController.GetQualityCheckById(1);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Status Code is not as Expected");
            var result = response.Content.ReadAsAsync<QualityCheckModel>().Result;

            Assert.IsNotNull(result, "There is no Quality check rule for the mentioned id");

            Assert.AreEqual(result.CreatedUser, "Test User");
            Assert.AreEqual(result.QualityCheckData.CreatedBy, 1);
            Assert.AreEqual(result.QualityCheckData.Description, "Test Rule");
            Assert.AreEqual(result.QualityCheckData.EnforceOrder, true);
            Assert.AreEqual(result.QualityCheckData.IsActive, true);
            Assert.AreEqual(result.QualityCheckData.IsVisibleToAll, true);
            Assert.AreEqual(result.QualityCheckData.EnforceOrder, true);
            Assert.AreEqual(result.QualityCheckData.ModifiedBy, 1);
            Assert.AreEqual(result.QualityCheckData.Name, "Test Rule");
            Assert.AreEqual(result.QualityCheckData.QualityCheckId, 1);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_Quality_Check_By_Id_When_No_QC_Rule_Available()
        {
            this.qcService = new StubIQCService()
            {
                GetQualityCheckByIdInt32 = (id) => { return null; }
            };

            QCController qcController = CreateRequest(HttpMethod.Get);

            // Perform
            var response = qcController.GetQualityCheckById(1);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            var result = response.Content.ReadAsAsync<QualityCheckModel>().Result;
            Assert.IsNull(result, "Result is not null");

        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_Quality_Check_By_Id_When_QC_Service_Is_Not_Available()
        {
            this.qcService = null;

            QCController qcController = CreateRequest(HttpMethod.Get);

            // Perform
            var response = qcController.GetQualityCheckById(1);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is null");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_Quality_Check_By_Id_When_Invalid_Model_State()
        {
            this.qcService = new StubIQCService();

            QCController qcController = CreateRequest(HttpMethod.Get);
            qcController.ModelState.AddModelError("Invalid", "Invlaid Model State");

            // Perform
            var response = qcController.GetQualityCheckById(1);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is null");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_Quality_Check_By_Id_Error_Server_Exeption()
        {
            this.qcService = new StubIQCService()
            {
                GetQualityCheckByIdInt32 = (id) => { throw new Exception("Cannot Retrieve Repositiory"); }
            };

            QCController qcController = CreateRequest(HttpMethod.Get);

            // Perform
            var response = qcController.GetQualityCheckById(1);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsTrue(result.Message.Contains("Cannot Retrieve Repositiory"), "Result is as Expected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_Quality_Check_By_Id_Error_Inner_Exeption()
        {
            this.qcService = new StubIQCService()
            {
                GetQualityCheckByIdInt32 = (id) => { throw new Exception("", new Exception("Some Inner Exception")); }
            };

            QCController qcController = CreateRequest(HttpMethod.Get);

            // Perform
            var response = qcController.GetQualityCheckById(1);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsTrue(result.Message.Contains("Some Inner Exception"), "Result is as Expected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Delete_Quality_Check_Success()
        {
            this.qcService = new StubIQCService()
            {
                DeleteQualityCheckRuleInt32 = (id) => { return true; }
            };

            QCController qcController = CreateRequest(HttpMethod.Delete);

            // Perform
            var response = qcController.Delete(1);

            //// Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Status Code is not as Expected");
            var result = response.Content.ReadAsAsync<bool>().Result;

            Assert.IsTrue(result);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Delete_Quality_Check_Fail()
        {
            this.qcService = new StubIQCService()
            {
                DeleteQualityCheckRuleInt32 = (id) => { return false; }
            };

            QCController qcController = CreateRequest(HttpMethod.Delete);

            // Perform
            var response = qcController.Delete(1);

            //// Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Status Code is not as Expected");
            var result = response.Content.ReadAsAsync<bool>().Result;

            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Delete_Quality_Check_When_QC_Service_Is_Not_Available()
        {
            this.qcService = null;

            QCController qcController = CreateRequest(HttpMethod.Delete);

            // Perform
            var response = qcController.Delete(1);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is null");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Delete_Quality_Check_When_Invalid_Model_State()
        {
            this.qcService = new StubIQCService();

            QCController qcController = CreateRequest(HttpMethod.Delete);
            qcController.ModelState.AddModelError("Invalid", "Invlaid Model State");

            // Perform
            var response = qcController.Delete(1);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is null");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Delete_Quality_Check_Error_Server_Exeption()
        {
            this.qcService = new StubIQCService()
            {
                DeleteQualityCheckRuleInt32 = (id) => { throw new Exception("Cannot Retrieve Repositiory"); }
            };

            QCController qcController = CreateRequest(HttpMethod.Delete);

            // Perform
            var response = qcController.Delete(1);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsTrue(result.Message.Contains("Cannot Retrieve Repositiory"), "Result is as Expected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Delete_Quality_Check_Error_Inner_Exeption()
        {
            this.qcService = new StubIQCService()
            {
                DeleteQualityCheckRuleInt32 = (id) => { throw new Exception("", new Exception("Some Inner Exception")); }
            };

            QCController qcController = CreateRequest(HttpMethod.Delete);

            // Perform
            var response = qcController.Delete(1);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsTrue(result.Message.Contains("Some Inner Exception"), "Result is as Expected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Check_Rule_Exists_Success()
        {
            this.qcService = new StubIQCService()
            {
                CheckRuleExistsString = (ruleName) => { return 1; }
            };

            QCController qcController = CreateRequest(HttpMethod.Post);

            // Perform
            var response = qcController.CheckRuleExists("Rule");

            //// Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Status Code is not as Expected");
            var result = response.Content.ReadAsAsync<int>().Result;

            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Check_Rule_Exists_When_QC_Service_Is_Not_Available()
        {
            this.qcService = null;

            QCController qcController = CreateRequest(HttpMethod.Post);

            // Perform
            var response = qcController.CheckRuleExists("Rule");

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is null");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Check_Rule_Exists_When_Invalid_Model_State()
        {
            this.qcService = new StubIQCService();

            QCController qcController = CreateRequest(HttpMethod.Post);
            qcController.ModelState.AddModelError("Invalid", "Invlaid Model State");

            // Perform
            var response = qcController.CheckRuleExists("Rule");

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is null");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Check_Rule_Exists_Error_Server_Exeption()
        {
            this.qcService = new StubIQCService()
            {
                CheckRuleExistsString = (ruleName) => { throw new Exception("Cannot Retrieve Repositiory"); }
            };

            QCController qcController = CreateRequest(HttpMethod.Post);

            // Perform
            var response = qcController.CheckRuleExists("Rule");

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsTrue(result.Message.Contains("Cannot Retrieve Repositiory"), "Result is as Expected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Check_Rule_Exists_Error_Inner_Exeption()
        {
            this.qcService = new StubIQCService()
            {
                CheckRuleExistsString = (ruleName) => { throw new Exception("", new Exception("Some Inner Exception")); }
            };

            QCController qcController = CreateRequest(HttpMethod.Post);

            // Perform
            var response = qcController.CheckRuleExists("Rule");
            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsTrue(result.Message.Contains("Some Inner Exception"), "Result is as Expected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task GetQualityCheckRulesAndFileSheets_ShouldReturnHttpStatusCodeNotFound_WhenFileServiceReturnsNullForFile()
        {
            IFileService fileService = new StubIFileService()
            {
                GetFileByFileIdInt32 = (fileId) => null
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                QCController fileController = new QCController(null, fileServiceFactory, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = await fileController.GetQualityCheckRulesAndFileSheets(0);
                Assert.AreEqual(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task GetQualityCheckRulesAndFileSheets_ShouldReturnHttpStatusCodeNotImplemented_WhenFileServiceReturnsAFileOtherThanXlsxAndCsv()
        {
            IFileService fileService = new StubIFileService()
            {
                GetFileByFileIdInt32 = (fileId) => new File() { Name = "abc.xyz" }
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                QCController fileController = new QCController(null, fileServiceFactory, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = await fileController.GetQualityCheckRulesAndFileSheets(0);
                Assert.AreEqual(HttpStatusCode.NotImplemented, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task GetQualityCheckRulesAndFileSheets_ShouldReturnHttpStatusCodeOK_WhenFileServiceReturnsAXlsxFile()
        {
            IFileService fileService = new StubIFileService()
            {
                GetFileByFileIdInt32 = (fileId) => new File() { Name = "abc.xlsx" },
                GetDocumentSheetDetailsFile = (file) => Task.FromResult<IEnumerable<FileSheet>>(null)
            };

            IFileServiceFactory fileServiceFactory = new StubIFileServiceFactory()
            {
                GetFileServiceString = instanceName => fileService
            };

            IQCService qcService = new StubIQCService();

            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                QCController fileController = new QCController(qcService, fileServiceFactory, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = await fileController.GetQualityCheckRulesAndFileSheets(0);
                Assert.AreEqual(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task GetQualityCheckIssues_ShouldReturnHttpStatusCodeOK()
        {
            IQCService qcService = new StubIQCService()
            {
                GetQualityCheckIssuesInt32Int32String = (fileId, qualityCheckId, sheetIds) => Task.FromResult(Enumerable.Empty<QualityCheckResult>())
            };

            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (service, principle) => new User();

                QCController fileController = new QCController(qcService, null, null);
                fileController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
                fileController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

                HttpResponseMessage httpResponseMessage = await fileController.GetQualityCheckIssues(0, 0, string.Empty);
                Assert.AreEqual(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            }
        }

        #region private methods

        private QCController CreateRequest(HttpMethod Method)
        {
            QCController repositoryTypesController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (us, cp) => new User();
                repositoryTypesController = new QCController(qcService, null, null);
            }

            repositoryTypesController.Request = new HttpRequestMessage(Method, string.Empty);
            repositoryTypesController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();
            return repositoryTypesController;
        }

        #endregion
    }
}
