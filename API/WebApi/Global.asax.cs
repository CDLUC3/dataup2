// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.WebApi.Security;
using Microsoft.WindowsAzure;

namespace Microsoft.Research.DataOnboarding.WebApi
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class WebApiApplication : System.Web.HttpApplication
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Auto-Generated Code")]
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            DependencyResolverConfig.RegisterDependencyResolver();
            HandlerConfig.RegisterHandlers(GlobalConfiguration.Configuration.MessageHandlers);

            GlobalConfiguration.Configuration.MessageHandlers.Add(new JWTTokenValidationHandler());
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHostBufferPolicySelector), new PolicySelectorWithoutBuffer());
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Get the error details
            Exception ex = Server.GetLastError();

            // Check exception type and default Http 500 (internal server error)
            Response.StatusCode = (ex is HttpException) ? (ex as HttpException).GetHttpCode() : (int)HttpStatusCode.InternalServerError;

            // Log exception 
            DiagnosticsProvider diagnostics = new DiagnosticsProvider(this.GetType());
            string error = string.Join("\n", "Unhandled Exception: ", ex.ToString());
            diagnostics.WriteErrorTrace(TraceEventId.Exception, error);

            // Clear buffers
            Server.ClearError();
            Response.End();
        }
    }
}