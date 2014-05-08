// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DataAccessService;
using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.DomainModel.ConceptualModel;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.Research.DataOnboarding.Services.UserService
{
    /// <summary>
    /// This class provides implementation to the <see cref="IUserService"/> interface. 
    /// </summary>
    public sealed class UserServiceProvider : IUserService
    {
        private IUserRepository userRepository;
        private IUnitOfWork unitOfWork;
        private IRepositoryDetails repositoryDetails;

        public UserServiceProvider(IUserRepository userRepository, IUnitOfWork unitOfWork, IRepositoryDetails repositoryDetails)
        {
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
            this.repositoryDetails = repositoryDetails;
        }

        public UserServiceProvider(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IUserService members

        /// <summary>
        /// Creates the User
        /// </summary>
        /// <param name="user">User to be created</param>
        /// <returns>User with updated values</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validating with Check helper")]
        public User RegisterUser(User user)
        {
            Check.IsNotNull<User>(user, "user");
            Check.IsNotEmptyOrWhiteSpace(user.NameIdentifier, "user.NameIdentifier");

            try
            {
                User existingUser = this.userRepository.GetUserByNameIdentifier(user.NameIdentifier);
                if (null != existingUser)
                {
                    throw new UserAlreadyExistsException()
                    {
                        NameIdentifier = user.NameIdentifier,
                        IdentityProvider = user.IdentityProvider
                    };
                }

                user.CreatedOn = DateTime.UtcNow;
                user.ModifiedOn = DateTime.UtcNow;
                user.IsActive = true;
                user.UserRoles.Add(new UserRole()
                {
                    RoleId = (int)Roles.User
                    
                });
                
                User registeredUser = this.userRepository.AddUser(user);
                unitOfWork.Commit();

                // TODO: v-rajask, The code below  was added to return the roles.
                // Entity framework does not pupoulate the complete graph (it only 
                // populates the keys. Role object on UserRole is empty. This 
                // issue needs investigation and proper fix. 
                Role userRole = new Role()
                {
                    Name = Roles.User.ToString()
                };
                registeredUser.UserRoles.First().Role = userRole;
                return registeredUser;
            }
            catch (UnitOfWorkCommitException uowce)
            {
                throw new UserDataUpdateException("Failed to register user.", uowce);
            }
        }

        /// <summary>
        /// Returns User
        /// </summary>
        /// <param name="nameIdentifier">NameIndentifier of the user</param>
        /// <returns>User object</returns>
        public User GetUserWithRolesByNameIdentifier(string nameIdentifier)
        {
            Check.IsNotEmptyOrWhiteSpace(nameIdentifier, "nameIdentifier");

            User retrievedUser = this.userRepository.GetUserByNameIdentifier(nameIdentifier);
            if (null == retrievedUser)
            {
                throw new UserNotFoundException()
                {
                    NameIdentifier = nameIdentifier
                };
            }
            return retrievedUser;            
        }

        /// <summary>
        /// Returns if the redirection required or not by checking the AuthTokens in the database
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="repositoryId">Repository Id</param>
        /// <returns>UserAuthTokenStatusModel Model</returns>
        public UserAuthTokenStatusModel GetAuthTokenStatus(int userId, int repositoryId)
        {
            UserAuthTokenStatusModel authTokenStatus = new UserAuthTokenStatusModel();
            Repository repository = this.repositoryDetails.GetRepositoryById(repositoryId);
            
            if (string.Compare(repository.BaseRepository.Name, BaseRepositoryEnum.SkyDrive.ToString(), StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                authTokenStatus.RedirectionRequired = false;
                return authTokenStatus;
            }
            else if ((bool)repository.IsImpersonating)
            {
                authTokenStatus.RedirectionRequired = false;
                return authTokenStatus;
            }
            else
            {
                AuthToken authToken = this.GetUserAuthToken(userId, repositoryId);

                if (authToken == null)
                {
                    authTokenStatus.RedirectionRequired = true;
                }
                else if (string.IsNullOrEmpty(authToken.AccessToken) && string.IsNullOrEmpty(authToken.RefreshToken))
                {
                    authTokenStatus.RedirectionRequired = true;
                }
                else if (string.IsNullOrEmpty(authToken.RefreshToken) && authToken.TokenExpiresOn <= DateTime.UtcNow.AddMinutes(1))
                {
                    authTokenStatus.RedirectionRequired = true;
                }
            }
           
            return authTokenStatus;
        }
       
        /// <summary>
        /// Gets the AuthToken
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="repositoryId">Repository Id</param>
        /// <returns>AuthToken For User and Repository</returns>
        public AuthToken GetUserAuthToken(int userId, int repositoryId)
        {
            return this.userRepository.GetUserAuthToken(userId, repositoryId);
        }

        /// <summary>
        /// Adds or updates AuthToken for the particulat user and repository
        /// </summary>
        /// <param name="userAuthToken">AuthToken to be updated or added</param>
        /// <returns>AuthToken with updated values</returns>
        public AuthToken AddUpdateAuthToken(AuthToken userAuthToken)
        {
            AuthToken result;

            if (userAuthToken.Id > 0 )
            {
                result = this.userRepository.UpdateAuthToken(userAuthToken);
            }
            else
            {
                // check if the auth token exists for the user and repository.
                AuthToken token = this.GetUserAuthToken(userAuthToken.UserId, userAuthToken.RespositoryId);

                if (token != null)
                {
                    userAuthToken.Id = token.Id;
                    result = this.userRepository.UpdateAuthToken(userAuthToken);
                }
                else
                {
                    result = this.userRepository.AddAuthToken(userAuthToken);
                }
            }

            unitOfWork.Commit();
            return result;
        }

        public User GetUserById(int userId)
        {
            User retrievedUser = this.userRepository.GetUserbyUserId(userId);
            if (null == retrievedUser)
            {
                throw new UserNotFoundException()
                {
                    UserId = userId
                };
            }
            return retrievedUser;
        }

        #endregion


        
    }
}
