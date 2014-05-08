// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.WebApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Research.DataOnboarding.DomainModel;
    using Microsoft.Research.DataOnboarding.Utilities;

    public class UserInformation : UserBase, IApiRequestModel<User>, IApiResponseModel<User>
    {
        #region Constructors

        public UserInformation()
        {
        }

        public UserInformation(User user)
        {
            this.InitializeFromEntity(user);
        }

        #endregion

        #region Properties

        public List<string> Roles { get; set; }

        #endregion

        #region IApiRequestModel<User> members

        public User ToEntity()
        {
            return new User()
            {
                NameIdentifier = this.NameIdentifier,
                IdentityProvider = this.IdentityProvider,
                FirstName = this.FirstName,
                MiddleName = this.MiddleName,
                LastName = this.LastName,
                EmailId = this.EmailId,
                Organization = this.Organization,
            };
        }
        #endregion

        #region IApiResponseModel<User> members

        public void FromEntity(User entity)
        {
            this.InitializeFromEntity(entity);
        }

        #endregion

        private void InitializeFromEntity(User userEntity)
        {
            Check.IsNotNull<User>(userEntity, "entity");

            this.UserId = userEntity.UserId;
            this.NameIdentifier = userEntity.NameIdentifier;
            this.IdentityProvider = userEntity.IdentityProvider;
            this.FirstName = userEntity.FirstName;
            this.MiddleName = userEntity.MiddleName;
            this.LastName = userEntity.LastName;
            this.EmailId = userEntity.EmailId;
            this.Organization = userEntity.Organization;
            if (null != userEntity.UserRoles && userEntity.UserRoles.Count > 0)
            {
                this.Roles = new List<string>();
                this.Roles.AddRange(from ur in userEntity.UserRoles
                                    select ur.Role.Name);
            }
        }
    }
}