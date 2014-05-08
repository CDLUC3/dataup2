// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.WebApi.Security
{
    using Microsoft.IdentityModel.Tokens.JWT;
    using Microsoft.Research.DataOnboarding.DomainModel;
    using Microsoft.Research.DataOnboarding.Services.UserService;
    using Microsoft.Research.DataOnboarding.Utilities;
    using Microsoft.Research.DataOnboarding.Utilities.Enums;
    using Microsoft.Research.DataOnboarding.WebApi.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IdentityModel.Services;
    using System.IdentityModel.Tokens;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    public class JWTTokenValidationHandler : DelegatingHandler
    {
        private readonly DiagnosticsProvider diagnostics;

        public JWTTokenValidationHandler()
            :base()
        {
            diagnostics = new DiagnosticsProvider(this.GetType());
        }

        // This function retrieves ACS token (in format of OAuth 2.0 Bearer Token type) from 
        // the Authorization header in the incoming HTTP request.
        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            IEnumerable<string> authzHeaders;

            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
            {
                // Fail if no Authorization header or more than one Authorization headers 
                // are found in the HTTP request 
                return false;
            }

            // Remove the bearer token scheme prefix and return the rest as ACS token 
            var bearerToken = authzHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ", StringComparison.CurrentCultureIgnoreCase) ? bearerToken.Substring(7) : bearerToken;
            return true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Using Check helper to validate input")]
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Check.IsNotNull(request, "request");
            HttpStatusCode statusCode;
            string token;
          
            if (request.RequestUri.AbsolutePath.Contains("/authenticate"))
            {
                return base.SendAsync(request, cancellationToken);
            }

            if (request.RequestUri.AbsolutePath.Contains("/signout"))
            {
                return base.SendAsync(request, cancellationToken);
            }

            if (request.RequestUri.AbsolutePath.Contains("/SignOutCallback"))
            {
                return base.SendAsync(request, cancellationToken);
            }

            if (request.RequestUri.AbsolutePath.Contains("/windowsLiveAuthorization"))
            {
                return base.SendAsync(request, cancellationToken);
            }

            if (request.RequestUri.AbsolutePath.Contains("api/GetSupportedIdentityProviders"))
            {
                return base.SendAsync(request, cancellationToken);
            }

            if (request.RequestUri.AbsolutePath.Contains("api/SignInCallBack"))
            {
                return base.SendAsync(request, cancellationToken);
            }

            if (!TryRetrieveToken(request, out token))
            {
                statusCode = HttpStatusCode.Unauthorized;
                return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode));
            }

            try
            {
                diagnostics.WriteInformationTrace(TraceEventId.Flow, "Validating bearer token: {0}", token);

                string issuersValue = ConfigurationManager.AppSettings["Issuers"];
                string signingSymmetricKey = ConfigurationManager.AppSettings["SigningSymmetricKey"];
                string allowedAudience = ConfigurationManager.AppSettings["AllowedAudience"];

                // Use JWTSecurityTokenHandler to validate the JWT token
                ApiJWTSecurityTokenHandler tokenHandler = new ApiJWTSecurityTokenHandler();

                List<string> issuers = new List<string>();
                issuers.AddRange(issuersValue.Split(new[] { ',' }));

                InMemorySymmetricSecurityKey securityKey = new InMemorySymmetricSecurityKey(Convert.FromBase64String(signingSymmetricKey));

                NamedKeySecurityToken namedToken = new NamedKeySecurityToken(allowedAudience, new List<SecurityKey>() { securityKey });

                // Set the expected properties of the JWT token in the TokenValidationParameters
                TokenValidationParameters validationParameters = new TokenValidationParameters()
                {
                    AllowedAudience = allowedAudience,
                    ValidIssuers = issuers,
                    SigningToken = namedToken
                };

                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters);
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)claimsPrincipal.Identity;
                try
                {
                    IUserService userService = (IUserService)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IUserService));
                    User user = IdentityHelper.GetCurrentUser(userService, claimsPrincipal);
                    foreach (var role in user.UserRoles)
                    {
                        if (!claimsIdentity.HasClaim(ClaimTypes.Role, role.Role.Name))
                        {
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Role.Name));
                        }
                    }
                }
                catch (UserNotFoundException)
                {
                    // No need to do anything here because GetUsersByNameIdentifier action in UsersController will take care of sending appropriate http response.
                }

                Thread.CurrentPrincipal = claimsPrincipal;
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = claimsPrincipal;
                }

                diagnostics.WriteInformationTrace(TraceEventId.Flow,
                                        "Authorization token validated successfully");

                return base.SendAsync(request, cancellationToken);
            }
            catch (SecurityTokenValidationException secutiryTokenValidationException)
            {
                FederatedAuthentication.WSFederationAuthenticationModule.SignOut(true);
                HttpResponseMessage response = request.CreateErrorResponse(HttpStatusCode.Unauthorized, secutiryTokenValidationException);
                return Task<HttpResponseMessage>.Factory.StartNew(() => response);
            }
        }
    }
}