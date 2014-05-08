using Microsoft.Research.DataOnboarding.DataAccessService.Providers.EntityFramework;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using OnboardingEntityFramework = Microsoft.Research.DataOnboarding.DataAccessService.Providers.EntityFramework;

namespace Microsoft.Research.DataOnboarding.DataAccessService.Tests.FunctionalTests
{
    [TestClass]
    public class FileRepositoryIntegrationTests
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
        public void Get_All_Files_Test()
        {
            IFileRepository repository = new FileRepository(testDBContext);

            int userId = 1;
            string postStatus = FileStatus.Posted.ToString();
            Func<File, bool> filter = file => file.CreatedBy == userId && (file.Status.Equals(postStatus, StringComparison.InvariantCulture) || file.isDeleted == null || file.isDeleted == false);
            var fileList = repository.GetFiles(filter);

            Assert.IsNotNull(fileList);

            Assert.AreEqual(fileList.Count(), 2);

            Assert.AreEqual(fileList.ToList()[0].FileId, 1);
            Assert.AreEqual(fileList.ToList()[1].FileId, 2);

            Assert.AreEqual(fileList.ToList()[0].Name, "Document One");
            Assert.AreEqual(fileList.ToList()[1].Name, "Document Two");
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Save_File_Data()
        {
            IFileRepository repository = new FileRepository(testDBContext);

            var file = repository.GetItem(1, 1);
            file.FileColumns = new List<FileColumn>();
            var newFilecolumn = new FileColumn()
            {
                FileColumnTypeId = 1,
                FileColumnUnitId = 1,
                EntityName = "entityName",
                EntityDescription = "entityDesc",
                Description = "Description"
            };
            file.FileColumns.Add(newFilecolumn);
            testDBContext.Commit();

            var fileColumns = GetFileColumns(file.FileId);

            Assert.AreEqual(1, fileColumns.Count);
            Assert.AreEqual(newFilecolumn.EntityName, fileColumns[0].EntityName);
            Assert.AreEqual(newFilecolumn.EntityDescription, fileColumns[0].EntityDescription);
            Assert.AreEqual(newFilecolumn.Description, fileColumns[0].Description);
            Assert.AreEqual(newFilecolumn.FileColumnUnitId, fileColumns[0].FileColumnUnitId);
            Assert.AreEqual(newFilecolumn.FileColumnTypeId, fileColumns[0].FileColumnTypeId);

            var columnTypeList = GetFileColumnTypes();
            //only one column type should be there, one more dummy row was added

            Assert.AreEqual(1, columnTypeList.Count());
            var columnsUnitsList = GetFileColumnUnits();

            Assert.AreEqual(1, columnsUnitsList.Count());
            testDBContext.Commit();
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Save_File_NullColumnType_DataTest()
        {
            IFileRepository repository = new FileRepository(testDBContext);

            var file = repository.GetItem(1, 1);
            file.FileColumns = new List<FileColumn>();
            var newFilecolumn = new FileColumn()
            {
                FileColumnTypeId = null,
                FileColumnUnitId = 1,
                EntityName = "entityName",
                EntityDescription = "entityDesc",
                Description = "Description"
            };
            file.FileColumns.Add(newFilecolumn);
            testDBContext.Commit();

            var fileColumns = GetFileColumns(file.FileId);

            Assert.AreEqual(1, fileColumns.Count);
            Assert.AreEqual(newFilecolumn.EntityName, fileColumns[0].EntityName);
            Assert.AreEqual(newFilecolumn.EntityDescription, fileColumns[0].EntityDescription);
            Assert.AreEqual(newFilecolumn.Description, fileColumns[0].Description);
            Assert.AreEqual(newFilecolumn.FileColumnUnitId, fileColumns[0].FileColumnUnitId);
            Assert.AreEqual(newFilecolumn.FileColumnTypeId, fileColumns[0].FileColumnTypeId);

            var columnTypeList = GetFileColumnTypes();
            //only one column type should be there, one more dummy row was added

            Assert.AreEqual(1, columnTypeList.Count());
            var columnsUnitsList = GetFileColumnUnits();

            Assert.AreEqual(1, columnsUnitsList.Count());
            testDBContext.Commit();
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Save_File_NullColumnUnit_DataTest()
        {
            IFileRepository repository = new FileRepository(testDBContext);

            var file = repository.GetItem(1, 1);
            file.FileColumns = new List<FileColumn>();
            var newFilecolumn1 = new FileColumn()
            {
                FileColumnTypeId = 1,
                FileColumnUnitId = null,
                EntityName = "entityName",
                EntityDescription = "entityDesc",
                Description = "Description"
            };

            var newFilecolumn2 = new FileColumn()
            {
                FileColumnTypeId = 1,
                FileColumnUnitId = 1,
                EntityName = "entityName1",
                EntityDescription = "entityDesc1",
                Description = "Description1"
            };

            var newFilecolumn3 = new FileColumn()
            {
                FileColumnTypeId = 1,
                FileColumnUnitId = 1,
                EntityName = "entityName2",
                EntityDescription = "entityDesc2",
                Description = "Description2"
            };

            file.FileColumns.Add(newFilecolumn1);
            file.FileColumns.Add(newFilecolumn2);
            file.FileColumns.Add(newFilecolumn3);

            testDBContext.Commit();

            var fileColumns = GetFileColumns(file.FileId);

            Assert.AreEqual(3, fileColumns.Count);
            Assert.AreEqual(newFilecolumn1.EntityName, fileColumns[0].EntityName);
            Assert.AreEqual(newFilecolumn1.EntityDescription, fileColumns[0].EntityDescription);
            Assert.AreEqual(newFilecolumn1.Description, fileColumns[0].Description);
            Assert.AreEqual(newFilecolumn1.FileColumnUnitId, fileColumns[0].FileColumnUnitId);
            Assert.AreEqual(newFilecolumn1.FileColumnTypeId, fileColumns[0].FileColumnTypeId);


            Assert.AreEqual(newFilecolumn2.EntityName, fileColumns[1].EntityName);
            Assert.AreEqual(newFilecolumn2.EntityDescription, fileColumns[1].EntityDescription);
            Assert.AreEqual(newFilecolumn2.Description, fileColumns[1].Description);
            Assert.AreEqual(newFilecolumn2.FileColumnUnitId, fileColumns[1].FileColumnUnitId);
            Assert.AreEqual(newFilecolumn2.FileColumnTypeId, fileColumns[1].FileColumnTypeId);

            Assert.AreEqual(newFilecolumn3.EntityName, fileColumns[2].EntityName);
            Assert.AreEqual(newFilecolumn3.EntityDescription, fileColumns[2].EntityDescription);
            Assert.AreEqual(newFilecolumn3.Description, fileColumns[2].Description);
            Assert.AreEqual(newFilecolumn3.FileColumnUnitId, fileColumns[2].FileColumnUnitId);
            Assert.AreEqual(newFilecolumn3.FileColumnTypeId, fileColumns[2].FileColumnTypeId);

            var columnTypeList = GetFileColumnTypes();
            //only one column type should be there, one more dummy row was added

            Assert.AreEqual(1, columnTypeList.Count());
            var columnsUnitsList = GetFileColumnUnits();

            Assert.AreEqual(1, columnsUnitsList.Count());
            testDBContext.Commit();
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_File_By_FileId_Test()
        {
            IFileRepository repository = new FileRepository(testDBContext);

            var file = repository.GetItem(1, 1);

            Assert.IsNotNull(file);

            Assert.AreEqual(file.FileId, 1);

            Assert.AreEqual(file.Name, "Document One");

            //  Assert.AreEqual(file.BlobId, "blobId");

            Assert.AreEqual(file.Citation, "Citation 1");

            Assert.AreEqual(file.CreatedBy, 1);

            Assert.AreEqual(file.Description, "Document 1");

            Assert.AreEqual(file.Identifier, "asdahgsdfsghadfsghad");

            Assert.AreEqual(file.isDeleted, false);

            Assert.AreEqual(file.MimeType, "Mime type 1");

            Assert.AreEqual(file.ModifiedBy, 1);

            Assert.IsNotNull(file.Repository);

            Assert.AreEqual(file.RepositoryId, 1);

            Assert.AreEqual(file.Size, 20.90);

            Assert.AreEqual(file.Status, "Uploaded");

            Assert.AreEqual(file.Title, "Document 1");

        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Update_File_With_FileData_Null_Test()
        {
            IFileRepository repository = new FileRepository(testDBContext);

            var file = repository.GetItem(1, 1);

            file.BlobId = "TestBlobid";
            file.Citation = "Updated Citation";
            file.Description = "Updated description";
            file.FileMetadataFields = null;
            file.FileColumns = null;
            file.FileQualityChecks = null;

            repository.UpdateFile(file);
            testDBContext.Commit();
            var updatedFile = repository.GetItem(1, 1);

            Assert.AreEqual(updatedFile.BlobId, "TestBlobid");
            Assert.AreEqual(updatedFile.Citation, "Updated Citation");
            Assert.AreEqual(updatedFile.Description, "Updated description");

            Assert.IsNull(file.FileMetadataFields);
            Assert.IsNull(file.FileColumns);
            Assert.IsNull(file.FileQualityChecks);
        }

        //[TestMethod]
        //[TestCategory(TestCategories.FunctionalTest)]
        //[TestCategory(TestCategories.Database)]
        //public void Delete_Mentioned_File_Test()
        //{
        //    IFileRepository repository = new FileRepository(testDBContext);

        //    var result = repository.UpdateFile(

        //    testDBContext.Commit();

        //    Assert.IsNotNull(result);

        //    var fileList = repository.GetFiles(1);

        //    Assert.AreEqual(fileList.Count(), 1);

        //    Assert.AreEqual(fileList.ToList()[0].FileId, 2);

        //    Assert.AreEqual(fileList.ToList()[0].Name, "Document Two");
        //}

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Remove_Mentioned_File_Permenently_Test()
        {
            IFileRepository repository = new FileRepository(testDBContext);

            var result = repository.DeleteFile(1, "Posted");

            testDBContext.Commit();

            Assert.IsNotNull(result);

            int userId = 1;
            string postStatus = FileStatus.Posted.ToString();
            Func<File, bool> filter = file => file.CreatedBy == userId && (file.Status.Equals(postStatus, StringComparison.InvariantCulture) || file.isDeleted == null || file.isDeleted == false);
            var fileList = repository.GetFiles(filter);

            Assert.AreEqual(fileList.Count(), 1);

            Assert.AreEqual(fileList.ToList()[0].FileId, 2);

            Assert.AreEqual(fileList.ToList()[0].Name, "Document Two");
        }

        #region private methods

        private void AddDefaultData()
        {
            User newUser = new User()
            {
                NameIdentifier = "s0Me1De9Tf!Er$tRing",
                FirstName = "TestFirstName",
                MiddleName = "TestMiddleName",
                LastName = "TestLastName",
                IdentityProvider = "Windows Live",
                Organization = "TestOrganization",
                EmailId = "testmail@domain.com",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                IsActive = true,
                UserAttributes = null
            };

            // User with userid 1
            AddUser(newUser);

            //  RepositoryType repoType1 = new RepositoryType() { IsActive = true, Name = "type1", RepositoryTypeId = 1, Repositories = null };

            // Adding new repository types 
            // AddRepositoryType(repoType1);

            BaseRepository baseRepo = new BaseRepository() { BaseRepositoryId = 1, Name = "Merrit" };

            AddBaseRepository(baseRepo);

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
                Name = "Repository1",
                RepositoryId = 1,
                UserAgreement = "Test Agreement1",
                BaseRepositoryId = 1
            };

            AddRepository(repositoryObject);

            File fileToAdd = new File() { Citation = "Citation 1", CreatedBy = 1, CreatedOn = DateTime.Now, Description = "Document 1", FileAttributes = null, FileColumns = null, FileId = 1, FileQualityChecks = null, Identifier = "asdahgsdfsghadfsghad", isDeleted = false, MimeType = "Mime type 1", ModifiedBy = 1, ModifiedOn = DateTime.Now, Name = "Document One", Repository = null, RepositoryId = 1, Size = 20.90, Status = "Uploaded", Title = "Document 1" };

            File fileToAdd1 = new File() { Citation = "Citation 2", CreatedBy = 1, CreatedOn = DateTime.Now, Description = "Document 2", FileAttributes = null, FileColumns = null, FileId = 2, FileQualityChecks = null, Identifier = "wrwe23423ewr", isDeleted = false, MimeType = "Mime type 2", ModifiedBy = 1, ModifiedOn = DateTime.Now, Name = "Document Two", Repository = null, RepositoryId = 1, Size = 20.90, Status = "Uploaded", Title = "Document 2" };

            AddFile(fileToAdd);
            AddFile(fileToAdd1);

            FileColumnUnit fileUnit = new FileColumnUnit()
            {
                Name = "Text",
                Status = true
            };
            AddFileColumnUnit(fileUnit);

            FileColumnType fileType = new FileColumnType()
            {
                Name = "Acre",
                Status = true
            };

            AddFileColumnType(fileType);

        }

        private void AddUser(User user)
        {
            IUserRepository repository = new UserRepository(testDBContext);
            User addedUser = repository.AddUser(user);
            testDBContext.Commit();
        }

        private void AddFile(File fileToAdd)
        {
            IFileRepository repository = new FileRepository(testDBContext);
            repository.AddFile(fileToAdd);
            testDBContext.Commit();
        }

        private void AddRepositoryType()
        {
            //IRepositoryDetails repository = new RepositoryDetails(testDBContext);
            //testDBContext.RepositoryTypes.Add(repoType);
            testDBContext.Commit();
        }

        private void AddRepository(Repository repo)
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);
            repository.AddRepository(repo);
            testDBContext.Commit();
        }

        private void AddBaseRepository(BaseRepository repo)
        {
            IRepositoryDetails repository = new RepositoryDetails(testDBContext);
            testDBContext.BaseRepositories.Add(repo);
            testDBContext.Commit();
        }

        private void AddFileColumnUnit(FileColumnUnit fileColumnUnit)
        {
            var updatedFile = testDBContext.FileColumnUnits.Add(fileColumnUnit);
            testDBContext.Commit();
        }

        private void AddFileColumnType(FileColumnType fileColumnType)
        {
            var updatedFile = testDBContext.FileColumnTypes.Add(fileColumnType);
            testDBContext.Commit();
        }

        private IDbSet<FileColumnType> GetFileColumnTypes()
        {
            return testDBContext.FileColumnTypes;
        }

        private IDbSet<FileColumnUnit> GetFileColumnUnits()
        {
            return testDBContext.FileColumnUnits;
        }

        private List<FileColumn> GetFileColumns(int fileId)
        {
            var fileColumnsList = new List<FileColumn>();
            var fileColumns = testDBContext.FileColumns.Where(f => f.FileId == fileId).ToList();
            foreach (var data in fileColumns)
            {
                fileColumnsList.Add(data);
            }
            return fileColumnsList;
        }

        #endregion
    }
}
