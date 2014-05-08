// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Research.DataOnboarding.DomainModel;
using Microsoft.Research.DataOnboarding.Utilities.Model;

namespace Microsoft.Research.DataOnboarding.Utilities.Extensions
{
    /// <summary>
    /// Extension class for Datafile class to set the data from the File type.
    /// </summary>
    public static class DataFileExtension
    {
        /// <summary>
        /// Extension method for Datafile class to set the data from the File type.
        /// </summary>
        /// <param name="dataFile">DataFile object.</param>
        /// <param name="file">File object.</param>
        public static void SetValuesFrom(this DataFile dataFile, File file)
        {
            Check.IsNotNull<DataFile>(dataFile, "dataFile");
            Check.IsNotNull<File>(file, "file");

            dataFile.ContentType = file.MimeType;
            dataFile.CreatedBy = file.CreatedBy;
            dataFile.FileExtentsion = System.IO.Path.GetExtension(file.Name);
            dataFile.FileInfo = file;
            dataFile.FileName = file.Name;
        }
    }
}
