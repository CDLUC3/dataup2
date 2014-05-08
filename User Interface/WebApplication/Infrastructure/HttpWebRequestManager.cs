// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using System.Net;
using System.Web;
using Microsoft.Research.DataOnboarding.WebApplication.Helpers;
using Utility = Microsoft.Research.DataOnboarding.Utilities;
using System;
using Microsoft.Research.DataOnboarding.WebApplication.Resource;
using System.Text;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System.Web.Script.Serialization;

namespace Microsoft.Research.DataOnboarding.WebApplication.Infrastructure
{
    public class HttpWebRequestManager
    {

        private HttpWebRequest webRequest;

        private byte[] data;

        public HttpWebRequest HttpWebRequest
        {
            get
            {
                return webRequest;
            }
            set
            {
                webRequest = value;
            }
        }

        /// <summary>
        /// Method to set Request Details
        /// </summary>
        /// <param name="requestParam">Request parameters</param>
        /// <param name="repositoryCredentials">Repository Credentials</param>
        public void SetRequestDetails(RequestParams requestParam, RepositoryCredentials repositoryCredentials = null)
        {
            Utility.Check.IsNotNull(requestParam, "RequestParams");

            var jwtToken = HttpContext.Current.Request.Cookies[Constants.APPJWTCOOKIETOKEN].Value;
            WebRequest request = System.Net.WebRequest.Create(requestParam.RequestURL);
            
            webRequest = request as HttpWebRequest;

            webRequest.Headers.Add(HttpRequestHeader.Authorization, jwtToken);

            if (repositoryCredentials != null)
            {
                string serilizedRepositoryCredentials = new JavaScriptSerializer().Serialize(repositoryCredentials);
                webRequest.Headers.Add(Utility.Constants.RepositoryCredentialHeaderName, serilizedRepositoryCredentials);
            }

            webRequest.Method = requestParam.RequestMode.ToString();
            if (!string.IsNullOrEmpty(requestParam.ContentType))
            {
                webRequest.ContentType = requestParam.ContentType;
            }
            if (requestParam.TimeOut > 0)
            {
                webRequest.Timeout = requestParam.TimeOut;
            }

            webRequest.AllowAutoRedirect = requestParam.AllowAutoRedirect;

            // TODO 
            if (requestParam.Values != null && requestParam.Values.Count>0)
            {
                UTF8Encoding encoding = new UTF8Encoding();
                data = encoding.GetBytes(requestParam.Values[0]);
                request.ContentLength = data.Length;
                Stream postStream = webRequest.GetRequestStream();
                postStream.Write(data, 0, data.Length);
                postStream.Close();
            }

        }

        /// <summary>
        /// Method to get web response 
        /// </summary>
        /// <returns>returns view</returns>
        public string RetrieveWebResponse()
        {
            string webResponse = string.Empty;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse) HttpWebRequest.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        webResponse = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException we)
            {
                if (we != null && we.Response != null)
                {
                    Stream st = ((System.Net.WebException)(we)).Response.GetResponseStream();
                    StreamReader reader = new StreamReader(st);
                    string text = reader.ReadToEnd();

                    HttpWebResponse response = we.Response as HttpWebResponse;
                    if (response.StatusCode.Equals(HttpStatusCode.NotFound))
                    {
                        if (text.Contains(Messages.CurruptFileErrMsg))
                        {
                            return Messages.CurruptFileErrMsg;
                        }
                        else
                        {
                            throw we;
                        }
                    }
                    else
                    {
                        return Utilities.Constants.ERROR;
                    }
                }
                else
                {
                    return Utilities.Constants.ERROR;
                }
            }
            return webResponse;
        }

        /// <summary>
        /// Method to get web response 
        /// </summary>
        /// <returns>returns view</returns>
        public string RetrieveWebResponse(out string redirection)
        {
            string webResponse = string.Empty;
            redirection = null;
            try
            {
                using (HttpWebResponse response = HttpWebRequest.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        webResponse = reader.ReadToEnd();
                        if(response.StatusCode ==  HttpStatusCode.Moved)
                        {
                            redirection = response.GetResponseHeader("Location");
                        }
                    }
                }
            }
            catch (WebException we)
            {
                if (we != null && we.Response != null)
                {
                    Stream st = ((System.Net.WebException)(we)).Response.GetResponseStream();
                    StreamReader reader = new StreamReader(st);
                    string text = reader.ReadToEnd();

                    HttpWebResponse response = we.Response as HttpWebResponse;
                    if (response.StatusCode.Equals(HttpStatusCode.NotFound))
                    {
                        if (text.Contains(Messages.CurruptFileErrMsg))
                        {
                            return Messages.CurruptFileErrMsg;
                        }
                        else
                        {
                            throw we;
                        }
                    }
                    else
                    {
                        return Utilities.Constants.ERROR;
                    }
                }
                else
                {
                    return Utilities.Constants.ERROR;
                }
            }

            return webResponse;
        }


        public HttpWebResponse GetHtppResponse()
        {
            try
            {
                return HttpWebRequest.GetResponse() as HttpWebResponse;
            }
            catch (WebException we)
            {
                throw we;
            }
        }



    }
}