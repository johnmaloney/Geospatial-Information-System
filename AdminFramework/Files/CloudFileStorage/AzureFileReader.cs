using Universal.Contracts.Files;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Files.CloudFileStorage
{
    /// <summary>
    /// Access to the Azure File Service Rest API.
    /// https://docs.microsoft.com/en-us/azure/storage/common/storage-rest-api-auth
    /// https://docs.microsoft.com/en-us/rest/api/storageservices/file-service-rest-api
    /// Credit also to this Repo: https://github.com/mstaples84/azurefileserviceauth
    /// 
    /// </summary>
    public class AzureFileReader : IFileReader
    {
        #region Fields
        
        private readonly string storageAccountName;
        private readonly string storageAccountKey;
        private readonly string rootDirectory;

        #endregion

        #region Properties



        #endregion

        #region Methods

        public AzureFileReader(string storageAccountName, string storageAccountKey, string rootDirectory)
        {
            this.storageAccountName = storageAccountName;
            this.storageAccountKey = storageAccountKey;
            this.rootDirectory = rootDirectory;
        }

        public async Task<IEnumerable<IFileMetadata>> ListAll(string directory = "")
        {
            var items = new List<IFile>();

            string subDirectory = !string.IsNullOrEmpty(directory) ? $"/{directory}" : string.Empty;
            // Construct the URI. This will look like this:
            //   https://myaccount.blob.core.windows.net/resource
            String uri = $"https://{storageAccountName}.file.core.windows.net/{rootDirectory}{subDirectory}?restype=directory&comp=list";

            // Set this to whatever payload you desire. Ours is null because 
            //   we're not passing anything in.
            Byte[] requestPayload = null;

            //Instantiate the request message with a null payload.
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri)
            { Content = (requestPayload == null) ? null : new ByteArrayContent(requestPayload) })
            {

                // Add the request headers for x-ms-date and x-ms-version.
                DateTime now = DateTime.UtcNow;
                SetDefaultHeaders(httpRequestMessage, now);
                // If you need any additional headers, add them here before creating
                //   the authorization header. 

                // Add the authorization header.
                httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
                   storageAccountName, storageAccountKey, now, httpRequestMessage);

                // Send the request.
                using (HttpResponseMessage httpResponseMessage = await new HttpClient().SendAsync(httpRequestMessage))
                {
                    // If successful (status code = 200), 
                    //   parse the XML response for the container names.
                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        String xmlString = await httpResponseMessage.Content.ReadAsStringAsync();
                        XElement x = XElement.Parse(xmlString);

                        if (x.Attribute("DirectoryPath") != null)
                            items.Add(new File { Directory = x.Attribute("DirectoryPath").Value });

                        foreach (XElement container in x.Elements("Entries"))
                        {
                            foreach (XElement dir in container.Elements("Directory"))
                            {
                                var name = dir.Value;
                                items.Add(new File { Directory = name });
                            }

                            foreach(XElement file in container.Elements("File"))
                            {
                                var name = file.Value;
                                items.Add(new File { Directory = directory, Name = name });
                            }
                        }
                    }
                }
            }

            return items;
        }

        public async Task<IFile> GetFile(string directory, string fileName)
        {
            // Construct the URI. This will look like this:
            //   https://myaccount.blob.core.windows.net/resource
            String uri = $"https://{storageAccountName}.file.core.windows.net/{rootDirectory}/{directory}/{fileName}";

            // Set this to whatever payload you desire. Ours is null because 
            //   we're not passing anything in.
            Byte[] responsePayload = null;

            //Instantiate the request message with a null payload.
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                // Add the request headers for x-ms-date and x-ms-version.
                DateTime now = DateTime.UtcNow;
                SetDefaultHeaders(httpRequestMessage, now);
                // If you need any additional headers, add them here before creating
                //   the authorization header. 

                // Add the authorization header.
                httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
                   storageAccountName, storageAccountKey, now, httpRequestMessage);

                // Send the request.
                using (HttpResponseMessage httpResponseMessage = await new HttpClient().SendAsync(httpRequestMessage))
                {
                    // If successful (status code = 200), 
                    //   parse the XML response for the container names.
                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        var fileContent = await httpResponseMessage.Content.ReadAsByteArrayAsync();
                        var file =  new File
                        {
                            Directory = directory,
                            Name = fileName,
                            ContentLength = fileContent.Length
                        };
                        file.AddDataContent(fileContent);
                        return file;
                    }
                }
                return null;
            }
        }

        public async Task CreateDirectory(string directoryName)
        {
            // Construct the URI. This will look like this:
            //   https://myaccount.blob.core.windows.net/resource
            String uri = $"https://{storageAccountName}.file.core.windows.net/{rootDirectory}/{directoryName}?restype=directory";

            //Instantiate the request message with a null payload.
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, uri))
            {
                // Add the request headers for x-ms-date and x-ms-version.
                DateTime now = DateTime.UtcNow;
                SetDefaultHeaders(httpRequestMessage, now);
                // If you need any additional headers, add them here before creating
                //   the authorization header. 

                // Add the authorization header.
                httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
                   storageAccountName, storageAccountKey, now, httpRequestMessage);

                // Send the request.
                using (HttpResponseMessage httpResponseMessage = await new HttpClient().SendAsync(httpRequestMessage))
                {
                    // If successful (status code = 200), 
                    //   parse the XML response for the container names.
                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        String xmlString = await httpResponseMessage.Content.ReadAsStringAsync();
                        XElement x = XElement.Parse(xmlString);
                        foreach (XElement container in x.Element("Shares").Elements("Share"))
                        {

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Put the file spec into Azure then upon sucess write the bytes to the file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task AddFile(IFile file)
        {
            byte[] fileBytes;
            if (!string.IsNullOrEmpty(file.TextContents))
                fileBytes = Encoding.UTF8.GetBytes(file.TextContents);
            else if (file.DataContents != null)
                fileBytes = file.DataContents;
            else
                fileBytes = Encoding.UTF8.GetBytes("No Content Uploaded With File");

            // Construct the URI. This will look like this:
            //   https://myaccount.blob.core.windows.net/resource
            String uri = $"https://{storageAccountName}.file.core.windows.net/{rootDirectory}/{file.Directory}/{file.Name}";

            var successfullyCreated = await CreateFileAsync(uri, fileBytes.Length);

            if(successfullyCreated)
            {
                await PutRangeAsync(new Uri($"{uri}?comp=range"), fileBytes);
            }
        }

        public async Task<bool> CreateFileAsync(string storageUri, int contentLength)
        {
            //Instantiate the request message with a empty payload.
            using (var httpRequestMessage =
                new HttpRequestMessage(HttpMethod.Put, storageUri)
                { Content = new StringContent("") })
            {
                var now = DateTime.UtcNow;

                SetDefaultHeaders(httpRequestMessage, now);
                // Required. This header specifies the maximum size for the file, up to 1 TiB.
                httpRequestMessage.Headers.Add("x-ms-content-length", contentLength.ToString());
                httpRequestMessage.Headers.Add("x-ms-type", "file");

                // If you need any additional headers, add them here before creating
                //   the authorization header. 

                // Add the authorization header.
                httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
                   storageAccountName, storageAccountKey, now, httpRequestMessage);

                using (HttpResponseMessage httpResponseMessage = await new HttpClient().SendAsync(httpRequestMessage))
                {
                    if (httpResponseMessage.StatusCode == HttpStatusCode.Created)
                    {
                        var response = await httpResponseMessage.Content.ReadAsStringAsync();
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Put Range Request
        /// </summary>
        /// <returns></returns>
        public async Task PutRangeAsync(Uri storageUri, byte[] bytes, int startBytes = 0, string writeMode = "Update")
        {
            if (string.IsNullOrEmpty(storageUri.Query) || !storageUri.Query.Contains("comp=range")) throw new Exception("Missing Query String comp=range");

            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, storageUri))
            {
                httpRequestMessage.Content = new ByteArrayContent(bytes);

                var contentlength = httpRequestMessage.Content.Headers.ContentLength - 1;

                var now = DateTime.UtcNow;
                SetDefaultHeaders(httpRequestMessage, now);

                httpRequestMessage.Headers.Add("x-ms-range", $"bytes={startBytes}-{contentlength.ToString()}");
                httpRequestMessage.Headers.Add("x-ms-write", writeMode);

                // Add the authorization header.
                httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
                    storageAccountName, storageAccountKey, now, httpRequestMessage);

                using (HttpResponseMessage httpResponseMessage = await new HttpClient().SendAsync(httpRequestMessage))
                {
                    if (httpResponseMessage.StatusCode == HttpStatusCode.Created)
                    {
                        var response = await httpResponseMessage.Content.ReadAsStringAsync();
                        
                    }
                }
            }
        }
        
        /// <summary>
        /// Set the default headers which are mandatory in all requests
        /// </summary>
        /// <param name="httpRequestMessage"></param>
        /// <param name="now"></param>
        private void SetDefaultHeaders(HttpRequestMessage httpRequestMessage, DateTime now)
        {
            httpRequestMessage.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
            httpRequestMessage.Headers.Add("x-ms-version", "2018-03-28");
        }

        #endregion
    }
}
