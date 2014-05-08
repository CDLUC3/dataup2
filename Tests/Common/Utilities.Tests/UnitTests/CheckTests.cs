using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DataOnboarding.TestUtilities;

namespace Microsoft.Research.DataOnboarding.Utilities.Tests.UnitTests
{
    /// <summary>
    /// Defines the unit tests for Check
    /// </summary>
    [TestClass]
    public class CheckShould
    {
        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throw_ArgumentNullException_For_Null_Objects()
        {
            Object nullObject = null;
            Check.IsNotNull<Object>(nullObject, "nullObject");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Return_Object_Value_If_NotNull()
        {
            StringBuilder nonNullObject = new StringBuilder("Test");
            StringBuilder returnVal = Check.IsNotNull<StringBuilder>(nonNullObject, "nonNullObject");
            Assert.AreEqual(nonNullObject, returnVal);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(ArgumentException))]
        public void Throw_ArgumentException_For_Null_Empty_Or_Whitespace()
        {
            string emptyStr = string.Empty;
            Check.IsNotEmptyOrWhiteSpace(emptyStr, "emptyStr");
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void Return_String_Value_If_Not_Null_Empty_Or_Whitespace()
        {
            string emptyStr = "Test";
            string returnStr = Check.IsNotEmptyOrWhiteSpace(emptyStr, "emptyStr");
            Assert.AreEqual(emptyStr, returnStr);
        }
    }
}
