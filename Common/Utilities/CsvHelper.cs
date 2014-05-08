// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.Utilities
{
    /// <summary>
    /// Class containing methods used to Parse CSV files.
    /// </summary>
    public static class CsvHelper
    {
        /// <summary>
        /// Method to spilt the comma separated value and return string of values
        /// </summary>
        /// <param name="input">input value</param>
        /// <returns>returns the collecion of strings</returns>
        public static string[] ReadCsvValues(string input)
        {
            string[] values = Helper.CsvSplitter.Split(input);

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = Escape(values[i]);
            }

            return values;
        }

        /// <summary>
        /// Method to escape the value from the given value
        /// </summary>
        /// <param name="value">value input</param>
        /// <returns>returns string</returns>
        public static string Escape(string value)
        {
            if (value.Contains(Constants.Quote))
            {
                value = value.Replace(Constants.Quote, Constants.EscapedQuote);
            }

            if (value.IndexOfAny(Helper.CharacteresThatMustBeQuoted) > -1)
            {
                value = Constants.Quote + value + Constants.Quote;
            }

            return value;
        }

        /// <summary>
        /// Method to un escape the string for the given value
        /// </summary>
        /// <param name="value">value input</param>
        /// <returns>returns the string</returns>
        public static string Unescape(string value)
        {
            if (value.StartsWith(Constants.Quote) && value.EndsWith(Constants.Quote))
            {
                value = value.Substring(1, value.Length - 2);

                if (value.Contains(Constants.EscapedQuote))
                {
                    value = value.Replace(Constants.EscapedQuote, Constants.Quote);
                }
            }

            return value;
        }
    }
}
