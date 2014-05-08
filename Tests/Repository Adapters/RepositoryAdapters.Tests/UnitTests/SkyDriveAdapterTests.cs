using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.SkyDrive;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.SkyDrive.Fakes;
using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Fakes;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RepositoryAdapters.Tests.UnitTests
{
    [TestClass]
    public class SkyDriveAdapterTests
    {
        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(NotImplementedException))]
        public void GetIdentifier_ShouldReturnNotImplementedException()
        {
            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                SkyDriveAdapter skyDriveAdapter = new SkyDriveAdapter();
                skyDriveAdapter.GetIdentifier(null, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(ArgumentNullException), "A null file was inappropriately allowed.")]
        public void PostFile_ShouldReturnArgumentNullException_WhenFileIsNull()
        {
            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                SkyDriveAdapter skyDriveAdapter = new SkyDriveAdapter();
                PublishSkyDriveFileModel publishFileModel = new PublishSkyDriveFileModel();
                skyDriveAdapter.PostFile(publishFileModel);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(ArgumentException), "An empty file name was inappropriately allowed.")]
        public void PostFile_ShouldReturnArgumentException_WhenFileNameIsEmpty()
        {
            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                SkyDriveAdapter skyDriveAdapter = new SkyDriveAdapter();
                PublishSkyDriveFileModel publishFileModel = new PublishSkyDriveFileModel()
                {
                    File = new DataFile()
                };

                skyDriveAdapter.PostFile(publishFileModel);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(ArgumentException), "A file name with whitespaces was inappropriately allowed.")]
        public void PostFile_ShouldReturnArgumentException_WhenFileNameHasWhitespaces()
        {
            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                SkyDriveAdapter skyDriveAdapter = new SkyDriveAdapter();
                PublishSkyDriveFileModel publishFileModel = new PublishSkyDriveFileModel()
                {
                    File = new DataFile() { FileName = " " }
                };

                skyDriveAdapter.PostFile(publishFileModel);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(ArgumentNullException), "A null file content was inappropriately allowed.")]
        public void PostFile_ShouldReturnArgumentNullException_WhenFileContentIsNull()
        {
            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                SkyDriveAdapter skyDriveAdapter = new SkyDriveAdapter();
                PublishSkyDriveFileModel publishFileModel = new PublishSkyDriveFileModel()
                {
                    File = new DataFile() { FileName = "SomeFileName" }
                };

                skyDriveAdapter.PostFile(publishFileModel);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(ArgumentException), "An empty file content was inappropriately allowed.")]
        public void PostFile_ShouldReturnArgumentException_WhenFileContentLengthIsZero()
        {
            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                SkyDriveAdapter skyDriveAdapter = new SkyDriveAdapter();
                PublishSkyDriveFileModel publishFileModel = new PublishSkyDriveFileModel()
                {
                    File = new DataFile() { FileName = "SomeFileName", FileContent = new byte[0] }
                };

                var operationStatus = skyDriveAdapter.PostFile(publishFileModel);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(ArgumentNullException), "A null Access Token was inappropriately allowed.")]
        public void PostFile_ShouldReturnInvalidAccessTokenErrorMessage_WhenAccessTokenIsNull()
        {
            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                SkyDriveAdapter skyDriveAdapter = new SkyDriveAdapter();
                PublishSkyDriveFileModel publishFileModel = new PublishSkyDriveFileModel()
                {
                    File = new DataFile() { FileName = "SomeFileName", FileContent = new byte[1] }
                };

                skyDriveAdapter.PostFile(publishFileModel);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void PostFile_ShouldReturnSucceededStatus_WhenFileUploadIsSuccessful()
        {
            OperationStatus operationStatus;
            string fileId = "1234";
            using (ShimsContext.Create())
            {
                ShimDiagnosticsProvider.ConstructorType = (dp, type) => { };
                SkyDriveAdapter skyDriveAdapter = new SkyDriveAdapter();
                PublishSkyDriveFileModel publishFileModel = new PublishSkyDriveFileModel()
                {
                    File = new DataFile() { FileName = "SomeFileName", FileContent = new byte[1] },
                    AuthToken = new AuthToken()
                };

                ShimSkyDriveAdapter.AllInstances.UploadFilePublishSkyDriveFileModel = (sda, psdfm) => fileId;
                ShimDiagnosticsProvider.AllInstances.WriteInformationTraceTraceEventIdString = (diagnosticsProvider, traceEventId, message) => { };
                operationStatus = skyDriveAdapter.PostFile(publishFileModel);
            }

            Assert.IsTrue(operationStatus.Succeeded);
            Assert.AreEqual(fileId, operationStatus.CustomReturnValues);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(Exception), "Upload file should will throw exception.")]
        public void PostFile_ShouldReturnExceptionMessage_WhenFileUploadThrowsError()
        {
            SkyDriveAdapter skyDriveAdapter = new SkyDriveAdapter();
            PublishSkyDriveFileModel publishFileModel = new PublishSkyDriveFileModel()
            {
                File = new DataFile() { FileName = "SomeFileName", FileContent = new byte[1] },
                AuthToken = new AuthToken()
            };

            OperationStatus operationStatus;
            string errorMessage = "Some error message";
            using (ShimsContext.Create())
            {
                ShimSkyDriveAdapter.AllInstances.UploadFilePublishSkyDriveFileModel = (sda, psdfm) => { throw new Exception(errorMessage); return string.Empty; };
                operationStatus = skyDriveAdapter.PostFile(publishFileModel);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void DownloadFile_ShouldReturnZipFile_WhenTheFileTobeDownloadedIsACsvFile()
        {
            SkyDriveAdapter skyDriveAdapter = new SkyDriveAdapter();
            DataFile dataFile = null;
            using (ShimsContext.Create())
            {
                ShimSkyDriveAdapter.AllInstances.DownloadFileStringAuthToken = (sda, fileId, authToken) => new byte[0];
                dataFile = skyDriveAdapter.DownloadFile("DownloadUrl", "Authorization", "SomeFileName.csv");
            }

            Assert.AreEqual(".zip", dataFile.FileExtentsion);
            Assert.AreEqual("SomeFileName.zip", dataFile.FileName);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void DownloadFile_ShouldReturnExcelFile_WhenTheFileTobeDownloadedIsAXlsxFile()
        {
            SkyDriveAdapter skyDriveAdapter = new SkyDriveAdapter();
            DataFile dataFile = null;
            using (ShimsContext.Create())
            {
                ShimSkyDriveAdapter.AllInstances.DownloadFileStringAuthToken = (sda, fileId, authToken) => new byte[0];
                dataFile = skyDriveAdapter.DownloadFile("DownloadUrl", "Authorization", "SomeFileName.xlsx");
            }

            Assert.AreEqual(".xlsx", dataFile.FileExtentsion);
            Assert.AreEqual("SomeFileName.xlsx", dataFile.FileName);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void DownloadFile_ShouldReturnNull_WhenThereIsAnExceptionWhileDownloadingFile()
        {
            SkyDriveAdapter skyDriveAdapter = new SkyDriveAdapter();
            DataFile dataFile = null;
            string errorMessage = "Some error message";

            try
            {
                using (ShimsContext.Create())
                {
                    ShimSkyDriveAdapter.AllInstances.DownloadFileStringAuthToken = (sda, fileId, authToken) => { throw new Exception(errorMessage); return new byte[0]; };
                    ShimDiagnosticsProvider.AllInstances.WriteErrorTraceTraceEventIdException = (diagnosticsProvider, traceEventId, message) => { };
                    dataFile = skyDriveAdapter.DownloadFile("DownloadUrl", "Authorization", "SomeFileName.xlsx");
                }
            }
            catch (Exception ex)
            {
                Assert.IsNull(dataFile);
                Assert.AreEqual(errorMessage, ex.Message.ToString());
            }
        }
    }
}
