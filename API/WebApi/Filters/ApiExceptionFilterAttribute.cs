// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace Microsoft.Research.DataOnboarding.WebApi.Filters
{
    /// <summary>
    /// Exception filter for unhandled exceptions
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Executes when Unhandled Exception is thrown.
        /// </summary>
        /// <param name="context">ActionExecuted Context</param>
        public override void OnException(HttpActionExecutedContext context)
        {
            DiagnosticsProvider diagnostics = new DiagnosticsProvider(this.GetType());
            Exception exception = context.Exception;
            diagnostics.WriteErrorTrace(TraceEventId.Exception, exception);
            context.Response = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception.Message);
        }
    }
}