using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.Research.DataOnboarding.WebApi.Api;
using Microsoft.Research.DataOnboarding.WebApi.Api;
using Microsoft.Research.DataOnboarding.WebApi.Helpers.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;


namespace Microsoft.Research.DataOnboarding.WebApi.Tests.UnitTests
{
    [TestClass]
    public class RepositoryControllerShould
    {
        IRepositoryService repositoryService = null;
       
        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_All_Available_Repositories_Success()
        {
            List<RepositoryDataModel> lstModel = new List<RepositoryDataModel>()
            {
                new RepositoryDataModel(){AuthenticationType="testAuthType", CreatedUser="testUser", RepositoryData= new Repository() { RepositoryId = 1, CreatedBy =1,  BaseRepositoryId =1, IsActive = true}},
                new RepositoryDataModel(){AuthenticationType="testAuthType1", CreatedUser="testUser1", RepositoryData= new Repository() { RepositoryId =3, CreatedBy =1, BaseRepositoryId = 1, IsActive = true}}
            };

            this.repositoryService = new Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes.StubIRepositoryService()
            {
                RetrieveRepositoriesBoolean = (includeAdmin) => { return lstModel; }
            };

            RepositoryController repositoryController = CreateRequest(HttpMethod.Get);

            // Perform
            var response = repositoryController.GetRepositories();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Status Code is not as Expected");
            var result = response.Content.ReadAsAsync<List<RepositoryDataModel>>().Result;

            Assert.IsTrue(result.Count >= 1, "There is no Repository in the List");
            Assert.AreEqual(result.Count, lstModel.Count, "Expected and Actula Repository Counts are not equal");

            Assert.AreEqual(result[0].AuthenticationType, "testAuthType");
            Assert.AreEqual(result[0].CreatedUser, "testUser");
            Assert.AreEqual(result[0].RepositoryData.RepositoryId, lstModel[0].RepositoryData.RepositoryId, "Repository Id's are not equal");
            Assert.AreEqual(result[0].RepositoryData.CreatedBy, lstModel[0].RepositoryData.CreatedBy, "User Id's are not equal");
            Assert.AreEqual(result[0].RepositoryData.BaseRepositoryId, lstModel[0].RepositoryData.BaseRepositoryId, "BaseRepository Id's are not equal");
            Assert.AreEqual(result[0].RepositoryData.IsActive, lstModel[0].RepositoryData.IsActive, "IsActive states are not equal");

            Assert.AreEqual(result[1].AuthenticationType, "testAuthType1");
            Assert.AreEqual(result[1].CreatedUser, "testUser1");
            Assert.AreEqual(result[1].RepositoryData.RepositoryId, lstModel[1].RepositoryData.RepositoryId, "Repository Id's are not equal");
            Assert.AreEqual(result[1].RepositoryData.CreatedBy, lstModel[1].RepositoryData.CreatedBy, "User Id's are not equal");
            Assert.AreEqual(result[1].RepositoryData.BaseRepositoryId, lstModel[1].RepositoryData.BaseRepositoryId, "BaseRepository Id's are not equal");
            Assert.AreEqual(result[1].RepositoryData.IsActive, lstModel[1].RepositoryData.IsActive, "IsActive states are not equal");
        }

