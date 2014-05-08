// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace Microsoft.Research.DataOnboarding.WebApplication.ViewModels
{
    public class MetaDataUnitsViewModel
    {
        [Display(Name = "Unit")]
        [DataType(DataType.Text)]
        public string Id { get; set; }

        [Display(Name = "Unit Name")]
        [DataType(DataType.Text)]
        public string Name { get; set; }
    }
}