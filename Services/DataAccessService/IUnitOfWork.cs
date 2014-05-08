// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
      
namespace Microsoft.Research.DataOnboarding.DataAccessService
{
    using System;

    /// <summary>
    /// Enables grouping of related work as a single transaction
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Commits the pending work
        /// </summary>
        /// <exception cref="UnitOfWorkCommitException">When commit operation fails</exception>
        void Commit();
    }
}
