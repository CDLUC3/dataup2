// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.DomainModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for User domain and conceptual models/projections
    /// The intent is to avoid code duplication across model types
    /// </summary>
    public abstract class UserBase
    {
        public int UserId { get; set; }
        public string NameIdentifier { get; set; }
        public string IdentityProvider { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Organization { get; set; }
        public string EmailId { get; set; }
        public bool IsActive { get; set; }
    }
}
