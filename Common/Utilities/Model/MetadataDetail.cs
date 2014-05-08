// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    /// <summary>
    /// Class that represents Metadata.
    /// </summary>
    public class MetadataDetail
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public bool IsRequired { get; set; }

        public string Description { get; set; }

        public string Value { get; set; }

        public float? MinimumAllowedValue { get; set; }

        public float? MaximumAllowedValue { get; set; }
    }


}
