// ----------------------------------------------------------------------- 
// <copyright file="WorkerRole.cs" company="Microsoft"> 
// copyright 2013
// </copyright> 
// -----------------------------------------------------------------------

using Microsoft.Practices.Unity;
using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.DataAccessService.Providers.EntityFramework;
using Microsoft.Research.DataOnboarding.FileService;
using Microsoft.Research.DataOnboarding.RepositoriesService;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.RepositoryAdapters;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using FSI = Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.WindowsAzure.Storage;

namespace Microsoft.Research.DataOnboarding.FilePurgeService
{
    public class WorkerRole : RoleEntryPoint
    {
        /// <summary>
        /// Reference to Diagnosic provider
        /// </summary>
        private DiagnosticsProvider diagnostics;

        /// <summary>
        /// Unity container instance.
        /// </summary>
        private static UnityContainer container;

        /// <summary>
        /// CancellationTokenSource for cancelling FilePurgeService and FileUpdateService
        /// </summary>
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public WorkerRole()
        {
            diagnostics = new DiagnosticsProvider(this.GetType());
        }

        /// <summary>
        /// Called by Windows Azure after the role instance has been initialized. This
        /// method serves as the main thread of execution for your role.
        /// </summary>
        public override void Run()
        {
            diagnostics.WriteInformationTrace(TraceEventId.Flow, "Starting Run method of file purger service worker role.");
            FSI.IFileService fileService = container.Resolve<FSI.IFileService>();
            IFileService filePurgerService = new FilePurgeService(fileService, container);
            IFileService fileUpdateService = new FileUpdateService(fileService);

            while (!tokenSource.IsCancellationRequested)
            {
                diagnostics.WriteInformationTrace(TraceEventId.Flow, "{0}: Starting purging and updating files.", DateTime.UtcNow);
                filePurgerService.Execute();
                fileUpdateService.Execute();
                diagnostics.WriteInformationTrace(TraceEventId.Flow, "{0}: Finished purging and updating files.", DateTime.UtcNow);
                Thread.Sleep(TimeSpan.FromHours(Helpers.Constants.ScheduledTimeInHours));
            }

            diagnostics.WriteInformationTrace(TraceEventId.Flow, "Ending Run method of file purger service worker role.");
        }

        /// <summary>
        /// Called by Windows Azure to initialize the role instance.
        /// </summary>
        /// <returns>True if initialization succeeds, False if it fails. The default implementation returns True.</returns>
        public override bool OnStart()
        {
            diagnostics.WriteInformationTrace(TraceEventId.Flow, "Starting OnStart method of file purger service worker role.");
            RegisterLocalResources();
            RegisterUnityContainer();
            RegisterRoleEnvironmentChangeDelegate();
            diagnostics.WriteInformationTrace(TraceEventId.Flow, "Ending OnStart method of file purger service worker role.");
            return base.OnStart();
        }

        /// <summary>
        /// Called to stop the worker role.
        /// </summary>
        public override void OnStop()
        {
            diagnostics.WriteInformationTrace(TraceEventId.Flow, "Starting OnStop method of file purger service worker role.");
            tokenSource.Cancel();
            base.OnStop();
            diagnostics.WriteInformationTrace(TraceEventId.Flow, "Ending OnStop method of file purger service worker role.");
        }

        /// <summary>
        /// Creates an instance of UnityContainer and registers the instances which needs to be injected
        /// to Controllers/Views/Services, etc.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Will be disposed by unity container.")]
        private static void RegisterUnityContainer()
        {
            container = new UnityContainer();
            container.RegisterType<IUnitOfWork, DataOnboardingContext>(new PerResolveLifetimeManager(), new InjectionConstructor("Name = DataOnboardingConnection"));

            RegisterRepositories(container);
            RegisterAdapters(container);
            RegisterServices(container);
        }

        /// <summary>
        /// Registers the required repositories.
        /// </summary>
        /// <param name="container">Instance of unity container</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Will be disposed by unity container.")]
        private static void RegisterRepositories(UnityContainer container)
        {
            if (container != null)
            {
                container.RegisterType<IFileRepository, FileRepository>();
                container.RegisterType<FSI.IBlobDataRepository, BlobDataRepository>();
                container.RegisterType<IRepositoryDetails, RepositoryDetails>();
                container.RegisterType<IUserRepository, UserRepository>();
                container.RegisterType<IRepositoryService, RepositoryService>();
            }
        }

        /// <summary>
        /// Registers the required adapters.
        /// </summary>
        /// <param name="container">Instance of unity container</param>
        private static void RegisterAdapters(UnityContainer container)
        {
            container.RegisterType<IRepositoryAdapterFactory, RepositoryAdapterFactory>();
        }

        /// <summary>
        /// Registers the required services
        /// </summary>
        /// <param name="container">Instance of unity container</param>        
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Will be disposed by unity container.")]
        private static void RegisterServices(UnityContainer container)
        {
            if (container != null)
            {
                container.RegisterType<FSI.IFileService, FileServiceProvider>();
            }
        }

        private static void RegisterRoleEnvironmentChangeDelegate()
        {
            RoleEnvironment.Changing += RoleEnvironment_Changing;
        }

        static void RoleEnvironment_Changing(object sender, RoleEnvironmentChangingEventArgs e)
        {
            if (e.Changes.OfType<RoleEnvironmentConfigurationSettingChange>().Count() > 0)
            {
                // Cancel the changing event to force role instance restart
                // e.Cancel = true;
            }
        }

        private static void RegisterLocalResources()
        {
            string transientFileStorageLocation = RoleEnvironment.GetLocalResource(Utilities.Constants.TransientFileStorage_AzureLocalResource).RootPath;
            Environment.SetEnvironmentVariable(Utilities.Constants.TempPathFolderName1, transientFileStorageLocation);
            Environment.SetEnvironmentVariable(Utilities.Constants.TempPathFolderName2, transientFileStorageLocation);
        }
    }
}
