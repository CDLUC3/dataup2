// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Web;

namespace Microsoft.Research.DataOnboarding.WebApi
{
    public class GetBufferlessInputStreamModule : IHttpModule
    {
        /// <summary>
        /// Initializes the http module.
        /// </summary>
        /// <param name="context">The HttpApplication object</param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += HttpApplication_BeginRequest;
        }

        /// <summary>
        /// Disposes managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {

        }

        private void HttpApplication_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication httpApplication = sender as HttpApplication;
            if (httpApplication != null)
            {
                NameValueCollection nameValueCollection = (NameValueCollection)ConfigurationManager.GetSection("bufferlessInputStreamControllers");
                foreach (string key in nameValueCollection.AllKeys)
                {
                    if (string.Equals(nameValueCollection[key], httpApplication.Request.Path, StringComparison.OrdinalIgnoreCase) && string.Equals(httpApplication.Request.HttpMethod, WebRequestMethods.Http.Post.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        //This will force the app.Request to cache the current
                        //stream as a bufferless input stream.
                        var stream = httpApplication.Request.GetBufferlessInputStream(true);
                        return;
                    }
                }
            }
        }
    }
}