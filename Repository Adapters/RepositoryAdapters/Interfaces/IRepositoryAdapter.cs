using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces
{
    public interface IRepositoryAdapter
    {
        /// <summary>
        /// Method to get the identifier 
        /// </summary>
        /// <param name="queryData">query data</param>
        /// <param name="repositoryModel">repository model</param>
        /// <returns>returns string</returns>
        string GetIdentifier(MerritQueryData queryData, RepositoryModel repositoryModel);

        /// <summary>
        /// method to post the file to repository
        /// </summary>
        /// <param name="request">request object</param>
        /// <param name="repositoryModel">repository model</param>
        /// <param name="file">file</param>
        /// <returns>returns operation status.</returns>
        OperationStatus PostFile(PublishFileModel publishFileModel);

        /// <summary>
        /// Method to download the fiel from the specific repository.
        /// </summary>
        /// <param name="downloadInput">Input data required to download the file from the repository.</param>
        /// <returns>Downloaded file data.</returns>
        DataFile DownloadFile(string downloadUrl, string authorization, string fileName);

        /// <summary>
        /// Fetches the AuthToken
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        AuthToken RefreshToken(string refreshToken);

        /// <summary>
        /// Verifies if the file exists in the repository
        /// </summary>
        /// <param name="fileIdentifier">File Identifier</param>
        /// <param name="authorization">AccessToken in case of skydrive and username password for Merrit</param>
        /// <returns>OperationStatus returns Success if the file exists </returns>
        OperationStatus CheckIfFileExists(string downloadURL, string fileIdentifier, string authorization);
    }
}
