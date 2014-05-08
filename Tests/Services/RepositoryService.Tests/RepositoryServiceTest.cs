using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.DataAccessService.Fakes;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Research.DataOnboarding.RepositoriesService;

namespace Microsoft.Research.DataOnboarding.RepositoryService.Tests
{
    [TestClass]
    public class RepositoryServiceTest
    {
        /// <summary>
        /// Variable to hold the user repository object.
        /// </summary>
        private IUserRepository userRepository = null;

        /// <summary>
        /// Variable to hold the repository details object.
        /// </summary>
        private IRepositoryDetails repositoryDetails = null;

        /// <summary>
        /// Variable to hold the file repository.
        /// </summary>
        private IFileRepository fileRepository = null;

        /// <summary>
        /// Variable to hold the unit of work object.
        /// </summary>
        private IUnitOfWork unitOfWork = null;

        private IRepositoryService repsoitoryService = null;

        /// <summary>
        /// Method to Initialize the stub interfaces.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            // Unit of work code
            this.unitOfWork =
                new StubIUnitOfWork()
                {
                    Commit = () => { return; }
                };

            // User repository
            this.userRepository = new StubIUserRepository()
            {
                GetUserbyUserIdInt32 = (userId) =>
                {
                    List<UserRole> userRoles = new List<UserRole>();
                    userRoles.Add(new UserRole() { Role = null, RoleId = 1, UserId = userId, UserRoleId = 1 });
                    User userNew = new User() { CreatedOn = DateTime.UtcNow, EmailId = "test@test.com", FirstName = "First", IdentityProvider = "identity", IsActive = true, LastName = "Last", MiddleName = "midele", ModifiedOn = DateTime.UtcNow, NameIdentifier = "nameIdentifier", Organization = "Test Org", UserAttributes = null, UserId = userId, UserRoles = userRoles };
                    return userNew;
                }
            };

            // File repository implementation
            this.fileRepository = new StubIFileRepository()
            {
                GetFilesByRepositoryInt32 = (repositoryId) =>
                    {
                        File fileToAdd = new File() { Citation = "Citation 1", CreatedBy = 1, CreatedOn = DateTime.Now, Description = "Document 1", FileAttributes = null, FileColumns = null, FileId = 1, FileQualityChecks = null, Identifier = "asdahgsdfsghadfsghad", isDeleted = false, MimeType = "Mime type 1", ModifiedBy = 1, ModifiedOn = DateTime.Now, Name = "Document One", Repository = null, RepositoryId = 1, Size = 20.90, Status = "Uploaded", Title = "Document 1" };
                        File fileToAdd1 = new File() { Citation = "Citation 2", CreatedBy = 1, CreatedOn = DateTime.Now, Description = "Document 2", FileAttributes = null, FileColumns = null, FileId = 2, FileQualityChecks = null, Identifier = "wrwe23423ewr", isDeleted = false, MimeType = "Mime type 2", ModifiedBy = 1, ModifiedOn = DateTime.Now, Name = "Document Two", Repository = null, RepositoryId = 1, Size = 20.90, Status = "Posted", Title = "Document 2" };

                        List<File> lstFiles = new List<File>();

                        lstFiles.Add(fileToAdd);
                        lstFiles.Add(fileToAdd);

                        return lstFiles;
                    },
                DeleteFileInt32StringBooleanBoolean = (fileId, status, isFileData, isHardDelete) =>
                    {
                        return true;
                    },
                UpdateFileFile = (modifiedFile) =>
                    {
                        return null;
                    }
            };

