// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.Research.DataOnboarding.Services.UserService;
using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.DomainModel;
using Fakes = Microsoft.Research.DataOnboarding.DataAccessService.Fakes;

namespace Microsoft.Research.DataOnboarding.UserService.Tests.UnitTests
{
    [TestClass]
    public class UserServiceShould
    {
        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Allow_New_User_Registration()
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
            IUserRepository userRepository =
                new Fakes.StubIUserRepository()
                    {
                        AddUserUser = (user) => { return fakeUser; }
                    };
            IUnitOfWork unitOfWork =
                new Fakes.StubIUnitOfWork()
                    {
                        Commit = () => { return; }
                    };
            User userToRegister = new User()
            {
                NameIdentifier = "s0Me1De9Tf!Er$tRing",
                FirstName = "SomeFirstName",
                MiddleName = "SomeMiddleName",
                LastName = "SomeLastName",
                IdentityProvider = "Windows Live",
                Organization = "SomeOrganization",
                EmailId = "someemail@someorganization.com"
            };

            // Act
            IUserService userService = new UserServiceProvider(userRepository, unitOfWork);
            User registeredUser = userService.RegisterUser(userToRegister);

            // Check if valid user is returned
            Assert.IsNotNull(registeredUser);

            // Check is a valid user id is associated with the registered user
            Assert.IsTrue(registeredUser.UserId > 0);

            // Check if inbound user data is same as the output
            Assert.AreEqual(registeredUser.NameIdentifier, userToRegister.NameIdentifier);

            Assert.AreEqual(registeredUser.UserRoles.Count, 1);

            Assert.IsNotNull(registeredUser.UserRoles.First().Role);

