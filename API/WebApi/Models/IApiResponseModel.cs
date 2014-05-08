// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.WebApi.Models
{
    /// <summary>
    /// This interface will be implemented by all the web API response models
    /// </summary>
    /// <typeparam name="TEntity">Entity to map properties from</typeparam>
    public interface IApiResponseModel<TEntity> where TEntity : class
    {
        /// <summary>
        /// This method maps the model properties from the entity instance
        /// </summary>
        /// <param name="entity">Entity instance to map from</param>
        void FromEntity(TEntity entity);
    }
}
