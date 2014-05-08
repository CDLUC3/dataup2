using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.DataAccessService.Fakes;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.QCService.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using QCS = Microsoft.Research.DataOnboarding.QCService;

namespace QCService.Tests
{
    [TestClass]
    public class QCServiceTest
    {
        /// <summary>
        /// StubIFileService private variable 
        /// </summary>
        private StubIQualityCheckRepository qcRepository = null;

        private IUnitOfWork unitOfWork = null;

        private IUserRepository userRepository = null;

        private IFileService fileService = null;

        private IBlobDataRepository blobRepository = null;

        private IQCService qcService = null;

        /// <summary>
        /// Method to Initialize the 
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            List<QualityCheckColumnRule> lstCol = new List<QualityCheckColumnRule>();
            QualityCheckColumnRule colRule = new QualityCheckColumnRule() { Description = "Column 1 desc", ErrorMessage = "error msg", HeaderName = "Column 1", IsActive = true, IsRequired = true, Order = 1, QualityCheck = null, QualityCheckColumnRuleId = 1, QualityCheckColumnTypeId = 1, QualityCheckId = 1, Range = "" };
            QualityCheckColumnRule colRule1 = new QualityCheckColumnRule() { Description = "Column 2 desc", ErrorMessage = "error msg1", HeaderName = "Column 2", IsActive = true, IsRequired = true, Order = 2, QualityCheck = null, QualityCheckColumnRuleId = 2, QualityCheckColumnTypeId = 2, QualityCheckId = 1, Range = "" };
            lstCol.Add(colRule);
            lstCol.Add(colRule1);

            // Unit of work code
            this.unitOfWork =
                new StubIUnitOfWork()
                {
                    Commit = () => { return; }
                };

            // User repository
            this.userRepository = new StubIUserRepository()
            {
                GetUserbyUserIdInt32 = (userId) =>
                    {
                        List<UserRole> userRoles = new List<UserRole>();
                        userRoles.Add(new UserRole() { Role = null, RoleId = 1, UserId = userId, UserRoleId = 1 });
                        User userNew = new User() { CreatedOn = DateTime.UtcNow, EmailId = "test@test.com", FirstName = "First", IdentityProvider = "identity", IsActive = true, LastName = "Last", MiddleName = "midele", ModifiedOn = DateTime.UtcNow, NameIdentifier = "nameIdentifier", Organization = "Test Org", UserAttributes = null, UserId = userId, UserRoles = userRoles };
                        return userNew;
                    }
            };

            // file Service implementation
            this.fileService = new StubIFileService()
            {
                GetFileByFileIdInt32 = (fileId) =>
                    {
                        File fileToAdd = new File() { Citation = "Citation 1", CreatedBy = 1, CreatedOn = DateTime.Now, Description = "Document 1", FileAttributes = null, FileColumns = null, FileId = fileId, FileQualityChecks = null, Identifier = "asdahgsdfsghadfsghad", isDeleted = false, MimeType = "Mime type 1", ModifiedBy = 1, ModifiedOn = DateTime.Now, Name = "Document One", Repository = null, RepositoryId = null, Size = 20.90, Status = "Uploaded", Title = "Document 1" };
                        return null;
                    }
            };

