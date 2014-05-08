// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.Services.UserService
{
    using Microsoft.Research.DataOnboarding.DomainModel;
    using Microsoft.Research.DataOnboarding.Utilities.Model;

    /// <summary>
    /// This interface defines the contract that must implemented by all the user service providers
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// This method registers a new user 
        /// </summary>
        /// <param name="user">User data</param>
        /// <returns>Added user</returns>
        /// <exception cref="ArgumentNullException">When the user is null</exception>
        /// <exception cref="ArgumentException">When the NameIdentifier attribute is null, empty or whitespace</exception>
        /// <exception cref="UserAlreadyExistsException">When the user being added already exists</exception>
        /// <exception cref="UserDataUpdateException">When the user data update fails</exception>
        User RegisterUser(User user);

        /// <summary>
        /// This method retrieves the user information and includes the user role information
        /// </summary>
        /// <param name="nameIdentifier">Name identifier issued by an identity provider</param>
        /// <returns>User data with roles</returns>
        /// <exception cref="ArgumentException">When nameIdentifier is null, empty or whitespace</exception>
        /// <exception cref="UserNotFoundException">When nameidentifier is not found in the database</exception>
        User GetUserWithRolesByNameIdentifier(string nameIdentifier);

        UserAuthTokenStatusModel GetAuthTokenStatus(int userId, int repositoryId);

        AuthToken GetUserAuthToken(int userId, int repositoryId);

        AuthToken AddUpdateAuthToken(AuthToken userAuthToken);

        User GetUserById(int userId);
    }
}
