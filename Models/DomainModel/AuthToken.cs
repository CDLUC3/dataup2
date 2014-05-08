// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.DomainModel
{
    public class AuthToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RespositoryId { get; set;}
        public string AccessToken { get; set;}
        public string RefreshToken { get; set; }
        public DateTime TokenExpiresOn { get; set; }
    }
}
