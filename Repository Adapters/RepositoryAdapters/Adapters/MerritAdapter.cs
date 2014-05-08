// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Ionic.Zip;
using Microsoft.Research.DataOnboarding.RepositoryAdapters.Interfaces;
using Microsoft.Research.DataOnboarding.Utilities;
using Microsoft.Research.DataOnboarding.Utilities.Extensions;
using Microsoft.Research.DataOnboarding.Utilities.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DM = Microsoft.Research.DataOnboarding.DomainModel;

namespace Microsoft.Research.DataOnboarding.RepositoryAdapters
{
    public class MerritAdapter : BaseAdapter, IRepositoryAdapter
    {
        private static string currentFileName = string.Empty;
       
        /// <summary>
        /// Initializes a new instance of the <see cref="MerritAdapter"/> class.
        /// </summary>
        public MerritAdapter()
        {
        }

        #region Pubilc Methods


        /// <summary>
        /// Method to get the identifier 
        /// </summary>
        /// <param name="queryData">query data</param>
        /// <param name="repositoryModel">repository model</param>
        /// <returns>returns string</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Repository link is passed from the client application and we are not doign the cast to URI")]
        public string GetIdentifier(MerritQueryData queryData, RepositoryModel repositoryModel)
        {
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;
            Stream dataStream = null;
            StreamReader reader = null;
            string restResponse;

            string requestBody = GetIdentifierRequestBody(queryData);

            if (queryData != null && repositoryModel != null)
            {
                IgnoreBadCertificates();

                httpRequest = (HttpWebRequest)HttpWebRequest.Create(repositoryModel.RepositoryLink);
                httpRequest.ContentType = "multipart/form-data; boundary=" + MerritConstants.Boundary;
                httpRequest.Method = "POST";
                byte[] buffer = Encoding.UTF8.GetBytes(requestBody);
                httpRequest.ContentLength = buffer.Length;
                httpRequest.Headers.Add("Authorization", "Basic " + repositoryModel.Authorization);

                try
                {
                    dataStream = httpRequest.GetRequestStream();
                    dataStream.Write(buffer, 0, buffer.Length);

                    httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                }
                catch (WebException webException)
                {
                    dataStream.Dispose();
                    httpResponse = (HttpWebResponse)webException.Response;

                    if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return "false|" + "The username or password you entered is incorrect.";
                    }
                    else if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        return "false|" + "Issue with the repository configuration, Contact administrator.";
                    }

                    return "false|" + webException.Message;
                }
            }

            if (httpResponse.StatusCode != HttpStatusCode.OK
                && httpResponse.StatusCode != HttpStatusCode.Unauthorized
                && httpResponse.StatusCode != HttpStatusCode.NotFound)
            {
                return "false|Network Exception";
            }

            dataStream = httpResponse.GetResponseStream();
            reader = new StreamReader(dataStream);

            restResponse = reader.ReadToEnd();

            reader.Close();
            httpResponse.Close();

            if (!restResponse.Contains("ark:"))
            {
                return "false|" + "Issue with the repository configuration, Contact administrator.";
            }

