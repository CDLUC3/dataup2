// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.Optimization;

namespace Microsoft.Research.DataOnboarding.WebApplication
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "Auto-Generated Code")]
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Auto-Generated Code")]
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/GeneralFiles").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/modernizr-*",
                        "~/Scripts/Microsoft*",
                        "~/Scripts/knockout*"));

            bundles.Add(new ScriptBundle("~/bundles/ApplicationSpecificFiles").Include(
                        "~/Scripts/Bootstrap.js",
                        "~/Scripts/filelist.js",
                        "~/Scripts/fileupload.js",
                         "~/Scripts/filedetail.js",
                        "~/Scripts/client.js",
                        "~/Scripts/admin.js",
                        "~/Scripts/Extensions.js",
                        "~/Scripts/admin.js",
                         "~/Scripts/fileupload.js",
                          "~/Scripts/filelist.js",
                          "~/Scripts/filedetail.js",
                          "~/Scripts/basicauthenticationpopup.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/PostPageScripts").Include(
                "~/Scripts/FileLevelMetadata.js",
                "~/Scripts/ColumnLevelMetadata.js",
                "~/Scripts/Metadata.js",
                "~/Scripts/BestPractices.js",
                "~/Scripts/QualityCheck.js",
                "~/Scripts/Citation.js",
                "~/Scripts/Post.js"));

            bundles.Add(new StyleBundle("~/Content/PostPageStyles").Include(
                "~/Content/Post.css",
                "~/Content/DataUp-Widgets.css",
                "~/Content/FileLevelMetadata.css",
                "~/Content/ColumnLevelMetadata.css",
                "~/Content/BestPractices.css",
                "~/Content/Citation.css",
                "~/Content/QualityCheck.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/master.css",
                "~/Content/Styles.css"
                ));

            bundles.Add(new StyleBundle("~/Content/HomePage").Include(
               "~/Content/Styles.css",
                "~/Content/master.css",
                "~/Content/filelist.css",
                "~/Content/basicAuthenticationPopup.css"
               ));

            bundles.Add(new ScriptBundle("~/bundles/LoginScripts").Include(
                        "~/Scripts/login.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/LoginStyles").Include(
                "~/Content/login.css"
                ));

            // BundleTable.EnableOptimizations = true;

        }
    }
}