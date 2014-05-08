using Microsoft.Research.DataOnboarding.TestUtilities;
using Microsoft.Research.DataOnboarding.Utilities.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Microsoft.Research.DataOnboarding.Utilities.Tests.UnitTests
{
    [TestClass]
    public class StreamExtensionsTests
    {
        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(AggregateException), "A stream of null was inappropriately allowed.")]
        public void GetBytesAsync_ShouldThrowArgumentException_WhenSourceStreamIsNull()
        {
            try
            {
                Stream stream = null;
                var task = stream.GetBytesAsync();
                task.Wait();

                Assert.Fail("Should have exceptioned above!");
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException is ArgumentException)
                    throw;
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        [ExpectedException(typeof(AggregateException), "A stream of 0 length was inappropriately allowed.")]
        public void GetBytesAsync_ShouldThrowArgumentException_WhenStreamLengthIsZero()
        {
            try
            {
                Stream stream = new MemoryStream();
                var task = stream.GetBytesAsync();
                task.Wait();

                Assert.Fail("Should have exceptioned above!");
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException is ArgumentException)
                    throw;
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void GetBytesAsync_ShouldReturnOutputByteArray_WhenStreamIsValidAndLengthLessThanChunkSize()
        {
            byte[] inputBytes = new byte[] { 1, 2, 3, 4, 5 };
            Stream stream = new MemoryStream(inputBytes);
            var task = stream.GetBytesAsync();
            task.Wait();
            byte[] outputBytes = task.Result;
            Assert.AreEqual(inputBytes.Length, outputBytes.Length);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void GetBytesAsync_ShouldReturnOutputByteArray_WhenStreamIsValidAndLengthEqualToChunkSize()
        {
            byte[] inputBytes = new byte[4096];
            Stream stream = new MemoryStream(inputBytes);
            var task = stream.GetBytesAsync();
            task.Wait();
            byte[] outputBytes = task.Result;
            Assert.AreEqual(inputBytes.Length, outputBytes.Length);
        }

        [TestMethod]
        [TestCategory(TestCategories.UnitTest)]
        public void GetBytesAsync_ShouldReturnOutputByteArray_WhenStreamIsValidAndLengthGreaterThanChunkSize()
        {
            byte[] inputBytes = new byte[5000];
            Stream stream = new MemoryStream(inputBytes);
            var task = stream.GetBytesAsync();
            task.Wait();
            byte[] outputBytes = task.Result;
            Assert.AreEqual(inputBytes.Length, outputBytes.Length);
        }
    }
}
