// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.WebApplication.ViewModels;
using Microsoft.Research.DataOnboarding.WebApplication.Helpers;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.Research.DataOnboarding.Utilities.Extensions;

namespace Microsoft.Research.DataOnboarding.WebApplication.Extensions
{
    /// <summary>
    /// Extension method class for quality check rule viewmodel.
    /// </summary>
    public static class QCRuleViewModelExtension
    {
        /// <summary>
        /// Extension method class for quality check rule viewmodel.
        /// </summary>
        /// <param name="rule">Quality check rule view model.</param>
        /// <param name="qualityCheck">Quality check model.</param>
        /// <param name="lstQCColumnTypes">Quality check column types.</param>
        public static void SetValuesFrom(this QCRuleViewModel rule, QualityCheckModel qualityCheck, IEnumerable<QualityCheckColumnType> lstQCColumnTypes)
        {
            Check.IsNotNull<QCRuleViewModel>(rule, "qcRuleViewModel");
            Check.IsNotNull<QualityCheckModel>(qualityCheck, "qualityCheckModel");
            Check.IsNotNull<QualityCheck>(qualityCheck.QualityCheckData, "qualityCheck");

            rule.CreatedDate = qualityCheck.QualityCheckData.CreatedOn.ToClientTime().ToString();
            rule.CreatedUser = qualityCheck.CreatedUser;
            rule.IsOrderRequired = qualityCheck.QualityCheckData.EnforceOrder == null ? false : Convert.ToBoolean(qualityCheck.QualityCheckData.EnforceOrder);
            rule.IsVisibleToAll = qualityCheck.QualityCheckData.IsVisibleToAll == null ? true : Convert.ToBoolean(qualityCheck.QualityCheckData.IsVisibleToAll);
            rule.QCRuleDescription = qualityCheck.QualityCheckData.Description;
            rule.QCRuleId = qualityCheck.QualityCheckData.QualityCheckId;
            rule.QCRuleName = qualityCheck.QualityCheckData.Name;
            rule.CreatedBy = qualityCheck.QualityCheckData.CreatedBy;
            rule.VisibilityOption = (qualityCheck.QualityCheckData.IsVisibleToAll != null && qualityCheck.QualityCheckData.IsVisibleToAll == true) ? 1 : 2;

            foreach (var columnRule in qualityCheck.QualityCheckData.QualityCheckColumnRules)
            {
                QCHeaderViewModel headerModel = new QCHeaderViewModel();
                headerModel.ColumnTypeId = columnRule.QualityCheckColumnTypeId;
                headerModel.ErrorMessage = columnRule.ErrorMessage;
                headerModel.HeaderName = columnRule.HeaderName;
                headerModel.IsRequired = columnRule.IsRequired == null ? false : Convert.ToBoolean(columnRule.IsRequired);
                headerModel.QCColumnRuleId = columnRule.QualityCheckColumnRuleId;
                headerModel.Order = columnRule.Order;
                headerModel.QCColumnTypes = new SelectList(lstQCColumnTypes, "QualityCheckColumnTypeId", "Name", columnRule.QualityCheckColumnTypeId);

                if (!string.IsNullOrWhiteSpace(columnRule.Range))
                {
                    var rangeArray = columnRule.Range.Split(new string[] { Utilities.Constants.RangeSeparator }, StringSplitOptions.None);
                    headerModel.RangeStart = rangeArray[0];
                    headerModel.RangeEnd = rangeArray[1];
                }

                rule.LstHeaderNames.Add(headerModel);
            }
        }

        /// <summary>
        /// Extension method class for quality check rule viewmodel to put data on quality check domain model class.
        /// </summary>
        /// <param name="rule">Quality check rule view model.</param>
        /// <param name="qualityCheck">Quality check model.</param>
        /// <returns>Updated qualityCheck model class object.</returns>
        public static QualityCheck SetValuesTo(this QCRuleViewModel rule, QualityCheck qualityCheck)
        {
            Check.IsNotNull<QCRuleViewModel>(rule, "qcRuleViewModel");
            Check.IsNotNull<QualityCheck>(qualityCheck, "qualityCheck");
            qualityCheck.QualityCheckColumnRules = new List<QualityCheckColumnRule>();

            if (rule.QCRuleId == 0)
            {
                qualityCheck.CreatedOn = DateTime.UtcNow;
                qualityCheck.CreatedBy = BaseController.UserId;
            }
            else
            {
                qualityCheck.CreatedOn = Convert.ToDateTime(rule.CreatedDate).ToUTCFromClientTime();
                qualityCheck.CreatedBy = rule.CreatedBy;
                qualityCheck.ModifiedOn = DateTime.UtcNow;
                qualityCheck.ModifiedBy = BaseController.UserId;
            }

            qualityCheck.EnforceOrder = rule.IsOrderRequired;
            qualityCheck.IsVisibleToAll = rule.IsVisibleToAll;
            qualityCheck.Description = rule.QCRuleDescription;
            qualityCheck.QualityCheckId = rule.QCRuleId;
            qualityCheck.Name = rule.QCRuleName;
            qualityCheck.IsActive = true;
            qualityCheck.IsVisibleToAll = rule.VisibilityOption == 1 ? true : false;

            foreach (var headerModel in rule.LstHeaderNames)
            {
                if (!string.IsNullOrEmpty(headerModel.HeaderName))
                {
                    QualityCheckColumnRule columnRule = new QualityCheckColumnRule();
                    columnRule.QualityCheckColumnTypeId = headerModel.ColumnTypeId;
                    columnRule.ErrorMessage = headerModel.ErrorMessage;
                    columnRule.Description = headerModel.HeaderName;
                    columnRule.HeaderName = headerModel.HeaderName;
                    columnRule.IsRequired = true;
                    columnRule.IsActive = true;
                    columnRule.QualityCheckColumnRuleId = headerModel.QCColumnRuleId;
                    columnRule.QualityCheckId = qualityCheck.QualityCheckId;
                    columnRule.Order = headerModel.Order;

                    if (!string.IsNullOrEmpty(headerModel.RangeStart) || !string.IsNullOrEmpty(headerModel.RangeEnd))
                    {
                        columnRule.Range = (headerModel.RangeStart == null ? string.Empty : headerModel.RangeStart) + Utilities.Constants.RangeSeparator + (headerModel.RangeEnd == null ? string.Empty : headerModel.RangeEnd);
                    }

                    qualityCheck.QualityCheckColumnRules.Add(columnRule);
                }
            }

            return qualityCheck;
        }
    }
}