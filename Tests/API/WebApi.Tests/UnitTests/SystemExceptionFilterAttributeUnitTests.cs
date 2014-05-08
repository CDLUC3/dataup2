// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.Research.DataOnboarding.Utilities.Fakes;
using Microsoft.Research.DataOnboarding.WebApi.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.Research.DataOnboarding.WebApi.Tests.UnitTests
{
    [TestClass]
    public class SystemExceptionFilterAttributeUnitTests
    {
        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void InterServer_Error_When_System_Exception_Occurs()
        {
            // Prepare
            using (ShimsContext.Create())
            {
                var actionContext = new HttpActionContext();
                var context = new HttpActionExecutedContext(actionContext, new Exception("Test Exception"));
                
                ShimDiagnosticsProvider.AllInstances.WriteErrorTraceTraceEventIdException = (diagnosticProvider, traceEventId, exception) => { };
                
                var systemExceptionFilterAttribute = new ApiExceptionFilterAttribute();
               
                // Perform
                systemExceptionFilterAttribute.OnException(context);

                // Assert
                Assert.AreEqual(HttpStatusCode.InternalServerError, context.Response.StatusCode);
            }
        }
    }
}
