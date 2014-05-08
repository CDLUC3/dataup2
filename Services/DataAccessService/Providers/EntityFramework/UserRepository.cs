// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.DataAccessService.Providers.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using Microsoft.Research.DataOnboarding.DataAccessService;
    using Microsoft.Research.DataOnboarding.DomainModel;
    using Microsoft.Research.DataOnboarding.Utilities;

    /// <summary>
    /// Implements user repository <see cref="IUserRepository"/> leveraging
    /// entity framework
    /// </summary>
    public class UserRepository : RepositoryBase, IUserRepository
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="dataContext">Data context</param>
        public UserRepository(IUnitOfWork dataContext)
            : base(dataContext)
        {
        }

        #endregion

        #region IUserRepository implementation

        public User AddUser(User newUser)
        {
            Check.IsNotNull<User>(newUser, "newUser");
            return Context.Users.Add(newUser);
        }

        public User UpdateUser(User modifiedUser)
        {
            Check.IsNotNull<User>(modifiedUser, "modifiedUser");
            User updatedUser = Context.Users.Attach(modifiedUser);
            Context.SetEntityState<User>(updatedUser, EntityState.Modified);
            return updatedUser;
        }

        public User GetUserByNameIdentifier(string nameIdentifier)
        {
            Check.IsNotEmptyOrWhiteSpace(nameIdentifier, "nameIdentifier");

            User user = Context.Users
                            .Include(u => u.UserRoles.Select(r => r.Role))
                            .Where(u => u.NameIdentifier.Equals(nameIdentifier))
                            .FirstOrDefault();
            return user;
        }

        public UserAttribute AddUserAttribute(UserAttribute attribute)
        {
            Check.IsNotNull<UserAttribute>(attribute, "attribute");
            return Context.UserAttributes.Add(attribute);
        }

        public IEnumerable<UserAttribute> GetUserAttributesByUserId(int userId)
        {
            return Context.UserAttributes.Where(ua => ua.UserId.Equals(userId)).AsEnumerable<UserAttribute>();
        }

        public UserRole AddUserRole(UserRole userRole)
        {
            Check.IsNotNull<UserRole>(userRole, "userRole");
            return Context.UserRoles.Add(userRole);
        }

        public IEnumerable<UserRole> GetUserRolesByUserId(int userId)
        {
            IEnumerable<UserRole> roles = from ur in Context.UserRoles
                                          where ur.UserId.Equals(userId)
                                          select ur;
            return roles;
        }

        /// <summary>
        /// Method to get the user data for the specific user id.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>User object.</returns>
        public User GetUserbyUserId(int userId)
        {
            return Context.Users.Where(user => user.UserId == userId).FirstOrDefault();
        }

        /// <summary>
        /// Adds the AuthToken
        /// </summary>
        /// <param name="authToken">AuthToken</param>
        /// <returns>AuthToken</returns>
        public AuthToken AddAuthToken(AuthToken authToken)
        {
            Check.IsNotNull<AuthToken>(authToken, "new UserAuthToken");
            return Context.AuthTokens.Add(authToken);
        }
      
        /// <summary>
        /// Updates Auth token for the User
        /// </summary>
        /// <param name="userAuthToken">AuthToken</param>
        /// <returns>AuthToken</returns>
        public AuthToken UpdateAuthToken(AuthToken userAuthToken)
        {
            Check.IsNotNull<AuthToken>(userAuthToken, "modified UserAuthToken");
            AuthToken originalAuthToken = Context.AuthTokens.Where(authToken => authToken.Id == userAuthToken.Id).FirstOrDefault();

            // get the entry using the originalAuthtoken
            var entry = Context.GetEntry<AuthToken>(originalAuthToken);

            entry.CurrentValues.SetValues(userAuthToken);
            return userAuthToken;
        }

        /// <summary>
        /// Retrieves the AuthToken
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="repositoryId">Repository Id</param>
        /// <returns>AuthToken</returns>
        public AuthToken GetUserAuthToken(int userId, int repositoryId)
        {
            return  Context.AuthTokens.Where(a => a.UserId == userId && a.RespositoryId == repositoryId).FirstOrDefault();
        }

        #endregion
    }
}
