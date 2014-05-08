// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.FileService.Interface
{
    /// <summary>
    /// Interface methods for file service class
    /// </summary>
    public interface IFileServiceFactory
    {
        /// <summary>
        /// Returns the Concret implementations of FileService
        /// </summary>
        /// <param name="instanceName">string instanceName</param>
        /// <returns>IFileService</returns>
        IFileService GetFileService(string instanceName);
    }
}