            // Quality check repository implementation
            this.qcRepository = new StubIQualityCheckRepository()
            {
                RetrieveQualityCheckRules = () =>
                    {
                        List<QualityCheck> lstQCRule = new List<QualityCheck>();
                        lstQCRule.Add(new QualityCheck() { CreatedBy = 1, CreatedOn = DateTime.UtcNow, Description = "Test Rule", EnforceOrder = true, FileQualityChecks = null, IsActive = true, IsVisibleToAll = true, ModifiedBy = 1, ModifiedOn = DateTime.UtcNow, Name = "Test Rule", QualityCheckColumnRules = lstCol, QualityCheckId = 1 });
                        // lstQCRule.Add(new QualityCheck() { CreatedBy = 1, CreatedOn = DateTime.UtcNow, Description = "Test Rule1", EnforceOrder = true, FileQualityChecks = null, IsActive = true, IsVisibleToAll = true, ModifiedBy = 1, ModifiedOn = DateTime.UtcNow, Name = "Test Rule1", QualityCheckColumnRules = lstCol, QualityCheckId = 1 });

                        return lstQCRule;
                    },
                RetrieveQCColumnTypes = () =>
                    {
                        List<QualityCheckColumnType> qcTypes = new List<QualityCheckColumnType>();
                        qcTypes.Add(new QualityCheckColumnType() { Description = "No check", Name = "None", QualityCheckColumnTypeId = 1 });
                        qcTypes.Add(new QualityCheckColumnType() { Description = "Number type", Name = "Numeric", QualityCheckColumnTypeId = 2 });
                        return qcTypes;
                    },
                GetQualityCheckByIDInt32 = (qualityCheckId) =>
                    {
                        QualityCheck qc = new QualityCheck() { CreatedBy = 1, CreatedOn = DateTime.UtcNow, Description = "Get by id desc", EnforceOrder = true, FileQualityChecks = null, IsActive = true, IsVisibleToAll = true, ModifiedBy = 1, ModifiedOn = DateTime.UtcNow, Name = "Get by id Rule", QualityCheckColumnRules = lstCol, QualityCheckId = qualityCheckId };
                        return qc;
                    },
                AddQualityCheckRuleQualityCheck = (qualityCheck) =>
                    {
                        qualityCheck.QualityCheckId = 2;
                        return qualityCheck;
                    },
                UpdateQualityCheckRuleQualityCheck = (qualityCheck) =>
                     {
                         qualityCheck.Name = "Updated rule From rep";
                         return qualityCheck;
                     },
                DeleteQualityCheckRuleInt32 = (qualityCheckId) =>
                      {
                          QualityCheck qc = new QualityCheck() { CreatedBy = 1, CreatedOn = DateTime.UtcNow, Description = "Delete", EnforceOrder = true, FileQualityChecks = null, IsActive = true, IsVisibleToAll = true, ModifiedBy = 1, ModifiedOn = DateTime.UtcNow, Name = "Delete Rule", QualityCheckColumnRules = lstCol, QualityCheckId = qualityCheckId };
                          return qc;
                      },
                DeleteColumnRulesInt32 = (qualityCheckId) =>
                    {
                        return true;
                    },
                AddFileQualityCheckFileQualityCheck = (fileQualityCheck) =>
                    {
                        fileQualityCheck.Status = true;
                        return fileQualityCheck;
                    },
                GetQualityCheckByNameString = (ruleName) =>
                    {
                        QualityCheck qc = new QualityCheck() { CreatedBy = 1, CreatedOn = DateTime.UtcNow, Description = "Get by rule name", EnforceOrder = true, FileQualityChecks = null, IsActive = true, IsVisibleToAll = true, ModifiedBy = 1, ModifiedOn = DateTime.UtcNow, Name = ruleName, QualityCheckColumnRules = lstCol, QualityCheckId = 10 };
                        return qc;
                    }
            };

            qcService = new QCS.QCService(this.qcRepository, this.unitOfWork, this.userRepository, this.fileService, this.blobRepository);
        }

        [TestMethod]
        public void Get_All_QC_Rules_Test()
        {
            var qcRules = qcService.GetQualityCheckRules(true);

            Assert.IsNotNull(qcRules);
            Assert.IsNotNull(qcRules[0].QualityCheckData);
            Assert.IsNotNull(qcRules[0].QualityCheckData.QualityCheckColumnRules);

            Assert.AreEqual(qcRules.Count, 1);
            Assert.AreEqual(qcRules[0].CreatedUser, "First Last");

            Assert.AreEqual(qcRules[0].QualityCheckData.QualityCheckColumnRules.Count, 2);

            var rule1 = qcRules[0].QualityCheckData.QualityCheckColumnRules.FirstOrDefault();

            Assert.AreEqual(rule1.Order, 1);
            Assert.AreEqual(rule1.Description, "Column 1 desc");
            Assert.AreEqual(rule1.ErrorMessage, "error msg");
            Assert.AreEqual(rule1.HeaderName, "Column 1");
            Assert.AreEqual(rule1.QualityCheckColumnTypeId, 1);
            Assert.AreEqual(rule1.QualityCheckId, 1);
        }

