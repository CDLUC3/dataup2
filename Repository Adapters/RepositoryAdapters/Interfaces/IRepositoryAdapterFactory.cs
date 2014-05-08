// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.RepositoryAdapters.SkyDrive;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces
{
    /// <summary>
    /// Interface methods for RepositoryAdapterFactory
    /// </summary>
    public interface IRepositoryAdapterFactory
    {
        /// <summary>
        /// Method to get the RepositoryAdapter instance
        /// </summary>
        /// <param name="instanceName">Instance Name ex : SkyDrive, Merrit</param>
        /// <returns></returns>
        IRepositoryAdapter GetRepositoryAdapter(string baseRepositoryName);
    }
}
