// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.WebApi.FileHandlers
{
    /// <summary>
    /// Interface for file handler factory.
    /// </summary>
    public interface IFileHandlerFactory
    {
        /// <summary>
        /// Returns an instance of IFileHandler.
        /// </summary>
        /// <param name="type">Content type of the file to be processed.</param>
        /// <param name="userId">Id of the user.</param>
        /// <returns>IFileHandler instance.</returns>
        IFileHandler GetFileHandler(string contentType, int userId);
    }
}
