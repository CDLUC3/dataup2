using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.RepositoryAdapters.SkyDrive
{
    [DataContract]
    public class Content
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "parent_id")]
        public string ParentId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "upload_location")]
        public string Location { get; set; }

        [DataMember(Name = "source")]
        public string Source { get; set; }

        [DataMember(Name = "link")]
        public string Link { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
