// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.WebApi
{
    using Microsoft.Research.DataOnboarding.WebApi.Filters;
    using Microsoft.Research.DataOnboarding.WebApi.Helpers;
    using System.Web.Http;

    public static class WebApiConfig
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Auto-Generated Code")]
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
              name: RouteConstants.WindowsLiveAuthRouteName,
              routeTemplate: "api/windowsLiveAuthorization",
              defaults: new { controller = "WindowsLiveAuthorization", action = "Authorize" });

            config.Routes.MapHttpRoute(
             name: RouteConstants.AuthenticationRouteName,
             routeTemplate: "api/authenticate",
             defaults: new { controller = "Authentication", action = "Authenticate" });

            config.Routes.MapHttpRoute(
                name: RouteConstants.FilesGetErrors,
                routeTemplate: "api/files/{fileId}/errors",
                defaults: new { controller = "Files", action = "GetErrors" });

            config.Routes.MapHttpRoute(
                name: RouteConstants.GetPostFileDetails,
                routeTemplate: "api/files/{fileId}/getpostfiledetails",
                defaults: new { controller = "Files", action = "GetPostFileDetails" });

            config.Routes.MapHttpRoute(
                name: RouteConstants.GetFileLevelMetadata,
                routeTemplate: "api/files/{fileId}/filelevelmetadata",
                defaults: new { controller = "Files", action = "GetFileLevelMetadata" });

            config.Routes.MapHttpRoute(
                name: RouteConstants.SaveFileLevelMetadata,
                routeTemplate: "api/files/{fileId}/savefilelevelmetadata",
                defaults: new { controller = "Files", action = "SaveFileLevelMetadata" });

            config.Routes.MapHttpRoute(
                name: RouteConstants.GetColumnLevelMetadata,
                routeTemplate: "api/files/{fileId}/columnlevelmetadata",
                defaults: new { controller = "Files", action = "GetColumnLevelMetadata" });

            config.Routes.MapHttpRoute(
                name: RouteConstants.GetColumnLevelMetadataFromFile,
                routeTemplate: "api/files/{fileId}/getcolumnlevelmetadatafromfile",
                defaults: new { controller = "Files", action = "GetColumnLevelMetadataFromFile" });

            config.Routes.MapHttpRoute(
                name: RouteConstants.SaveColumnLevelMetadata,
                routeTemplate: "api/files/{fileId}/savecolumnlevelmetadata",
                defaults: new { controller = "Files", action = "SaveColumnLevelMetadata" });

            config.Routes.MapHttpRoute(
                name: RouteConstants.GetCitation,
                routeTemplate: "api/files/{fileId}/citation",
                defaults: new { controller = "Files", action = "GetCitation" });

            config.Routes.MapHttpRoute(
                name: RouteConstants.SaveCitation,
                routeTemplate: "api/files/{fileId}/savecitation",
                defaults: new { controller = "Files", action = "SaveCitation" });

            config.Routes.MapHttpRoute(
            name: RouteConstants.GetSupportedIdentityProviderseRouteName,
            routeTemplate: "api/GetSupportedIdentityProviders",
            defaults: new { controller = "Authentication", action = "GetSupportedIdentityProviders" });

            config.Routes.MapHttpRoute(
            name: RouteConstants.SignInCallBackRouteName,
            routeTemplate: "api/SignInCallBack",
            defaults: new { controller = "Authentication", action = "SignInCallBack" });

            config.Routes.MapHttpRoute(
                name: RouteConstants.FilesRemoveErrors,
                routeTemplate: "api/files/{fileId}/removeerrors",
                defaults: new { controller = "Files", action = "RemoveErrors" });

            config.Routes.MapHttpRoute(
               name: RouteConstants.DownloadFileRouteName,
               routeTemplate: "api/files/download",
              defaults: new { controller = "Files", action = "DownloadFileFromRepository" });

            config.Routes.MapHttpRoute(
                name: RouteConstants.DefaultApiRouteName,
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
              name: RouteConstants.SignOutRouteName,
              routeTemplate: "signout",
              defaults: new { controller = "Authentication", action = "SignOut" });

            config.Routes.MapHttpRoute(
             name: RouteConstants.SignOutCallbackRouteName,
             routeTemplate: "SignOutCallback",
             defaults: new { controller = "Authentication", action = "SignOutCallback" });

            config.Filters.Add(new ApiExceptionFilterAttribute());
            // Other configuration code...

            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}
