using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DataOnboarding.DataAccessService.Providers.EntityFramework;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.TestUtilities;
using OnboardingEntityFramework = Microsoft.Research.DataOnboarding.DataAccessService.Providers.EntityFramework;
using System;

namespace Microsoft.Research.DataOnboarding.DataAccessService.Tests.FunctionalTests
{
    [TestClass]
    public class RepositoryDetailsDatabaseIntegrationTests
    {
        private static string unitTestDatabaseFilePath;
        private static string unitTestDatabaseConnectionString;
        private OnboardingEntityFramework.IDataOnboardingContext testDBContext;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            unitTestDatabaseFilePath = string.Join("\\", context.DeploymentDirectory, "dataonboarding_userrepositorytests.sdf");
            unitTestDatabaseConnectionString = string.Concat("Data Source = ", unitTestDatabaseFilePath);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            using (var context = new OnboardingEntityFramework.DataOnboardingContext(unitTestDatabaseConnectionString))
            {
                Helper.CreateSqlCeDataBaseFromEntityFrameworkDbContext(context, unitTestDatabaseFilePath);
            }
            testDBContext = new OnboardingEntityFramework.DataOnboardingContext(unitTestDatabaseConnectionString);

            AddDefaultData();
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_All_Repositories_Test()
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);

            var repositories = repository.RetrieveRepositories().ToList();

            Assert.IsNotNull(repositories);

