// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.WebApi.Models
{    
    /// <summary>
    /// This interface will be implemented by all the web API request models
    /// </summary>
    /// <typeparam name="TEntity">Entity to map properties to</typeparam>
    public interface IApiRequestModel<TEntity> where TEntity : class
    {
        /// <summary>
        /// This method marshalls from conceptual type to entity type
        /// </summary>
        /// <returns>Entity instance</returns>
        TEntity ToEntity();
    }
}