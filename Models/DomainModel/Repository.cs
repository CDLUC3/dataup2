
namespace Microsoft.Research.DataOnboarding.DomainModel
{
    using System;
    using System.Collections.Generic;
    public partial class Repository
    {
        public Repository()
        {
            //  this.Files = new List<File>();
            //   this.RepositoryMetadata = new List<RepositoryMetadata>();
            // this.BaseRepository = new BaseRepository();
            // this.RepositoryType = new RepositoryType();
        }

        public int RepositoryId { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string Name { get; set; }
        public Nullable<bool> IsImpersonating { get; set; }
        public string ImpersonatingUserName { get; set; }
        public string ImpersonatingPassword { get; set; }
        public string UserAgreement { get; set; }
        public string HttpGetUriTemplate { get; set; }
        public string HttpPostUriTemplate { get; set; }
        public string HttpDeleteUriTemplate { get; set; }
        public string HttpIdentifierUriTemplate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string AllowedFileTypes { get; set; }
        public int BaseRepositoryId { get; set; }
        public bool IsVisibleToAll { get; set; }
        public virtual BaseRepository BaseRepository { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public Nullable<System.DateTime> TokenExpiresOn { get; set; }

        public virtual ICollection<RepositoryMetadata> RepositoryMetadata { get; set; }
    }
}
