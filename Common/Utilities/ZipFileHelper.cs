// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.IO;
using Ionic.Zip;

namespace Microsoft.Research.DataOnboarding.Utilities
{
    /// <summary>
    /// Helper for Zip functionality.
    /// </summary>
    public static class ZipFileHelper
    {
        /// <summary>
        /// Creates Zip file 
        /// </summary>
        /// <param name="inputFolderPath">Folder to zip</param>
        public static Stream ZipFiles(string inputFolderPath)
        {
            ArrayList arrOfFiles = GenerateFileList(inputFolderPath);

            MemoryStream compressStream = new MemoryStream();
            using (ZipFile zip = new ZipFile())
            {
                foreach (string file in arrOfFiles)
                {
                    if (File.Exists(file))
                    {
                        zip.AddFile(file, string.Empty);
                    }
                }
                zip.Save(compressStream);
            }

            return compressStream;
        }

        /// <summary>
        /// Creates a zip file on local and send the path.
        /// </summary>
        /// <param name="inputFolderPath">Input path to zip.</param>
        /// <returns>Path of the output file.</returns>
        public static void SaveAsZipFile(string inputFolderPath, string targetFileName)
        {
            ArrayList arrOfFiles = GenerateFileList(inputFolderPath);    

            using (ZipFile zip = new ZipFile())
            {
                foreach (string file in arrOfFiles)
                {
                    if (File.Exists(file))
                    {
                        zip.AddFile(file, string.Empty);
                    }
                }
                zip.Save(targetFileName);
            }
        }

        /// <summary>
        /// Generate file List
        /// </summary>
        /// <param name="inputDirectory">Directory input</param>
        /// <returns>Array list.</returns>
        public static ArrayList GenerateFileList(string inputDirectory)
        {
            ArrayList files = new ArrayList();
            bool empty = true;
                        
            foreach (string file in Directory.GetFiles(inputDirectory))
            {
                files.Add(file);
                empty = false;
            }

            if (empty)
            {
                if (Directory.GetDirectories(inputDirectory).Length == 0)
                {
                    // if directory is completely empty, add it
                    files.Add(inputDirectory + @"/");
                }
            }

            // Zip All files in the sub folders.
            foreach (string dirs in Directory.GetDirectories(inputDirectory))
            {
                foreach (object obj in GenerateFileList(dirs))
                {
                    files.Add(obj);
                }
            }

            return files;
        }
    }
}
