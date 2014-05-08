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

namespace Microsoft.Research.DataOnboarding.WebApplication.Infrastructure
{
    /// <summary>
    /// class to send the request using web client
    /// </summary>
    public class WebClientRequestManager 
    {
        public HttpStatusCode StatusCode { get; set; }
        public string RedirectionURL { get; set; }

        public WebClientRequestManager()
        {
        }
                
        public string UploadValues(RequestParams requestParam)
        {
            string responseString = "true";
            Utility.Check.IsNotNull(requestParam, "RequestParams");
            //  call the api with data file 
            using (WebClientRequest client = new WebClientRequest(requestParam))
            {
                client.Headers.Add("Authorization", HttpContext.Current.Request.Cookies[Constants.APPJWTCOOKIETOKEN].Value);
                byte[] response = client.UploadValues(new Uri(requestParam.RequestURL), requestParam.RequestMode.ToString(), requestParam.Values);
                this.RedirectionURL =  client.ResponseHeaders["Location"];
                responseString = Encoding.UTF8.GetString(response);
            }

            return responseString;
        }
       
      }
}
