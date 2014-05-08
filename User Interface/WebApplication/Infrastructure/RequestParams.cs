// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Specialized;

namespace Microsoft.Research.DataOnboarding.WebApplication.Infrastructure
{
    /// <summary>
    /// class to pass request parameters
    /// </summary>
    public class RequestParams
    {
        public RequestParams()
        {
            //set the default mode as GET
            RequestMode = Infrastructure.RequestMode.GET;
            Values = new NameValueCollection();
            AllowAutoRedirect = true;
        }

        /// <summary>
        /// Get or sets the Type of Request
        /// </summary>
        public RequestMode RequestMode { get; set; }

        /// <summary>
        /// Get or sets the  Request URL
        /// </summary>
        public string RequestURL { get; set; }

        /// <summary>
        /// Get or sets the  Content Type
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        ///  Get or sets the time out value
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// Get or sets the name value collection values
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification= "Property is used to add and get the values" )]
        public NameValueCollection Values { get; set; }

        /// <summary>
        /// Gets or sets indicating AllowAutoRedirect or not
        /// </summary>
        public bool AllowAutoRedirect { get; set; }
    }
}