using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.DataAccessService.Fakes;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.FileService;
using Microsoft.Research.DataOnboarding.FileService.FileProcesser;
using Microsoft.Research.DataOnboarding.FileService.FileProcesser.Fakes;
using Microsoft.Research.DataOnboarding.FileService.Interface;
using Microsoft.Research.DataOnboarding.FileService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.FileService.Models;
using Microsoft.Research.DataOnboarding.RepositoriesService;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface.Fakes;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces.Fakes;
using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.Research.DataOnboarding.Utilities.Extensions.Fakes;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fakes = Microsoft.Research.DataOnboarding.DataAccessService.Fakes;

namespace FileService.Tests
{
    /// <summary>
    /// class to test the File Service provider methods
    /// </summary>
    [TestClass]
    public class FileServiceProviderTest
    {
        /// <summary>
        /// StubIFileService private variable 
        /// </summary>
        private StubIFileService fileService = null;

        /// <summary>
        /// Method to Initialize the 
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.fileService = new StubIFileService()
            {
                CheckFileExistsStringInt32 = (fileName, userId) =>
                {
                    return true;
                },
                UploadFileDataDetail = (dataDetail) =>
                {
                    DataDetail objDetail = new DataDetail();
                    objDetail.FileDetail.FileId = 1;
                    objDetail.FileDetail.CreatedBy = 1;
                    return objDetail;
                }
            };
        }