            // Repository details fake implementation
            this.repositoryDetails = new StubIRepositoryDetails()
            {
                RetrieveRepositories = () =>
                    {
                        Repository repositoryObject = new Repository()
                        {
                            AllowedFileTypes = "xlsx,nc,csv",
                            CreatedBy = 1,
                            /// Files = null,
                            CreatedOn = DateTime.Now,
                            HttpDeleteUriTemplate = "http://test.com",
                            HttpGetUriTemplate = "http://test.com",
                            HttpIdentifierUriTemplate = "http://test.com",
                            HttpPostUriTemplate = "http://test.com",
                            ImpersonatingPassword = "pwd",
                            ImpersonatingUserName = "user1",
                            IsActive = true,
                            IsImpersonating = true,
                            ModifiedBy = 1,
                            ModifiedOn = DateTime.Now,
                            Name = "Repository1",
                            RepositoryId = 1,
                            UserAgreement = "Test Agreement1",
                            BaseRepositoryId = 1,
                            IsVisibleToAll = true
                        };

                        Repository repositoryObject1 = new Repository()
                        {
                            AllowedFileTypes = "xlsx,csv",
                            CreatedBy = 1,
                            //Files = null,
                            CreatedOn = DateTime.Now,
                            HttpDeleteUriTemplate = "http://gmail.com",
                            HttpGetUriTemplate = "http://gmail.com",
                            HttpIdentifierUriTemplate = "http://gmail.com",
                            HttpPostUriTemplate = "http://gmail.com",
                            ImpersonatingPassword = "pwd2",
                            ImpersonatingUserName = "user2",
                            IsActive = true,
                            IsImpersonating = true,
                            ModifiedBy = 1,
                            ModifiedOn = DateTime.Now,
                            Name = "Repository2",
                            RepositoryId = 2,
                            UserAgreement = "Test Agreement1",
                            BaseRepositoryId = 1,
                            IsVisibleToAll = true
                        };

                        List<Repository> lstRep = new List<Repository>();
                        lstRep.Add(repositoryObject);
                        lstRep.Add(repositoryObject1);
                        return lstRep;
                    },
                RetrieveRepositoryTypes = () =>
                    {
                        List<BaseRepository> lstBaseRep = new List<BaseRepository>();
                        lstBaseRep.Add(new BaseRepository() { BaseRepositoryId = 1, Name = "Merritt" });
                        lstBaseRep.Add(new BaseRepository() { BaseRepositoryId = 2, Name = "Sky" });
                        return lstBaseRep;
                    },
                AddRepositoryRepository = (repository) =>
                    {
                        repository.RepositoryId = 10;
                        repository.Name = "From add method";
                        return repository;
                    },
                UpdateRepositoryRepository = (repository) =>
                    {
                        repository.Name = "From update method";
                        return repository;
                    },
                GetRepositoryByIdInt32 = (repositoryId) =>
                    {
                        Repository repositoryObject = new Repository()
                        {
                            AllowedFileTypes = "xlsx,nc,csv",
                            CreatedBy = 1,
                            /// Files = null,
                            CreatedOn = DateTime.Now,
                            HttpDeleteUriTemplate = "http://test.com",
                            HttpGetUriTemplate = "http://test.com",
                            HttpIdentifierUriTemplate = "http://test.com",
                            HttpPostUriTemplate = "http://test.com",
                            ImpersonatingPassword = "pwd",
                            ImpersonatingUserName = "user1",
                            IsActive = true,
                            IsImpersonating = true,
                            ModifiedBy = 1,
                            ModifiedOn = DateTime.Now,
                            Name = "Get by id method",
                            RepositoryId = repositoryId,
                            UserAgreement = "Test Agreement1",
                            BaseRepositoryId = 1,
                            IsVisibleToAll = true
                        };

                        return repositoryObject;
                    },
                GetBaseRepositoryNameInt32 = (baseRepositoryId) =>
                {
                    return "base rep Name";
                },
                DeleteRepositoryInt32 = (repositoryId) =>
                    {
                        Repository repositoryObject = new Repository()
                        {
                            AllowedFileTypes = "xlsx,nc,csv",
                            CreatedBy = 1,
                            /// Files = null,
                            CreatedOn = DateTime.Now,
                            HttpDeleteUriTemplate = "http://test.com",
                            HttpGetUriTemplate = "http://test.com",
                            HttpIdentifierUriTemplate = "http://test.com",
                            HttpPostUriTemplate = "http://test.com",
                            ImpersonatingPassword = "pwd",
                            ImpersonatingUserName = "user1",
                            IsActive = true,
                            IsImpersonating = true,
                            ModifiedBy = 1,
                            ModifiedOn = DateTime.Now,
                            Name = "Delete rep method",
                            RepositoryId = repositoryId,
                            UserAgreement = "Test Agreement1",
                            BaseRepositoryId = 1,
                            IsVisibleToAll = true
                        };

                        return repositoryObject;
                    },
                GetRepositoryByNameString = (repositoryName) =>
                    {
                        Repository repositoryObject = new Repository()
                        {
                            AllowedFileTypes = "xlsx,nc,csv",
                            CreatedBy = 1,
                            /// Files = null,
                            CreatedOn = DateTime.Now,
                            HttpDeleteUriTemplate = "http://test.com",
                            HttpGetUriTemplate = "http://test.com",
                            HttpIdentifierUriTemplate = "http://test.com",
                            HttpPostUriTemplate = "http://test.com",
                            ImpersonatingPassword = "pwd",
                            ImpersonatingUserName = "user1",
                            IsActive = true,
                            IsImpersonating = true,
                            ModifiedBy = 1,
                            ModifiedOn = DateTime.Now,
                            Name = repositoryName,
                            RepositoryId = 12,
                            UserAgreement = "Test Agreement1",
                            BaseRepositoryId = 1,
                            IsVisibleToAll = true
                        };

                        return repositoryObject;
                    }
            };

