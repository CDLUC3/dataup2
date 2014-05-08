// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.WebApi.Helpers
{
    using Microsoft.Research.DataOnboarding.DomainModel;
    using Microsoft.Research.DataOnboarding.Services.UserService;
    using Microsoft.Research.DataOnboarding.Utilities;
    using System.Linq;
    using System.Security.Claims;

    public static class IdentityHelper
    {
        /// <summary>
        /// This method inspects <see cref="ClaimsPrincipal"/> to get the NameClaimType
        /// and retrieve the value of the same. 
        /// </summary>
        /// <param name="principal">Claims principal to inspect</param>
        /// <returns>Name claim type</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Using Check helper to validate input")]
        public static string GetNameClaimTypeValue(ClaimsPrincipal principal)
        {
            Check.IsNotNull(principal, "principal");
            return principal.FindFirst(principal.Identities.First().NameClaimType).Value;
        }

        /// <summary>
        /// This method retrieves the user from the principal <see cref="ClaimsPrincipal"/> object
        /// passed as parameter and returns the primary key for the user entity
        /// </summary>
        /// <param name="userService">User service</param>
        /// <param name="principal">Principal object</param>
        /// <returns>User entity</returns>
        public static User GetCurrentUser(IUserService userService, ClaimsPrincipal principal)
        {
            Check.IsNotNull(principal, "principal");
            Check.IsNotNull(userService, "userService");
            string nameIdentifier = GetNameClaimTypeValue(principal);
            User retrievedUser = userService.GetUserWithRolesByNameIdentifier(nameIdentifier);
            return retrievedUser;
        }

        /// <summary>
        /// This method retrieves the user by using name identifier.
        /// </summary>
        /// <param name="userService">User service</param>
        /// <param name="nameIdentifier">Name Identifier</param>
        /// <returns>User entity</returns>
        public static User GetUser(IUserService userService, string nameIdentifier)
        {
            Check.IsNotNull(userService, "userService");
            User retrievedUser = userService.GetUserWithRolesByNameIdentifier(nameIdentifier);
            return retrievedUser;
        }
    }
}
