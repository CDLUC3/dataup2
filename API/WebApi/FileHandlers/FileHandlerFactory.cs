// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.Utilities;

namespace Microsoft.Research.DataOnboarding.WebApi.FileHandlers
{
    /// <summary>
    /// File handler factory.
    /// </summary>
    public class FileHandlerFactory : IFileHandlerFactory
    {
        private IFileService fileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileHandlerFactory"/> class.
        /// </summary>
        /// <param name="fileService">IFileService instance.</param>
        /// <param name="user">User details.</param>
        public FileHandlerFactory(IFileService fileService)
        {
            this.fileService = fileService;
        }

        /// <summary>
        /// Returns an instance of IFileHandler.
        /// </summary>
        /// <param name="type">Content type of the file to be processed.</param>
        /// <returns>IFileHandler instance.</returns>
        public IFileHandler GetFileHandler(string contentType, int userId)
        {
            IFileHandler fileHandler;

            switch (contentType)
            {
                case Constants.APPLICATION_XZIP:
                    fileHandler = new ZipFileHandler(this.fileService, userId);
                    break;
                default:
                    fileHandler = new DefaultFileHandler(this.fileService);
                    break;
            }

            return fileHandler;
        }
    }
}