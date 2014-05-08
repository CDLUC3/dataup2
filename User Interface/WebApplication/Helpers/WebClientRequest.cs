// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.Research.DataOnboarding.WebApplication.Helpers;
using Utility = Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.WebApplication.Infrastructure;

namespace Microsoft.Research.DataOnboarding.WebApplication.Helpers
{
    public class WebClientRequest : WebClient
    {
        public bool AllowAutoRedirect { get; set; }

        public WebClientRequest(RequestParams requestParam)
        {
            this.AllowAutoRedirect = requestParam.AllowAutoRedirect;
        }

        protected override System.Net.WebRequest GetWebRequest(Uri address)
        {
            // Allow the base method to create the WebRequest and try to cast it to HttpWebRequest
            System.Net.HttpWebRequest request = base.GetWebRequest(address) as System.Net.HttpWebRequest;

            if (request == null)
                // TODO: create resource string for exception message
                throw new ArgumentException("WebClient requires the address argument to use the http protocol.",
                "address");

            // ** disable auto-redirection **
            request.AllowAutoRedirect = this.AllowAutoRedirect;
            request.Timeout = 120000;
            return request;
        }
       
    }
}