// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using System.Web.Mvc;

namespace Microsoft.Research.DataOnboarding.WebApplication.Extensions
{
    /// <summary>
    /// Extension Class for adding additional functionalities to Controller
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// It will render a partial view 
        /// </summary>
        /// <param name="controller">Controller</param>
        /// <param name="viewName">partial view name</param>
        /// <param name="model">model object</param>
        /// <returns>html string of a partial view</returns>
        public static string RenderViewToString(this Controller controller, string viewName, object model)
        {
            using (var writer = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                controller.ViewData.Model = model;
                var viewCxt = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, writer);
                viewCxt.View.Render(viewCxt, writer);
                return writer.ToString();
            }
        }

    }
}