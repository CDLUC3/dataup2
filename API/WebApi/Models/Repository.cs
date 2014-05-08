namespace Microsoft.Research.DataOnboarding.WebApi.Models
{
    /// <summary>
    /// Model class for repository.
    /// </summary>
    public class Repository
    {
        /// <summary>
        /// Gets or sets repository id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets repository name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets base repository id.
        /// </summary>
        public int BaseRepositoryId { get; set; }

        /// <summary>
        /// Gets or sets indicating whether the repository is impersonating.
        /// </summary>
        public bool IsImpersonating { get; set; }

        /// <summary>
        /// Gets or sets the user agreement.
        /// </summary>
        public string UserAgreement { get; set; }
    }
}
