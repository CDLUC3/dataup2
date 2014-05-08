// ----------------------------------------------------------------------- 
// <copyright file="IFileService.cs" company="Microsoft"> 
// copyright 2013
// </copyright> 
// -----------------------------------------------------------------------

using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.FilePurgeService
{
    /// <summary>
    /// Interface for purging files
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Execute Action.
        /// </summary>
        void Execute();
    }
}
