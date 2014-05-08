// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    /// <summary>
    /// Model class for repository data with created user.
    /// </summary>
    public class RepositoryDataModel
    {
        /// <summary>
        /// Gets or sets Repository object.
        /// </summary>
        public Repository RepositoryData { get; set; }

        /// <summary>
        /// Gets or sets created user name.
        /// </summary>
        public string CreatedUser { get; set; }

        /// <summary>
        /// Gets or sets authentication type ( Repository type)
        /// </summary>
        public string AuthenticationType { get; set; }
    }
}
