// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace Microsoft.Research.DataOnboarding.DataAccessService.Providers.EntityFramework
{
    public class QualityCheckRepository : RepositoryBase, IQualityCheckRepository
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QualityCheckRepository"/> class.
        /// </summary>
        /// <param name="dataContext">Data context.</param>
        public QualityCheckRepository(IUnitOfWork dataContext)
            : base(dataContext)
        {
        }

        #endregion

        /// <summary>
        /// Method to get all the available quality check rules.
        /// </summary>
        /// <returns>Quality check rules collection.</returns>
        public IEnumerable<QualityCheck> RetrieveQualityCheckRules()
        {
            return Context.QualityChecks
                .Include(qc => qc.QualityCheckColumnRules);
        }

        /// <summary>
        /// Method to get the quality check data by mentioned id.
        /// </summary>
        /// <param name="qualityCheckId">Quality check id.</param>
        /// <returns>Quality check object.</returns>
        public QualityCheck GetQualityCheckByID(int qualityCheckId)
        {
            return Context.QualityChecks
                .Include(qc => qc.QualityCheckColumnRules)
                .Where(qualChk => qualChk.QualityCheckId == qualityCheckId).FirstOrDefault();
        }

        /// <summary>
        /// Method to to add the new quality check rule.
        /// </summary>
        /// <param name="qualityCheck">Quality check rule.</param>
        /// <returns>Added quality check object.</returns>
        public QualityCheck AddQualityCheckRule(QualityCheck qualityCheck)
        {
            Check.IsNotNull<QualityCheck>(qualityCheck, "newQualityCheck");

            var addedQualityCheck = Context.QualityChecks.Add(qualityCheck);

            foreach (var columnRule in qualityCheck.QualityCheckColumnRules)
            {
                Context.SetEntityState<QualityCheckColumnRule>(columnRule, EntityState.Added);
            }

            return addedQualityCheck;
        }

        /// <summary>
        /// Method to to update the existing quality check rule.
        /// </summary>
        /// <param name="qualityCheck">Quality check rule.</param>
        /// <returns>Updated quality check object.</returns>
        public QualityCheck UpdateQualityCheckRule(QualityCheck qualityCheck)
        {
            Check.IsNotNull<QualityCheck>(qualityCheck, "modifiedQualityCheck");

            QualityCheck updatedQualityCheck = Context.QualityChecks.Attach(qualityCheck);


            foreach (var columnRule in qualityCheck.QualityCheckColumnRules)
            {
                Context.SetEntityState<QualityCheckColumnRule>(columnRule, EntityState.Added);
            }

            Context.SetEntityState<QualityCheck>(updatedQualityCheck, EntityState.Modified);

            return updatedQualityCheck;
        }

        /// <summary>
        /// Method to to delete the existing quality check rule.
        /// </summary>
        /// <param name="qualityCheckId">Quality check rule id.</param>
        /// <returns>Deleted quality check object.</returns>
        public QualityCheck DeleteQualityCheckRule(int qualityCheckId)
        {
            QualityCheck qualityCheckToDelete = Context.QualityChecks
                .Include(qc => qc.QualityCheckColumnRules)
                .Where(qualChk => qualChk.QualityCheckId == qualityCheckId).FirstOrDefault();
            Check.IsNotNull<QualityCheck>(qualityCheckToDelete, "qualityCheckToDelete");

            var delColumnRules = new List<QualityCheckColumnRule>();
            foreach (var columnRule in qualityCheckToDelete.QualityCheckColumnRules)
            {
                if (columnRule != null)
                {
                    delColumnRules.Add(columnRule);
                }
            }
            foreach (var deleteColumnRule in delColumnRules)
            {
                Context.SetEntityState<QualityCheckColumnRule>(deleteColumnRule, EntityState.Deleted);
                Context.QualityCheckColumnRules.Remove(deleteColumnRule);
            }
            Context.SetEntityState<QualityCheck>(qualityCheckToDelete, EntityState.Deleted);
            return Context.QualityChecks.Remove(qualityCheckToDelete);
        }

        /// <summary>
        /// Method to delete all the column rules for a quality check id.
        /// </summary>
        /// <param name="qualityCheckId">Quality check id.</param>
        /// <returns>Tue incase of success</returns>
        public bool DeleteColumnRules(int qualityCheckId)
        {
            var qualityCheckToDelete = Context.QualityCheckColumnRules.Where(columnRule => columnRule.QualityCheckId == qualityCheckId).ToList();
            Check.IsNotNull<List<QualityCheckColumnRule>>(qualityCheckToDelete, "qualityCheckToDelete");

            if (qualityCheckToDelete.Count > 0)
            {
                var delColumnRules = new List<QualityCheckColumnRule>();
                foreach (var columnRule in qualityCheckToDelete)
                {
                    delColumnRules.Add(columnRule);
                }
                foreach (var deleteColumnRule in delColumnRules)
                {
                    Context.SetEntityState<QualityCheckColumnRule>(deleteColumnRule, EntityState.Deleted);
                    Context.QualityCheckColumnRules.Remove(deleteColumnRule);
                }
            }
            // Context.SetEntityState<QualityCheck>(qualityCheckToDelete, EntityState.Modified);
            return true;
        }

        /// <summary>
        /// Method to to add the new file quality check rule data.
        /// </summary>
        /// <param name="fileQualityCheck">File quality check rule.</param>
        /// <returns>Added file quality check object.</return
        public FileQualityCheck AddFileQualityCheck(FileQualityCheck fileQualityCheck)
        {
            Check.IsNotNull<FileQualityCheck>(fileQualityCheck, "newQualityCheck");
            return Context.FileQualityChecks.Add(fileQualityCheck);
        }

        /// <summary>
        /// Method to get all the available quality check column types.
        /// </summary>
        /// <returns>Collection of column types.</returns>
        public IEnumerable<QualityCheckColumnType> RetrieveQCColumnTypes()
        {
            return Context.QualityCheckColumnTypes;
        }

        /// <summary>
        /// Method to check for the duplicate rule name.
        /// </summary>
        /// <param name="ruleName">Rule name.</param>
        /// <returns>Quality check object.</returns>
        public QualityCheck GetQualityCheckByName(string ruleName)
        {
            Check.IsNotNull<string>(ruleName, "ruleName");
            return Context.QualityChecks.Where(qcRule => qcRule.Name.Equals(ruleName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }
    }
}
