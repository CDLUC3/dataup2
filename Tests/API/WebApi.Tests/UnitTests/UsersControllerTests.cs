using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.Services.UserService;
using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.Research.DataOnboarding.WebApi.Api;
using Microsoft.Research.DataOnboarding.WebApi.Models;
using Microsoft.Research.DataOnboarding.WebApi.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Http.Routing.Fakes;
using System.Web.Script.Serialization;
using FakeUserService = Microsoft.Research.DataOnboarding.Services.UserService.Fakes;

namespace Microsoft.Research.DataOnboarding.WebApi.Tests.UnitTests
{
    [TestClass]
    public class UsersControllerShould
    {
        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_User_Data_For_Valid_NameIdentifier()
        {
            // Prepare
            IIdentity i = new System.Security.Principal.Fakes.StubIIdentity()
            {
                IsAuthenticatedGet = () => { return true; }
            };
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, "s0Me1De9Tf!Er$tRing"));
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(i, claims, string.Empty, ClaimTypes.NameIdentifier, string.Empty);
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity as IIdentity);
            User fakeUser = new User()
                {
                    UserId = 1,
                    NameIdentifier = "s0Me1De9Tf!Er$tRing",
                    FirstName = "SomeFirstName",
                    MiddleName = "SomeMiddleName",
                    LastName = "SomeLastName",
                    IdentityProvider = "Windows Live",
                    Organization = "SomeOrganization",
                    EmailId = "someemail@someorganization.com",
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    IsActive = true,
                    UserRoles = {
                                    new UserRole 
                                    {
                                        RoleId=2, 
                                        UserId=1, 
                                        Role = new Role(){ Name = "User" }
                                    }
                                }
                };
            IUserService userService = new Microsoft.Research.DataOnboarding.Services.UserService.Fakes.StubIUserService()
            {
                GetUserWithRolesByNameIdentifierString = (nameIdentifier) => { return fakeUser;}
            };
            System.Threading.Thread.CurrentPrincipal = principal;
            UsersController userController = new UsersController(userService);
            userController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
            userController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

            // Perform
            var message = userController.GetUsersByNameIdentifier();

            // Assert
            Assert.AreEqual(message.StatusCode, HttpStatusCode.OK);
            var result = message.Content.ReadAsAsync<User>().Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.NameIdentifier, fakeUser.NameIdentifier);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Return_Error_Response_If_UserService_Is_Null_On_Getting_User_Date()
        {
            // Prepare
            IIdentity i = new System.Security.Principal.Fakes.StubIIdentity()
            {
                IsAuthenticatedGet = () => { return true; }
            };

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, "s0Me1De9Tf!Er$tRing"));
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(i, claims, string.Empty, ClaimTypes.NameIdentifier, string.Empty);
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity as IIdentity);
            IUserService userService = null;
            System.Threading.Thread.CurrentPrincipal = principal;

            UsersController userController = new UsersController(userService);
            userController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
            userController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

            // Perform
            var message = userController.GetUsersByNameIdentifier();

            // Assert
            Assert.AreEqual(message.StatusCode, HttpStatusCode.InternalServerError);
            var result = message.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Message, MessageStrings.User_Service_Is_Null);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Return_Error_If_User_Not_Found()
        {
            // Prepare
            IIdentity i = new System.Security.Principal.Fakes.StubIIdentity()
            {
                IsAuthenticatedGet = () => { return true; }
            };

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, "s0Me1De9Tf!Er$tRing"));
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(i, claims, string.Empty, ClaimTypes.NameIdentifier, string.Empty);
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity as IIdentity);
            IUserService userService = new FakeUserService.StubIUserService()
                {
                    GetUserWithRolesByNameIdentifierString = (nameIdentifier) => { throw new UserNotFoundException(); }
                };
            System.Threading.Thread.CurrentPrincipal = principal;
            UsersController userController = new UsersController(userService);
            userController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
            userController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

            // Perform
            var message = userController.GetUsersByNameIdentifier();

            // Assert
            Assert.AreEqual(message.StatusCode, HttpStatusCode.NotFound);
            var result = message.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Message, MessageStrings.User_Not_Found);

        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Allow_Registration_Of_Valid_User()
        {
            // Prepare
            IIdentity i = new System.Security.Principal.Fakes.StubIIdentity()
            {
                IsAuthenticatedGet = () => { return true; }
            };
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, "s0Me1De9Tf!Er$tRing"));
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(i, claims, string.Empty, ClaimTypes.NameIdentifier, string.Empty);
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity as IIdentity);
            User fakeUser = new User()
            {
                UserId = 1,
                NameIdentifier = "s0Me1De9Tf!Er$tRing",
                FirstName = "SomeFirstName",
                MiddleName = "SomeMiddleName",
                LastName = "SomeLastName",
                IdentityProvider = "Windows Live",
                Organization = "SomeOrganization",
                EmailId = "someemail@someorganization.com",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                IsActive = true,
                UserRoles = {
                                    new UserRole 
                                    {
                                        RoleId=2, 
                                        UserId=1, 
                                        Role = new Role(){ Name = "User" }
                                    }
                                }
            };
            IUserService userService = new Microsoft.Research.DataOnboarding.Services.UserService.Fakes.StubIUserService()
            {
                RegisterUserUser = (user) => { return fakeUser; }
            };
            System.Threading.Thread.CurrentPrincipal = principal;
            
            // The localhost url below is provided to allow UrlHelper to 
            // generate return url. It does not add any hosting dependency.
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/users");
            UsersController userController = new UsersController(userService);
            userController.Request = request;
            userController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();
            UserInformation userToRegister = new UserInformation()
            {
                FirstName = "SomeFirstName",
                MiddleName = "SomeMiddleName",
                LastName = "SomeLastName",
                Organization = "SomeOrganization",
                EmailId = "someemail@someorganization.com",
            };

            HttpResponseMessage response;
            using (ShimsContext.Create())
            {
                ShimUrlHelper.AllInstances.LinkStringObject = (urlHelper, routeName, routeValues) => "http://www.abc.com";

                // Perform
                response = userController.PostUsers(userToRegister);
            }

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            var userinfo = response.Content.ReadAsAsync<UserInformation>().Result;
            Assert.IsNotNull(userinfo);
            Assert.AreEqual(userinfo.EmailId, fakeUser.EmailId);
            Assert.IsTrue(userinfo.Roles.Count == 1);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Return_Error_For_Invalid_User_Data_On_Registration()
        {
            // Prepare
            IIdentity i = new System.Security.Principal.Fakes.StubIIdentity()
            {
                IsAuthenticatedGet = () => { return true; }
            };
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, "s0Me1De9Tf!Er$tRing"));
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(i, claims, string.Empty, ClaimTypes.NameIdentifier, string.Empty);
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity as IIdentity);
            IUserService userService = new Microsoft.Research.DataOnboarding.Services.UserService.Fakes.StubIUserService();
            System.Threading.Thread.CurrentPrincipal = principal;
            UserInformation userToRegister = null;            
            UsersController userController = new UsersController(userService);
            userController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
            userController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

            // Perform
            var response = userController.PostUsers(userToRegister);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Message, MessageStrings.Invalid_User_Data);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Return_Error_If_UserService_Is_Null_On_Registration()
        {
            // Prepare
            IIdentity i = new System.Security.Principal.Fakes.StubIIdentity()
            {
                IsAuthenticatedGet = () => { return true; }
            };
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, "s0Me1De9Tf!Er$tRing"));
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(i, claims, string.Empty, ClaimTypes.NameIdentifier, string.Empty);
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity as IIdentity);
            IUserService userService = null;
            System.Threading.Thread.CurrentPrincipal = principal;
            UsersController userController = new UsersController(userService);
            userController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
            userController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

            // Perform
            var response = userController.PostUsers(new UserInformation());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError);
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Message, MessageStrings.User_Service_Is_Null);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Return_Error_On_Registering_An_Existing_User()
        {
            // Prepare
            IIdentity i = new System.Security.Principal.Fakes.StubIIdentity()
            {
                IsAuthenticatedGet = () => { return true; }
            };
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, "s0Me1De9Tf!Er$tRing"));
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(i, claims, string.Empty, ClaimTypes.NameIdentifier, string.Empty);
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity as IIdentity);
            IUserService userService = new FakeUserService.StubIUserService()
                {
                    RegisterUserUser = (user) => { throw new UserAlreadyExistsException(); }
                };
            System.Threading.Thread.CurrentPrincipal = principal;
            UsersController userController = new UsersController(userService);
            userController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
            userController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

            // Perform
            var response = userController.PostUsers(new UserInformation());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Message, MessageStrings.User_Already_Exists);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Return_Error_If_Update_Fails_On_Registering_New_User()
        {
            // Prepare
            IIdentity i = new System.Security.Principal.Fakes.StubIIdentity()
            {
                IsAuthenticatedGet = () => { return true; }
            };
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, "s0Me1De9Tf!Er$tRing"));
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(i, claims, string.Empty, ClaimTypes.NameIdentifier, string.Empty);
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity as IIdentity);
            IUserService userService = new FakeUserService.StubIUserService()
            {
                RegisterUserUser = (user) => { throw new UserDataUpdateException(); }
            };
            System.Threading.Thread.CurrentPrincipal = principal;
            UsersController userController = new UsersController(userService);
            userController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
            userController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

            // Perform
            var response = userController.PostUsers(new UserInformation());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError);
            var result = response.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Message, string.Concat(MessageStrings.User_Data_Update_Error_Message + MessageStrings.Contact_Support));
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_User_By_Name_Identifier_Error_In_Argument()
        {
            // Prepare
            User fakeUser = new User()
            {
                UserId = 1,
                NameIdentifier = "s0Me1De9Tf!Er$tRing",
                FirstName = "SomeFirstName",
                MiddleName = "SomeMiddleName",
                LastName = "SomeLastName",
                IdentityProvider = "Windows Live",
                Organization = "SomeOrganization",
                EmailId = "someemail@someorganization.com",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                IsActive = true,
                UserRoles = {
                                    new UserRole 
                                    {
                                        RoleId=2, 
                                        UserId=1, 
                                        Role = new Role(){ Name = "User" }
                                    }
                                }
            };
            IUserService userService = new Microsoft.Research.DataOnboarding.Services.UserService.Fakes.StubIUserService()
            {
                GetUserWithRolesByNameIdentifierString = (nameIdentifier) => { throw new ArgumentException("", "NameIdentifier"); }
            };

            UsersController userController = new UsersController(userService);
            userController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
            userController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

            // Perform
            var message = userController.GetUsersByNameIdentifier();

            // Assert
            Assert.AreEqual(message.StatusCode, HttpStatusCode.BadRequest);
            var result = message.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Message, string.Format(CultureInfo.CurrentCulture, MessageStrings.Argument_Error_Message_Template, "NameIdentifier"), "Expected and Actual Results are not Same");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Post_User_Invalid_Model_State()
        {
            // Prepare
            User fakeUser = new User()
            {
                UserId = 1,
                NameIdentifier = "s0Me1De9Tf!Er$tRing",
                FirstName = "SomeFirstName",
                MiddleName = "SomeMiddleName",
            };

            IUserService userService = new Microsoft.Research.DataOnboarding.Services.UserService.Fakes.StubIUserService();


            UsersController userController = new UsersController(userService);
            userController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
            userController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();
            userController.ModelState.AddModelError("Invalid", "Invalid Model State");
            // Perform
            var message = userController.PostUsers(new UserInformation());

            // Assert
            Assert.AreEqual(message.StatusCode, HttpStatusCode.BadRequest, "Status code is not as Expected");
            var result = message.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is not as Expected");

        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Post_User_Internal_Server_Error_On_Posting_User()
        {
            // Prepare
            User fakeUser = new User()
            {
                UserId = 1,
                NameIdentifier = "s0Me1De9Tf!Er$tRing",
                FirstName = "SomeFirstName",
                MiddleName = "SomeMiddleName",
            };

            IUserService userService = new Microsoft.Research.DataOnboarding.Services.UserService.Fakes.StubIUserService()
            {
                RegisterUserUser = user => { throw new ArgumentNullException("InternalServerError"); }
            };

            UsersController userController = new UsersController(userService);
            userController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
            userController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

            // Perform
            var message = userController.PostUsers(new UserInformation());

            // Assert
            Assert.AreEqual(message.StatusCode, HttpStatusCode.InternalServerError, "Status code is not as Expected");
            var result = message.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result, "Result is not as Expected");

        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Post_User_Error_Argument_Exception()
        {
            // Prepare
            User fakeUser = new User()
            {
                UserId = 1,
                NameIdentifier = "s0Me1De9Tf!Er$tRing",
                FirstName = "SomeFirstName",
                MiddleName = "SomeMiddleName",
            };

            IUserService userService = new Microsoft.Research.DataOnboarding.Services.UserService.Fakes.StubIUserService()
            {
                RegisterUserUser = user => { throw new ArgumentException("", "ArgumentException"); }
            };

            UsersController userController = new UsersController(userService);
            userController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
            userController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();

            // Perform
            var message = userController.PostUsers(new UserInformation());

            // Assert
            Assert.AreEqual(message.StatusCode, HttpStatusCode.BadRequest, "Status code is not as Expected");
            var result = message.Content.ReadAsAsync<HttpError>().Result;
            Assert.IsNotNull(result.Message, string.Format(CultureInfo.CurrentCulture, MessageStrings.Argument_Error_Message_Template, "ArgumentException"), "Expected and Actual results are not same");

        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Should_Allow_Serialization_Of_User_Objet_To_JSON()
        {
            UserInformation fakeUser = new UserInformation()
            {
                UserId = 1,
                NameIdentifier = "s0Me1De9Tf!Er$tRing",
                FirstName = "SomeFirstName",
                MiddleName = "SomeMiddleName",
                LastName = "SomeLastName",
                IdentityProvider = "Windows Live",
                Organization = "SomeOrganization",
                EmailId = "someemail@someorganization.com",
                IsActive = true
            };
            fakeUser.Roles = new List<string>();
            fakeUser.Roles.Add("User");

            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            jsonSerializer.MaxJsonLength = int.MaxValue;
            string jsonUserString = jsonSerializer.Serialize(fakeUser);

            var jsonFormatter = new JsonMediaTypeFormatter();
            string jsonFormatterOutput = MediaFormatterTestHelper.Serialize<UserInformation>(jsonFormatter, fakeUser);

            Assert.AreEqual(jsonUserString, jsonFormatterOutput);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Should_Deserialize_User_Data_From_JSON()
        {
            string jsonData = "{\"EmailId\":\"seqiumtest@outlook.com\",\"FirstName\":\"sequim\",\"IdentityProvider\":null,\"IsActive\":false,\"LastName\":\"tester\",\"MiddleName\":\"\",\"NameIdentifier\":\"\",\"Organization\":\"ESSI\",\"Roles\":[],\"UserId\":0}";
            var jsonFormatter = new JsonMediaTypeFormatter();
            UserInformation user = MediaFormatterTestHelper.Deserialize<UserInformation>(jsonFormatter, jsonData);
            Assert.IsNotNull(user);
            Assert.AreEqual(user.EmailId, "seqiumtest@outlook.com");
            Assert.AreEqual(user.FirstName, "sequim");
        }

        
    }
}
