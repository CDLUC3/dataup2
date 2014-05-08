using Microsoft.Practices.Unity;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Constants = Microsoft.Research.DataOnboarding.FilePurgeService.Helpers.Fakes;
using FSI = Microsoft.Research.DataOnboarding.FileService.Interface;

namespace Microsoft.Research.DataOnboarding.FilePurgeService.Tests
{
    [TestClass]
    public class FilePurgeServiceTests
    {
        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Execute_ShouldCallGetFilesToBePurged()
        {
            bool getFilesToBePurgedWasCalled = false;
            FSI.IFileService fileservice = new StubIFileService()
            {
                GetFilesToBePurgedDouble = validStorageDays =>
                {
                    getFilesToBePurgedWasCalled = true;
                    return new List<File>();
                }
            };

            IUnityContainer unityContainer = new UnityContainer();

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                Constants.ShimConstants.UploadedFilesExpirationDurationInHoursGet = () => 3;
                ShimDiagnosticsProvider.AllInstances.WriteInformationTraceTraceEventIdStringObjectArray = (diagnosticsProvider, traceEventId, message, args) => { };
                IFileService filePurgeService = new FilePurgeService(fileservice, unityContainer);
                filePurgeService.Execute();
            }

            Assert.IsTrue(getFilesToBePurgedWasCalled);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Execute_ShouldCallDeleteFileForEachFileReturnedByGetFilesToBePurged()
        {
            int numberOfCallsToDeleteFile = 0;
            FSI.IFileService fileservice = new StubIFileService()
            {
                GetFilesToBePurgedDouble = validStorageDays => GetFiles(),
                DeleteFileInt32Int32 = (userId, fileId) =>
                {
                    numberOfCallsToDeleteFile++;
                    return true;
                }
            };

            IUnityContainer unityContainer = new UnityContainer();

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                Constants.ShimConstants.UploadedFilesExpirationDurationInHoursGet = () => 3;
                ShimDiagnosticsProvider.AllInstances.WriteInformationTraceTraceEventIdStringObjectArray = (diagnosticsProvider, traceEventId, message, args) => { };
                FilePurgeService filePurgeService = new FilePurgeService(fileservice, unityContainer);
                filePurgeService.Execute();
            }

            Assert.AreEqual(4, numberOfCallsToDeleteFile);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Execute_UnhandledExceptionShouldWriteErrorTrace()
        {
            bool writeErrorTraceWasCalled = false;
            FSI.IFileService fileservice = new StubIFileService()
            {
                GetFilesToBePurgedDouble = validStorageDays => { throw new Exception(); }
            };

            IUnityContainer unityContainer = new UnityContainer();

            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                Constants.ShimConstants.UploadedFilesExpirationDurationInHoursGet = () => 3;
                ShimDiagnosticsProvider.AllInstances.WriteErrorTraceTraceEventIdStringObjectArray = (diagnosticsProvider, traceEventId, message, args) => { writeErrorTraceWasCalled = true; };
                FilePurgeService filePurgeService = new FilePurgeService(fileservice, unityContainer);
                filePurgeService.Execute();
            }

            Assert.IsTrue(writeErrorTraceWasCalled);
        }

        private static List<File> GetFiles()
        {
            List<File> files = new List<File>();

            files.Add(new File()
            {
                FileId = 1,
                CreatedBy = 1003,
                BlobId = "3f7890b7-fde2-4306-8af6-13de806c7f72",
                Title = "Sample - Copy",
                Name = "Sample - Copy.xlsx",
                Size = 7956,
                Status = FileStatus.Uploaded.ToString(),
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                CreatedOn = new DateTime(2013, 7, 17, 6, 13, 36),
                ModifiedOn = new DateTime(2013, 7, 17, 6, 13, 36),
                isDeleted = false,
                LifelineInHours = 71
            });

            files.Add(new File()
            {
                FileId = 2,
                CreatedBy = 1003,
                BlobId = "807ACEB5-1DDC-43B4-8B31-A5BE0347BC22",
                Title = "Sample - Copy (2)",
                Name = "Sample - Copy (2).xlsx",
                Size = 8372,
                Status = FileStatus.Posted.ToString(),
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                CreatedOn = new DateTime(2013, 7, 14, 23, 32, 23),
                ModifiedOn = new DateTime(2013, 7, 14, 23, 32, 23),
                isDeleted = false,
                LifelineInHours = 26
            });

            files.Add(new File()
            {
                FileId = 3,
                CreatedBy = 1004,
                BlobId = "807ACEB5-1DDC-43B4-8B31-A5BE0347BC22",
                Title = "Sample - Copy (3)",
                Name = "Sample - Copy (3).xlsx",
                Size = 8431,
                Status = FileStatus.Uploaded.ToString(),
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                CreatedOn = new DateTime(2013, 7, 17, 10, 32, 23),
                ModifiedOn = new DateTime(2013, 7, 17, 10, 32, 23),
                isDeleted = false,
                LifelineInHours = 26
            });

            files.Add(new File()
            {
                FileId = 4,
                CreatedBy = 1003,
                BlobId = "03F9B3B1-A767-433E-83CB-BA701C5FE502",
                Title = "Sample - Copy (4)",
                Name = "Sample - Copy (4).xlsx",
                Size = 9382,
                Status = FileStatus.Uploaded.ToString(),
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                CreatedOn = new DateTime(2013, 7, 14, 5, 32, 23),
                ModifiedOn = new DateTime(2013, 7, 14, 5, 32, 23),
                isDeleted = false,
                LifelineInHours = 26
            });

            return files;
        }
    }
}
