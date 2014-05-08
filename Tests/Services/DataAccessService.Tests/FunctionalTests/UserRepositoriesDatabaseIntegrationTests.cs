// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DataOnboarding.DataAccessService.Providers.EntityFramework;
using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.DataAccessService;

namespace Microsoft.Research.DataOnboarding.DataAccessService.Tests.FunctionalTests
{
    [TestClass]
    public class UserRepositoriesDatabaseIntegrationTests
    {
        private static readonly string UserRoleName = "User";
        private static readonly string AdministratorRoleName = "Administrator";
        private static string unitTestDatabaseFilePath;
        private static string unitTestDatabaseConnectionString;
        private IDataOnboardingContext testDBContext;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            unitTestDatabaseFilePath = string.Join("\\", context.DeploymentDirectory, "dataonboarding_userrepositorytests.sdf");
            unitTestDatabaseConnectionString = string.Concat("Data Source = ", unitTestDatabaseFilePath);
        }
        
        [TestInitialize]
        public void TestInitialize()
        {
            using (var context = new DataOnboardingContext(unitTestDatabaseConnectionString))
            {
                Helper.CreateSqlCeDataBaseFromEntityFrameworkDbContext(context, unitTestDatabaseFilePath);
            }
            testDBContext = new DataOnboardingContext(unitTestDatabaseConnectionString);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Add_New_User()
        {
            User newUser = new User()
            {
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
            };

            IUserRepository repository = new UserRepository(testDBContext as IUnitOfWork);
            User addedUser = repository.AddUser(newUser);

            // Check if a non-null user is returned from the call
            Assert.IsNotNull(addedUser);

            // Check id the DbContext cached the user object in its local User collection
            Assert.IsTrue(testDBContext.Users.Local.Count == 1);

            // Check if the locally cached user object has the same data as the new User that was added above
            Assert.AreEqual(testDBContext.Users.Local[0].NameIdentifier, newUser.NameIdentifier);

            // Check if the state of the added user is set to Added
            Assert.AreEqual(testDBContext.GetEntityState(addedUser), EntityState.Added);

            testDBContext.Commit();
            User retrieved = testDBContext.Users.Find(1);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Update_Existing_User()
        {
            // Prepare : 
            // 1. Create a new user 
            // 2. Fetch the added user
            // 3. Update user data
            User newUser = new User()
            {
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
            AddUser(newUser);
            User userToUpdate = testDBContext.Users.Find(1);
            userToUpdate.EmailId = "somethingelseemail@somethingelseorganization.com";
            userToUpdate.Organization = "somethingelseorganization";

            // Perform update
            IUserRepository repository = new UserRepository(testDBContext as IUnitOfWork);
            User updatedUser = repository.UpdateUser(userToUpdate);

            // Assert if the updated user object is valid
            Assert.IsNotNull(updatedUser);

            // Asset that the updated user is added to the tracked object in the context
            Assert.IsTrue(testDBContext.Users.Local.Count == 1);

            // Assert email id before and after update are same
            Assert.AreEqual(testDBContext.Users.Local[0].EmailId, userToUpdate.EmailId);

            // Assert organization before and after is the same
            Assert.AreEqual(testDBContext.Users.Local[0].Organization, userToUpdate.Organization);

            // Assert that the locally tracked user object to update has the modified entity state
            Assert.AreEqual(testDBContext.GetEntityState(updatedUser), EntityState.Modified);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_Existing_User_By_NameIdentifier()
        {
            AddDefaultRoles();
            User newUser = new User()
            {
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
            AddUser(newUser);
            UserRole userRole = new UserRole()
            {
                UserId = 1,
                RoleId = 1
            };
            testDBContext.UserRoles.Add(userRole);
            testDBContext.Commit();


            // Perform get
            IUserRepository repository = new UserRepository(testDBContext as IUnitOfWork);
            User retrievedUser = repository.GetUserByNameIdentifier(newUser.NameIdentifier);

            // Check if a valid user is returned
            Assert.IsNotNull(retrievedUser);

            // Check if the name identifier matches
            Assert.AreEqual(newUser.NameIdentifier, retrievedUser.NameIdentifier);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Return_Null_User_Value_For_Non_Existing_NameIdentifier()
        {
            // Perform get
            IUserRepository repository = new UserRepository(testDBContext as IUnitOfWork);
            User retrievedUser = repository.GetUserByNameIdentifier("some_non_existing_identifier");

            // Check if a valid user is returned
            Assert.IsNull(retrievedUser);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Add_User_Role()
        {
            // Prepare
            AddDefaultRoles();

            User newUser = new User()
            {
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
            AddUser(newUser);
            User addedUser = testDBContext.Users.Find(1);
            UserRole userRole = new UserRole()
            {
                UserId = addedUser.UserId,
                RoleId = GetUserRoleId()
            };

            // Perform
            IUserRepository repository = new UserRepository(testDBContext as IUnitOfWork);
            UserRole addedUserRole = repository.AddUserRole(userRole);

            // Check if a valid user role is returned
            Assert.IsNotNull(addedUserRole);

            // Check if the count of tracked objects is 1
            Assert.IsTrue(testDBContext.UserRoles.Local.Count == 1);

            // Check is the entity state of the added object is set to Added
            Assert.AreEqual(testDBContext.GetEntityState(addedUserRole), EntityState.Added);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_UserRoles_For_Valid_UserId()
        {
            // Prepare
            AddDefaultRoles();
            User newUser = new User()
            {
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
            AddUserWithDefaultRoles(newUser);
            User addedUser = testDBContext.Users.Find(1);

            // Perform
            IUserRepository repository = new UserRepository(testDBContext as IUnitOfWork);
            IEnumerable<UserRole> userRoles = repository.GetUserRolesByUserId(addedUser.UserId);

            // Check if valid user roles collection is returned
            Assert.IsNotNull(userRoles);

            // Check if the count of items in user role collection is 2, since the preparation code
            // added 2 roles to user
            Assert.AreEqual(userRoles.Count(), 2);

            // Check if the administrator role exists
            Assert.AreEqual(userRoles.Count<UserRole>(r => r.RoleId.Equals(GetAdministratorRoleId())), 1);

            // Check if the user role exists
            Assert.AreEqual(userRoles.Count<UserRole>(r => r.RoleId.Equals(GetUserRoleId())), 1);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Return_Empty_UserRoles_For_InValid_UserId()
        {
            // Perform
            IUserRepository repository = new UserRepository(testDBContext as IUnitOfWork);
            IEnumerable<UserRole> userRoles = repository.GetUserRolesByUserId(-1); // Invalid user id passed

            // Check is the count is zero
            Assert.AreEqual(userRoles.Count(), 0);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Add_User_Attribute()
        {
            // Prepare
            User newUser = new User()
            {
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
            AddUser(newUser);
            User addedUser = testDBContext.Users.Find(1);
            UserAttribute userAttribute = new UserAttribute()
            {
                UserId = addedUser.UserId,
                Key = "Key1",
                Value = "Value1"
            };

            // Perform
            IUserRepository repository = new UserRepository(testDBContext as IUnitOfWork);
            UserAttribute addedUserAttribute = repository.AddUserAttribute(userAttribute); // Invalid user id passed

            // Check is a valid user attribute is returned
            Assert.IsNotNull(addedUserAttribute);

            // Check if the count of tracked objects in the data context is 1
            Assert.IsTrue(testDBContext.UserAttributes.Local.Count == 1);

            // Check if the tracked object is the same one as the added user attribute
            Assert.AreEqual(testDBContext.UserAttributes.Local[0].Key, userAttribute.Key);

            // Check the entity state
            Assert.AreEqual(testDBContext.GetEntityState(addedUserAttribute), EntityState.Added);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_User_Attributes_For_Valid_User()
        {
            // Prepare
            UserAttribute userAttribute1 = new UserAttribute()
            {
                Key = "Key1",
                Value = "Value1"
            };
            UserAttribute userAttribute2 = new UserAttribute()
            {
                Key = "Key2",
                Value = "Value2"
            };
            User newUser = new User()
            {
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
                UserAttributes = new List<UserAttribute>() { userAttribute1, userAttribute2 }
            };
            AddUser(newUser);
            User addedUser = testDBContext.Users.Find(1);

            // Perform
            IUserRepository repository = new UserRepository(testDBContext);
            IEnumerable<UserAttribute> attributes = repository.GetUserAttributesByUserId(addedUser.UserId);

            // Check if a valid collection of user attributes is returned 
            Assert.IsNotNull(attributes);

            // Check if the count of attributes is 2, since we added 2 attributes in the preparation code
            Assert.AreEqual(attributes.Count(), 2);

            // Check if attribute 1 is returned
            Assert.AreEqual(attributes.Count<UserAttribute>(ua => ua.Key.Equals(userAttribute1.Key)), 1);

            // Check is attribute 2 is returned
            Assert.AreEqual(attributes.Count<UserAttribute>(ua => ua.Key.Equals(userAttribute2.Key)), 1);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Return_Empty_UserAttributes_For_InValid_UserId()
        {
            // Perform
            IUserRepository repository = new UserRepository(testDBContext as IUnitOfWork);
            IEnumerable<UserAttribute> userAttributes = repository.GetUserAttributesByUserId(-1); // Invalid user id passed

            // Check is the count is zero
            Assert.AreEqual(userAttributes.Count(), 0);
        }

        #region Private Methods
        private void AddUser(User user)
        {
            IUserRepository repository = new UserRepository(testDBContext as IUnitOfWork);
            User addedUser = repository.AddUser(user);
            testDBContext.Commit();
        }

        private int GetAdministratorRoleId()
        {
            Role adminRole = testDBContext.Roles.Where(r => r.Name.Equals(AdministratorRoleName)).FirstOrDefault() as Role;
            return adminRole.RoleId;
        }

        private int GetUserRoleId()
        {
            Role userRole = testDBContext.Roles.Where(r => r.Name.Equals(UserRoleName)).FirstOrDefault() as Role;
            return userRole.RoleId;
        }

        private void AddUserWithDefaultRoles(User user)
        {
            Role addedUserRole = testDBContext.Roles.Where(r => r.Name.Equals(UserRoleName)).FirstOrDefault() as Role;
            Role addedAdminRole = testDBContext.Roles.Where(r => r.Name.Equals(AdministratorRoleName)).FirstOrDefault() as Role;

            UserRole userUserRole = new UserRole()
            {
                RoleId = addedUserRole.RoleId
            };
            UserRole adminUserRole = new UserRole()
            {
                RoleId = addedAdminRole.RoleId
            };

            user.UserRoles = new List<UserRole>() { userUserRole, adminUserRole };
            AddUser(user);
        }

        private void AddDefaultRoles()
        {
            Role userRole = new Role()
            {
                Name = UserRoleName
            };
            Role adminRole = new Role()
            {
                Name = AdministratorRoleName
            };

            testDBContext.Roles.Add(userRole);
            testDBContext.Roles.Add(adminRole);
            testDBContext.Commit();
        }
        #endregion
    }
}
