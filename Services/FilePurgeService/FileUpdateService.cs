// ----------------------------------------------------------------------- 
// <copyright file="FileUpdateService.cs" company="Microsoft"> 
// copyright 2013
// </copyright> 
// -----------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FSI = Microsoft.Research.DataOnboarding.FileService.Interface;

namespace Microsoft.Research.DataOnboarding.FilePurgeService
{
    [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Not necessary.")]
    public class FileUpdateService : IFileService
    {
        /// <summary>
        /// Reference to Diagnosic provider
        /// </summary>
        private DiagnosticsProvider diagnostics;

        /// <summary>
        /// File service that fetches and updates the files
        /// </summary>
        private FSI.IFileService fileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePurgeService" /> class.
        /// </summary>
        /// <param name="fileService">File service instance.</param>
        public FileUpdateService(FSI.IFileService fileService)
        {
            this.diagnostics = new DiagnosticsProvider(this.GetType());
            this.fileService = fileService;
        }

        /// <summary>
        /// Execute Action.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We need to catch any exception and log it. This should not break the application.")]
        public void Execute()
        {
            try
            {
                var uploadedFiles = fileService.GetUploadedFiles();
                if (uploadedFiles != null)
                {
                    diagnostics.WriteInformationTrace(TraceEventId.Flow, "{1} : Found {0} files which has to be updated", uploadedFiles.Count(), DateTime.UtcNow);
                    var uploadedFilesExpirationDurationInHours = Helpers.Constants.UploadedFilesExpirationDurationInHours;
                    var scheduledTimeInHours = Helpers.Constants.ScheduledTimeInHours;
                    foreach (var file in uploadedFiles)
                    {
                        diagnostics.WriteInformationTrace(TraceEventId.Flow, "{2} : Updating {0} ({1}) ", file.FileId, file.Name, DateTime.UtcNow);
                        var lifelineInHours = file.ModifiedOn.Value.AddHours(uploadedFilesExpirationDurationInHours).Subtract(DateTime.UtcNow).TotalHours;
                        file.LifelineInHours = (short)(lifelineInHours < scheduledTimeInHours ? scheduledTimeInHours : lifelineInHours);
                        try
                        {
                            bool isUpdateSuccessful = fileService.UpdateFile(file);
                            diagnostics.WriteInformationTrace(TraceEventId.Flow, "{3} : Updated {0} ({1}) : {2}", file.FileId, file.Name, isUpdateSuccessful ? "Success" : "Failure", DateTime.UtcNow);
                        }
                        catch (Exception ex)
                        {
                            diagnostics.WriteErrorTrace(TraceEventId.Flow, "Failure to delete {0} {1}", file.FileId, file.Name + Environment.NewLine + ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                diagnostics.WriteErrorTrace(TraceEventId.Exception, "{1} : Exception occurred : {0}", ex.ToString(), DateTime.UtcNow);
            }
        }
    }
}
