// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.RepositoryAdapters
{
    public class BaseAdapter
    {
        public OperationStatus SendHttpRequest(HttpWebRequest httpRequest)
        {
            OperationStatus status;

            using (HttpWebResponse response = httpRequest.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    status = OperationStatus.CreateSuccessStatus();
                }
                else
                {
                    status = OperationStatus.CreateFailureStatus("File does not exist");
                }
            }

            return status;
        }
    }
}
