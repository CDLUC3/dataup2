using Microsoft.Research.DataOnboarding.DataAccessService.Providers.EntityFramework;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnboardingEntityFramework = Microsoft.Research.DataOnboarding.DataAccessService.Providers.EntityFramework;

namespace Microsoft.Research.DataOnboarding.DataAccessService.Tests.FunctionalTests
{
    [TestClass]
    public class QualityCheckRepositoryIntegrationTests
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

        #region Test methods

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_All_Quality_Check_Test()
        {
            IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            var qcRules = repository.RetrieveQualityCheckRules().ToList();

            Assert.IsNotNull(qcRules);

            Assert.AreEqual(qcRules.Count(), 1);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_Null_QC_Rule_By_Id_Test()
        {
            IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            var qcRule = repository.GetQualityCheckByID(6);

            Assert.IsNull(qcRule);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_QC_Rule_By_Id_Test()
        {
            IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            var qcRule = repository.GetQualityCheckByID(1);

            Assert.IsNotNull(qcRule);

            Assert.AreEqual(qcRule.CreatedBy, 1);
            Assert.AreEqual(qcRule.Description, "Test rule");
            Assert.AreEqual(qcRule.EnforceOrder, true);
            Assert.AreEqual(qcRule.IsActive, true);
            Assert.AreEqual(qcRule.IsVisibleToAll, true);
            Assert.AreEqual(qcRule.ModifiedBy, 1);
            Assert.AreEqual(qcRule.Name, "Test Rule");
            Assert.AreEqual(qcRule.QualityCheckId, 1);
            Assert.AreEqual(qcRule.Name, "Test Rule");
            Assert.IsNull(qcRule.FileQualityChecks);

            Assert.IsNotNull(qcRule.QualityCheckColumnRules);
            Assert.AreEqual(qcRule.QualityCheckColumnRules.Count, 2);

            var selColRule = qcRule.QualityCheckColumnRules.Where(rul => rul.QualityCheckColumnRuleId == 1).FirstOrDefault();

            Assert.AreEqual(selColRule.Description, "Column 1");
            Assert.AreEqual(selColRule.ErrorMessage, "Col1 is required");
            Assert.AreEqual(selColRule.HeaderName, "Col1");
            Assert.AreEqual(selColRule.IsActive, true);
            Assert.AreEqual(selColRule.IsRequired, true);
            Assert.AreEqual(selColRule.Order, 1);
            Assert.AreEqual(selColRule.QualityCheckColumnRuleId, 1);
            Assert.AreEqual(selColRule.QualityCheckId, 1);
            Assert.AreEqual(selColRule.Range, "");
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_All_QC_Rule_Column_Types()
        {
            IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            var qcColRuleTypes = repository.RetrieveQCColumnTypes();

            Assert.IsNotNull(qcColRuleTypes);
            Assert.AreEqual(qcColRuleTypes.Count(), 2);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throw_Exception_On_Get_Null_QCRule_By_Name_Test()
        {
            IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            repository.GetQualityCheckByName(null);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_QC_Rule_By_Not_Available_Rule_Name_Test()
        {
            IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            var qcRule = repository.GetQualityCheckByName("Not There");

            Assert.IsNull(qcRule);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Get_QC_Rule_By_Rule_Name_Test()
        {
            IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            var qcRule = repository.GetQualityCheckByName("Test Rule");

            Assert.IsNotNull(qcRule);

            Assert.AreEqual(qcRule.CreatedBy, 1);
            Assert.AreEqual(qcRule.Description, "Test rule");
            Assert.AreEqual(qcRule.EnforceOrder, true);
            Assert.AreEqual(qcRule.IsActive, true);
            Assert.AreEqual(qcRule.IsVisibleToAll, true);
            Assert.AreEqual(qcRule.ModifiedBy, 1);
            Assert.AreEqual(qcRule.Name, "Test Rule");
            Assert.AreEqual(qcRule.QualityCheckId, 1);
            Assert.AreEqual(qcRule.Name, "Test Rule");
            Assert.IsNull(qcRule.FileQualityChecks);

            Assert.IsNotNull(qcRule.QualityCheckColumnRules);
            Assert.AreEqual(qcRule.QualityCheckColumnRules.Count, 2);

            var selColRule = qcRule.QualityCheckColumnRules.Where(rul => rul.QualityCheckColumnRuleId == 1).FirstOrDefault();

            Assert.AreEqual(selColRule.Description, "Column 1");
            Assert.AreEqual(selColRule.ErrorMessage, "Col1 is required");
            Assert.AreEqual(selColRule.HeaderName, "Col1");
            Assert.AreEqual(selColRule.IsActive, true);
            Assert.AreEqual(selColRule.IsRequired, true);
            Assert.AreEqual(selColRule.Order, 1);
            Assert.AreEqual(selColRule.QualityCheckColumnRuleId, 1);
            Assert.AreEqual(selColRule.QualityCheckId, 1);
            Assert.AreEqual(selColRule.Range, "");

        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throw_Exception_On_Add_Null_File_Quality_Check_Test()
        {
            IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            repository.AddFileQualityCheck(null);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Add_File_Quality_Check_Test()
        {
            //IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            ////File fileToAdd = new File() { Citation = "Citation 1", CreatedBy = 1, CreatedOn = DateTime.Now, Description = "Document 1", FileAttributes = null, FileColumns = null, FileId = 1, FileQualityChecks = null, Identifier = "asdahgsdfsghadfsghad", isDeleted = false, MimeType = "Mime type 1", ModifiedBy = 1, ModifiedOn = DateTime.Now, Name = "Document One", Repository = null, RepositoryId = null, Size = 20.90, Status = "Uploaded", Title = "Document 1" };

            ////AddFile(fileToAdd);

            //FileQualityCheck fileQCToAdd = new FileQualityCheck() { FileId = 1, FileQualityCheckId = 1, LastRunDateTime = DateTime.Parse("2005-09-01"), QualityCheckId = 1, Status = true };

            //repository.AddFileQualityCheck(fileQCToAdd);

            //testDBContext.Commit();

            //var addedFileQC = GetFIleQCByID(1);

            //Assert.IsNotNull(addedFileQC);
            //Assert.AreEqual(addedFileQC.FileId, fileQCToAdd.FileId);
            //Assert.AreEqual(addedFileQC.FileQualityCheckId, fileQCToAdd.FileQualityCheckId);
            //// Assert.AreEqual(addedFileQC.LastRunDateTime, fileQCToAdd.LastRunDateTime);
            //Assert.AreEqual(addedFileQC.QualityCheckId, fileQCToAdd.QualityCheckId);
            //Assert.AreEqual(addedFileQC.Status, fileQCToAdd.Status);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throw_Exception_On_Delete_Not_Available_Quality_Check_Test()
        {
            IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            repository.DeleteQualityCheckRule(8);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Delete_Column_Rules_For_Not_Available_QC_Rule_Test()
        {
            IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            repository.DeleteColumnRules(8);
        }


        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Delete_Column_Rules_For_No_column_Rules_For_QC_Rule_Test()
        {
            IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            repository.DeleteColumnRules(2);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Delete_Column_Rules_QC_Rule_Test()
        {
            IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            repository.DeleteColumnRules(1);

            testDBContext.Commit();

            var ruleList = repository.GetQualityCheckByID(1);

            Assert.IsNotNull(ruleList);
            Assert.AreEqual(ruleList.QualityCheckColumnRules.Count, 0);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Delete_Quality_Check_Rule_Test()
        {
            IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            repository.DeleteQualityCheckRule(1);

            testDBContext.Commit();

            var lstRules = repository.RetrieveQualityCheckRules();

            Assert.AreEqual(lstRules.Count(), 0);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throw_Exception_On_Update_Null_QCRule_Test()
        {
            IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            repository.UpdateQualityCheckRule(null);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Update_Existing_QC_Rule_Test()
        {
            IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            var selQcRule = repository.GetQualityCheckByID(1);

            selQcRule.Description = "Updated description";
            selQcRule.EnforceOrder = false;
            selQcRule.IsActive = false;
            selQcRule.IsVisibleToAll = false;
            selQcRule.Name = "Updated Name";

            repository.UpdateQualityCheckRule(selQcRule);

            testDBContext.Commit();

            var updatedQcRule = repository.GetQualityCheckByID(1);


            Assert.AreEqual(updatedQcRule.Description, "Updated description");
            Assert.AreEqual(updatedQcRule.Name, "Updated Name");
            Assert.IsFalse((bool)updatedQcRule.EnforceOrder);
            Assert.IsFalse((bool)updatedQcRule.IsVisibleToAll);
            Assert.IsFalse((bool)updatedQcRule.IsActive);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throw_Exception_On_Add_Null_QCRule_Test()
        {
            IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            repository.AddQualityCheckRule(null);
        }

        [TestMethod]
        [TestCategory(TestCategories.FunctionalTest)]
        [TestCategory(TestCategories.Database)]
        public void Add_QC_Rule_Test()
        {
            IQualityCheckRepository repository = new QualityCheckRepository(testDBContext);

            QualityCheckColumnRule colRule = new QualityCheckColumnRule() { Description = "Column 4", ErrorMessage = "Col1 is required", HeaderName = "Col1", IsActive = true, IsRequired = true, Order = 1, QualityCheckColumnRuleId = 1, QualityCheckColumnTypeId = 1, QualityCheckId = 2, Range = "" };
            QualityCheck qcRule = new QualityCheck()
            {
                CreatedBy = 1,
                CreatedOn = DateTime.UtcNow,
                Description = "Added rule",
                EnforceOrder = true,
                FileQualityChecks = null,
                IsActive = true,
                IsVisibleToAll = true,
                ModifiedBy = 1,
                ModifiedOn = DateTime.UtcNow,
                Name = "Added rule",
                QualityCheckColumnRules = new List<QualityCheckColumnRule>() { colRule },
                QualityCheckId = 2
            };

            repository.AddQualityCheckRule(qcRule);
            testDBContext.Commit();

            var addedQcRule = repository.GetQualityCheckByID(2);

            Assert.IsNotNull(addedQcRule);

            Assert.AreEqual(addedQcRule.CreatedBy, qcRule.CreatedBy);
            Assert.AreEqual(addedQcRule.Description, qcRule.Description);
            Assert.AreEqual(addedQcRule.EnforceOrder, qcRule.EnforceOrder);
            Assert.AreEqual(addedQcRule.IsActive, qcRule.IsActive);
            Assert.AreEqual(addedQcRule.IsVisibleToAll, qcRule.IsVisibleToAll);
            Assert.AreEqual(addedQcRule.ModifiedBy, qcRule.ModifiedBy);
            Assert.AreEqual(addedQcRule.Name, qcRule.Name);
            Assert.AreEqual(addedQcRule.QualityCheckId, qcRule.QualityCheckId);
            Assert.AreEqual(addedQcRule.Name, qcRule.Name);

            Assert.IsNull(addedQcRule.FileQualityChecks);
            Assert.IsNotNull(addedQcRule.QualityCheckColumnRules);

            var selColRule = addedQcRule.QualityCheckColumnRules.Where(rul => rul.QualityCheckId == 2).FirstOrDefault();

            Assert.AreEqual(selColRule.Description, colRule.Description);
            Assert.AreEqual(selColRule.ErrorMessage, colRule.ErrorMessage);
            Assert.AreEqual(selColRule.HeaderName, colRule.HeaderName);
            Assert.AreEqual(selColRule.IsActive, colRule.IsActive);
            Assert.AreEqual(selColRule.IsRequired, colRule.IsRequired);
            Assert.AreEqual(selColRule.Order, colRule.Order);
            Assert.AreEqual(selColRule.QualityCheckColumnRuleId, colRule.QualityCheckColumnRuleId);
            Assert.AreEqual(selColRule.QualityCheckId, colRule.QualityCheckId);
            Assert.AreEqual(selColRule.Range, colRule.Range);
        }

        #endregion

        #region private methods

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

            //Adding Quality check column rules data

            QualityCheckColumnType qcColType = new QualityCheckColumnType() { Description = "No type check", Name = "None", QualityCheckColumnTypeId = 1, QualityCheckColumnRules = null };
            QualityCheckColumnType qcColType1 = new QualityCheckColumnType() { Description = "Number type", Name = "Numeric", QualityCheckColumnTypeId = 2, QualityCheckColumnRules = null };

            AddQualityCheckColumnType(qcColType);
            AddQualityCheckColumnType(qcColType1);

            QualityCheck qcRule = new QualityCheck() { CreatedBy = 1, Description = "Test rule", CreatedOn = DateTime.UtcNow, EnforceOrder = true, FileQualityChecks = null, IsActive = true, IsVisibleToAll = true, ModifiedBy = 1, ModifiedOn = DateTime.UtcNow, Name = "Test Rule", QualityCheckId = 1, QualityCheckColumnRules = null };

            AddQualityCheckRule(qcRule);

            QualityCheckColumnRule colRule = new QualityCheckColumnRule() { Description = "Column 1", ErrorMessage = "Col1 is required", HeaderName = "Col1", IsActive = true, IsRequired = true, Order = 1, QualityCheckColumnRuleId = 1, QualityCheckColumnTypeId = 1, QualityCheckId = 1, Range = "" };
            QualityCheckColumnRule colRule1 = new QualityCheckColumnRule() { Description = "Column 2", ErrorMessage = "Col2 is required", HeaderName = "Col2", IsActive = true, IsRequired = false, Order = 2, QualityCheckColumnRuleId = 1, QualityCheckColumnTypeId = 2, QualityCheckId = 1, Range = "1-19999" };

            AddQualityCheckColumnRule(colRule);
            AddQualityCheckColumnRule(colRule1);
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

        private void AddQualityCheckColumnType(QualityCheckColumnType qcColumnType)
        {
            testDBContext.QualityCheckColumnTypes.Add(qcColumnType);
            testDBContext.Commit();
        }

        private void AddQualityCheckRule(QualityCheck qcRule)
        {
            testDBContext.QualityChecks.Add(qcRule);
            testDBContext.Commit();
        }

        private void AddQualityCheckColumnRule(QualityCheckColumnRule qcColumnRule)
        {
            testDBContext.QualityCheckColumnRules.Add(qcColumnRule);
            testDBContext.Commit();
        }

        private void AddBaseRepository(BaseRepository repo)
        {
            testDBContext.BaseRepositories.Add(repo);
            testDBContext.Commit();
        }

        private void AddFile(File fileToAdd)
        {
            testDBContext.Files.Add(fileToAdd);
            testDBContext.Commit();
        }

        private FileQualityCheck GetFIleQCByID(int fileQcId)
        {
            return testDBContext.FileQualityChecks.Where(filQc => filQc.FileQualityCheckId == fileQcId).FirstOrDefault();
        }

        #endregion
    }
}
