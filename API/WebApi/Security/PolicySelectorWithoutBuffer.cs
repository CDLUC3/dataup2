// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.WebHost;

namespace Microsoft.Research.DataOnboarding.WebApi.Security
{
    public class PolicySelectorWithoutBuffer : WebHostBufferPolicySelector
    {
        public override bool UseBufferedInputStream(object hostContext)
        {
            var context = hostContext as HttpContextBase;
            if (context != null)
            {
                if (string.Compare(context.Request.RequestContext.RouteData.Values["controller"].ToString(), "blob", StringComparison.InvariantCultureIgnoreCase) == 0)
                    return false;
            }
            return true;
        }
        public override bool UseBufferedOutputStream(HttpResponseMessage response)
        {
            return base.UseBufferedOutputStream(response);
        }
    }  
}