// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.DataAccessService
{
    /// <summary>
    /// Interface to expose quality check related methods.
    /// </summary>
    public interface IQualityCheckRepository
    {
        /// <summary>
        /// Method to get all the available quality check rules.
        /// </summary>
        /// <returns>Quality check rules collection.</returns>
        IEnumerable<QualityCheck> RetrieveQualityCheckRules();

        /// <summary>
        /// Method to get the quality check data by mentioned id.
        /// </summary>
        /// <param name="qualityCheckId">Quality check id.</param>
        /// <returns>Quality check object.</returns>
        QualityCheck GetQualityCheckByID(int qualityCheckId);

        /// <summary>
        /// Method to to add the new quality check rule.
        /// </summary>
        /// <param name="qualityCheck">Quality check rule.</param>
        /// <returns>Added quality check object.</returns>
        QualityCheck AddQualityCheckRule(QualityCheck qualityCheck);

        /// <summary>
        /// Method to to update the existing quality check rule.
        /// </summary>
        /// <param name="qualityCheck">Quality check rule.</param>
        /// <returns>Updated quality check object.</returns>
        QualityCheck UpdateQualityCheckRule(QualityCheck qualityCheck);

        /// <summary>
        /// Method to to delete the existing quality check rule.
        /// </summary>
        /// <param name="qualityCheckId">Quality check rule id.</param>
        /// <returns>Deleted quality check object.</returns>
        QualityCheck DeleteQualityCheckRule(int qualityCheckId);

        /// <summary>
        /// Method to to add the new file quality check rule data.
        /// </summary>
        /// <param name="fileQualityCheck">File quality check rule.</param>
        /// <returns>Added file quality check object.</returns>
        FileQualityCheck AddFileQualityCheck(FileQualityCheck fileQualityCheck);

        /// <summary>
        /// Method to get all the available quality check column types.
        /// </summary>
        /// <returns>Collection of column types.</returns>
        IEnumerable<QualityCheckColumnType> RetrieveQCColumnTypes();

        /// <summary>
        /// Method to check for the duplicate rule name.
        /// </summary>
        /// <param name="ruleName">Rule name.</param>
        /// <returns>Quality check object.</returns>
        QualityCheck GetQualityCheckByName(string ruleName);

        /// <summary>
        /// Method to delete all the column rules for a quality check id.
        /// </summary>
        /// <param name="qualityCheckId">Quality check id.</param>
        /// <returns>Tue incase of success</returns>
        bool DeleteColumnRules(int qualityCheckId);
    }
}
