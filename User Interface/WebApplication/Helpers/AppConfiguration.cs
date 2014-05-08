using System.Runtime.Serialization;

namespace Microsoft.Research.DataOnboarding.WebApplication.Helpers
{
    /// <summary>
    /// Model to hold the configuration information that can be serailized.
    /// </summary>
    public class AppConfiguration
    {
        /// <summary>
        /// Gets or sets BaseWebApiUri
        /// </summary>
        public string BaseWebApiUri { get; set; }

        /// <summary>
        /// Gets or sets FileApiUri
        /// </summary>
        public string FileApiUri { get; set; }

        /// <summary>
        /// Gets or sets UserApiUri
        /// </summary>
        public string UserApiUri { get; set; }

        /// <summary>
        /// Gets or sets RepositoryApiUri
        /// </summary>
        public string RepositoryApiUri { get; set; }

        /// <summary>
        /// Gets or sets RepositoryTypesApiUri
        /// </summary>
        public string RepositoryTypesApiUri { get; set; }

        /// <summary>
        /// Gets or sets QCApiUri
        /// </summary>
        public string QCApiUri { get; set; }

        /// <summary>
        /// Gets or sets WindowsLiveAuthUri
        /// </summary>
        public string WindowsLiveAuthUri { get; set; }

        /// <summary>
        /// Gets or sets BlobApiUri
        /// </summary>
        public string BlobApiUri { get; set; }

        /// <summary>
        /// Gets or sets SignOutApiUri
        /// </summary>
        public string SignOutApiUri { get; set; }

        /// <summary>
        /// Gets or sets PublishApiUri
        /// </summary>
        public string PublishApiUri { get; set; }

        /// <summary>
        /// Gets or sets SupportedIdentityProvidersAPIUri
        /// </summary>
        public string SupportedIdentityProvidersAPIUri { get; set; }

        /// <summary>
        /// Gets or sets the File Type Delimeter
        /// </summary>
        public string FileTypeDelimeter { get; set; }

        /// <summary>
        /// Gets or sets the AuthTokenUri
        /// </summary>
        public string AuthTokenUri { get; set; }

        /// <summary>
        /// Gets or sets the SignOutCallback
        /// </summary>
        public string SignOutCallbackUri { get; set; }
    }
}