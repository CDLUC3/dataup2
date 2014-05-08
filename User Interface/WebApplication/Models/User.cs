// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Microsoft.Research.DataOnboarding.WebApplication.Models
{
    /// <summary>
    /// Class to store the user model information and to pass the object to API and get 
    /// </summary>
    [DataContract]
    public class User
    {
        /// <summary>
        /// Gets or sets the user Id.
        /// </summary>
        [DataMember]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is Active or not.
        /// </summary>        
        [DataMember]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the Name Identifier.
        /// </summary>
        [DataMember]
        public string NameIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the First Name.
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the Middle Name
        /// </summary>
        [DataMember]
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the Last Name
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the Organization
        /// </summary>
        [DataMember]
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets the Email Id
        /// </summary>
        [DataMember]
        public string EmailId { get; set; }

        /// <summary>
        /// Gets or sets the Roles
        /// </summary>
        [DataMember]
        public List<string> Roles { get; set; }
    }
}