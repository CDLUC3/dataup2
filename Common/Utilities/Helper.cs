// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.Research.DataOnboarding.Utilities
{
    /// <summary>
    /// class to keep helper functions
    /// </summary>
    public static class Helper
    {
        public static char[] CharacteresThatMustBeQuoted
        {
            get
            {
                return new char[] { ',', '"', '\n' };
            }
        }

        public static Regex CsvSplitter
        {
            get
            {
                return new Regex(@",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))");
            }
        }

        /// <summary>
        /// Method to DeSerialize the xml object
        /// </summary>
        /// <typeparam name="T">generic object</typeparam>
        /// <param name="xml">xml content</param>
        /// <param name="rootName">root name</param>
        /// <returns>returns deserialized generic object</returns>
        public static T DeSerializeObject<T>(string xml, string rootName)
        {
            DataContractSerializer dataContractSerializer;
            if (string.IsNullOrEmpty(rootName))
            {
                dataContractSerializer = new DataContractSerializer(typeof(T));
            }
            else
            {
                dataContractSerializer = new DataContractSerializer(typeof(T), rootName, string.Empty);
            }

            object output = null;
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                memoryStream.Position = 0L;

                XmlDictionaryReaderQuotas readerQuota = new XmlDictionaryReaderQuotas();
                readerQuota.MaxArrayLength = int.MaxValue;
                readerQuota.MaxStringContentLength = int.MaxValue;

                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(memoryStream, Encoding.UTF8, readerQuota, null);
                output = dataContractSerializer.ReadObject(reader);
            }

            return (T)output;
        }

        /// <summary>
        /// Helper method to check whether current file extension is allowed or not.
        /// </summary>
        /// <param name="fileExt">File extension.</param>
        /// <param name="allowedFileTypes">Allowed extensions.</param>
        /// <param name="fileDelimiter">Delimiter to split.</param>
        /// <returns>True if success else false.</returns>
        public static bool CheckFileTypeExists(string fileExt, string allowedFileTypes, string fileDelimiter)
        {
            bool isSupports = false;

            if (string.IsNullOrWhiteSpace(allowedFileTypes))
            {
                isSupports = true;
            }
            else
            {
                var fileTypeList = allowedFileTypes.Split(new string[] { fileDelimiter }, StringSplitOptions.None);

                var currentFileType = fileTypeList.Where(fileType => fileType.Equals(fileExt, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(currentFileType))
                {
                    isSupports = true;
                }
            }

            return isSupports;
        }

        /// <summary>
        /// Gets content type from resource file
        /// </summary>
        /// <param name="extension">extension of a file</param>
        /// <returns></returns>
        public static string GetContentType(string extension)
        {
            extension = extension.Remove(0, 1);
            var manager = Resource.ContentTypes.ResourceManager;
            return manager.GetString(extension);
        }

        /// <summary>
        /// Method to check the given integer list is in ascending order or not.
        /// </summary>
        /// <param name="lstInt">list of integers.</param>
        /// <returns>If the list is in order then true else false.</returns>
        public static bool CheckIntListAscOrder(List<int> lstInt)
        {
            bool result = true;
            if (lstInt != null && lstInt.Count > 1)
            {
                for (int i = 1; i < lstInt.Count; i++)
                {
                    if (lstInt[i - 1] > lstInt[i])
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }
    }
}
