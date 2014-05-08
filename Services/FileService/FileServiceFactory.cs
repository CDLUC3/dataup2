// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces;
using Microsoft.Research.DataOnboarding.Services.UserService;
using Microsoft.Research.DataOnboarding.Utilities.Enums;

namespace Microsoft.Research.DataOnboarding.FileService
{
    public class FileServiceFactory : IFileServiceFactory
    {
        /// <summary>
        /// contains reference to IFileRepository
        /// </summary>
        IFileRepository fileDataRepository;

        /// <summary>
        /// contains reference IBlobDataRepository
        /// </summary>
        IBlobDataRepository blobDataRepository;

        /// <summary>
        /// contains IUnitOfWork
        /// </summary>
        IUnitOfWork unitOfWork;

        /// <summary>
        /// contains IRepositoryDetails
        /// </summary>
        IRepositoryDetails repositoryDetails;

        /// <summary>
        /// contains IRepositoryService
        /// </summary>
        IRepositoryService repositoryService;

        /// <summary>
        /// contains IUserService
        /// </summary>
        IUserService userService;

        /// <summary>
        /// contains IRepositoryAdapterFactory
        /// </summary>
        IRepositoryAdapterFactory repositoryAdapterFactory;

        /// <summary>
        /// creates the instance of FileServiceFactory
        /// </summary>
        /// <param name="fileDataRepository">fileDataRepository</param>
        /// <param name="blobDataRepository">IBlobDataRepository</param>
        /// <param name="unitOfWork">IUnitOfWork</param>
        /// <param name="repositoryDetails">IRepositoryDetails</param>
        /// <param name="repositoryService">IRepositoryService</param>
        /// <param name="userService">IUserService</param>
        /// <param name="repositoryAdapterFactory">IRepositoryAdapterFactory</param>
        public FileServiceFactory(IFileRepository fileDataRepository, IBlobDataRepository blobDataRepository, IUnitOfWork unitOfWork, IRepositoryDetails repositoryDetails, IRepositoryService repositoryService, IUserService userService, IRepositoryAdapterFactory repositoryAdapterFactory)
        {
            this.fileDataRepository = fileDataRepository;
            this.blobDataRepository = blobDataRepository;
            this.unitOfWork = unitOfWork;
            this.repositoryDetails = repositoryDetails;
            this.repositoryService = repositoryService;
            this.userService = userService;
            this.repositoryAdapterFactory = repositoryAdapterFactory;
        }

        /// <summary>
        /// Returns the concreate instance of the IFileService which is specifc to each repository
        /// </summary>
        /// <param name="baseRepositoryName">BaseRepository Name.</param>
        /// <returns>FileService</returns>
        public IFileService GetFileService(string baseRepositoryName)
        {
            BaseRepositoryEnum baseRepository;
            
            Enum.TryParse<BaseRepositoryEnum>(baseRepositoryName, out baseRepository);

            switch (baseRepository)
            {
                case BaseRepositoryEnum.SkyDrive:
                    return new SkyDriveFileService(this.fileDataRepository, this.blobDataRepository, this.unitOfWork, this.repositoryDetails, this.repositoryService, this.userService, repositoryAdapterFactory);

                case BaseRepositoryEnum.Merritt:
                    return new MerritFileService(this.fileDataRepository, this.blobDataRepository, this.unitOfWork, this.repositoryDetails, this.repositoryService, this.repositoryAdapterFactory, this.userService);
                   
                default:
                    return new FileServiceProvider(this.fileDataRepository, this.blobDataRepository, this.unitOfWork,this.repositoryDetails,this.repositoryService, this.repositoryAdapterFactory);
            }
        }
    }
}
