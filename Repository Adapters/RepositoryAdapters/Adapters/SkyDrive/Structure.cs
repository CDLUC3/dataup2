using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.RepositoryAdapters.SkyDrive
{
    [DataContract]
    public class Structure
    {
        [DataMember(Name = "data")]
        public List<Content> Items { get; set; }
    }
}
