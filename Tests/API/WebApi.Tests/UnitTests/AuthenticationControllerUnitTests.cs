// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.Research.DataOnboarding.Utilities.Fakes;
using Microsoft.Research.DataOnboarding.WebApi.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IdentityModel.Services.Configuration.Fakes;
using System.IdentityModel.Services.Fakes;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Fakes;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.Fakes;

namespace Microsoft.Research.DataOnboarding.WebApi.Tests.UnitTests
{
    [TestClass]
    public class AuthenticationControllerUnitTests
    {
        /// <summary>
        /// The test checks if the redirection URL is per the ACS logout Url format
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Allow_SignOut()
        {
            // Prepare
            HttpResponseMessage httpResponseMessage = null;
            string issuer = "https://unittest.accesscontrol.windows.net/v2/wsfederation";
            string wtrealm = "urn:dataonboardingapi:unittest";
            string callbackUrl = "http://unittest:123/landingpage";
            string apiUrl = "http://unittest-api-dataonboarding.cloudapp.net";
            
            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.AllInstances.WriteInformationTraceTraceEventIdString = (diagnosticsProvider, traceEventId, message) => { };
                ShimFederatedAuthentication.WSFederationAuthenticationModuleGet = () =>
                {
                    var wsFederationAuthenticationModule = new ShimWSFederationAuthenticationModule();
                    wsFederationAuthenticationModule.SignOutBoolean = (param) =>
                    {
                    };

                    return wsFederationAuthenticationModule;
                };

                ShimFederatedAuthentication.FederationConfigurationGet = () =>
                    {
                        var federationConfiguration = new ShimFederationConfiguration();
                        federationConfiguration.WsFederationConfigurationGet = () =>
                            {
                                var acsConfig = new ShimWsFederationConfiguration();
                                acsConfig.RealmGet = () =>
                                    {
                                        return wtrealm;
                                    };

                                acsConfig.IssuerGet = ()=>
                                    {
                                        return issuer; ;
                                    };

                                return acsConfig;
                            };

                        return federationConfiguration;
                    };

                var authenticationController = new AuthenticationController();

                authenticationController.Request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                
                // Perform
                httpResponseMessage = authenticationController.SignOut(callbackUrl);
            }

            // Assert
            Assert.AreEqual(HttpStatusCode.Moved, httpResponseMessage.StatusCode, "Stataus is not as expected(Moved)");
            string wReply = string.Format("{0}/SignOutCallback?callback={1}", apiUrl, callbackUrl);

            string expectedCallbackUrl = string.Format("{0}?wa=wsignout1.0&wreply={1}&wtrealm={2}", issuer, HttpUtility.UrlEncode(wReply), wtrealm);
            
            Assert.AreEqual(HttpUtility.UrlDecode(expectedCallbackUrl), HttpUtility.UrlDecode(httpResponseMessage.Headers.Location.ToString()));
        }
        
        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Allow_SignOutCallBack()
        {
            // Prepare
            HttpResponseMessage httpResponseMessage = null;
            string callbackUrl = "http://unittest:123/landingpage";
            var authenticationController = new AuthenticationController();
            authenticationController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
           
            // Perform
            httpResponseMessage = authenticationController.SignOutCallback(callbackUrl);

            // Assert
            Assert.AreEqual(HttpStatusCode.Moved, httpResponseMessage.StatusCode, "Stataus is not as expected(Moved)");
            Assert.AreEqual(callbackUrl, HttpUtility.UrlDecode(httpResponseMessage.Headers.Location.ToString()));

            // Check FedAuth cookie
            bool cookieExists = false;
            string[] cookies = (string[]) httpResponseMessage.Headers.GetValues("set-cookie");
            foreach (string cookie in cookies)
            {
                if (cookie.Contains("FedAuth"))
                {
                    cookieExists = true;
                    string[] attributes = cookie.Split(';');
                    DateTime expiryDate = Convert.ToDateTime(attributes[1].Substring(attributes[1].LastIndexOf('=') + 1));
                    Assert.IsTrue(DateTime.Now > expiryDate, "FedAuth cookie is not set to expire");
                }
            }

            Assert.IsTrue(cookieExists, "FedAuth cookie doese not exist");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Return_BadRequest_SignOut_When_Callback_Is_NUll()
        {
            // Prepare
            HttpResponseMessage httpResponseMessage = null;
            string callbackUrl = null;
            var authenticationController = new AuthenticationController();
            authenticationController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
            
            // Perform

            httpResponseMessage = authenticationController.SignOut(callbackUrl);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode, "Stataus is not as expected(Bad Request)");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Return_BadRequest_SignOutCallBack_When_CallBack_IS_Null()
        {
            // Prepare
            HttpResponseMessage httpResponseMessage = null;
            string callbackUrl = null;
            var authenticationController = new AuthenticationController();
            authenticationController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);

            // Perform
            httpResponseMessage = authenticationController.SignOutCallback(callbackUrl);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode, "Stataus is not as expected(Bad Request)");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Return_BadRequest_SignOut_When_Callback_Is_Empty_String()
        {
            // Prepare
            HttpResponseMessage httpResponseMessage = null;
            string callbackUrl = string.Empty;
            var authenticationController = new AuthenticationController();
            authenticationController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);

            // Perform
            httpResponseMessage = authenticationController.SignOut(callbackUrl);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode, "Stataus is not as expected(Bad Request)");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Return_BadRequest_SignOutCallBack_When_CallBack_IS_Empty_String()
        {
            // Prepare
            HttpResponseMessage httpResponseMessage = null;
            string callbackUrl = string.Empty;
            var authenticationController = new AuthenticationController();
            authenticationController.Request = new HttpRequestMessage(HttpMethod.Get, string.Empty);

            // Perform
            httpResponseMessage = authenticationController.SignOutCallback(callbackUrl);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode, "Stataus is not as expected(Bad Request)");
        }
    }
}
