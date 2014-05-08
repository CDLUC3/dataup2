// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    /// <summary>
    /// Represents the Metadata details.
    /// </summary>
    public class MetadataDetailList
    {
        public MetadataDetailList()
        {
            this.Metadata = new Collection<MetadataDetail>();
            this.ParamterMetadata = new Collection<ParameterMetadataDetail>();
        }

        public int MetadataTypeID { get; set; }
        public Collection<MetadataDetail> Metadata { get; set; }
        public Collection<ParameterMetadataDetail> ParamterMetadata { get; set; }
    }
}