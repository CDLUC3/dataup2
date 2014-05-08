// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.Research.DataOnboarding.RepositoryAdapters.SkyDrive
{
    [DataContract]
    public class OAuthToken
    {
        [DataMember(Name = SkyDriveConstants.AccessToken)]
        public string AccessToken { get; set; }

        [DataMember(Name = SkyDriveConstants.AuthenticationToken)]
        public string AuthenticationToken { get; set; }

        [DataMember(Name = SkyDriveConstants.RefreshToken)]
        public string RefreshToken { get; set; }

        [DataMember(Name = SkyDriveConstants.ExpiresIn)]
        public string ExpiresIn { get; set; }

        [DataMember(Name = SkyDriveConstants.Scope)]
        public string Scope { get; set; }

        public DateTime TokenExpiresOn { get; set; }
    }
}