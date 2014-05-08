// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
      
namespace Microsoft.Research.DataOnboarding.DomainModel
{
    public partial class UserAttribute
    {
        public int UserAttributeId { get; set; }
        public int UserId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public virtual User User { get; set; }
    }
}
