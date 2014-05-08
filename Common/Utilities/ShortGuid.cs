using System;

namespace Microsoft.Research.DataOnboarding.Utilities
{
    /// <summary>
    /// Class for generating short guids
    /// </summary>
    public class ShortGuid
    {
        /// <summary>
        /// Creates a new short guid.
        /// </summary>
        /// <returns>Return the newly generated short guid.</returns>
        public static string NewShortGuid()
        {
            Guid guid = Guid.NewGuid();
            return Convert.ToBase64String(guid.ToByteArray());
        }
    }
}
