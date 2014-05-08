using Microsoft.Research.DataOnboarding.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    public class PublishSkyDriveFileModel : PublishFileModel
    {
        public AuthToken AuthToken { get; set; }
        public PostFileModel PostFileModel { get; set; }
        public Repository Repository { get; set; }
    }
}
