// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.Utilities;

namespace Microsoft.Research.DataOnboarding.FilePurgeService.Helpers
{
    /// <summary>
    /// Class representing all the constants used across the application.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Gets the schedule time for the worker role in hours.
        /// </summary>
        public static double ScheduledTimeInHours
        {
            get
            {
                return ConfigReader<double>.GetSetting("ScheduledTimeInHours", 24);
            }
        }

        /// <summary>
        /// Gets the number of hours for which we have to keep the files in azure storage.
        /// </summary>
        public static double UploadedFilesExpirationDurationInHours
        {
            get
            {
                return ConfigReader<double>.GetSetting("UploadedFilesExpirationDurationInHours", 72);
            }
        }
    }
}