            Assert.AreEqual(registeredUser.UserRoles.First().Role.Name, "User");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(UserAlreadyExistsException))]
        public void Throw_Exception_On_Registering_Duplicate_NameIdentifier()
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
                IsActive = true
            };
            IUserRepository userRepository =
                new Microsoft.Research.DataOnboarding.DataAccessService.Fakes.StubIUserRepository()
                {
                    GetUserByNameIdentifierString = (nameIdentifier) => { return fakeUser; }
                };
            IUnitOfWork unitOfWork =
                new Fakes.StubIUnitOfWork()
                {
                    Commit = () => { return; }
                };
            User userToRegister = new User()
                {
                    NameIdentifier = "s0Me1De9Tf!Er$tRing",
                    FirstName = "SomeFirstName",
                    MiddleName = "SomeMiddleName",
                    LastName = "SomeLastName",
                    IdentityProvider = "Windows Live",
                    Organization = "SomeOrganization",
                    EmailId = "someemail@someorganization.com"
                };

            // Act
            IUserService userService = new UserServiceProvider(userRepository, unitOfWork);
            try
            {
                User registeredUser = userService.RegisterUser(userToRegister);
            }
            catch (UserAlreadyExistsException uaee)
            {
                Assert.AreEqual(uaee.NameIdentifier, userToRegister.NameIdentifier);
                throw;
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throw_Exception_For_Null_Input_Parameter()
        {
            // Prepare
            IUserRepository userRepository =
                new Microsoft.Research.DataOnboarding.DataAccessService.Fakes.StubIUserRepository();
            IUnitOfWork unitOfWork = new Fakes.StubIUnitOfWork();
            User userToRegister = default(User);

            // Act
            IUserService userService = new UserServiceProvider(userRepository, unitOfWork);
            User registeredUser = userService.RegisterUser(userToRegister);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(ArgumentException))]
        public void Throw_Exception_if_NameIdentifier_Is_Null_Empty_Or_Whitespace()
        {
            // Prepare
            IUserRepository userRepository =
                new Microsoft.Research.DataOnboarding.DataAccessService.Fakes.StubIUserRepository();
            IUnitOfWork unitOfWork = new Fakes.StubIUnitOfWork();
            User userToRegister = new User()
                {
                    NameIdentifier = string.Empty,        // Invalid name identifier
                    FirstName = "SomeFirstName",
                    MiddleName = "SomeMiddleName",
                    LastName = "SomeLastName",
                    IdentityProvider = "Windows Live",
                    Organization = "SomeOrganization",
                    EmailId = "someemail@someorganization.com"
                };

            // Act
            IUserService userService = new UserServiceProvider(userRepository, unitOfWork);
            User registeredUser = userService.RegisterUser(userToRegister);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(UserDataUpdateException))]
        public void Throw_Exception_When_New_User_Registration_Fails()
        {
            // Prepare
            IUnitOfWork unitOfWork = new Fakes.StubIUnitOfWork();
            IUserRepository userRepository =
                new Fakes.StubIUserRepository()
                {
                    AddUserUser = (user) => { throw new UnitOfWorkCommitException("Some data update issue"); }
                };
            User userToRegister = new User()
            {
                NameIdentifier = "s0Me1De9Tf!Er$tRing",
                FirstName = "SomeFirstName",
                MiddleName = "SomeMiddleName",
                LastName = "SomeLastName",
                IdentityProvider = "Windows Live",
                Organization = "SomeOrganization",
                EmailId = "someemail@someorganization.com"
            };

            // Perform
            IUserService userService = new UserServiceProvider(userRepository, unitOfWork);
            userService.RegisterUser(userToRegister);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_User_With_Roles_For_Valid_NameIdentifier()
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
            IUnitOfWork unitOfWork =
                new Fakes.StubIUnitOfWork();
            IUserRepository userRepository =
                new Fakes.StubIUserRepository()
                {
                    GetUserByNameIdentifierString = (nameIdentifier) => { return fakeUser; }
                };

            // Perform
            IUserService userService = new UserServiceProvider(userRepository, unitOfWork);
            User retrievedUser = userService.GetUserWithRolesByNameIdentifier("s0Me1De9Tf!Er$tRing");

            // Assert
            Assert.IsNotNull(retrievedUser);
            Assert.AreEqual(retrievedUser.NameIdentifier, fakeUser.NameIdentifier);
            Assert.IsTrue(retrievedUser.UserRoles.Count > 0);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(ArgumentException))]
        public void Throw_Exception_If_NameIdentifier_Is_Null_Empty_Or_Whitespace()
        {
            // Prepare
            IUnitOfWork unitOfWork = new Fakes.StubIUnitOfWork();
            IUserRepository userRepository = new Fakes.StubIUserRepository();

            // Perform
            IUserService userService = new UserServiceProvider(userRepository, unitOfWork);
            userService.GetUserWithRolesByNameIdentifier(" ");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(UserNotFoundException))]
        public void Throw_Exception_If_NameIdentifier_Does_Not_Exist()
        {
            // Prepare
            IUnitOfWork unitOfWork = new Fakes.StubIUnitOfWork();
            IUserRepository userRepository =
                new Fakes.StubIUserRepository()
                {
                    GetUserByNameIdentifierString = (nameIdentifier) => { return null; }
                };

            // Perform
            IUserService userService = new UserServiceProvider(userRepository, unitOfWork);
            userService.GetUserWithRolesByNameIdentifier("somenameidentifier");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Get_User_By_UserId()
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
            IUnitOfWork unitOfWork =
                new Fakes.StubIUnitOfWork();
            IUserRepository userRepository =
                new Fakes.StubIUserRepository()
                {
                    GetUserbyUserIdInt32 = (userId) => { return fakeUser; }
                };

            // Perform
            IUserService userService = new UserServiceProvider(userRepository, unitOfWork);
            User retrievedUser = userService.GetUserById(1);

            // Assert
            Assert.IsNotNull(retrievedUser);
            Assert.AreEqual(retrievedUser.UserId, fakeUser.UserId);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(UserNotFoundException))]
        public void  Throw_Exception_If_User_With_Given_Id_Does_Not_Exist()
        {
            // Prepare
            IUnitOfWork unitOfWork = new Fakes.StubIUnitOfWork();
            IUserRepository userRepository =
                new Fakes.StubIUserRepository()
                {
                    GetUserbyUserIdInt32 = (userId) => { return null; }
                };

            // Perform
            IUserService userService = new UserServiceProvider(userRepository, unitOfWork);
            userService.GetUserById(1);
        }
    }
}