        private RepositoryController CreateRequest(HttpMethod Method)
        {
            RepositoryController repositoryController;
            using (ShimsContext.Create())
            {
                ShimIdentityHelper.GetCurrentUserIUserServiceClaimsPrincipal = (us, cp) => new User();
                repositoryController = new RepositoryController(repositoryService, null);
            }

            repositoryController.Request = new HttpRequestMessage(Method, string.Empty);
            repositoryController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();
            return repositoryController;
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_All_Available_Repositories_When_No_Repository_Available()
        {
            this.repositoryService = new Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes.StubIRepositoryService()
            {
                RetrieveRepositoriesBoolean = (includeAdmin) => { return null; }
            };

            RepositoryController repositoryController = CreateRequest(HttpMethod.Get);
            // Perform
            var response = repositoryController.GetRepositories();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            var result = response.Content.ReadAsAsync<List<Repository>>().Result;
            Assert.IsNull(result, "Result is not null");
           
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_All_Available_Repositories_When_Repository_Service_Is_Not_Available()
        {
            this.repositoryService = null;

            RepositoryController repositoryController = CreateRequest(HttpMethod.Get);
            // Perform
            var response = repositoryController.GetRepositories();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is null"); 
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_All_Available_Repositories_Invalid_Model_State()
        {
            this.repositoryService = new Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes.StubIRepositoryService();

            RepositoryController repositoryController = CreateRequest(HttpMethod.Get);
            repositoryController.ModelState.AddModelError("Invalid", "Invlaid Model State");

            // Perform
            var response = repositoryController.GetRepositories();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is null");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_All_Available_Repositories_Error_Server_Exeption()
        {
            this.repositoryService = new Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes.StubIRepositoryService()
            {
                RetrieveRepositoriesBoolean = (includeAdmin) => { throw new Exception("Cannot Retrieve Repositiory"); }
            };

            RepositoryController repositoryController = CreateRequest(HttpMethod.Get);
         
            // Perform
            var response = repositoryController.GetRepositories();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsTrue(result.Message.Contains("Cannot Retrieve Repositiory"), "Result is as Expected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_All_Available_Repositories_Error_Inner_Exeption()
        {
            this.repositoryService = new Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes.StubIRepositoryService()
            {
                RetrieveRepositoriesBoolean = (includeAdmin) => { throw new Exception("", new Exception("Some Inner Exception")); }
            };

            RepositoryController repositoryController = CreateRequest(HttpMethod.Get);

            // Perform
            var response = repositoryController.GetRepositories();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsTrue(result.Message.Contains("Some Inner Exception"), "Result is as Expected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_Repository_By_Id_Success()
        {
            Repository fakeRepository = new Repository() { RepositoryId = 3, CreatedBy = 1, BaseRepositoryId = 1, IsActive = true };

            this.repositoryService = new Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes.StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = id => { return fakeRepository; }
            };

            RepositoryController repositoryController = CreateRequest(HttpMethod.Get);
            // Perform
            var response = repositoryController.GetRepository(fakeRepository.RepositoryId);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<Repository>().Result;
            Assert.IsNotNull(result, "Result is null");

            Assert.AreEqual(result.RepositoryId, fakeRepository.RepositoryId, "Repository Id's are not Same.");
            Assert.AreEqual(result.CreatedBy, fakeRepository.CreatedBy, "User Id's are not same");
            //Assert.AreEqual(result.RepositoryTypeId, fakeRepository.RepositoryTypeId, "RepositoryType Id's are not same");
            Assert.AreEqual(result.BaseRepositoryId, fakeRepository.BaseRepositoryId, "BaseRepository Id's are not same");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_Repository_By_Id_Failure()
        {
            Repository fakeRepository = new Repository() { RepositoryId = 3, CreatedBy = 1, BaseRepositoryId = 1, IsActive = true };

            this.repositoryService = new Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes.StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = id => { return null; }
            };

            RepositoryController repositoryController = CreateRequest(HttpMethod.Get);
            // Perform
            var response = repositoryController.GetRepository(fakeRepository.RepositoryId);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<Repository>().Result;
            Assert.IsNull(result, "Result is not null");
        }
        
        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_Repository_By_Id_When_No_Repository_Service_Available()
        {
            Repository fakeRepository = new Repository() { RepositoryId = 3, CreatedBy = 1, BaseRepositoryId = 1, IsActive = true };

            this.repositoryService = null;

            RepositoryController repositoryController = CreateRequest(HttpMethod.Get);
            // Perform
            var response = repositoryController.GetRepository(fakeRepository.RepositoryId);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is null");
            Assert.AreEqual(result.Message, "Repository service outage. Contact Data Onboarding support.", "Result is not null");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_Repository_By_Id_When_Model_State_Invalid()
        {
            Repository fakeRepository = new Repository() { RepositoryId = 3, CreatedBy = 1, BaseRepositoryId = 1, IsActive = true };

            this.repositoryService = new Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes.StubIRepositoryService(); 

            RepositoryController repositoryController = CreateRequest(HttpMethod.Get);
            repositoryController.ModelState.AddModelError("Invalid", "Invalid Model State");
            // Perform
            var response = repositoryController.GetRepository(fakeRepository.RepositoryId);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is null");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_Repository_By_Id_Internal_Server_Error()
        {
            Repository fakeRepository = new Repository() { RepositoryId = 3, CreatedBy = 1, BaseRepositoryId = 1, IsActive = true };

            this.repositoryService = new Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes.StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = id => { throw new Exception("Cannot get Repository"); }
            };

            RepositoryController repositoryController = CreateRequest(HttpMethod.Get);
           
            // Perform
            var response = repositoryController.GetRepository(fakeRepository.RepositoryId);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsTrue(result.Message.Contains("Cannot get Repository"), "Result is not as expected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_Repository_By_Id_Error_Inner_Exception()
        {
            Repository fakeRepository = new Repository() { RepositoryId = 3, CreatedBy = 1, BaseRepositoryId = 1, IsActive = true };

            this.repositoryService = new Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes.StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = id => { throw new Exception("", new Exception("Some Inner Exception")); }
            };

            RepositoryController repositoryController = CreateRequest(HttpMethod.Get);

            // Perform
            var response = repositoryController.GetRepository(fakeRepository.RepositoryId);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsTrue(result.Message.Contains("Some Inner Exception"), "Result is not as expected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_Repository_By_Id_When_Invalid_Repository_Id()
        {
            Repository fakeRepository = new Repository() { RepositoryId = -2 };

            this.repositoryService = new Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes.StubIRepositoryService()
            {
                GetRepositoryByIdInt32 = id => { throw new ArgumentNullException("InvalidRepositoryId"); }
            };

            RepositoryController repositoryController = CreateRequest(HttpMethod.Get);

            // Perform
            var response = repositoryController.GetRepository(fakeRepository.RepositoryId);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.AreEqual(result.Message, "Invalid repository id.", "Result is not as expected");
        }


    }
}
