
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    /// <summary>
    /// Model to hold the Repository Credentials.
    /// </summary>
    [DataContract]
    public class RepositoryCredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryCredentials"/> class.
        /// </summary>
        public RepositoryCredentials()
        {
            Attributes = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the Attributes
        /// </summary>
        [DataMember]
        public Dictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// Gets or sets the value for the key
        /// </summary>
        /// <param name="key">Key Name.</param>
        /// <returns>Key Value.</returns>
        public string this[string key]
        {
            get
            {
                string keyValue;

                if (this.Attributes.TryGetValue(key, out keyValue))
                {
                    return keyValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                string keyValue;
                if (!this.Attributes.TryGetValue(key, out keyValue))
                {
                    this.Attributes.Add(key, value);
                }
                else
                {
                    this.Attributes[key] = value;
                }
            }
        }
    }
}
