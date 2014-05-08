// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
   
    public class UserAuthTokenStatusModel
    {
        public UserAuthTokenStatusModel()
        {
           this.RedirectionRequired = false;
        }

        public bool RedirectionRequired { get; set; }
    }
}