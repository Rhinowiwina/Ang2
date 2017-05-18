using System;
using System.Globalization;
using System.IO;
using LS.AzureStorage;
using LS.Core;
using LS.Core.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Exceptionless;
using Exceptionless.Models;
using System.Collections.Generic;
using System.Text;

namespace LS.Repositories {
    public class ExternalStorageAzureRepository : IExternalStorageRepository {
        private static readonly string AzureConnectionStringPrefix = "DefaultEndpointsProtocol=https;";
        private static readonly string AzureConnectionStringAccountPrefix = "AccountName=";
        private static readonly string AzureConnectionStringKeyPrefix = ";AccountKey=";

        private readonly CloudStorageAccount _azureAccount;
        private readonly string _containerName;

        public ExternalStorageAzureRepository(string azureConnectionAccount, string azureConnectionKey, string containerName) {

            var azureConnectionString = BuildConnectionString(azureConnectionAccount, azureConnectionKey);

            _azureAccount = azureConnectionString.GetAccount();
            _containerName = containerName;
        }

        public ServiceProcessingResult Save(Stream stream, string blobName, string contentType = "application/octet-stream") {
            var processingResult = new ServiceProcessingResult();

            try {
                var client = GetBlobContainer();
                var blob = client.GetBlockBlobReference(blobName);
                using (stream) {
                    blob.UploadFromStream(stream);
                    blob.Properties.ContentType = MimeContentGenerator.GetMimeTypeFor(contentType);
                    blob.SetProperties();
                }

                processingResult.IsSuccessful = true;
            } catch (Exception ex) {
                ex.ToExceptionless()
                      .SetMessage("ExternalStorageAzureRepository Save Failed")
                      .MarkAsCritical()
                      .Submit();
                processingResult.IsSuccessful = false;
            }

            return processingResult;
        }

        public ServiceProcessingResult Save(string fileName, string filePath) {
            var processingResult = new ServiceProcessingResult();
            try {
                using (var fileStream = System.IO.File.OpenRead(filePath + fileName)) {
                    return Save(fileStream, fileName);
                }
            } catch (Exception ex) {
                ex.ToExceptionless()
                  .SetMessage("ExternalStorageAzureRepository Save Failed (From file)")
                  .MarkAsCritical()
                  .Submit();
                processingResult.IsSuccessful = false;
            }
            return processingResult;
        }

        public ServiceProcessingResult Save(byte[] byteArray, string blobName, string contentType = "application/octet-stream") {
            var stream = new MemoryStream(byteArray);

            return Save(stream, blobName, contentType);
        }

        public DataAccessResult RenameFile(string sourceFileName, string destFilename) {
            var result = new DataAccessResult();

            result.IsSuccessful = false;
            result.Error = new ProcessingError("Error renaming file for Azure stroage (unsupported function)", "Error renaming file for Azure stroage (unsupported function)", false);
            ExceptionlessClient.Default.CreateException(new Exception("Error renaming file for Azure stroage (unsupported function)"))
                .MarkAsCritical()
                .Submit();
            return result;
        }

        public DataAccessResult<List<Core.Interfaces.IExternalStorageListDirectory>> GetDirectoryList(string prefix, string delimeter) {
            var result = new DataAccessResult<List<Core.Interfaces.IExternalStorageListDirectory>>();
            result.IsSuccessful = false;
            result.Error = new ProcessingError("Error getting folders from Azure storage (GetDirectoryList unsupported)", "Error getting folders from Azure storage (GetDirectoryList unsupported)", false, false);
            ExceptionlessClient.Default.CreateException(new Exception("Error getting folders from Azure storage (GetDirectoryList unsupported)"))
                .MarkAsCritical()
                .Submit();
            return result;
        }

        public bool DoesFileExist(string fileName) {
            var container = GetBlobContainer();
            var blob = container.GetBlockBlobReference(fileName);
            return blob.Exists();
        }

        private CloudBlobContainer GetBlobContainer() {
            var blobClient = _azureAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(_containerName);

            return container;
        }

        private string BuildConnectionString(string azureConnectionAccount, string azureConnectionKey) {
            return AzureConnectionStringPrefix + AzureConnectionStringAccountPrefix + azureConnectionAccount + AzureConnectionStringKeyPrefix + azureConnectionKey;
        }

        public DataAccessResult<string> GeneratePreSignedUrl(string fileName) {
            var result = new DataAccessResult<string> { IsSuccessful = true };
            try {
                var client = GetBlobContainer();
                var blob = client.GetBlockBlobReference(fileName);
                var sas = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(1),
                });
                result.Data = string.Format(CultureInfo.InvariantCulture, "{0}{1}", blob.Uri, sas);
            } catch (Exception ex) {
                ex.ToExceptionless()
                      .SetMessage("Unable to retrieve temporary URL for :" + fileName)
                      .MarkAsCritical()
                      .Submit();
                result.IsSuccessful = false;
                result.Error = new ProcessingError(ex.Message, "Unable to retrieve temporary URL for :" + fileName, true);
            }
            return result;
        }

        public DataAccessResult<string> GetAsBase64(string fileName) {
            var result = new DataAccessResult<string>();
             try {
                var client = GetBlobContainer();
                var blob = client.GetBlockBlobReference(fileName);

                
                using (var memoryStream = new MemoryStream()) {
                    blob.DownloadToStream(memoryStream);

                    long length = memoryStream.Length;
                    byte[] bytes = new byte[length];
                    int bytesToRead = (int)length;
                    int numBytesRead = 0;
                    do {
                        int chunkSize = 1000;
                        if (chunkSize > bytesToRead) {
                            chunkSize = bytesToRead;
                        }
                        int n = memoryStream.Read(bytes, numBytesRead, chunkSize);
                        numBytesRead += n;
                        bytesToRead -= n;
                    }
                    while (bytesToRead > 0);
                    String contents = Encoding.UTF8.GetString(bytes);
                    result.Data = Convert.ToBase64String(bytes);
                }
            } catch (Exception ex) {
                ex.ToExceptionless()
                      .SetMessage("Unable to download blob from Azure:" + fileName)
                      .MarkAsCritical()
                      .Submit();
                result.IsSuccessful = false;
                result.Error = new ProcessingError(ex.Message, "Unable to download blob from Azure:" + fileName, true);
            }

            return result;
        }

        public DataAccessResult<Stream> Get(string fileName) {
            var result = new DataAccessResult<Stream>();

            try {
                var client = GetBlobContainer();
                var blob = client.GetBlockBlobReference(fileName);

                
                using (var memoryStream = new MemoryStream()) {
                    blob.DownloadToStream(memoryStream);
                    result.Data = memoryStream;
                }

            } catch (Exception ex) {
                ex.ToExceptionless()
                      .SetMessage("Unable to download blob from Azure:" + fileName)
                      .MarkAsCritical()
                      .Submit();
                result.IsSuccessful = false;
                result.Error = new ProcessingError(ex.Message, "Unable to download blob from Azure:" + fileName, true);
            }

            return result;
        }
    }
}