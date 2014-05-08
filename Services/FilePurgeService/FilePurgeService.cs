// ----------------------------------------------------------------------- 
// <copyright file="FilePurgeService.cs" company="Microsoft"> 
// copyright 2013
// </copyright> 
// -----------------------------------------------------------------------

using Microsoft.Practices.Unity;
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
    public class FilePurgeService : IFileService
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
        /// Unity container instance.
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePurgeService" /> class.
        /// </summary>
        /// <param name="fileService">File service instance.</param>
        public FilePurgeService(FSI.IFileService fileService, IUnityContainer container)
        {
            this.diagnostics = new DiagnosticsProvider(this.GetType());
            this.fileService = fileService;
            this.container = container;
        }

        /// <summary>
        /// Execute Action.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We need to catch any exception and log it. This should not break the application.")]
        public void Execute()
        {
            try
            {
                var filesToBePurged = fileService.GetFilesToBePurged(Helpers.Constants.UploadedFilesExpirationDurationInHours);
                if (filesToBePurged != null)
                {
                    diagnostics.WriteInformationTrace(TraceEventId.Flow, "{1} : Found {0} files which has to be purged", filesToBePurged.Count(), DateTime.UtcNow);
                    foreach (var file in filesToBePurged)
                    {
                        diagnostics.WriteInformationTrace(TraceEventId.Flow, "{2} : Deleting {0} ({1}) ", file.FileId, file.Name, DateTime.UtcNow);
                        try
                        {
                            bool isDeleteSuccessful = fileService.DeleteFile(file.CreatedBy, file.FileId);
                            diagnostics.WriteInformationTrace(TraceEventId.Flow, "{3} : Deleted {0} ({1}) : {2}", file.FileId, file.Name, isDeleteSuccessful ? "Success" : "Failure", DateTime.UtcNow);
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
