// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.RepositoriesService.Interface;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.WebApi.Resources;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.Research.DataOnboarding.WebApi.Api
{
    /// <summary>
    ///  class to manage the repository types related API methods
    /// </summary>
    [Authorize(Roles = "Administrator")]
    public class RepositoryTypesController : ApiController
    {
        /// <summary>
        /// Statis message.
        /// </summary>
        string message = string.Empty;

        /// <summary>
        /// Status code.
        /// </summary>
        HttpStatusCode status = HttpStatusCode.OK;

        /// <summary>
        /// Repository service interface.
        /// </summary>
        private IRepositoryService repositoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryTypesController" /> class.
        /// </summary>
        /// <param name="repositoryService">Repository service object.</param>
        public RepositoryTypesController(IRepositoryService repositoryService)
        {
            this.repositoryService = repositoryService;
        }

        /// <summary>
        /// Method to get all the available repository types.
        /// </summary>
        /// <param name="types">Types input string.</param>
        /// <returns>Collection of types as response.</returns>
        [HttpGet]
        public HttpResponseMessage Get()
        {
            // Check if the model binding was successful and is in a valid state
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Empty);
            }
            try
            {
                // Check if the repository service is valid
                Check.IsNotNull(this.repositoryService, "repositoryService");

                IEnumerable<BaseRepository> repositoryTypeList = this.repositoryService.RetrieveRepositoryTypes();

                return Request.CreateResponse<IEnumerable<BaseRepository>>(HttpStatusCode.OK, repositoryTypeList);

            }
            catch (ArgumentNullException ane)
            {
                if (ane.ParamName.Equals("repositoryService"))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, MessageStrings.Repository_Service_Is_Null);
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, MessageStrings.Invalid_Repository_id);
            }
            catch (Exception ex)
            {
                string error = ex.Message + ex.StackTrace + ex.GetType().ToString();
                if (null != ex.InnerException)
                {
                    error += ex.InnerException.Message + ex.InnerException.StackTrace + ex.InnerException.GetType().ToString();
                }
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, error);
            }
        }
    }
}