            this.repsoitoryService = new RepositoriesService.RepositoryService(repositoryDetails, unitOfWork, userRepository, fileRepository);
        }


        [TestMethod]
        public void Get_All_Repositories_Test()
        {
            var repList = this.repsoitoryService.RetrieveRepositories(true).ToList();

            Assert.IsNotNull(repList);
            Assert.IsNotNull(repList[0].RepositoryData);

            Assert.AreEqual(repList.Count, 2);
            Assert.AreEqual(repList[0].CreatedUser, "First Last");
            Assert.AreEqual(repList[0].AuthenticationType, string.Empty);

            Assert.AreEqual(repList[0].CreatedUser, "First Last");
            Assert.AreEqual(repList[0].AuthenticationType, string.Empty);
            Assert.AreEqual(repList[0].RepositoryData.AllowedFileTypes, "xlsx,nc,csv");
            Assert.AreEqual(repList[0].RepositoryData.CreatedBy, 1);
            Assert.AreEqual(repList[0].RepositoryData.RepositoryId, 1);
            Assert.AreEqual(repList[0].RepositoryData.Name, "Repository1");
            Assert.AreEqual(repList[0].RepositoryData.HttpIdentifierUriTemplate, "http://test.com");
        }

        [TestMethod]
        public void Get_All_Repository_Types_Test()
        {
            var repTypeList = this.repsoitoryService.RetrieveRepositoryTypes().ToList();

            Assert.IsNotNull(repTypeList);
            Assert.AreEqual(repTypeList.Count, 2);

            Assert.AreEqual(repTypeList[0].BaseRepositoryId, 1);
            Assert.AreEqual(repTypeList[0].Name, "Merritt");

            Assert.AreEqual(repTypeList[1].BaseRepositoryId, 2);
            Assert.AreEqual(repTypeList[1].Name, "Sky");
        }

        [TestMethod]
        public void Get_Repository_By_Id_Test()
        {
            var repository = this.repsoitoryService.GetRepositoryById(21);

            Assert.IsNotNull(repository);
            Assert.AreEqual(repository.AllowedFileTypes, "xlsx,nc,csv");
            Assert.AreEqual(repository.CreatedBy, 1);
            Assert.AreEqual(repository.RepositoryId, 21);
            Assert.AreEqual(repository.Name, "Get by id method");
            Assert.AreEqual(repository.HttpDeleteUriTemplate, "http://test.com");
            Assert.AreEqual(repository.HttpGetUriTemplate, "http://test.com");
            Assert.AreEqual(repository.ImpersonatingUserName, "user1");
            Assert.AreEqual(repository.UserAgreement, "Test Agreement1");
        }