        [TestMethod]
        public void Check_File_Exists()
        {
            StubIFileRepository fileRepository = new StubIFileRepository()
            {
                GetItemInt32String = (userId, fileName) =>
                {
                    return new File() { Name = "test", FileId = 100, Status = "Uploaded" };
                }
            };


            IUnitOfWork unitOfWork =
                new Fakes.StubIUnitOfWork()
                {
                    Commit = () => { return; }
                };
            StubIBlobDataRepository blobDataRepository = new StubIBlobDataRepository()
            {
                GetBlobContentString = (name) =>
                {
                    return new DataDetail();
                }
            };
            IRepositoryDetails repositoryDetails = new StubIRepositoryDetails();
            IRepositoryService repositoryService = new StubIRepositoryService();
            IRepositoryAdapterFactory repositoryAdapterFactory = new StubIRepositoryAdapterFactory();

            var fileProvider = new FileServiceProvider(fileRepository, blobDataRepository, unitOfWork, repositoryDetails, repositoryService, repositoryAdapterFactory);
            var result = fileProvider.CheckFileExists("test", 100);
            Assert.AreEqual(true, result);
            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void Check_File_Exists_Invaid_FileId()
        {
            StubIFileRepository fileRepository = new StubIFileRepository()
            {
                GetItemInt32String = (userId, fileName) =>
                {
                    return new File();
                }
            };
            IUnitOfWork unitOfWork =
                new Fakes.StubIUnitOfWork()
                {
                    Commit = () => { return; }
                };
            StubIBlobDataRepository blobDataRepository = new StubIBlobDataRepository()
            {
                GetBlobContentString = (name) =>
                {
                    return new DataDetail();
                }
            };
            IRepositoryDetails repositoryDetails = new StubIRepositoryDetails();
            IRepositoryService repositoryService = new StubIRepositoryService();
            IRepositoryAdapterFactory repositoryAdapterFactory = new StubIRepositoryAdapterFactory();

            var fileProvider = new FileServiceProvider(fileRepository, blobDataRepository, unitOfWork, repositoryDetails, repositoryService, repositoryAdapterFactory);
            var result = fileProvider.CheckFileExists("test", 200);
            Assert.AreEqual(false, result);
            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void Check_File_Exists_Invaid_FileName()
        {
            StubIFileRepository fileRepository = new StubIFileRepository()
            {
                GetItemInt32String = (userId, fileName) =>
                {
                    return new File() { Name = "wrongfilename", FileId = 100, Status = "Uploaded" };
                }
            };
            IUnitOfWork unitOfWork =
                new Fakes.StubIUnitOfWork()
                {
                    Commit = () => { return; }
                };
            StubIBlobDataRepository blobDataRepository = new StubIBlobDataRepository()
            {
                GetBlobContentString = (name) =>
                {
                    return new DataDetail();
                }
            };

            IRepositoryDetails repositoryDetails = new StubIRepositoryDetails();
            IRepositoryService repositoryService = new StubIRepositoryService();
            IRepositoryAdapterFactory repositoryAdapterFactory = new Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces.Fakes.StubIRepositoryAdapterFactory();

            var fileProvider = new FileServiceProvider(fileRepository, blobDataRepository, unitOfWork, repositoryDetails, repositoryService, repositoryAdapterFactory);
            var result = fileProvider.CheckFileExists("test", 200);
            Assert.AreEqual(true, result);
            Assert.AreEqual(true, true);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(DataFileNotFoundException), "An empty file name was inappropriately allowed.")]
        public async Task GetErrors_ShouldThrowDataFileNotFoundException_WhenFileDoesntExist()
        {
            IFileRepository fileRepository = new StubIFileRepository()
            {
                GetItemInt32Int32 = (userId, FileId) => null
            };

            var fileService = new FileServiceProvider(fileRepository, null, null, null, null, null);
            await fileService.GetErrors(0, 0);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(InvalidOperationException), "An empty file name was inappropriately allowed.")]
        public async Task GetErrors_ShouldThrowInvalidOperationException_WhenFileExtensionIsOtherThanXLSX()
        {
            IFileRepository fileRepository = new StubIFileRepository()
            {
                GetItemInt32Int32 = (userId, FileId) => new File() { Name = "abc.xyz" }
            };

            var fileService = new FileServiceProvider(fileRepository, null, null, null, null, null);
            await fileService.GetErrors(0, 0);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task GetErrors_ShouldReturnFileAndFileSheets()
        {
            IFileRepository fileRepository = new StubIFileRepository()
            {
                GetItemInt32Int32 = (userId, FileId) => new File() { Name = "abc.xlsx" }
            };

            IList<FileSheet> fileSheets = new List<FileSheet>()
            {
                new FileSheet() { SheetId = "1", SheetName = "SomeTabName" },
                new FileSheet() { SheetId = "1", SheetName = "SomeTabName" }
            };

            IFileProcesser fileProcessor = new StubIFileProcesser()
            {
                GetErrorsFile = file => Task.FromResult(fileSheets)
            };

            using (ShimsContext.Create())
            {
                ShimFileFactory.GetFileTypeInstanceStringIBlobDataRepositoryIFileRepositoryIRepositoryService = (fileExtension, blobDataRepository, fileDataRepository, repositoryService) => fileProcessor;
                var fileService = new FileServiceProvider(fileRepository, null, null, null, null, null);
                var fileAndFileSheets = await fileService.GetErrors(0, 0);
                Assert.IsNotNull(fileAndFileSheets.Item1);
                Assert.IsNotNull(fileAndFileSheets.Item2);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(DataFileNotFoundException), "An empty file name was inappropriately allowed.")]
        public async Task RemoveErrors_ShouldThrowDataFileNotFoundException_WhenFileDoesntExist()
        {
            IFileRepository fileRepository = new StubIFileRepository()
            {
                GetItemInt32Int32 = (userId, FileId) => null
            };

            var fileService = new FileServiceProvider(fileRepository, null, null, null, null, null);
            await fileService.RemoveErrors(0, 0, null, null);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(InvalidOperationException), "An empty file name was inappropriately allowed.")]
        public async Task RemoveErrors_ShouldThrowInvalidOperationException_WhenFileExtensionIsOtherThanXLSX()
        {
            IFileRepository fileRepository = new StubIFileRepository()
            {
                GetItemInt32Int32 = (userId, FileId) => new File() { Name = "abc.xyz" }
            };

            var fileService = new FileServiceProvider(fileRepository, null, null, null, null, null);
            await fileService.RemoveErrors(0, 0, null, null);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public async Task RemoveErrors_ShouldRemoveErrorsAndUpdateTheFile()
        {
            IFileRepository fileRepository = new StubIFileRepository()
            {
                GetItemInt32Int32 = (userId, FileId) => new File() { Name = "abc.xlsx" },
                UpdateFileFile = file => file
            };

            IBlobDataRepository blobDataRepository = new StubIBlobDataRepository()
            {
                UploadFileDataDetail = dataDetail => true
            };

            IUnitOfWork unitOfWork = new StubIUnitOfWork();

            IFileProcesser fileProcessor = new StubIFileProcesser()
            {
                RemoveErrorStreamStringIEnumerableOfErrorType = (file, sheetName, ErrorTypeList) => { System.IO.Stream stream = new System.IO.MemoryStream(); return Task.FromResult(stream); }
            };

            using (ShimsContext.Create())
            {
                ShimFileFactory.GetFileTypeInstanceStringIBlobDataRepositoryIFileRepositoryIRepositoryService = (fe, bdr, fdr, rs) => fileProcessor;
                ShimStreamExtensions.GetBytesAsyncStream = stream => Task.FromResult(new byte[0]);
                var fileService = new FileServiceProvider(fileRepository, blobDataRepository, unitOfWork, null, null, null);
                await fileService.RemoveErrors(0, 0, null, null);
                Assert.IsTrue(true);
            }
        }
    }
}
