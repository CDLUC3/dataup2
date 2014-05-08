using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.Utilities.Fakes;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.Research.DataOnboarding.WebApi.FileHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Research.DataOnboarding.WebApi.Tests.UnitTests
{
    [TestClass]
    public class ZipFileHandlerTests
    {
        [TestMethod]
        public void Upload_ShouldThrowArgumentException_WhenDataFileIsNull()
        {
            IFileService fileService = new StubIFileService();
            DefaultFileHandler defaultFileHandler = new DefaultFileHandler(fileService);
            try
            {
                defaultFileHandler.Upload(null);
                Assert.Fail("Should have exceptioned above!");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentException));
                Assert.IsTrue(ex.Message.ToString().Contains("dataFile"));
            }
        }

        [TestMethod]
        public void Upload_ShouldReturnUploadedFileWithFileIdGreaterThanZero_WhenAFileIsUploaded()
        {
            IFileService fileService = new StubIFileService()
            {
                UploadFileDataDetail = dd => { dd.FileDetail.FileId = 1; return dd; }
            };

            DefaultFileHandler defaultFileHandler = new DefaultFileHandler(fileService);
            DataFile dataFile = new DataFile() { FileContent = new byte[0] };
            IEnumerable<DataDetail> dataDetails;
            List<DataFile> dataFiles = new List<DataFile>();
            dataFiles.Add(new DataFile() { FileContent = new byte[0] });
            dataFiles.Add(new DataFile() { FileContent = new byte[0] });

            using (ShimsContext.Create())
            {
                ShimZipUtilities.GetListOfFilesFromStreamStreamInt32 = (zipstream, userId) => dataFiles;
                dataDetails = defaultFileHandler.Upload(dataFile);
            }

            Assert.IsTrue(dataDetails.All(fd => fd.FileDetail.FileId > 0));
        }
    }
}
