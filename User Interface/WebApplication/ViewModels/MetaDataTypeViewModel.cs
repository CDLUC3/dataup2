// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace Microsoft.Research.DataOnboarding.WebApplication.ViewModels
{
    public class MetaDataTypeViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Metadata is required.")]
        [Display(Name = "Metadata")]
        [DataType(DataType.Text)]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Metadata Type is required.")]
        [Display(Name = "Metadata Type")]
        [DataType(DataType.Text)]
        public string Name { get; set; }
    }
}