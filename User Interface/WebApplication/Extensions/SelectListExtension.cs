// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.Research.DataOnboarding.WebApplication.Extensions
{
    /// <summary>
    /// Extension class for Select List to add the first item.
    /// </summary>
    public static class SelectListExtension
    {
        /// <summary>
        /// Extension method for Select List to add the first item.
        /// </summary>
        /// <param name="selectList">Select list object.</param>
        /// <param name="value">Value string.</param>
        /// <param name="text">Text string.</param>
        /// <returns>Updated select list object.</returns>
        public static SelectList SetFirstItem(this SelectList selectList, string value, string text)
        {
            List<SelectListItem> listItems = selectList.ToList();
            listItems.Insert(0, new SelectListItem() { Value = value, Text = text });
            return new SelectList((IEnumerable<SelectListItem>)listItems, "Value", "Text");
        }
    }
}