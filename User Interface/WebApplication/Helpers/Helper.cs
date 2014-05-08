// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Mvc;

namespace Microsoft.Research.DataOnboarding.WebApplication.Helpers
{
    /// <summary>
    /// Helper class for helper methods.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Helper method to format the file size.
        /// </summary>
        /// <param name="bytes">Value in bytes.</param>
        /// <returns>Value in converted format.</returns>
        public static string FormatFileSize(long bytes)
        {
            if (bytes > Constants.MEGABYTESSIZE)
            {
                return (((float)bytes / (float)Constants.MEGABYTESSIZE)).ToString(string.Format("{0} {1}", "0.00", Constants.MEGABYTEUNITNAME));
            }
            else if (bytes > Constants.KILOBYTESSIZE)
            {
                return ((float)bytes / (float)Constants.KILOBYTESSIZE).ToString(string.Format("{0} {1}", "0.00", Constants.KILOBYTEUNITNAME));
            }
            else
            {
                return bytes + " " + Constants.BYTESUNITNAME;
            }
        }

        /// <summary>
        /// Helper method to return the visibility options as selecte list
        /// </summary>
        /// <returns>required slect list.</returns>
        public static SelectList GetVisibilityList()
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Insert(0, new SelectListItem() { Value = "2", Text = "Visible to Admin" });
            listItems.Insert(1, new SelectListItem() { Value = "1", Text = "Visible to All" });
            return new SelectList((IEnumerable<SelectListItem>)listItems, "Value", "Text");
        }
    }
}