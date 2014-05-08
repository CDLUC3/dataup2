using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.DomainModel
{
    public partial class BaseRepository
    {
        public BaseRepository()
        {
           // this.Repositories = new List<Repository>();
        }

        public int BaseRepositoryId { get; set; }
        public string Name { get; set; }

       // public virtual ICollection<Repository> Repositories { get; set; }
    }
}
