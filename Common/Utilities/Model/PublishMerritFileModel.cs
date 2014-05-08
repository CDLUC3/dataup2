using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    public class PublishMerritFileModel: PublishFileModel
    {
        public MerritQueryData MerritQueryData { get; set; }
        public RepositoryModel RepositoryModel { get; set; }
    }
}
