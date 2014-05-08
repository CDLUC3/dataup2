using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.Research.DataOnboarding.WebApi.Api;
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
    public class RepositoryTypesControllerTest
    {
        IRepositoryService repositoryService = null;

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_All_Available_RepositoryTypes_Success()
        {
            List<BaseRepository> lstModel = new List<BaseRepository>()
            {
                new BaseRepository(){ Name="Merritt", BaseRepositoryId=1},
                new BaseRepository(){ Name="Sky", BaseRepositoryId=2}               
            };

            this.repositoryService = new Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes.StubIRepositoryService()
            {
                RetrieveRepositoryTypes = () => { return lstModel; }
            };

            RepositoryTypesController repositoryTypeController = CreateRequest(HttpMethod.Get);

            // Perform
            var response = repositoryTypeController.Get();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Status Code is not as Expected");
            var result = response.Content.ReadAsAsync<List<BaseRepository>>().Result;

            Assert.IsTrue(result.Count >= 1, "There is no Repository type in the List");
            Assert.AreEqual(result.Count, lstModel.Count, "Expected and Actula Repository Counts are not equal");

            Assert.AreEqual(result[0].BaseRepositoryId, 1);
            Assert.AreEqual(result[0].Name, "Merritt");

            Assert.AreEqual(result[1].BaseRepositoryId, 2);
            Assert.AreEqual(result[1].Name, "Sky");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_All_Available_Repository_Types_When_No_Repository_Type_Available()
        {
            this.repositoryService = new Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes.StubIRepositoryService()
            {
                RetrieveRepositoriesBoolean = (includeAdmin) => { return null; }
            };

            RepositoryTypesController repositoryTypeController = CreateRequest(HttpMethod.Get);
            // Perform
            var response = repositoryTypeController.Get();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            var result = response.Content.ReadAsAsync<List<Repository>>().Result;
            Assert.IsNull(result, "Result is not null");

        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_All_Available_Repository_types_When_Repository_Service_Is_Not_Available()
        {
            this.repositoryService = null;

            RepositoryTypesController repositoryTypeController = CreateRequest(HttpMethod.Get);
            // Perform
            var response = repositoryTypeController.Get();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is null");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_All_Available_Repository_Types_Invalid_Model_State()
        {
            this.repositoryService = new Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes.StubIRepositoryService();

            RepositoryTypesController repositoryTypeController = CreateRequest(HttpMethod.Get);
            repositoryTypeController.ModelState.AddModelError("Invalid", "Invlaid Model State");

            // Perform
            var response = repositoryTypeController.Get();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is null");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_All_Available_Repository_Types_Error_Server_Exeption()
        {
            this.repositoryService = new Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes.StubIRepositoryService()
            {
                RetrieveRepositoryTypes = () => { throw new Exception("Cannot Retrieve Repositiory"); }
            };

            RepositoryTypesController repositoryTypeController = CreateRequest(HttpMethod.Get);

            // Perform
            var response = repositoryTypeController.Get();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsTrue(result.Message.Contains("Cannot Retrieve Repositiory"), "Result is as Expected");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_All_Available_Repository_Types_Error_Inner_Exeption()
        {
            this.repositoryService = new Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes.StubIRepositoryService()
            {
                RetrieveRepositoryTypes = () => { throw new Exception("", new Exception("Some Inner Exception")); }
            };

            RepositoryTypesController repositoryTypeController = CreateRequest(HttpMethod.Get);

            // Perform
            var response = repositoryTypeController.Get();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError, "Expexted and actual status are not equal");
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsTrue(result.Message.Contains("Some Inner Exception"), "Result is as Expected");
        }

        #region private methods

        private RepositoryTypesController CreateRequest(HttpMethod Method)
        {
            RepositoryTypesController repositoryTypesController = new RepositoryTypesController(repositoryService);
            repositoryTypesController.Request = new HttpRequestMessage(Method, string.Empty);
            repositoryTypesController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();
            return repositoryTypesController;
        }

        #endregion
    }
}
