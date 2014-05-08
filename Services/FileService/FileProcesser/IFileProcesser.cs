// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DM = Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.FileService.FileProcesser
{
    /// <summary>
    /// Interface for file related methods.
    /// </summary>   
    public interface IFileProcesser
    {
        /// <summary>
        /// Method to down load the file along with the metadata
        /// </summary>
        /// <returns>Data detail object.</returns>
        DataDetail DownloadDocument(DM.File fileDetails);

        /// <summary>
        /// Method to get document sheet details
        /// </summary>
        /// <param name="fileDetail">FileDetail object</param>
        /// <returns>returns the document Sheet</returns>
        Task<IEnumerable<FileSheet>> GetDocumentSheetDetails(DM.File fileDetail);

        /// <summary>
        /// Method to get document sheet details
        /// </summary>
        /// <param name="fileDetail">FileDetail object</param>
        /// <returns>returns the document Sheet</returns>
        Task<IEnumerable<ColumnLevelMetadata>> GetColumnMetadataFromFile(DM.File fileDetail);

        /// Method to Retrieve Quality Check Rules
        /// </summary>
        /// <param name="fileDetail">File Detail oject</param>
        /// <param name="qualityCheck">Quality Check object</param>
        /// <param name="qualityCheckTypes">Quality check types</param>
        /// <param name="sheetIds">Sheet Ids</param>
        /// <returns></returns>
        Task<IEnumerable<QualityCheckResult>> GetQualityCheckIssues(DM.File fileDetail, QualityCheck qualityCheck, IEnumerable<QualityCheckColumnType> qualityCheckTypes, string sheetIds);

        /// <summary>
        /// Method to get the errors available on the input excel file.
        /// </summary>
        /// <param name="file">File</param>
        /// <returns>List of errors.</returns>
        Task<IList<FileSheet>> GetErrors(DM.File file);

        /// <summary>
        /// Removes an error from a file sheet.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="sheetName">Sheet Name</param>
        /// <param name="errorTypes">Error types</param>
        /// <returns>Stream of the file.</returns>
        Task RemoveError(Stream stream, string sheetName, IEnumerable<ErrorType> errorTypes);
    }
}
