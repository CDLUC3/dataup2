// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.Research.DataOnboarding.DomainModel.ConceptualModel
{
    /// <summary>
    /// Model class for Citation.
    /// </summary>
    [DataContract]
    public class Citation
    {
        /// <summary>
        /// Gets or sets PublicationYear.
        /// </summary>
        [DataMember]
        public string PublicationYear { get; set; }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Version.
        /// </summary>
        [DataMember]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets Publisher.
        /// </summary>
        [DataMember]
        public string Publisher { get; set; }
    }
}
