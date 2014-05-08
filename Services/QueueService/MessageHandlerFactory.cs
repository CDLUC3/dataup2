// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;

namespace Microsoft.Research.DataOnboarding.QueueService
{
    /// <summary>
    /// Implements the methods for IMessageHandlerFactory interface.
    /// </summary>
    public class MessageHandlerFactory : IMessageHandlerFactory
    {
        /// <summary>
        /// Contains the reference to queueRepository.
        /// </summary>
        private IQueueRepository queueRepository;

        /// <summary>
        /// Contains the reference to fileServiceFactory.
        /// </summary>
        private IFileServiceFactory fileServiceFactory;

        /// <summary>
        /// Contains the reference to repositoryService.
        /// </summary>
        private IRepositoryService repositoryService;

        /// <summary>
        /// Contains the reference to blobRepository.
        /// </summary>
        private IBlobDataRepository blobRepository;

        /// <summary>
        /// Reference to Diagnostic provider.
        /// </summary>
        private DiagnosticsProvider diagnostics;

         /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerFactory"/> class.
        /// </summary>
        /// <param name="fileServiceFactory">IFileServiceFactory to instantiate IFileService.</param>
        /// <param name="repositoryService">IRepositoryService instance.</param>
        /// <param name="queueRepository">IQueueRepository instance.</param>
        /// <param name="blobRepository">IBlobRepository instance.</param>
        public MessageHandlerFactory(IFileServiceFactory fileServiceFactory, IRepositoryService repositoryService, IQueueRepository queueRepository, IBlobDataRepository blobRepository)
        {
            this.fileServiceFactory = fileServiceFactory;
            this.repositoryService = repositoryService;
            this.queueRepository = queueRepository;
            this.blobRepository = blobRepository;
            diagnostics = new DiagnosticsProvider(this.GetType());
        }

        /// <summary>
        /// Returns the instance of MessageHandler.
        /// </summary>
        /// <param name="handlerName">MessageHandler Enumeration value.</param>
        /// <returns>IMessageHandler Instance.</returns>
        public IMessageHandler GetMessageHandler(MessageHandlerEnum handlerName)
        {
            IMessageHandler messageHandler = null;

            switch (handlerName)
            {
                case MessageHandlerEnum.PublishMessageHandler:
                    messageHandler = new PublishQueueService(this.fileServiceFactory, this.repositoryService, this.queueRepository);
                    break;
                case MessageHandlerEnum.VerifyMessageHandler:
                    messageHandler = new VerifyFileQueueService(this.fileServiceFactory, this.repositoryService, this.queueRepository, this.blobRepository);
                    break;
            }

            return messageHandler;
        }
    }
}