// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Research.DataOnboarding.Utilities.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Serialize objects
        /// </summary>
        /// <typeparam name="T">Type of object which needs to serialized</typeparam>
        /// <param name="obj">object to be serialized</param>
        /// <param name="rootName">The name of the XML element that encloses the content to serialize or deserialize.</param>
        /// <returns>serialized string</returns>
        public static string SerializeObject<T>(this T obj, string rootName)
        {
            if (obj != null)
            {
                DataContractSerializer se;
                if (string.IsNullOrWhiteSpace(rootName))
                {
                    se = new DataContractSerializer(typeof(T));
                }
                else
                {
                    se = new DataContractSerializer(typeof(T), rootName, string.Empty);
                }

                byte[] tempArr;
                using (MemoryStream ms = new MemoryStream())
                {
                    se.WriteObject(ms, obj);
                    ms.Position = 0;
                    tempArr = new byte[ms.Length];
                    ms.Read(tempArr, 0, Convert.ToInt32(ms.Length));
                }

                return new UTF8Encoding().GetString(tempArr);
            }

            return null;
        }
    }
}
