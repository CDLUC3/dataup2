// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using System.Resources;
using System.Reflection;

namespace Microsoft.Research.DataOnboarding.Utilities
{
    public sealed class ZipUtilities
    {
        /// <summary>
        /// Creates Zip file 
        /// </summary>
        /// <param name="inputFolderPath">input folder path to do zip</param>
        /// <returns>returns the memory stream object</returns>
        public static MemoryStream ZipFiles(string inputFolderPath)
        {
            MemoryStream compressStream = new MemoryStream();
            try
            {
                // generate file list
                ArrayList arrOfFiles = GenerateFileList(inputFolderPath);

                using (ZipFile zip = new ZipFile())
                {
                    foreach (string file in arrOfFiles)
                    {
                        zip.AddFile(file, string.Empty);
                    }
                    zip.Save(compressStream);
                }
                return compressStream;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Unzip the Files to destination folder
        /// </summary>
        /// <param name="zipPathAndFile">Zip file Path </param>
        /// <param name="outputFolder">Destination folder  </param>
        /// <param name="deleteZipFile">Deletes Zip file after unzip</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Created this method as generic")]
        public static void UnZipFiles(string zipPathAndFile, string outputFolder, bool deleteZipFile)
        {
            FileStream streamWriter = null;
            try
            {
                ClearReadOnlyAttribute(zipPathAndFile);
                using (ZipFile zip = ZipFile.Read(zipPathAndFile))
                {
                    foreach (ZipEntry entry in zip.Entries)
                    {
                        string ofileName = Path.GetFileName(entry.FileName);
                        if (!string.IsNullOrEmpty(ofileName))
                        {
                            if (entry.FileName.IndexOf(".ini") < 0)
                            {
                                string fullPath = outputFolder + MerritConstants.PathDelimter + ofileName;
                                fullPath = fullPath.Replace("\\ ", MerritConstants.PathDelimter);
                                using (var stream = entry.OpenReader())
                                {
                                    streamWriter = File.Create(fullPath);
                                    int size = 2048;
                                    byte[] data = new byte[2048];
                                    while (true)
                                    {
                                        size = stream.Read(data, 0, data.Length);
                                        if (size > 0)
                                        {
                                            streamWriter.Write(data, 0, size);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    streamWriter.Close();
                                }
                            }
                        }
                    }
                }
                if (deleteZipFile)
                {
                    File.Delete(zipPathAndFile);
                }
            }
            catch
            {
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Generate file List
        /// </summary>
        /// <param name="inputDirectory">input directory</param>
        /// <returns>returns string</returns>
        public static ArrayList GenerateFileList(string inputDirectory)
        {
            ArrayList files = new ArrayList();
            bool empty = true;

            //// add each file in directory
            foreach (string file in Directory.GetFiles(inputDirectory))
            {
                files.Add(file);
                empty = false;
            }

            if (empty)
            {
                if (Directory.GetDirectories(inputDirectory).Length == 0)
                {
                    //// if directory is completely empty, add it
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

        private static void ClearReadOnlyAttribute(string file)
        {
            FileAttributes fa;

            // Make sure the file exists, or else this is worthless
            if (!File.Exists(file))
            {
                return;
            }

            // Get the old file attributes.
            fa = File.GetAttributes(file);

            // Clear the read only bit.
            fa = fa & ~FileAttributes.ReadOnly;
            File.SetAttributes(file, fa);
        }

        /// <summary>
        /// Get the DataFiles from zip file stream uploaded from client
        /// </summary>
        /// <param name="zipstream">stream uploaded from client</param>
        /// <param name="userid">user id</param>
        /// <returns>list of data files</returns>
        public static List<Model.DataFile> GetListOfFilesFromStream(Stream zipstream, int userid)
        {
            List<Model.DataFile> dataFileList = new List<Model.DataFile>();
            using (ZipFile zip = ZipFile.Read(zipstream))
            {
                foreach (ZipEntry entry in zip.Entries)
                {
                    if (entry.FileName.IndexOf('.') > 0)
                    {

                        using (var stream = entry.OpenReader())
                        {
                            long size = stream.Length;
                            byte[] data = new byte[stream.Length];
                            while (true)
                            {
                                size = stream.Read(data, 0, data.Length);
                                if (size <= 0)
                                {
                                    break;
                                }
                            }
                            dataFileList.Add(new Model.DataFile
                            {
                                FileContent = data,
                                FileName = Path.GetFileName(entry.FileName),
                                FileExtentsion = Path.GetExtension(entry.FileName),
                                ContentType =Helper.GetContentType(Path.GetExtension(entry.FileName)),
                                CreatedBy = userid
                            });
                        }
                    }
                }
            }
            return dataFileList;
        }

        

        /// <summary>
        /// fetch the list of files in the zip stream
        /// </summary>
        /// <param name="zipstream">Zip stream uploaded by user </param>
        /// <returns>list of files in a zip</returns>
        public static Dictionary<string, long> GetZippedFileDetails(Stream zipstream)
        {
            Dictionary<string, long> fileDetails = new Dictionary<string, long>();

            if (zipstream != null)
            {
                using (ZipFile zip = ZipFile.Read(zipstream))
                {
                    foreach (ZipEntry entry in zip.Entries)
                    {
                        if (entry.FileName.IndexOf('.') > 0)
                        {
                            using (var stream = entry.OpenReader())
                            {
                                if (!fileDetails.ContainsKey(Path.GetFileName(entry.FileName.ToLower())))
                                {
                                    fileDetails.Add(Path.GetFileName(entry.FileName.ToLower()), stream.Length);
                                }
                            }
                        }
                    }
                }
            }
            return fileDetails;
        }
    }
}
