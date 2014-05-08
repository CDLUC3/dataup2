using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.Utilities.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for Enums.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Converts an string or an integer value to respective Enum.
        /// </summary>
        /// <typeparam name="TInput">Input type.</typeparam>
        /// <typeparam name="TEnum">Enum type.</typeparam>
        /// <param name="value">Input value.</param>
        /// <param name="defaultValue">Default Enum value.</param>
        /// <returns>Converted Enum value.</returns>
        public static TEnum ToEnum<TInput, TEnum>(this TInput value, TEnum defaultValue)
          where TEnum : struct
        {
            TEnum result = defaultValue;

            Type type = typeof(TEnum);

            if (value != null && type.IsEnum)
            {
                if (Enum.TryParse<TEnum>(value.ToString(), true, out result))
                {
                   // nothing
                }
            }

            return result;
        }
    }
}
