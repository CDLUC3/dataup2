using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Microsoft.Research.DataOnboarding.WebApi.Models
{
    public class WLiveConfiguration : ConfigurationSection
    {
        public const string ClientIDParam = "client_id";
        public const string ClientSecretParam = "client_secret";
        public const string RedirectURIParam = "redirect_uri";

        [ConfigurationProperty("OAuthUrl")]
        public string OAuthUrl
        {
            get
            {
                return (string) this["OAuthUrl"];
            }
            set
            {
                this["OAuthUrl"] = value;
            }
        }

        [ConfigurationProperty("OAuthAuthZUrl")]
        public string OAuthAuthZUrl {get;set;}

        [ConfigurationProperty("ClientId")]
        public string ClientId {get;set;}

        [ConfigurationProperty("ClientSecret")]
        public string ClientSecret {get;set;}

        [ConfigurationProperty("SkydriveBaseUrl")]
        public string SkydriveBaseUrl {get;set;}

        [ConfigurationProperty("RedicrectionUrl")]
        public string RedicrectionUrl {get;set;}

        [ConfigurationProperty("SkyDriveUpdateScope")]
        public string SkyDriveUpdateScope {get;set;}

        [ConfigurationProperty("WindowsLiveOfflinseAccessScope")]
        public string WindowsLiveOfflinseAccessScope {get;set;}
    }
}