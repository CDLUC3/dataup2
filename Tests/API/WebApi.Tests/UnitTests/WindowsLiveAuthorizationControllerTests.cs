using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.SkyDrive;
using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.Research.DataOnboarding.Utilities.Fakes;
using Microsoft.Research.DataOnboarding.WebApi.Api;
using Microsoft.Research.DataOnboarding.WebApi.Api.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Fakes;
using System.Web.Fakes;

namespace Microsoft.Research.DataOnboarding.WebApi.Tests.UnitTests
{
    [TestClass]
    public class WindowsLiveAuthorizationControllerTests
    {
        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void AuthorizeUser_ShouldReturnWindowsLiveLoginUrl_WhenRequestUrlHasNoOAuthCode()
        {
            // Prepare
            HttpResponseMessage httpResponseMessage = null;
            using (ShimsContext.Create())
            {
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.AllInstances.RequestGet = httpContext => httpRequest;

                NameValueCollection queryStringCollection = new NameValueCollection();
                ShimHttpRequest.AllInstances.QueryStringGet = hr => queryStringCollection;

                ShimDiagnosticsProvider.AllInstances.WriteInformationTraceTraceEventIdString = (diagnosticsProvider, traceEventId, message) => { };

                var windowsLiveAuthorizationController = new WindowsLiveAuthorizationController();
                windowsLiveAuthorizationController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);

                ShimHttpRequestMessageExtensions.GetQueryNameValuePairsHttpRequestMessage = httpRequestMessage =>
                {
                    var queryNameValuePairs = new List<KeyValuePair<string, string>>();
                    queryNameValuePairs.Add(new KeyValuePair<string, string>("callBackUrl", "somecallBackUrl"));
                    return queryNameValuePairs;
                };

                // Perform
                httpResponseMessage = windowsLiveAuthorizationController.AuthorizeUser();
            }

            // Assert
            Assert.AreEqual(httpResponseMessage.StatusCode, HttpStatusCode.Moved, "Stataus is not as expected(Moved)");
            Assert.AreEqual("https://login.live.com/oauth20_authorize.srf?client_id=00000000400ED482&scope=wl.skydrive_update+wl.offline_access&response_type=code&redirect_uri=http:%2f%2fdev-api-dataonboarding.cloudapp.net%2fapi%2fwindowsLiveAuthorization%2fAuthorizeUser%3fcallBackUrl%3dsomecallBackUrl",
                httpResponseMessage.Headers.Location.ToString());
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void AuthorizeUser_ShouldReturnWindowsLiveTokens_WhenRequestUrlHasOAuthCode()
        {
            // Prepare
            HttpResponseMessage httpResponseMessage = null;
            using (ShimsContext.Create())
            {
                ShimHttpContext.CurrentGet = () => new ShimHttpContext();

                var httpRequest = new ShimHttpRequest();
                ShimHttpContext.AllInstances.RequestGet = httpContext => httpRequest;

                NameValueCollection queryStringCollection = new NameValueCollection();
                ShimHttpRequest.AllInstances.QueryStringGet = hr => queryStringCollection;
                //TODO Ram 
               queryStringCollection.Add(SkyDriveConstants.OAuth_Code, "SomeCode");

                ShimDiagnosticsProvider.AllInstances.WriteInformationTraceTraceEventIdString = (diagnosticsProvider, traceEventId, message) => { };

                var windowsLiveAuthorizationController = new WindowsLiveAuthorizationController();
                windowsLiveAuthorizationController.Request = new HttpRequestMessage(HttpMethod.Post, string.Empty);

                ShimHttpRequestMessageExtensions.GetQueryNameValuePairsHttpRequestMessage = httpRequestMessage =>
                {
                    var queryNameValuePairs = new List<KeyValuePair<string, string>>();
                    queryNameValuePairs.Add(new KeyValuePair<string, string>(SkyDriveConstants.OAuth_Code, "somecode"));
                    queryNameValuePairs.Add(new KeyValuePair<string, string>("callBackUrl", "http://dev-api-dataonboarding.cloudapp.net/api/windowsLiveAuthorization/AuthorizeUser"));
                    return queryNameValuePairs;
                };

                ShimWindowsLiveAuthorizationController.AllInstances.GetWindowsLiveTokensStringString = (wlac, code, redirectUrl) =>
                    new OAuthToken()
                    {
                        AccessToken = "AccessToken",
                        AuthenticationToken = "AuthenticationToken",
                        ExpiresIn = "ExpiresIn",
                        RefreshToken = "RefreshToken",
                        Scope = "Scope",
                        TokenExpiresOn = new DateTime(2013, 8, 15)
                    };

                // Perform
                httpResponseMessage = windowsLiveAuthorizationController.AuthorizeUser();
            }

            // Assert
            Assert.AreEqual(httpResponseMessage.StatusCode, HttpStatusCode.Moved, "Stataus is not as expected(Moved)");
            Assert.AreEqual("http://dev-api-dataonboarding.cloudapp.net/api/windowsLiveAuthorization/AuthorizeUser?accessToken=AccessToken&refreshToken=RefreshToken&tokenExpiresOn=8/15/2013 12:00:00 AM",
                httpResponseMessage.Headers.Location.ToString());
        }
    }
}