        [TestMethod]
        public void Add_New_QC_Rule_Test()
        {
            QualityCheck ruleToAdd = new QualityCheck() { CreatedBy = 1, CreatedOn = DateTime.UtcNow, Description = "Added Rule", EnforceOrder = true, FileQualityChecks = null, IsActive = true, IsVisibleToAll = true, ModifiedBy = 1, ModifiedOn = DateTime.UtcNow, Name = "Added Rule", QualityCheckColumnRules = null, QualityCheckId = 0 };

            var result = qcService.AddUpdateQualityCheck(ruleToAdd);

            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void Update_Exisitng_QC_Rule_Test()
        {
            QualityCheck ruleToUpdate = new QualityCheck() { CreatedBy = 1, CreatedOn = DateTime.UtcNow, Description = "Updated Rule Desc", EnforceOrder = true, FileQualityChecks = null, IsActive = true, IsVisibleToAll = true, ModifiedBy = 1, ModifiedOn = DateTime.UtcNow, Name = "Updated Rule", QualityCheckColumnRules = null, QualityCheckId = 4 };

            var result = qcService.AddUpdateQualityCheck(ruleToUpdate);

            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void Delete_Existing_QC_Rule_Test()
        {
            var delRule = qcService.DeleteQualityCheckRule(8);

            Assert.IsTrue(delRule);
        }

        [TestMethod]
        public void Get_All_Column_Types_test()
        {
            var lstColTyps = qcService.RetrieveQCColumnTypes();

            Assert.IsNotNull(lstColTyps);
            Assert.AreEqual(lstColTyps.Count(), 2);
        }

        [TestMethod]
        public void Get_QC_Rule_By_Id_Test()
        {
            var selRule = qcService.GetQualityCheckById(8);

            Assert.IsNotNull(selRule);
            Assert.AreEqual(selRule.CreatedUser, "First Last");

            Assert.AreEqual(selRule.QualityCheckData.QualityCheckColumnRules.Count, 2);

            //  Description = "Get by id desc", EnforceOrder = true, FileQualityChecks = null, IsActive = true, IsVisibleToAll = true, ModifiedBy = 1, ModifiedOn = DateTime.UtcNow, Name = "Get by id Rule", QualityCheckColumnRules = lstCol, QualityCheckId = qualityCheckId };

            Assert.AreEqual(selRule.QualityCheckData.CreatedBy, 1);
            Assert.AreEqual(selRule.QualityCheckData.Name, "Get by id Rule");
            Assert.AreEqual(selRule.QualityCheckData.Description, "Get by id desc");
            Assert.AreEqual(selRule.QualityCheckData.EnforceOrder, true);
            Assert.AreEqual(selRule.QualityCheckData.QualityCheckId, 8);

            var rule1 = selRule.QualityCheckData.QualityCheckColumnRules.FirstOrDefault();

            Assert.AreEqual(rule1.Order, 1);
            Assert.AreEqual(rule1.Description, "Column 1 desc");
            Assert.AreEqual(rule1.ErrorMessage, "error msg");
            Assert.AreEqual(rule1.HeaderName, "Column 1");
            Assert.AreEqual(rule1.QualityCheckColumnTypeId, 1);
            Assert.AreEqual(rule1.QualityCheckId, 1);
        }

        [TestMethod]
        public void Check_Duplicate_Rule_Names_Test()
        {
            var ruleId = qcService.CheckRuleExists("Some rule name");

            Assert.AreEqual(ruleId, 10);
        }
    }
}
