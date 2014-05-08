// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Text;

namespace Microsoft.Research.DataOnboarding.Utilities.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// The extension method create a Base64 encoded string from a normal string.
        /// </summary>
        /// <param name="toEncode">The String containing the characters to encode.</param>
        /// <returns>The Base64 encoded string.</returns>
        public static string EncodeTo64(this string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.Unicode.GetBytes(toEncode);

            string returnValue = Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;
        }

        /// <summary>
        /// The extension method can be used to decode the base 64 encoded string .
        /// </summary>
        /// <param name="encodedData">The String containing the characters to decode.</param>
        /// <returns>A String containing the results of decoding the specified sequence of bytes.</returns>
        public static string DecodeFrom64(this string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);

            string returnValue = Encoding.Unicode.GetString(encodedDataAsBytes);

            return returnValue;
        }

        /// <summary>
        /// Extension method to convert the string to the integer.
        /// </summary>
        /// <param name="strData">String data.</param>
        /// <returns>Conevrted int.</returns>
        public static int ToInt(this string strData)
        {
            int returnValue = 0;

            if (!string.IsNullOrEmpty(strData))
            {
                if (!int.TryParse(strData, out returnValue))
                {
                    returnValue = 0;
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Extension method to convert the string to the double.
        /// </summary>
        /// <param name="strData">String data.</param>
        /// <returns>Conevrted double.</returns>
        public static double ToDouble(this string strData)
        {
            double returnValue = 0;

            if (!string.IsNullOrEmpty(strData))
            {
                if (!double.TryParse(strData, out returnValue))
                {
                    returnValue = 0;
                }
            }
            return returnValue;
        }

        public static bool IsAlphaNum(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            for (int i = 0; i < str.Length; i++)
            {
                if (!(char.IsLetter(str[i])) && (!(char.IsNumber(str[i]))))
                    return false;
            }

            return true;
        }

        public static bool IsNumeric(this string str)
        {
            bool isNumeric = true;
            try
            {
                double.Parse(str);
            }
            catch
            {
                isNumeric = false;
            }
            return isNumeric;
        }
    }
}
