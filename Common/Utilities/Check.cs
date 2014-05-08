// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;

namespace Microsoft.Research.DataOnboarding.Utilities
{
    /// <summary>
    /// Utility class with helper methods to validate input data
    /// </summary>
    public static class Check
    {
        /// <summary>
        /// Validates if the value is not null. 
        /// </summary>
        /// <typeparam name="T">Class to be validated</typeparam>
        /// <param name="value">Instance to be validated</param>
        /// <param name="parameterName">Instance name</param>
        /// <exception cref="System.ArgumentNullException">Thrown when argument is null</exception>
        /// <returns>Input instance</returns>
        public static T IsNotNull<T>(T value, string parameterName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        /// <summary>
        /// Method to check whether given object is null and also to check any of the members of the object is null or not.
        /// </summary>
        /// <typeparam name="T">Object type to check.</typeparam>
        /// <param name="objectToCheck">Object to check.</param>        
        public static void CheckNotNull<T>(this T objectToCheck) where T : class
        {
            if (objectToCheck == null)
            {
                throw new ArgumentNullException(objectToCheck.GetType().ToString());
            }

            NullChecker<T>.Check(objectToCheck);
        }

        /// <summary>
        /// Validates if a string value is not null, empty or white space
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="parameterName">variable name</param>
        /// <exception cref="System.ArgumentException">Thrown when string is null, empty or whitespace</exception>
        /// <returns>string value</returns>
        public static string IsNotEmptyOrWhiteSpace(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                        string.Format(CultureInfo.InvariantCulture, "{0} is Null or Whitespace which are invalid", parameterName), parameterName);
            }

            return value;
        }
    }
}