        [TestMethod]
        public void Add_Repository_Test()
        {
            Repository repositoryObject = new Repository()
            {
                AllowedFileTypes = "xlsx,nc,csv",
                CreatedBy = 1,
                /// Files = null,
                CreatedOn = DateTime.Now,
                HttpDeleteUriTemplate = "http://test.com",
                HttpGetUriTemplate = "http://test.com",
                HttpIdentifierUriTemplate = "http://test.com",
                HttpPostUriTemplate = "http://test.com",
                ImpersonatingPassword = "pwd",
                ImpersonatingUserName = "user1",
                IsActive = true,
                IsImpersonating = true,
                ModifiedBy = 1,
                ModifiedOn = DateTime.Now,
                Name = "Added repository",
                RepositoryId = 0,
                UserAgreement = "Test Agreement1",
                BaseRepositoryId = 1,
                IsVisibleToAll = true
            };

            var result = this.repsoitoryService.AddUpdateRepository(repositoryObject);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Update_Repository_Test()
        {
            Repository repositoryObject = new Repository()
            {
                AllowedFileTypes = "xlsx,nc,csv",
                CreatedBy = 1,
                /// Files = null,
                CreatedOn = DateTime.Now,
                HttpDeleteUriTemplate = "http://test.com",
                HttpGetUriTemplate = "http://test.com",
                HttpIdentifierUriTemplate = "http://test.com",
                HttpPostUriTemplate = "http://test.com",
                ImpersonatingPassword = "pwd",
                ImpersonatingUserName = "user1",
                IsActive = true,
                IsImpersonating = true,
                ModifiedBy = 1,
                ModifiedOn = DateTime.Now,
                Name = "Updated repository",
                RepositoryId = 12,
                UserAgreement = "Test Agreement1",
                BaseRepositoryId = 1,
                IsVisibleToAll = true,
                RepositoryMetadata = new List<RepositoryMetadata>()
            };

            var result = this.repsoitoryService.AddUpdateRepository(repositoryObject);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddUpdateRepository_ShouldThrowNullRefernceException_WhenAccessTokenIsNull()
        {
            Repository repositoryObject = new Repository()
            {
                IsImpersonating = true,
                BaseRepositoryId = 2
            };

            try
            {
                var result = this.repsoitoryService.AddUpdateRepository(repositoryObject);
                Assert.Fail("Should have exceptioned above!");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentNullException));
                Assert.IsTrue(ex.Message.ToString().Contains("accessToken"));
            }
        }

        [TestMethod]
        public void AddUpdateRepository_ShouldThrowNullRefernceException_WhenRefreshTokenIsNull()
        {
            Repository repositoryObject = new Repository()
            {
                IsImpersonating = true,
                BaseRepositoryId = 2,
                AccessToken = "AccessToken"
            };

            try
            {
                var result = this.repsoitoryService.AddUpdateRepository(repositoryObject);
                Assert.Fail("Should have exceptioned above!");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentNullException));
                Assert.IsTrue(ex.Message.ToString().Contains("refreshToken"));
            }
        }

        [TestMethod]
        public void AddUpdateRepository_ShouldThrowNullRefernceException_WhenTokenExpiresOnIsNull()
        {
            Repository repositoryObject = new Repository()
            {
                IsImpersonating = true,
                BaseRepositoryId = 2,
                AccessToken = "AccessToken",
                RefreshToken = "RefreshToken"
            };

            try
            {
                var result = this.repsoitoryService.AddUpdateRepository(repositoryObject);
                Assert.Fail("Should have exceptioned above!");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentNullException));
                Assert.IsTrue(ex.Message.ToString().Contains("tokenExpiresOn"));
            }
        }

        [TestMethod]
        public void Delete_Repository_Test()
        {
            var result = this.repsoitoryService.DeleteRepository(1);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Check_Repository_Exists_Test()
        {
            var result = this.repsoitoryService.CheckRepositoryExists("Test Repository");

            Assert.AreEqual(result, 12);
        }

    }
}
