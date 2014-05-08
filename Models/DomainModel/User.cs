// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.DomainModel
{
    using System.Collections.Generic;

    public partial class User : UserBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "auto generated code")]
        public User()
        {
            //this.Files = new List<File>();
            this.UserAttributes = new List<UserAttribute>();
            this.UserRoles = new List<UserRole>();
            //this.Repositories = new List<Repository>();
            //this.RepositoryMetadataCollection = new List<RepositoryMetadata>();
            //this.QualityChecks = new List<QualityCheck>();
        }

        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "auto generated code")]
        //public virtual ICollection<File> Files { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "auto generated code")]
        public virtual ICollection<UserAttribute> UserAttributes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "auto generated code")]
        public virtual ICollection<UserRole> UserRoles { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "auto generated code")]
        //public virtual ICollection<Repository> Repositories { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "auto generated code")]
        //public virtual ICollection<RepositoryMetadata> RepositoryMetadataCollection { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "auto generated code")]
        //public virtual ICollection<QualityCheck> QualityChecks { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "auto generated code")]
        //public virtual ICollection<QualityCheck> QualityChecks { get; set; }
    }
}
