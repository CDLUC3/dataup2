// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    /// <summary>
    /// RepositoryBase class
    /// </summary>
    public class RepositoryModel
    {
        /// <summary>
        /// authorization field
        /// </summary>
        private string authorization;

        /// <summary>
        /// repositoryName field
        /// </summary>
        private string repositoryName;

        /// <summary>
        /// repositoryUrl field
        /// </summary>
        private string repositoryUrl;

        /// <summary>
        /// Variable for selected repository data.
        /// </summary>
        private Repository selectedRepository;

        public RepositoryModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase" /> class.
        /// </summary>
        /// Initializes a new instance of the <see cref="RepositoryBase" /> class.
        /// <param name="authentication">Authentication parameter</param>
        /// <param name="name">Name parameter</param>
        /// <param name="url">Url parameter </param>
        public RepositoryModel(string authentication, string name, string url, Repository selectedRepository)
        {
            this.authorization = authentication;
            this.repositoryName = name;
            this.repositoryUrl = url;
            this.selectedRepository = selectedRepository;
        }

        /// <summary>
        /// Gets or sets the  Authorization
        /// </summary>
        public string Authorization
        {
            get
            {
                return this.authorization;
            }

            set
            {
                this.authorization = value;
            }
        }

        /// <summary>
        /// Gets or sets the RepositoryName
        /// </summary>
        public string RepositoryName
        {
            get { return this.repositoryName; }
            set { this.repositoryName = value; }
        }

        /// <summary>
        /// Gets or sets the RepositoryLink
        /// </summary>
        public string RepositoryLink
        {
            get { return this.repositoryUrl; }
            set { this.repositoryUrl = value; }
        }

        /// <summary>
        /// Gets or sets the selected reopsitory.
        /// </summary>
        public Repository SelectedRepository
        {
            get { return this.selectedRepository; }
            set { this.selectedRepository = value; }
        }
    }
}
