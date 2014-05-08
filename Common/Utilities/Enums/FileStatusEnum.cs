// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.Utilities.Enums
{
    /// <summary>
    /// Enum of file status.
    /// </summary>
    public enum FileStatus
    {
        /// <summary>
        /// None status enum value.
        /// </summary>
        None = 0,

        /// <summary>
        /// Uploaded status enum value.
        /// </summary>
        Uploaded = 1,

        /// <summary>
        /// Posted status enum value.
        /// </summary>
        Posted = 2,

        /// <summary>
        /// Inqueue status enum value.
        /// </summary>
        Inqueue = 3,

        /// <summary>
        /// Verifying status enum value.
        /// </summary>
        Verifying = 4,

        /// <summary>
        /// Error status enum value.
        /// </summary>
        Error = 5,
    }
}