            // Get Identifier from the response.
            return string.Concat("true|", restResponse.Substring(restResponse.IndexOf("ark:", StringComparison.Ordinal)).Trim());
        }

        /// <summary>
        /// method to post the file to repository
        /// </summary>
        /// <param name="request">request object</param>
        /// <param name="repositoryModel">repository model</param>
        /// <param name="file">file</param>
        /// <returns>returns operation status.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Repository link is passed from the client application and we are not doign the cast to URI")]
        public OperationStatus PostFile(PublishFileModel publishFileModel)
        {
            OperationStatus status;

            Encoding encoding = Encoding.UTF8;
            string fileName = string.Empty;
            HttpWebResponse httpResponse = null;
            Stream dataStream = null;
            StreamReader reader = null;
            string restResponse;
            PublishMerritFileModel publishMerritFileModel = (PublishMerritFileModel)publishFileModel;
            DataFile file = publishFileModel.File;
            MerritQueryData request = publishMerritFileModel.MerritQueryData;
            RepositoryModel repositoryModel = publishMerritFileModel.RepositoryModel;

            if (file != null && !string.IsNullOrEmpty(file.FileName))
            {
                fileName = file.FileName.Trim();
            }
       
            ////Ensure there's no directory path in the file name
            fileName = Path.GetFileName(fileName);

            currentFileName = Path.GetFileNameWithoutExtension(fileName);

            ////create temporary directory to unzip the files and zip the files again
            string tempFolder = MerritConstants.TempDownloadPath;

            Directory.CreateDirectory(tempFolder);

            DownloadFile(fileName, file.FileContent, file.FileExtentsion, file.IsCompressed, tempFolder);

            try
            {
                //// Create erc, eml and data-mainfest files
                CreateFile(MerritConstants.FileCreationType.ManiFest, Path.Combine(tempFolder, MerritConstants.MrtManifestFile), request, encoding, file.FileInfo, repositoryModel.SelectedRepository);
                CreateFile(MerritConstants.FileCreationType.ERC, Path.Combine(tempFolder, MerritConstants.MrtErcFile), request, encoding, file.FileInfo, repositoryModel.SelectedRepository);
                CreateFile(MerritConstants.FileCreationType.EML, Path.Combine(tempFolder, MerritConstants.MrtEmlFile), request, encoding, file.FileInfo, repositoryModel.SelectedRepository, publishFileModel.FileColumnTypes, publishFileModel.FileColumnUnits);
            }
            catch
            {
                ////supressed this error
            }

            ////actual zipping functionality will start here
            MemoryStream memoryStream = ZipUtilities.ZipFiles(tempFolder);
            memoryStream.Seek(0, SeekOrigin.Begin);

            byte[] byteArray = memoryStream.ToArray();

            string profile = request["Profile"].Value;
            string arkIdentity = request["ARK"].Value;
            string email = request["Creator: Email"] != null ? request["Creator: Email"].Value : MerritConstants.AdminEmail;

            Stream formDataStream = new System.IO.MemoryStream();
            try
            {
                string boundary = "--" + MerritConstants.Boundary;

                StringBuilder body = new StringBuilder();
                body.AppendLine(boundary);
                body.AppendLine("Content-disposition: form-data; name=\"file\"; filename=\"" + Path.GetFileNameWithoutExtension(fileName) + ".zip\"");
                body.AppendLine("Content-type: application/zip");
                body.AppendLine();

                formDataStream.Write(encoding.GetBytes(body.ToString()), 0, encoding.GetByteCount(body.ToString()));
                formDataStream.Write(byteArray, 0, byteArray.Length);
                formDataStream.Write(encoding.GetBytes(MerritConstants.CLRF), 0, encoding.GetByteCount(MerritConstants.CLRF));

                body = new StringBuilder();
                body.AppendLine(boundary);
                body.AppendLine("Content-disposition: form-data; name=\"type\"");
                body.AppendLine();
                body.AppendLine("container");
                body.AppendLine(boundary);
                body.AppendLine("Content-disposition: form-data; name=\"profile\"");
                body.AppendLine();
                body.AppendLine(profile);

                body.AppendLine(boundary);
                body.AppendLine("Content-disposition: form-data; name=\"primary-identifier\"");
                body.AppendLine();
                body.AppendLine(arkIdentity);
                body.AppendLine(boundary);
                body.AppendLine("Content-disposition: form-data; name=\"notification\"");
                body.AppendLine();
                if (string.IsNullOrWhiteSpace(email))
                {
                    body.AppendLine(MerritConstants.AdminEmail);
                }
                else
                {
                    body.AppendLine(email);
                }
                body.AppendLine(boundary);

                body.AppendLine("Content-disposition: form-data; name=\"digestType\"");
                body.AppendLine();
                body.AppendLine("md5");
                body.AppendLine(boundary);

                body.AppendLine("Content-disposition: form-data; name=\"digestValue\"");
                body.AppendLine();
                body.AppendLine(GenerateCheckSum(byteArray));
                body.AppendLine(boundary);

                body.AppendLine("Content-disposition: form-data; name=\"responseForm\"");
                body.AppendLine();

                ////Not sure is this right value or not. stress test required
                body.AppendLine("anvl");
                body.AppendLine(boundary + "--");

                formDataStream.Write(encoding.GetBytes(body.ToString()), 0, encoding.GetByteCount(body.ToString()));
                formDataStream.Position = 0;

                HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(repositoryModel.RepositoryLink);

                httpRequest.ContentType = "multipart/form-data; boundary=" + MerritConstants.Boundary;
                httpRequest.Method = "POST";
                httpRequest.Timeout = Constants.PostFileTimeOutMinutes * 60 * 1000;
                byte[] buffer = new byte[formDataStream.Length];
                formDataStream.Read(buffer, 0, buffer.Length);

                httpRequest.ContentLength = buffer.Length;
                httpRequest.Headers.Add("Authorization", "Basic " + repositoryModel.Authorization);

                dataStream = httpRequest.GetRequestStream();
                dataStream.Write(buffer, 0, buffer.Length);

                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                dataStream = httpResponse.GetResponseStream();
                reader = new StreamReader(dataStream);
                restResponse = reader.ReadToEnd();

                if (!restResponse.ToUpper().Contains("BATCHID"))
                {
                    status = OperationStatus.CreateFailureStatus("Issue with the repository configuration, Contact administrator.");
                }
                else
                {
                    status = OperationStatus.CreateSuccessStatus();
                }

            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (formDataStream != null)
                {
                    formDataStream.Dispose();
                }
                if (httpResponse != null)
                {
                    httpResponse.Close();
                }
            }

            return status;
        }

        /// <summary>
        /// Method to download the fiel from the specific repository.
        /// </summary>
        /// <param name="downloadInput">Input data required to download the file from the repository.</param>
        /// <returns>Downloaded file data.</returns>
        public DataFile DownloadFile(string downloadUrl, string authorization, string fileName)
        {
            DataFile dataFile = new DataFile();

            Stream outStream = null;
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(downloadUrl);
                httpRequest.Method = "GET";

                httpRequest.Headers.Add("Authorization", "Basic " + authorization);
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                HttpWebResponse ws = (HttpWebResponse)httpRequest.GetResponse();
                outStream = ws.GetResponseStream();

                string zipFileName = Path.GetFileNameWithoutExtension(fileName) + ".zip";

                if (outStream != null)
                {
                    //// writing stream to a temp path

                    string filePath = string.Format(@"{0}{1}\", Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

                    string totalFilePath = string.Format("{0}{1}", filePath, zipFileName);


                    DirectoryInfo dir = Directory.CreateDirectory(filePath);

                    using (Stream zipStream = System.IO.File.OpenWrite(totalFilePath))
                    {
                        outStream.CopyToStream(zipStream);
                    }

                    using (ZipFile tmpZipFile = ZipFile.Read(totalFilePath))
                    {
                        tmpZipFile.ExtractAll(filePath, ExtractExistingFileAction.OverwriteSilently);
                    }

                    // FileInfo[] directories = null;
                    var fileList = dir.GetFiles("*", SearchOption.AllDirectories).ToList();

                    // If the file is xlsx file then download only the actual file

                    if (Path.GetExtension(fileName).Equals(MerritConstants.XLSX, StringComparison.OrdinalIgnoreCase))
                    {
                        var fileDownload = fileList.Find(s => s.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
                        dataFile.FileContent = File.ReadAllBytes(fileDownload.FullName);
                        dataFile.FileName = fileName;
                        dataFile.ContentType = Helper.GetContentType(Path.GetExtension(fileDownload.FullName));
                    }
                    else
                    {
                        string subDirPath = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                        string fileNameWithOutExt = Path.GetFileNameWithoutExtension(fileName);

                        var fileDownload = fileList.Where(file => (file.Name.Contains(fileNameWithOutExt) || file.Name.Contains("metadata")) && (Path.GetExtension(file.Name) != ".zip")).ToList();

                        if (fileDownload != null && fileDownload.Count > 0)
                        {
                            dir.CreateSubdirectory(subDirPath);

                            foreach (var fileInf in fileDownload)
                            {
                                fileInf.CopyTo(filePath + subDirPath + "\\" + fileInf.Name);
                            }
                        }
                        dataFile.FileName = fileNameWithOutExt + ".zip";
                        dataFile.ContentType = MerritConstants.ZipMimeType;
                        dataFile.FileContent = ZipFileHelper.ZipFiles(filePath + subDirPath).GetBytes();
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse errResp = ex.Response;
                    using (Stream respStream = errResp.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(respStream))
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }
            return dataFile;
        }

        /// <summary>
        /// Retreives the RefreshToken
        /// </summary>
        /// <param name="refreshToken">refreshToken</param>
        /// <returns>AuthToken</returns>
        public DM.AuthToken RefreshToken(string refreshToken)
        {
            return null;
        }

        /// <summary>
        /// Verifies if the file exists in the repository
        /// </summary>
        /// <param name="fileIdentifier">File Identifier</param>
        /// <param name="authorization">AccessToken in case of skydrive and username password for Merrit</param>
        /// <returns>OperationStatus returns Success if the file exists </returns>
        public OperationStatus CheckIfFileExists(string downloadURL, string fileIdentifier, string authorization)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(downloadURL);
            httpRequest.Method = "Head";

            httpRequest.Headers.Add("Authorization", "Basic " + authorization);
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

            OperationStatus status = base.SendHttpRequest(httpRequest);
            return status;
        }

        #endregion



        #region private methods

        /// <summary>
        /// Method to download the file to temp directory
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileContents"></param>
        /// <param name="fileExtentsion"></param>
        /// <param name="isCompressed"></param>
        /// <param name="tempFolder"></param>
        private static void DownloadFile(string fileName, byte[] fileContents, string fileExtentsion, bool isCompressed, string tempFolder)
        {
            string tempFileName = fileName;

            bool isMultiCSVPassed = false;
            if (isCompressed)
            {
                switch (fileExtentsion.ToLowerInvariant())
                {
                    case MerritConstants.ZIP:
                        tempFileName = string.Format(CultureInfo.InvariantCulture, MerritConstants.TempFileName, MerritConstants.ZIP);
                        isMultiCSVPassed = true;
                        break;
                    case MerritConstants.TAR:
                        tempFileName = string.Format(CultureInfo.InvariantCulture, MerritConstants.TempFileName, MerritConstants.TAR);
                        isMultiCSVPassed = true;
                        break;
                    case MerritConstants.GZIP:
                        tempFileName = string.Format(CultureInfo.InvariantCulture, MerritConstants.TempFileName, MerritConstants.GZIP);
                        isMultiCSVPassed = true;
                        break;
                }
            }

            string filePath = Path.Combine(tempFolder, tempFileName);
            FileStream fs = File.Create(filePath);
            fs.Close();

            File.WriteAllBytes(filePath, fileContents);

            if (isMultiCSVPassed)
            {
                ZipUtilities.UnZipFiles(filePath, tempFolder, true);
                File.Delete(filePath);
            }
        }

        private void CreateFile(MerritConstants.FileCreationType fileCreationType, string filePath, MerritQueryData request, Encoding encoding, DM.File file, DM.Repository repository, IEnumerable<DM.FileColumnType> fileColumnTypes = null, IEnumerable<DM.FileColumnUnit> fileColumnUnits = null)
        {
            StringBuilder arguments = new StringBuilder();

            switch (fileCreationType)
            {
                case MerritConstants.FileCreationType.ManiFest:
                    arguments.AppendLine("#%dataonem_0.1");
                    arguments.AppendLine("#%profile | http://uc3.cdlib.org/registry/ingest/manifest/mrt-dataone- manifest");
                    arguments.AppendLine("#%prefix | dom: | http://uc3.cdlib.org/ontology/dataonem#");
                    arguments.AppendLine("#%prefix | mrt: | http://uc3.cdlib.org/ontology/mom#");
                    arguments.AppendLine("#%fields | dom:scienceMetadataFile | dom:scienceMetadataFormat | dom:scienceDataFile | mrt:mimeType");
                    ArrayList files = ZipUtilities.GenerateFileList(Path.GetDirectoryName(filePath));
                    foreach (string fileName in files)
                    {
                        var tempName = System.IO.Path.GetFileName(fileName);
                        arguments.AppendLine(MerritConstants.MrtErcFile + " | ERC | " + tempName + " | text/plain");
                        arguments.AppendLine(MerritConstants.MrtEmlFile + " | eml://ecoinformatics.org/eml-2.1.1 | " + tempName + " | text/xml");
                    }
                    arguments.AppendLine("#%eof");
                    break;
                case MerritConstants.FileCreationType.ERC:
                    string who = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", request["who"].Key, request["who"].Value);
                    string what = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", request["what"].Key, request["what"].Value);
                    string when = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", request["when"].Key, request["when"].Value);
                    string where = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", request["where"].Key, request["where"].Value);

                    arguments.AppendLine("erc:");

                    arguments.AppendLine(who);
                    arguments.AppendLine(what);
                    arguments.AppendLine(when);
                    arguments.AppendLine(where);
                    break;
                case MerritConstants.FileCreationType.EML:
                    WriteEML(request, filePath, file, repository, fileColumnTypes, fileColumnUnits);
                    break;
            }

            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (encoding != null)
                {
                    fileStream.Write(encoding.GetBytes(arguments.ToString()), 0, encoding.GetByteCount(arguments.ToString()));
                }
            }
        }

        private string GetIdentifierRequestBody(MerritQueryData request)
        {
            StringBuilder body = new StringBuilder();

            // Update Profile.
            string mszProfile = request["Profile"].Value;

            body.AppendLine("--" + MerritConstants.Boundary);
            body.AppendLine("Content-disposition: form-data; name=\"profile\"");

            body.AppendLine();
            body.AppendLine(mszProfile);
            body.AppendLine("--" + MerritConstants.Boundary);

            // Update ERC
            string who = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", request["Who"].Key, request["Who"].Value);
            string what = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", request["What"].Key, request["What"].Value);
            string when = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", request["When"].Key, request["When"].Value);

            string mszErc = string.Format(CultureInfo.InvariantCulture, "{0}%0A{1}%0A{2}", who, what, when);

            body.AppendLine("Content-disposition: form-data; name=\"erc\"");

            ////body.AppendLine("Content-type: text/plain");
            body.AppendLine();
            body.AppendLine("erc: " + mszErc);
            body.AppendLine("--" + MerritConstants.Boundary + "--");

            return body.ToString();
        }

        private void WriteEML(MerritQueryData request, string filePath, DM.File file, DM.Repository repositoryData, IEnumerable<DM.FileColumnType> fileColumnTypes = null, IEnumerable<DM.FileColumnUnit> fileColumnUnits = null)
        {
            //  XmlDocument metadataDocument = new XmlDocument();
            XmlDocument emlDocument = new XmlDocument();

            // metadataDocument.LoadXml(request.MetaDataDetail.MetadataMappingXML);
            emlDocument.LoadXml(request.MetadataXML);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(emlDocument.NameTable);
            nsmgr.AddNamespace("eml", MerritConstants.EmlNamespace);

            var repositoryMetadata = repositoryData.RepositoryMetadata.FirstOrDefault();
            if (repositoryMetadata != null)
            {
                ReplaceMetadata(file, emlDocument, nsmgr, repositoryMetadata);
            }
            // Write Parameter Metadata properties.
            if (file.FileColumns != null && file.FileColumns.Count > 0)
            {
                WriteParamMetadata(request, emlDocument, file, fileColumnTypes, fileColumnUnits);
            }

            ////Added this following conditions outside the above for loop to avoid the following conditions not to repeat every time

            ////If contact element is not there, then insert the value from creator/surname
            ManageContactElement(emlDocument);

            ////If no project title element ,then add the project title element value
            ManageProjectTitleElement(emlDocument);

            ////If datatable has no entityname ,then add the entity name
            ManageEntityNameElement(emlDocument);

            ////If datatable has no attributeList, then add attributeList node
            ManageAttributeListElement(emlDocument);

            ////set the coordinate value for geographical location 
            ManageGeographicLocationCoordinates(emlDocument);

            ////check the temporal coverage conidtions
            ManageTemporalCoverage(emlDocument);

            ////Remove the empty elements and remove the parent node when the no child node items are present
            RemoveEmptyElements(emlDocument);

            emlDocument.Save(filePath);
        }

        private static void ReplaceMetadata(DM.File file, XmlDocument emlDocument, XmlNamespaceManager nsmgr, DM.RepositoryMetadata repositoryMetadata)
        {
            foreach (var repositoryMetaDataField in repositoryMetadata.RepositoryMetadataFields)
            {
                try
                {
                    var fieldName = repositoryMetaDataField.Name;
                    var fieldMapping = repositoryMetaDataField.Mapping;
                    //var fieldDataType = repositoryMetaData.MetadataType.ToString();
                    var fileMetaData = file.FileMetadataFields.Where(f => f.RepositoryMetadataFieldId == repositoryMetaDataField.RepositoryMetadataFieldId).FirstOrDefault();
                    var filedValue = string.Empty;
                    if (fileMetaData != null)
                    {
                        filedValue = fileMetaData.MetadataValue.ToString();
                    }
                    fieldMapping = fieldMapping.Replace('.', '/').Replace("eml", string.Empty);
                    if (!string.IsNullOrEmpty(fieldMapping))
                    {
                        fieldMapping = "./" + fieldMapping;

                        XmlElement root1 = emlDocument.DocumentElement;
                        XmlNode oldNode = root1.SelectSingleNode(fieldMapping, nsmgr);
                        if (oldNode != null)
                        {
                            if (string.Compare(fieldName, "keyword(s)", StringComparison.Ordinal) == 0)
                            {
                                if (!string.IsNullOrWhiteSpace(filedValue))
                                {
                                    var keywords = filedValue.Split(',');

                                    var keywordSet = oldNode.ParentNode;
                                    keywordSet.RemoveChild(oldNode);

                                    foreach (var keyword in keywords)
                                    {
                                        var keywordNode = oldNode.Clone();
                                        keywordNode.InnerText = keyword;

                                        keywordSet.InsertBefore(keywordNode, keywordSet.LastChild);
                                    }
                                }
                            }
                            else if (string.Compare(fieldName, "keyword thesaurus used", StringComparison.Ordinal) == 0 && string.IsNullOrWhiteSpace(filedValue))
                            {
                                var keywordset = oldNode.ParentNode;
                                keywordset.RemoveChild(oldNode);
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(filedValue))
                                {
                                    oldNode.InnerText = filedValue;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    //suppress this errr
                }
            }
        }

        private void WriteParamMetadata(MerritQueryData request, XmlDocument emlDocument, DM.File file, IEnumerable<DM.FileColumnType> fileColumnTypes = null, IEnumerable<DM.FileColumnUnit> fileColumnUnits = null)
        {
            XmlNamespaceManager emlNamespaceMgr = new XmlNamespaceManager(emlDocument.NameTable);
            emlNamespaceMgr.AddNamespace("eml", MerritConstants.EmlNamespace);

            XmlElement emlRoot = emlDocument.DocumentElement;

            XmlNode dataTableRef = emlRoot.SelectSingleNode(MerritConstants.SheetTableXPath, emlNamespaceMgr);
            XmlNode parentDataTable = dataTableRef.ParentNode;
            StringBuilder attibuteListBuilder;
            XElement scaleNode;
            var fileColumnType = string.Empty;
            var fileColumnUnit = string.Empty;
            List<string> sheetIds = new List<string>();

            //foreach (var param in request.ParameterMetadata)
            foreach (var param in file.FileColumns)
            {
                var sheetTable = dataTableRef.Clone();
                attibuteListBuilder = new StringBuilder();
                if (!sheetIds.Contains(param.EntityName))
                {
                    sheetIds.Add(param.EntityName);
                    foreach (var attributeParam in file.FileColumns.Where(fc => fc.EntityName == param.EntityName).ToList())
                    {
                        fileColumnType = string.Empty;
                        fileColumnUnit = string.Empty;
                        if (attributeParam.FileColumnTypeId > 0 && fileColumnTypes != null)
                        {
                            scaleNode = new XElement("measurementScale");

                            fileColumnType = fileColumnTypes.Where(fc => fc.FileColumnTypeId == attributeParam.FileColumnTypeId).FirstOrDefault().Name;
                            if (attributeParam.FileColumnUnitId > 0 && fileColumnUnits != null)
                            {
                                fileColumnUnit = fileColumnUnits.Where(fc => fc.FileColumnUnitId == attributeParam.FileColumnUnitId).FirstOrDefault().Name;
                            }
                            if (!string.IsNullOrWhiteSpace(fileColumnType))
                            {
                                switch (fileColumnType.ToUpperInvariant())
                                {
                                    case "TEXT":
                                        scaleNode = new XElement("measurementScale", new XElement("nominal", new XElement("nonNumericDomain", new XElement("textDomain", new XElement("definition", System.Security.SecurityElement.Escape(attributeParam.Description))))));
                                        break;
                                    case "DATETIME":
                                        scaleNode = new XElement("measurementScale", new XElement("dateTime", new XElement("formatString", "The user has designated this column as some form of date and/or time. No standard is specified due to DataUp coding constraints.")));
                                        break;
                                    case "NUMERIC":
                                        XElement unitsElement = new XElement("unit", new XElement("standardUnit", System.Security.SecurityElement.Escape(fileColumnUnit)));
                                        if (!string.IsNullOrWhiteSpace(fileColumnUnit))
                                        {
                                            unitsElement = new XElement("unit", new XElement("customUnit", System.Security.SecurityElement.Escape(fileColumnUnit)));
                                        }
                                        scaleNode = new XElement("measurementScale", new XElement("interval", unitsElement, new XElement("numericDomain", new XElement("numberType", "real"))));
                                        break;
                                }
                            }

                            var attibuteNode = new XElement("attribute", new XElement("attributeName", System.Security.SecurityElement.Escape(attributeParam.Name)), new XElement("attributeDefinition", System.Security.SecurityElement.Escape(attributeParam.Description)), scaleNode);

                            attibuteListBuilder.AppendLine(attibuteNode.ToString().Trim());
                        }
                    }
                    sheetTable.SelectSingleNode("entityName").InnerText = string.Format(CultureInfo.InvariantCulture, "{0} - {1}", System.Security.SecurityElement.Escape(currentFileName), System.Security.SecurityElement.Escape(param.EntityName));
                    sheetTable.SelectSingleNode("entityDescription").InnerText = System.Security.SecurityElement.Escape(param.EntityDescription);
                    sheetTable.SelectSingleNode("attributeList").InnerXml = attibuteListBuilder.ToString();

                    parentDataTable.InsertBefore(sheetTable, dataTableRef);
                }
            }

            parentDataTable.RemoveChild(dataTableRef);
        }

        private static void ManageContactElement(XmlDocument emlDocument)
        {
            var rootElement = emlDocument.DocumentElement;
            var datasetContactElement = rootElement.SelectSingleNode(MerritConstants.XPathDatasetContact);

            ////If contact element is not there as part of the dataset/contact add the dataset/contact/surname
            if (datasetContactElement == null)
            {
                var surNameValue = rootElement.SelectSingleNode(MerritConstants.XPathDataSetSurName).InnerText;
                if (string.IsNullOrWhiteSpace(surNameValue))
                {
                    surNameValue = "Create surname";
                }

                var datasetElement = rootElement.SelectSingleNode("dataset");

                ////create new xml elements
                var contactElement = emlDocument.CreateElement("contact");
                var individualElement = emlDocument.CreateElement("individualName");
                var surNameElement = emlDocument.CreateElement("surName");

                surNameElement.InnerText = surNameValue;

                individualElement.AppendChild(surNameElement);
                contactElement.AppendChild(individualElement);
                datasetElement.AppendChild(contactElement);
            }
        }

        private static void ManageTemporalCoverage(XmlDocument emlDocument)
        {
            var rootElement = emlDocument.DocumentElement;
            var temporalCoverageElement = rootElement.SelectSingleNode(MerritConstants.XPathDataSetTemporalCoverage);
            var additionalMetadataElement = rootElement.SelectSingleNode("additionalMetadata");

            ////If temp coverage element is not there and Additional meta data elemenet exists ,delete the describes node from AdditionalMetaData element
            if (temporalCoverageElement == null && additionalMetadataElement != null)
            {
                var describesElement = additionalMetadataElement.SelectSingleNode("describes");
                if (describesElement != null)
                {
                    additionalMetadataElement.RemoveChild(describesElement);
                }
            }
        }

        private static void RemoveEmptyElements(XmlDocument emlDocument)
        {
            var rootElement = emlDocument.DocumentElement;

            ////Delete the Empty Nodes that doesnot have inner text
            foreach (XmlNode node in rootElement.ChildNodes)
            {
                if (node.HasChildNodes)
                {
                    RemoveEmptyElements(node);
                }
                else if (string.IsNullOrWhiteSpace(node.InnerText))
                {
                    node.ParentNode.RemoveChild(node);
                }

                ////Delete the parent nodes that doesn't have the child nodes
                RemoveEmptyParentNodes(node);
            }
        }

        private static void RemoveEmptyElements(XmlNode nodes)
        {
            var nodesList = nodes.ChildNodes;
            var removeRequired = false;
            XmlNode prevNode = nodes;
            foreach (XmlNode node in nodesList)
            {
                removeRequired = false;
                if (node.Name != "#comment" && node.Name != "entityDescription" && node.Name != "attributeList")
                {
                    if (node.HasChildNodes && node.ChildNodes.Count > 1)
                    {
                        RemoveEmptyElements(node);
                    }
                    else if (string.IsNullOrWhiteSpace(node.InnerText))
                    {
                        node.ParentNode.RemoveChild(node);

                        ////The following logic is to delete the parent node when there is no value in child nodes
                        if (nodesList.Count > 0)
                        {
                            removeRequired = true;
                            prevNode = nodesList[0].ParentNode;
                        }
                    }
                }
            }

            ////send this parent node again to Remove method,since if we delete one of the node ,for loop will get exit because of there is change in nodeLists count
            if (removeRequired && prevNode != null)
            {
                RemoveEmptyElements(prevNode);
            }
        }

        private static void RemoveEmptyParentNodes(XmlNode nodes)
        {
            foreach (XmlNode node in nodes)
            {
                if (node.Name != "#comment" && node.Name != "entityDescription" && node.Name != "attributeList")
                {
                    if (!node.HasChildNodes && node.InnerText.Length == 0)
                    {
                        node.ParentNode.RemoveChild(node);
                    }
                    else
                    {
                        RemoveEmptyParentNodes(node);
                    }
                }
            }
        }

        private static void ManageGeographicLocationCoordinates(XmlDocument emlDocument)
        {
            var rootElement = emlDocument.DocumentElement;
            var geographicDescElement = rootElement.SelectSingleNode(MerritConstants.XPathDataSetGeoDesc);
            var boundingCoordinatesElement = rootElement.SelectSingleNode(MerritConstants.XPathDataSetGeoCoordinates);

            if (geographicDescElement != null && boundingCoordinatesElement != null)
            {
                var westBoundingCoordinateElement = boundingCoordinatesElement.SelectSingleNode("westBoundingCoordinate");
                var eastBoundingCoordinateElement = boundingCoordinatesElement.SelectSingleNode("eastBoundingCoordinate");
                var northBoundingCoordinateElement = boundingCoordinatesElement.SelectSingleNode("northBoundingCoordinate");
                var southBoundingCoordinateElement = boundingCoordinatesElement.SelectSingleNode("southBoundingCoordinate");
                if (westBoundingCoordinateElement != null && string.IsNullOrWhiteSpace(westBoundingCoordinateElement.InnerText))
                {
                    westBoundingCoordinateElement.InnerText = MerritConstants.DefaultCoordinateValue;
                }
                if (eastBoundingCoordinateElement != null && string.IsNullOrWhiteSpace(eastBoundingCoordinateElement.InnerText))
                {
                    eastBoundingCoordinateElement.InnerText = MerritConstants.DefaultCoordinateValue;
                }
                if (northBoundingCoordinateElement != null && string.IsNullOrWhiteSpace(northBoundingCoordinateElement.InnerText))
                {
                    northBoundingCoordinateElement.InnerText = MerritConstants.DefaultCoordinateValue;
                }
                if (southBoundingCoordinateElement != null && string.IsNullOrWhiteSpace(southBoundingCoordinateElement.InnerText))
                {
                    southBoundingCoordinateElement.InnerText = MerritConstants.DefaultCoordinateValue;
                }
            }
        }

        private static void ManageAttributeListElement(XmlDocument emlDocument)
        {
            var rootElement = emlDocument.DocumentElement;
            var dataTableElement = rootElement.SelectSingleNode(MerritConstants.XPathDatasetDataTable);

            if (dataTableElement != null)
            {
                var attributeListElement = dataTableElement.SelectSingleNode("attributeList");
                if (attributeListElement == null)
                {
                    ////create elements
                    attributeListElement = emlDocument.CreateElement("attributeList");

                    ////set the attribute name node
                    var attributeNameElement = emlDocument.CreateElement("attributeName");
                    attributeNameElement.InnerText = MerritConstants.MissingRequiredElement;

                    ////set the attribute definition node
                    var attributeDefinitionElement = emlDocument.CreateElement("attributeDefinition");
                    attributeDefinitionElement.InnerText = MerritConstants.MissingRequiredElement;

                    ////set the measurement scale node
                    var measurementScaleElement = emlDocument.CreateElement("measurementScale");
                    var nominalElement = emlDocument.CreateElement("nominal");
                    var nonNumericDomainElement = emlDocument.CreateElement("nonNumericDomain");
                    var textDomainElement = emlDocument.CreateElement("textDomain");
                    var definitionElement = emlDocument.CreateElement("definition");
                    definitionElement.InnerText = MerritConstants.MissingRequiredElement;

                    ////add the measurement scale nodes
                    textDomainElement.AppendChild(definitionElement);
                    nonNumericDomainElement.AppendChild(textDomainElement);
                    nominalElement.AppendChild(nonNumericDomainElement);
                    measurementScaleElement.AppendChild(nominalElement);

                    ////append the nodes to attribute list node
                    attributeListElement.AppendChild(attributeNameElement);
                    attributeListElement.AppendChild(attributeDefinitionElement);
                    attributeListElement.AppendChild(measurementScaleElement);

                    ////add to the datatable node
                    dataTableElement.AppendChild(attributeListElement);
                }
            }
        }

        private static void ManageEntityNameElement(XmlDocument emlDocument)
        {
            var rootElement = emlDocument.DocumentElement;
            var dataTableElement = rootElement.SelectSingleNode(MerritConstants.XPathDatasetDataTable);

            if (dataTableElement != null)
            {
                var entityNameElement = dataTableElement.SelectSingleNode("entityName");
                if (entityNameElement == null)
                {
                    entityNameElement = emlDocument.CreateElement("entityName");
                    entityNameElement.InnerText = MerritConstants.MissingRequiredElement;
                    dataTableElement.AppendChild(entityNameElement);
                }
                else if (string.IsNullOrWhiteSpace(entityNameElement.InnerText))
                {
                    entityNameElement.InnerText = MerritConstants.MissingRequiredElement;
                }
            }
        }

        private static void ManageProjectTitleElement(XmlDocument emlDocument)
        {
            var rootElement = emlDocument.DocumentElement;
            var projectElement = rootElement.SelectSingleNode(MerritConstants.XPathDatasetProject);

            if (projectElement != null)
            {
                var titleElement = projectElement.SelectSingleNode("title");
                if (titleElement == null)
                {
                    titleElement = emlDocument.CreateElement("title");
                    titleElement.InnerText = MerritConstants.MissingRequiredElement;
                    projectElement.AppendChild(titleElement);
                }
                else if (string.IsNullOrWhiteSpace(titleElement.InnerText))
                {
                    titleElement.InnerText = MerritConstants.MissingRequiredElement;
                }
            }
        }

        private static List<string> GetStandardUnits()
        {
            List<string> units = new List<string>() { };

            try
            {
                string[] metadataValue;

                string standardUnitsPath = "StandardUnits.txt";

                if (File.Exists(standardUnitsPath))
                {
                    metadataValue = File.ReadAllLines(standardUnitsPath);
                }
                else
                {
                    metadataValue = File.ReadAllLines(@"D:\\Practice\\MerritSamplePOC\\MerritSamplePOC\\StandardUnits.txt");
                }

                units = metadataValue.Select(p => p.Trim()).ToList();
            }
            catch
            {
                throw;
            }

            return units;
        }

        private static string GenerateCheckSum(byte[] byteArray)
        {
            StringBuilder checkSum = new StringBuilder();

            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(byteArray);

                for (int i = 0; i < data.Length; i++)
                {
                    checkSum.Append(data[i].ToString("x2", CultureInfo.InvariantCulture));
                }
            }
            return checkSum.ToString();
        }

        /// <summary>
        /// Together with the AcceptAllCertifications method right
        /// below this causes to bypass errors caused by SLL-Errors.
        /// </summary>
        public static void IgnoreBadCertificates()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);
        }

        /// <summary>
        /// In Short: the Method solves the Problem of broken Certificates.
        /// Sometime when requesting Data and the sending Webserverconnection
        /// is based on a SSL Connection, an Error is caused by Servers whoes
        /// Certificate(s) have Errors. Like when the Cert is out of date
        /// and much more... So at this point when calling the method,
        /// this behaviour is prevented
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certification"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns>true</returns>
        private static bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        #endregion
    }
}
