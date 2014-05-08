// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;

namespace Microsoft.Research.DataOnboarding.WebApi
{
    public class CorsHandler : DelegatingHandler
    {
        private const string Origin = "Origin";
        private const string AccessControlRequestMethod = "Access-Control-Request-Method";
        private const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
        private const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        private const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        private const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";

        private readonly DiagnosticsProvider diagnostics;

        public CorsHandler()
        {
            diagnostics = new DiagnosticsProvider(this.GetType());
        }

        /// <summary>
        /// Static method to add response headers.
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <param name="response">Response object.</param>
        private static void AddCorsResponseHeaders(HttpRequestMessage request, HttpResponseMessage response)
        {
            response.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());

            string accessControlRequestMethod = request.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault();
            if (accessControlRequestMethod != null)
            {
                response.Headers.Add(AccessControlAllowMethods, accessControlRequestMethod);
            }

            string requestedHeaders = string.Join(", ", request.Headers.GetValues(AccessControlRequestHeaders));
            if (!string.IsNullOrEmpty(requestedHeaders))
            {
                response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);
            }
        }

        /// <summary>
        /// Send asynchronous method.
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <param name="cancellationToken">CancellationToken object.</param>
        /// <returns>Response message.</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            diagnostics.WriteVerboseTrace(TraceEventId.InboundParameters,
                                        "HttpMethod:{0}, Url:{1}", request.Method.ToString(), request.RequestUri.AbsoluteUri);
            return request.Headers.Contains(Origin) ?
                this.ProcessCorsRequest(request, ref cancellationToken) :
                base.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// Method to process core request.
        /// </summary>
        /// <param name="request">Request message.</param>
        /// <param name="cancellationToken">CancellationToken object.</param>
        /// <returns>Response message.</returns>
        private Task<HttpResponseMessage> ProcessCorsRequest(HttpRequestMessage request, ref CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Options)
            {
                diagnostics.WriteVerboseTrace(TraceEventId.InboundParameters,
                                        "[HTTP OPTIONS] Origin:{0},Method:{1},Headers:{2}", 
                                        request.Headers.GetValues(Origin).First(),
                                        request.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault(),
                                        string.Join(", ", request.Headers.GetValues(AccessControlRequestHeaders)));
                return Task.Factory.StartNew<HttpResponseMessage>(() =>
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    AddCorsResponseHeaders(request, response);
                    return response;
                }, cancellationToken);
            }
            else
            {
                return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>(task =>
                {
                    HttpResponseMessage resp = task.Result;
                    resp.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());
                    return resp;
                });
            }
        }
    }
}