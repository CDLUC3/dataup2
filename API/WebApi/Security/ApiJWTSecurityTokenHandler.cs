// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
      
namespace Microsoft.Research.DataOnboarding.WebApi.Security
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IdentityModel.Tokens;
    using System.Linq;
    using System.Security.Claims;
    using System.Web;
    using Microsoft.IdentityModel.Tokens.JWT;
    using Microsoft.Research.DataOnboarding.Utilities;
    using Microsoft.Research.DataOnboarding.Utilities.Enums;

    /// <summary>
    /// This class extends the implementation of JWTSecurityTokenHandler 
    /// to read configuration settings from the token handler configuration
    /// if its initialized by WIF. It falls back on App settings if the configuration
    /// object is null. 
    /// </summary>
    public class ApiJWTSecurityTokenHandler : JWTSecurityTokenHandler
    {
        private readonly DiagnosticsProvider diagnostics;

        public ApiJWTSecurityTokenHandler()
            :base()
        {
            diagnostics = new DiagnosticsProvider(this.GetType());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Using Check helper to validate input")]
        public override ClaimsPrincipal ValidateToken(JWTSecurityToken jwt, TokenValidationParameters validationParameters)
        {
            Check.IsNotNull(jwt, "jwt");
            Check.IsNotNull(validationParameters, "validationParameters");

            diagnostics.WriteInformationTrace(TraceEventId.InboundParameters,
                                              "JWT token details from ACS: {0}, {1}, {2}, {3}",
                                              jwt.Issuer, jwt.ValidFrom, jwt.ValidTo, jwt.RawData);

            string audienceUrl;
            if (this.Configuration != null)
            {
                audienceUrl = this.Configuration.AudienceRestriction.AllowedAudienceUris[0].ToString();
            }
            else
            {
                audienceUrl = validationParameters.AllowedAudience;
            }
            
            // set up valid issuers
            if ((validationParameters.ValidIssuer == null) &&
                (validationParameters.ValidIssuers == null || !validationParameters.ValidIssuers.Any()))
            {
                List<string> issuers = new List<string>();
                issuers.AddRange(ConfigurationManager.AppSettings["Issuers"].Split(new[] { ',' }));
                validationParameters.ValidIssuers = issuers;
            }
            
            // setup signing token.
            if (validationParameters.SigningToken == null)
            {
                var resolver = (NamedKeyIssuerTokenResolver)this.Configuration.IssuerTokenResolver;
                if (resolver.SecurityKeys != null)
                {
                    List<SecurityKey> skeys;
                    if (resolver.SecurityKeys.TryGetValue(audienceUrl, out skeys))
                    {
                        var tok = new NamedKeySecurityToken(audienceUrl, skeys);
                        validationParameters.SigningToken = tok;
                    }
                }
            }

            diagnostics.WriteInformationTrace(TraceEventId.Flow, "Successfully validated JWT token from ACS");

            return base.ValidateToken(jwt, validationParameters);
        }
    }
}