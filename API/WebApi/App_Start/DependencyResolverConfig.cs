// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Practices.Unity;
using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.DataAccessService.Providers.EntityFramework;
using Microsoft.Research.DataOnboarding.FileService;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.QCService.Interface;
using Microsoft.Research.DataOnboarding.QueueService;
using Microsoft.Research.DataOnboarding.RepositoriesService;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.RepositoryAdapters;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces;
using Microsoft.Research.DataOnboarding.Services.UserService;
using Microsoft.Research.DataOnboarding.WebApi.FileHandlers;
using System.Diagnostics.CodeAnalysis;
using System.Web.Http;
using Unity.WebApi;
using QCS = Microsoft.Research.DataOnboarding.QCService;

namespace Microsoft.Research.DataOnboarding.WebApi
{
    /// <summary>
    /// Dependency resolver configuration file.
    /// </summary>
    public static class DependencyResolverConfig
    {
        public static void RegisterDependencyResolver()
        {
            var container = BuildUnityContainer();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling"), SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<IUnitOfWork, DataOnboardingContext>(new PerResolveLifetimeManager(), new InjectionConstructor("Name = DataOnboardingConnection"));
            container.RegisterType<IBlobDataRepository, BlobDataRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IRepositoryDetails, RepositoryDetails>();
            container.RegisterType<IQualityCheckRepository, QualityCheckRepository>();
            container.RegisterType<IFileRepository, FileRepository>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IRepositoryService, RepositoryService>();
            container.RegisterType<IUserService, UserServiceProvider>();
            container.RegisterType<IFileService, FileServiceProvider>();
            container.RegisterType<IQCService, QCS.QCService>();
            container.RegisterType<IQueueRepository, QueueRepository>();
            container.RegisterType<IPublishQueueService, PublishQueueService>();
            container.RegisterType<IFileServiceFactory, FileServiceFactory>();
            container.RegisterType<IRepositoryAdapterFactory, RepositoryAdapterFactory>();
            container.RegisterType<IFileHandlerFactory, FileHandlerFactory>();
            container.RegisterType<IMessageHandlerFactory, MessageHandlerFactory>();

            return container;
        }
    }
}