// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Web;

namespace Microsoft.Research.DataOnboarding.WebApplication.Extensions
{
    /// <summary>
    /// Extension class for Date time to get the client date time.
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// Converts the specified Date Time to the client Date Time.
        /// </summary>
        /// <param name="dateTime">Date time object to be converted.</param>
        /// <returns>Date time in client's time zone.</returns>
        public static DateTime ToClientTime(this DateTime dateTime)
        {
            var timeOffSet = HttpContext.Current.Session["__TimezoneOffset"];

            if (timeOffSet != null)
            {
                var offset = int.Parse(timeOffSet.ToString());
                dateTime = dateTime.AddMinutes(-1 * offset);
            }

            return dateTime;
        }

        /// <summary>
        /// Converts the specified client time to UTC Date Time.
        /// </summary>
        /// <param name="dateTime">Date time object to be converted.</param>
        /// <returns>Date time in UTC time zone.</returns>
        public static DateTime ToUTCFromClientTime(this DateTime dateTime)
        {
            var timeOffSet = HttpContext.Current.Session["__TimezoneOffset"];

            if (timeOffSet != null)
            {
                var offset = int.Parse(timeOffSet.ToString());
                dateTime = dateTime.AddMinutes(1 * offset);
            }

            return dateTime;
        }
    }
}