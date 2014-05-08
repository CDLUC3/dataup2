// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.SkyDrive;
using Microsoft.Research.DataOnboarding.Utilities.Enums;
using System;
using System.Collections.Specialized;

namespace Microsoft.Research.DataOnboarding.RepositoryAdapters
{
    /// <summary>
    /// Concrete class for IRepositoryAdapterFactory
    /// </summary>
    public class RepositoryAdapterFactory : IRepositoryAdapterFactory
    {
        /// <summary>
        /// Returns the instance of RepositoryAdapter
        /// </summary>
        /// <param name="baseRepository">BaseRepository Name.</param>
        /// <returns>Repository Adapter</returns>
        public IRepositoryAdapter GetRepositoryAdapter(string baseRepositoryName)
        {
            BaseRepositoryEnum baseRepository = (BaseRepositoryEnum) Enum.Parse(typeof(BaseRepositoryEnum), baseRepositoryName);
                        
            switch (baseRepository)
            {
                case BaseRepositoryEnum.SkyDrive:
                    return new SkyDriveAdapter();

                case BaseRepositoryEnum.Merritt:
                    return new MerritAdapter();

                default:
                   return new MerritAdapter();
            }
        }
    }
}
