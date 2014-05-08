using Microsoft.Research.DataOnboarding.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    public class PublishModel
    {
        public File File { get; set; }
        public Repository Repository { get; set; }
        public int UserId { get; set; }
        public AuthToken AuthToken { get; set; }

        /// <summary>
        /// Gets or sets user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Get or sets RPS authentication token.
        /// </summary>
        public string RPSToken { get; set; }

        /// <summary>
        /// Gets or sets who is positing the file.
        /// </summary>
        public string Who { get; set; }

        /// <summary>
        /// Gets or sets what user is posting.
        /// </summary>
        public string What { get; set; }

        /// <summary>
        /// Gets or sets when user is posting.
        /// </summary>
        public DateTime When { get; set; }
    }
}
