// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
      
namespace Microsoft.Research.DataOnboarding.TestUtilities
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Defines test categories
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class TestCategories
    {
        public const string UnitTest = "UnitTest";
        public const string FunctionalTest = "FunctionalTest";
        public const string Database = "Database";
    }
}
