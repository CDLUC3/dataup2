// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.FileProcesser;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.QCService.Interface;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.QCService
{
    /// <summary>
    /// Class for quality check related methods.
    /// </summary>
    public class QCService : IQCService
    {
        /// <summary>
        /// Variable to hold the quality check repository object.
        /// </summary>
        private IQualityCheckRepository qualityCheckRepository = null;

        /// <summary>
        /// Variable to hold the user repository object.
        /// </summary>
        private IUserRepository userRepository = null;

        /// <summary>
        /// Variable to hold the unit of work object.
        /// </summary>
        private IUnitOfWork unitOfWork = null;

        /// <summary>
        /// Variable to hold File service object
        /// </summary>
        private IFileService fileService = null;

        /// <summary>
        /// Variable to hold blob service object
        /// </summary>
        private IBlobDataRepository blobRepository = null;

        /// <summary>
        ///  Initializes a new instance of the <see cref="FileServiceProvider"/> class.
        /// </summary>
        /// <param name="qualityCheckRepository">Quality check repository object.</param>
        /// <param name="unitOfWork">Object of IUnitWork.</param>
        public QCService(IQualityCheckRepository qualityCheckRepository, IUnitOfWork unitOfWork, IUserRepository userRepository, IFileService fileService, IBlobDataRepository blobRepository)
        {
            this.qualityCheckRepository = qualityCheckRepository;
            this.unitOfWork = unitOfWork;
            this.userRepository = userRepository;
            this.fileService = fileService;
            this.blobRepository = blobRepository;
        }

        /// <summary>
        /// Method to get all the available quality check rules.
        /// </summary>
        /// <param name="includeAdminRules">bool indicates if rules marked as AdminOnly should be returned or not</param>
        /// <returns>Quality check rules collection.</returns>
        public IList<QualityCheckModel> GetQualityCheckRules(bool includeAdminRules)
        {
            IList<QualityCheckModel> lstQCRules = new List<QualityCheckModel>();
            var lstQCModel = this.qualityCheckRepository.RetrieveQualityCheckRules();
            foreach (var qcModel in lstQCModel)
            {
                bool isVisibleToAll = qcModel.IsVisibleToAll.HasValue? (bool)qcModel.IsVisibleToAll: true;
                if (includeAdminRules || isVisibleToAll)
                {
                    QualityCheckModel model = new QualityCheckModel();
                    if (qcModel.QualityCheckColumnRules != null && qcModel.QualityCheckColumnRules.Count > 0)
                    {
                        qcModel.QualityCheckColumnRules = qcModel.QualityCheckColumnRules.OrderBy(qc => qc.Order).ToList();
                    }
                    model.QualityCheckData = qcModel;
                    var user = this.userRepository.GetUserbyUserId(qcModel.CreatedBy);
                    model.CreatedUser = user.FirstName + " " + user.LastName;
                    lstQCRules.Add(model);
                }
            }

            return lstQCRules;
        }

        /// <summary>
        /// Method to add or update the existing quality check rule.
        /// </summary>
        /// <param name="qualityCheck">Quality check object.</param>
        /// <returns>True incase of succeess.</returns>
        public bool AddUpdateQualityCheck(QualityCheck qualityCheck)
        {
            bool updateResult = false;

            if (qualityCheck.QualityCheckId > 0)
            {
                // Delete all the existing column rules
                if (this.qualityCheckRepository.DeleteColumnRules(qualityCheck.QualityCheckId))
                {
                    this.unitOfWork.Commit();

                    var updatedQualityCheck = this.qualityCheckRepository.UpdateQualityCheckRule(qualityCheck);

                    if (updatedQualityCheck != null && updatedQualityCheck.QualityCheckId == qualityCheck.QualityCheckId)
                    {
                        updateResult = true;
                        this.unitOfWork.Commit();
                    }
                }
            }
            else
            {
                var addedQualityCheck = this.qualityCheckRepository.AddQualityCheckRule(qualityCheck);

                if (addedQualityCheck != null)
                {
                    updateResult = true;
                    this.unitOfWork.Commit();
                }
            }

            return updateResult;
        }

        /// <summary>
        /// Method to delete the existing quality check rule.
        /// </summary>
        /// <param name="qualityCheckRuleId">Quality check rule id.</param>
        /// <returns>True incase of succeess.</returns>
        public bool DeleteQualityCheckRule(int qualityCheckRuleId)
        {
            bool deleteResult = false;
            var deleteQualityCheck = this.qualityCheckRepository.DeleteQualityCheckRule(qualityCheckRuleId);

            if (deleteQualityCheck != null && deleteQualityCheck.QualityCheckId == deleteQualityCheck.QualityCheckId)
            {
                deleteResult = true;
                this.unitOfWork.Commit();
            }

            return deleteResult;
        }

        /// <summary>
        /// Method to get all the available quality check column types.
        /// </summary>
        /// <returns>Collection of quality check column types.</returns>
        public IEnumerable<QualityCheckColumnType> RetrieveQCColumnTypes()
        {
            return this.qualityCheckRepository.RetrieveQCColumnTypes();
        }

        /// <summary>
        /// Method to get the quality check object by its id.
        /// </summary>
        /// <param name="qcId">Quality check id.</param>
        /// <returns>Quality check object.</returns>
        public QualityCheckModel GetQualityCheckById(int qcId)
        {
            QualityCheckModel qcRuleModel = new QualityCheckModel();
            var qcModel = this.qualityCheckRepository.GetQualityCheckByID(qcId);

            if (qcModel.QualityCheckColumnRules != null && qcModel.QualityCheckColumnRules.Count > 0)
            {
                qcModel.QualityCheckColumnRules = qcModel.QualityCheckColumnRules.OrderBy(qc => qc.Order).ToList();
            }
            //qcModel.QualityCheckColumnRules = qcModel.QualityCheckColumnRules.OrderBy(qc => qc.Order).ToList();
            qcRuleModel.QualityCheckData = qcModel;
            var user = this.userRepository.GetUserbyUserId(qcModel.CreatedBy);
            qcRuleModel.CreatedUser = user.FirstName + " " + user.LastName;
            return qcRuleModel;
        }

        /// <summary>
        /// Method to check for the duplicate rule name.
        /// </summary>
        /// <param name="ruleName">Rule name.</param>
        /// <returns>Quality check rule id.</returns>
        public int CheckRuleExists(string ruleName)
        {
            int result = 0;
            var qcRule = this.qualityCheckRepository.GetQualityCheckByName(ruleName);

            if (qcRule != null && qcRule.Name.Equals(ruleName, StringComparison.InvariantCultureIgnoreCase))
            {
                result = qcRule.QualityCheckId;
            }

            return result;
        }

        /// <summary>
        /// Method to Retrieve Quality check issues
        /// </summary>
        /// <param name="fileId">File Id</param>
        /// <param name="ruleId">Rule Id</param>
        /// <param name="sheetIds">Sheet Ids</param>
        /// <returns>returns the collection of Quality Check issues</returns>
        public async Task<IEnumerable<QualityCheckResult>> GetQualityCheckIssues(int fileId, int qualityCheckId, string sheetIds)
        {
            var file = this.fileService.GetFileByFileId(fileId);
            var qualityCheck = this.qualityCheckRepository.GetQualityCheckByID(qualityCheckId);

            var qualityCheckTypes = this.qualityCheckRepository.RetrieveQCColumnTypes();

            IFileProcesser fileProcesser = FileFactory.GetFileTypeInstance(System.IO.Path.GetExtension(file.Name), this.blobRepository);

            return await fileProcesser.GetQualityCheckIssues(file, qualityCheck, qualityCheckTypes, sheetIds);
        }
    }
}
