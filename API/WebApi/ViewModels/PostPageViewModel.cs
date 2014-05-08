using Microsoft.Research.DataOnboarding.WebApi.Models;
using System.Collections.Generic;

namespace Microsoft.Research.DataOnboarding.WebApi.ViewModels
{
    /// <summary>
    /// View model for post page.
    /// </summary>
    public class PostPageViewModel
    {
        /// <summary>
        /// Initializes a new instance of PostPageViewModel
        /// </summary>
        public PostPageViewModel()
        {
            this.RepositoryList = new List<Repository>();
        }

        /// <summary>
        /// Gets or sets file id.
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Gets or sets file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the repository id.
        /// </summary>
        public int RepositoryId { get; set; }

        /// <summary>
        /// Gets or sets user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets repository list
        /// </summary>
        public List<Repository> RepositoryList { get; set; }
    }
}