            Assert.AreEqual(repositories.Count(), 2);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_All_Repository_Types_Test()
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);

            var repositories = repository.RetrieveRepositoryTypes().ToList();

            Assert.IsNotNull(repositories);

            Assert.AreEqual(repositories.Count(), 1);

            Assert.AreEqual(repositories[0].BaseRepositoryId, 1);
            Assert.AreEqual(repositories[0].Name, "Merrit");
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Update_Repository_Test()
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);

            var selRep = repository.GetRepositoryById(1);

            // Updating data
            selRep.AllowedFileTypes = "Updated FileTypes";
            selRep.HttpDeleteUriTemplate = "Updated HttpDeleteUriTemplate";
            selRep.HttpGetUriTemplate = "Updated HttpGetUriTemplate";
            selRep.HttpIdentifierUriTemplate = "Updated HttpIdentifierUriTemplate";
            selRep.HttpPostUriTemplate = "Updated HttpPostUriTemplate";
            selRep.ImpersonatingPassword = "Updated ImpersonatingPassword";
            selRep.ImpersonatingUserName = "Updated ImpersonatingUserName";
            selRep.IsImpersonating = true;
            selRep.IsVisibleToAll = false;
            selRep.Name = "Updated Name";
            selRep.UserAgreement = "Updated UserAgreement";

            repository.UpdateRepository(selRep);

            testDBContext.Commit();

            var updatedRep = repository.GetRepositoryById(1);

            Assert.IsNotNull(updatedRep);

            Assert.AreEqual(updatedRep.AllowedFileTypes, "Updated FileTypes");
            Assert.AreEqual(updatedRep.HttpDeleteUriTemplate, "Updated HttpDeleteUriTemplate");
            Assert.AreEqual(updatedRep.HttpGetUriTemplate, "Updated HttpGetUriTemplate");
            Assert.AreEqual(updatedRep.HttpIdentifierUriTemplate, "Updated HttpIdentifierUriTemplate");
            Assert.AreEqual(updatedRep.HttpPostUriTemplate, "Updated HttpPostUriTemplate");
            Assert.AreEqual(updatedRep.ImpersonatingPassword, "Updated ImpersonatingPassword");
            Assert.AreEqual(updatedRep.ImpersonatingUserName, "Updated ImpersonatingUserName");
            Assert.AreEqual(updatedRep.Name, "Updated Name");
            Assert.AreEqual(updatedRep.UserAgreement, "Updated UserAgreement");

            Assert.IsTrue((bool)updatedRep.IsImpersonating);
            Assert.IsFalse(updatedRep.IsVisibleToAll);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_Repository_By_ID_Empty_Test()
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);

            var selRep = repository.GetRepositoryById(5);

            Assert.IsNull(selRep);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_Repository_By_ID_Test()
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);

            var selRep = repository.GetRepositoryById(1);

            Assert.IsNotNull(selRep);

            Assert.AreEqual(selRep.AllowedFileTypes, "xlsx,nc,csv");
            Assert.AreEqual(selRep.HttpDeleteUriTemplate, "http://google.com");
            Assert.AreEqual(selRep.HttpGetUriTemplate, "http://google.com");
            Assert.AreEqual(selRep.HttpIdentifierUriTemplate, "http://google.com");
            Assert.AreEqual(selRep.HttpPostUriTemplate, "http://google.com");
            Assert.AreEqual(selRep.ImpersonatingPassword, "pwd");
            Assert.AreEqual(selRep.ImpersonatingUserName, "user1");
            Assert.AreEqual(selRep.ModifiedBy, 1);
            Assert.AreEqual(selRep.CreatedBy, 1);
            Assert.AreEqual(selRep.Name, "Repository1");
            Assert.AreEqual(selRep.RepositoryId, 1);
            Assert.AreEqual(selRep.UserAgreement, "Test Agreement1");
            Assert.AreEqual(selRep.BaseRepositoryId, 1);

            Assert.IsTrue((bool)selRep.IsActive);
            Assert.IsTrue((bool)selRep.IsImpersonating);
            Assert.IsTrue(selRep.IsVisibleToAll);

        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_Base_Reopository_Name_Empty_Test()
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);

            var repName = repository.GetBaseRepositoryName(2);

            Assert.IsNotNull(repName);
            Assert.AreEqual(repName, string.Empty);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_Base_Reopository_Name_Test()
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);

            var repName = repository.GetBaseRepositoryName(1);

            Assert.IsNotNull(repName);
            Assert.AreEqual(repName, "Merrit");
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_Repository_By_Name_Empty_Test()
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);

            var selRep = repository.GetRepositoryByName("EmptyRepository");

            Assert.IsNull(selRep);
        }


        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_Repository_By_Name_Test()
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);

            var selRep = repository.GetRepositoryByName("Repository1");

            Assert.AreEqual(selRep.AllowedFileTypes, "xlsx,nc,csv");
            Assert.AreEqual(selRep.HttpDeleteUriTemplate, "http://google.com");
            Assert.AreEqual(selRep.HttpGetUriTemplate, "http://google.com");
            Assert.AreEqual(selRep.HttpIdentifierUriTemplate, "http://google.com");
            Assert.AreEqual(selRep.HttpPostUriTemplate, "http://google.com");
            Assert.AreEqual(selRep.ImpersonatingPassword, "pwd");
            Assert.AreEqual(selRep.ImpersonatingUserName, "user1");
            Assert.AreEqual(selRep.ModifiedBy, 1);
            Assert.AreEqual(selRep.CreatedBy, 1);
            Assert.AreEqual(selRep.Name, "Repository1");
            Assert.AreEqual(selRep.RepositoryId, 1);
            Assert.AreEqual(selRep.UserAgreement, "Test Agreement1");
            Assert.AreEqual(selRep.BaseRepositoryId, 1);

            Assert.IsTrue((bool)selRep.IsActive);
            Assert.IsTrue((bool)selRep.IsImpersonating);
            Assert.IsTrue(selRep.IsVisibleToAll);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throw_Exception_On_Delete_Null_Repository_Test()
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);
            repository.DeleteRepository(8);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Delete_Repository_Test()
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);

            Repository delRep = repository.DeleteRepository(1);

            Assert.AreEqual(delRep.Name, "Repository1");
            Assert.AreEqual(delRep.RepositoryId, 1);

            testDBContext.Commit();
            var repList = repository.RetrieveRepositories();

            Assert.AreEqual(repList.Count(), 1);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Add_Repository_Test()
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);

            Repository repositoryObject = new Repository()
            {
                AllowedFileTypes = "xlsx,nc,csv",
                CreatedBy = 1,
                // Files = null,
                CreatedOn = DateTime.Now,
                HttpDeleteUriTemplate = "http://google.com",
                HttpGetUriTemplate = "http://google.com",
                HttpIdentifierUriTemplate = "http://google.com",
                HttpPostUriTemplate = "http://google.com",
                ImpersonatingPassword = "pwd",
                ImpersonatingUserName = "user1",
                IsActive = true,
                IsImpersonating = true,
                ModifiedBy = 1,
                ModifiedOn = DateTime.Now,
                Name = "AddedRepository",
                RepositoryId = 3,
                UserAgreement = "Test Agreement Added",
                BaseRepositoryId = 1
            };

            AddRepository(repositoryObject);

            var addedRepository = repository.GetRepositoryById(3);

            Assert.AreEqual(addedRepository.AllowedFileTypes, repositoryObject.AllowedFileTypes);

            Assert.AreEqual(addedRepository.CreatedBy, repositoryObject.CreatedBy);

            Assert.AreEqual(addedRepository.CreatedOn, repositoryObject.CreatedOn);

            Assert.AreEqual(addedRepository.HttpDeleteUriTemplate, repositoryObject.HttpDeleteUriTemplate);

            Assert.AreEqual(addedRepository.HttpGetUriTemplate, repositoryObject.HttpGetUriTemplate);

            Assert.AreEqual(addedRepository.HttpIdentifierUriTemplate, repositoryObject.HttpIdentifierUriTemplate);

            Assert.AreEqual(addedRepository.HttpPostUriTemplate, repositoryObject.HttpPostUriTemplate);

            Assert.AreEqual(addedRepository.ImpersonatingPassword, repositoryObject.ImpersonatingPassword);

            Assert.AreEqual(addedRepository.ImpersonatingUserName, repositoryObject.ImpersonatingUserName);

            Assert.AreEqual(addedRepository.IsActive, repositoryObject.IsActive);

            Assert.AreEqual(addedRepository.IsImpersonating, repositoryObject.IsImpersonating);

            Assert.AreEqual(addedRepository.ModifiedBy, repositoryObject.ModifiedBy);

            Assert.AreEqual(addedRepository.ModifiedOn, repositoryObject.ModifiedOn);

            Assert.AreEqual(addedRepository.Name, repositoryObject.Name);

            Assert.AreEqual(addedRepository.RepositoryId, repositoryObject.RepositoryId);

            Assert.AreEqual(addedRepository.UserAgreement, repositoryObject.UserAgreement);
        }

        #region Private Methods

        private void AddDefaultData()
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
                UserAttributes = null
            };

            // User with userid 1
            AddUser(newUser);

            // Adding metadata types
            MetadataType metaType = new MetadataType() { MetadataTypeId = 1, Name = "Text", Status = true };
            MetadataType metaType1 = new MetadataType() { MetadataTypeId = 2, Name = "Numaric", Status = true };
            MetadataType metaType2 = new MetadataType() { MetadataTypeId = 3, Name = "Email", Status = true };

            AddMetaDataType(metaType);
            AddMetaDataType(metaType1);
            AddMetaDataType(metaType2);

            BaseRepository baseRepo = new BaseRepository() { BaseRepositoryId = 1, Name = "Merrit" };

            AddBaseRepository(baseRepo);

            Repository repositoryObject = new Repository()
            {
                AllowedFileTypes = "xlsx,nc,csv",
                CreatedBy = 1,
                /// Files = null,
                CreatedOn = DateTime.Now,
                HttpDeleteUriTemplate = "http://google.com",
                HttpGetUriTemplate = "http://google.com",
                HttpIdentifierUriTemplate = "http://google.com",
                HttpPostUriTemplate = "http://google.com",
                ImpersonatingPassword = "pwd",
                ImpersonatingUserName = "user1",
                IsActive = true,
                IsImpersonating = true,
                ModifiedBy = 1,
                ModifiedOn = DateTime.Now,
                Name = "Repository1",
                RepositoryId = 1,
                UserAgreement = "Test Agreement1",
                BaseRepositoryId = 1,
                IsVisibleToAll = true
            };

            Repository repositoryObject1 = new Repository()
            {
                AllowedFileTypes = "xlsx,csv",
                CreatedBy = 1,
                //Files = null,
                CreatedOn = DateTime.Now,
                HttpDeleteUriTemplate = "http://gmail.com",
                HttpGetUriTemplate = "http://gmail.com",
                HttpIdentifierUriTemplate = "http://gmail.com",
                HttpPostUriTemplate = "http://gmail.com",
                ImpersonatingPassword = "pwd2",
                ImpersonatingUserName = "user2",
                IsActive = true,
                IsImpersonating = true,
                ModifiedBy = 1,
                ModifiedOn = DateTime.Now,
                Name = "Repository2",
                RepositoryId = 2,
                UserAgreement = "Test Agreement1",
                BaseRepositoryId = 1,
                IsVisibleToAll = true
            };

            // Adding 2 new repositories
            AddRepository(repositoryObject);
            AddRepository(repositoryObject1);

            // Adding repository metadata and metadata fields
            RepositoryMetadata repMetadata = new RepositoryMetadata() { CreatedBy = 1, CreatedOn = DateTime.UtcNow, IsActive = true, ModifiedBy = 1, ModifiedOn = DateTime.UtcNow, Name = "Merrit Test metadata", RepositoryId = 1, RepositoryMetadataId = 1, RepositoryMetadataFields = null };

            AddRepositoryMetadata(repMetadata);

            RepositoryMetadataField repMDField = new RepositoryMetadataField() { Description = "Create Name", IsRequired = true, Mapping = "Test Mapping", MetadataTypeId = 1, Name = "Create Name", Order = 1, Range = "", RepositoryMetadataFieldId = 1, RepositoryMetadataId = 1 };
            RepositoryMetadataField repMDField1 = new RepositoryMetadataField() { Description = "Create Phone", IsRequired = true, Mapping = "Test Mapping", MetadataTypeId = 1, Name = "Create Phone", Order = 2, Range = "", RepositoryMetadataFieldId = 2, RepositoryMetadataId = 1 };
            RepositoryMetadataField repMDField2 = new RepositoryMetadataField() { Description = "Create Email", IsRequired = true, Mapping = "Test Mapping", MetadataTypeId = 1, Name = "Create Email", Order = 3, Range = "", RepositoryMetadataFieldId = 3, RepositoryMetadataId = 1 };

            AddRepositoryMetadataFields(repMDField);
            AddRepositoryMetadataFields(repMDField1);
            AddRepositoryMetadataFields(repMDField2);
        }

        private void AddRepository(Repository repo)
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);
            repository.AddRepository(repo);
            testDBContext.Commit();
        }

        private void AddUser(User user)
        {
            IUserRepository repository = new UserRepository(testDBContext);
            User addedUser = repository.AddUser(user);
            testDBContext.Commit();
        }

        private void AddMetaDataType(MetadataType metaDataType)
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);
            testDBContext.MetadataTypes.Add(metaDataType);
            testDBContext.Commit();
        }

        private void AddRepositoryMetadata(RepositoryMetadata repMetadata)
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);
            testDBContext.RepositoryMetadata.Add(repMetadata);
            testDBContext.Commit();
        }
        private void AddRepositoryMetadataFields(RepositoryMetadataField repMetadataField)
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);
            testDBContext.RepositoryMetadataFields.Add(repMetadataField);
            testDBContext.Commit();
        }

        private void AddBaseRepository(BaseRepository repo)
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);
            testDBContext.BaseRepositories.Add(repo);
            testDBContext.Commit();
        }

        #endregion
    }
}
