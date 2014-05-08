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
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Microsoft.Research.DataOnboarding.FileService.FileProcesser.Extensions
{
    /// <summary>
    /// class contains the extension methods of work book part
    /// </summary>
    public static class WorkbookPartExtension
    {
        // Given the main workbook part, and a text value, insert the text into the shared
        // string table. Create the table if necessary. If the value already exists, return
        // its index. If it doesn't exist, insert it and return its new index.
        public static int InsertSharedStringItem(this WorkbookPart wbPart, string value)
        {
            // Insert a value into the shared string table, creating the table if necessary.
            // Insert the string if it's not already there.
            // Return the index of the string.

            int index = 0;
            bool found = false;
            var stringTablePart = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

            // If the shared string table is missing, something's wrong.
            // Just return the index that you found in the cell.
            // Otherwise, look up the correct text in the table.
            if (stringTablePart == null)
            {
                // Create it.
                stringTablePart = wbPart.AddNewPart<SharedStringTablePart>();
            }

            SharedStringTable stringTable;

            if (stringTablePart != null && stringTablePart.SharedStringTable != null)
            {
                stringTable = stringTablePart.SharedStringTable;
            }
            else
            {
                stringTable = new SharedStringTable();
                stringTablePart.SharedStringTable = new SharedStringTable();
            }

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in stringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == value)
                {
                    found = true;
                    break;
                }
                index += 1;
            }

            if (!found)
            {
                if (stringTablePart.SharedStringTable.Count == null)
                {
                    stringTablePart.SharedStringTable.AppendChild(new SharedStringItem(new Text(value)));
                    stringTablePart.SharedStringTable.Save();
                }
                else
                {
                    stringTable.AppendChild(new SharedStringItem(new Text(value)));

                    // stringTablePart.SharedStringTable.AppendChild(new SharedStringItem(new Text(value)));
                    stringTable.Save();
                }
            }

            return index;
        }
    }
}
