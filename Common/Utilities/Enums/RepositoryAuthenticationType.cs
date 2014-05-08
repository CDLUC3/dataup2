// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.Utilities.Enums
{
    /// <summary>
    /// Enum of Repository Authentication Types.
    /// </summary>
    public enum RepositoryAuthenticationType
    {
        /// <summary>
        /// None value.
        /// </summary>
        None = 0,

        /// <summary>
        /// User has to provide the credentials.
        /// </summary>
        UserProvided = 1,

        /// <summary>
        /// Admin will provide while creating the repository.
        /// </summary>
        SystemProvided = 2,

        /// <summary>
        /// Live id authentication for reposiotry.
        /// </summary>
        LiveIDAuthentication = 3,
    }
}
