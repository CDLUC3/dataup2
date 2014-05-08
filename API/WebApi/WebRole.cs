using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Web.Administration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WebApi
{
    [ExcludeFromCodeCoverage]
    public class WebRole : RoleEntryPoint
    {
        private const string WebRoleName = "Web";
        private const string CompilationSectionPath = "system.web/compilation";
        private const string AspNetTempDirectoryAttributeKey = "tempDirectory";

        public override bool OnStart()
        {
            /***************************************************************************
             * In case of DEBUG build (mostly dev boxes) use the default ASPNET temp folder
             * For other build types use the local resource configured on Azure. 
             ***************************************************************************/
#if !DEBUG
            UpdateAspNetTempFolderPath();
#endif

            RoleEnvironment.Changing += RoleEnvironment_Changing;

            return base.OnStart();
        }

        static void RoleEnvironment_Changing(object sender, RoleEnvironmentChangingEventArgs e)
        {
            if (e.Changes.OfType<RoleEnvironmentConfigurationSettingChange>().Count() > 0)
            {
                // Cancel the changing event to force role instance restart
                //e.Cancel = true;
            }
        }

        /// <summary>
        /// This method updates the default ASP.NET temporary folder location
        /// to the local resource configured in the Azure web role configuration
        /// <see cref="http://code.msdn.microsoft.com/windowsazure/How-to-increase-the-size-3939500b"/>
        /// <see cref="http://blogs.msdn.com/b/kwill/archive/2011/07/18/how-to-increase-the-size-of-the-windows-azure-web-role-asp-net-temporary-folder.aspx"/>
        /// </summary>
        private static void UpdateAspNetTempFolderPath()
        {
            string transientFileStorageLocation = RoleEnvironment.GetLocalResource(Constants.TransientFileStorage_AzureLocalResource).RootPath;

            // Instantiate the IIS ServerManager
            ServerManager iisManager = new ServerManager();
            // Get handle to the website 
            Application app = iisManager.Sites[string.Join("_", RoleEnvironment.CurrentRoleInstance.Id, WebRoleName)].Applications[0];
            // Get handle to web.config
            Configuration webConfig = app.GetWebConfiguration();
            // Get handle to the web compilation section of the web configuration
            ConfigurationSection compilationSection = webConfig.GetSection(CompilationSectionPath);
            // Set the ASP.NET temp folder path to the transient file local storage
            compilationSection.Attributes[AspNetTempDirectoryAttributeKey].Value = transientFileStorageLocation;
            // Save changes
            iisManager.CommitChanges();
        }
    }
}
