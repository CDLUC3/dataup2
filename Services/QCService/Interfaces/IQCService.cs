// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.QCService.Interface
{
    /// <summary>
    /// Interface to expose QC related methods.
    /// </summary>
    public interface IQCService
    {
        /// <summary>
        /// Method to get all the available quality check rules.
        /// </summary>
        /// <param name="includeAdminRules">bool indicates if rules marked as AdminOnly should be returned or not</param>
        /// <returns>Quality check rules collection.</returns>
        IList<QualityCheckModel> GetQualityCheckRules(bool includeAdminRules);

        /// <summary>
        /// Method to get the quality check object by its id.
        /// </summary>
        /// <param name="qcId">Quality check id.</param>
        /// <returns>Quality check object.</returns>
        QualityCheckModel GetQualityCheckById(int qcId);

        /// <summary>
        /// Method to add or update the existing quality check rule.
        /// </summary>
        /// <param name="qualityCheck">Quality check object.</param>
        /// <returns>True incase of succeess.</returns>
        bool AddUpdateQualityCheck(QualityCheck qualityCheck);

        /// <summary>
        /// Method to delete the existing quality check rule.
        /// </summary>
        /// <param name="qualityCheckRuleId">Quality check rule id.</param>
        /// <returns>True incase of succeess.</returns>
        bool DeleteQualityCheckRule(int qualityCheckRuleId);

        /// <summary>
        /// Method to get all the available quality check column types.
        /// </summary>
        /// <returns>Collection of quality check column types.</returns>
        IEnumerable<QualityCheckColumnType> RetrieveQCColumnTypes();

        /// <summary>
        /// Method to check for the duplicate rule name.
        /// </summary>
        /// <param name="ruleName">Rule name.</param>
        /// <returns>Quality check rule id.</returns>
        int CheckRuleExists(string ruleName);

        /// <summary>
        /// Method to get Quality Check rules
        /// </summary>
        /// <param name="fileId">File Id</param>
        /// <param name="ruleId">Rule Id</param>
        /// <param name="sheetIds">Sheet Ids</param>
        /// <returns>Collection of quality check issues.</returns>
        Task<IEnumerable<QualityCheckResult>> GetQualityCheckIssues(int fileId, int ruleId, string sheetIds);
    }
}
