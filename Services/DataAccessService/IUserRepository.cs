// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
      
namespace Microsoft.Research.DataOnboarding.DataAccessService
{
    using System.Collections.Generic;
    using Microsoft.Research.DataOnboarding.DomainModel;

    /// <summary>
    /// Defines contracts for User object
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Creates a new user record
        /// </summary>
        /// <param name="newUser">User data</param>
        /// <returns>Newly created user</returns>
        /// <exception cref="ArgumentNullException">When newUser parameter is null</exception>
        User AddUser(User newUser);

        /// <summary>
        /// Modifies data of an existing user record
        /// </summary>
        /// <param name="modifiedUser">User data to be updated</param>
        /// <returns>Updated user</returns>
        /// <exception cref="ArgumentNullException">When modifiedUser is null</exception>
        User UpdateUser(User modifiedUser);

        /// <summary>
        /// Retrieves the user including the roles associated with the user
        /// from name identifier passed as input
        /// </summary>
        /// <param name="nameIdentifier">Name identifier string</param>
        /// <returns>User information</returns>
        /// <exception cref="ArgumentException">When nameIdentifier is null, empty or whitespace</exception>
        User GetUserByNameIdentifier(string nameIdentifier);

        /// <summary>
        /// Adds a user attribute
        /// </summary>
        /// <param name="attribute">User attribute to augment</param>
        /// <returns>User attribute added to the tracking list of the data context</returns>
        /// <exception cref="ArgumentNullException">When attribute is null</exception>
        UserAttribute AddUserAttribute(UserAttribute attribute);

        /// <summary>
        /// Retrieves user attributes 
        /// </summary>
        /// <param name="userId">Unique identifier of the user</param>
        /// <returns>List of user attributes</returns>
        IEnumerable<UserAttribute> GetUserAttributesByUserId(int userId);

        /// <summary>
        /// Adds a user role
        /// </summary>
        /// <param name="userRole">User role to augment</param>
        /// <returns>User role added to tracking list in the data context</returns>
        /// <exception cref="ArgumentNullException">When userRole is null</exception>
        UserRole AddUserRole(UserRole userRole);

        /// <summary>
        /// Retrieves a collection of user roles. 
        /// </summary>
        /// <param name="userId">Unique identifier of the user</param>
        /// <returns>List of roles assigned to user</returns>
        IEnumerable<UserRole> GetUserRolesByUserId(int userId);

        /// <summary>
        /// Method to get the user data for the specific user id.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>User object.</returns>
        User GetUserbyUserId(int userId);

        /// <summary>
        /// Method to update the UserAuthToken
        /// </summary>
        /// <param name="userAuthToken">UserAuthToken object</param>
        /// <returns>UserAuthToken object</returns>
        AuthToken AddAuthToken(AuthToken userAuthToken);

        /// <summary>
        /// Method to update the UserAuthToken
        /// </summary>
        /// <param name="userAuthToken">UserAuthToken object</param>
        /// <returns>UserAuthToken object</returns>
        AuthToken UpdateAuthToken(AuthToken userAuthToken);
      
        /// <summary>
        /// Retrieves the AuthToken
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="repositoryId">Repository Id</param>
        /// <returns>AuthToken</returns>
        AuthToken GetUserAuthToken(int userId, int repositoryId);
    }
}
