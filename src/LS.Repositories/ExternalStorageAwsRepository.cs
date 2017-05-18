using System;
using System.IO;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;
using LS.Core;
using LS.Core.Interfaces;
using System.Text;
using System.Collections.Generic;
using Exceptionless;
using Exceptionless.Models;

namespace LS.Repositories {
    public class IExternalStorageListDirectory {
        public string ETag { get; set; }
        public string Key { get; set; }
        public DateTime LastModified { get; set; }
        public string Owner { get; set; }
        public string Size { get; set; }
        public string StorageClass { get; set; }
    }


    public class ExternalStorageAwsRepository : IExternalStorageRepository {
        private readonly AmazonS3Client _client;
        private readonly string _bucketName;

        public ExternalStorageAwsRepository(string awsAccessKeyId, string awsSecretAccessKey, string bucketName) {
            // TODO: add parameter for region?
            var region = Amazon.RegionEndpoint.USEast1;
            _client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, region);

            _bucketName = bucketName;
        }

        public ServiceProcessingResult Save(byte[] byteArray, string fileName, string contentType = "application/octet-stream") {
            var processingResult = new ServiceProcessingResult();

            try {
                var request = new PutObjectRequest {
                    BucketName = _bucketName,
                    InputStream = new MemoryStream(byteArray),
                    Key = fileName,
                };

                _client.PutObject(request);

                processingResult.IsSuccessful = true;
            } catch (Exception ex) {
                ex.ToExceptionless()
                   .SetMessage("Save failed in external storge repository.")
                   .MarkAsCritical()
                   .Submit();
                processingResult.IsSuccessful = false;
            }

            return processingResult;
        }
        public ServiceProcessingResult Save(string fileName, string filePath) {
            var processingResult = new ServiceProcessingResult();

            try {
                var request = new PutObjectRequest {
                    BucketName = _bucketName,
                    FilePath = Path.Combine(filePath, fileName),
                    Key = fileName
                };

                _client.PutObject(request);

                processingResult.IsSuccessful = true;
            } catch (Exception ex) {
                ex.ToExceptionless()
                   .SetMessage("Save failed in external storge repository.")
                   .MarkAsCritical()
                   .Submit();
                processingResult.IsSuccessful = false;
            }

            return processingResult;
        }

        public bool DoesFileExist(string fileName) {
            S3FileInfo s3FileInfo = new S3FileInfo(_client, _bucketName, fileName);
            return s3FileInfo.Exists;
        }

        public DataAccessResult RenameFile(string sourceFileName, string destFilename) {
            var result = new DataAccessResult();

            try {
                CopyObjectRequest request = new CopyObjectRequest {
                    SourceBucket = _bucketName,
                    SourceKey = sourceFileName,
                    DestinationBucket = _bucketName,
                    DestinationKey = destFilename
                };
                _client.CopyObject(request);
            } catch (Exception ex) {
                result.IsSuccessful = false;
                result.Error = new ProcessingError("An error occurred while copying the file for a rename", "An error occurred while copying the file for a rename", false);
            }

            try {
                DeleteObjectRequest delRequest = new DeleteObjectRequest {
                    BucketName = _bucketName,
                    Key = sourceFileName
                };
                _client.DeleteObject(delRequest);

                result.IsSuccessful = true;
            } catch (Exception ex) {
                result.IsSuccessful = false;
                result.Error = new ProcessingError("An error occurred while deleting the file for a rename", "An error occurred while deleting the file for a rename", false);
            }

            return result;
        }

        public DataAccessResult<List<Core.Interfaces.IExternalStorageListDirectory>> GetDirectoryList(string prefix, string delimeter) {
            var result = new DataAccessResult<List<Core.Interfaces.IExternalStorageListDirectory>>();

            try {
                var request = new ListObjectsRequest {
                    BucketName = _bucketName,
                    Prefix = prefix,
                    Delimiter = delimeter
                };

                var response = _client.ListObjects(request);

                var returnList = new List<Core.Interfaces.IExternalStorageListDirectory>();
                foreach (var file in response.S3Objects) {
                    var returnObj = new Core.Interfaces.IExternalStorageListDirectory {
                        ETag = file.ETag,
                        Key = file.Key,
                        LastModified = file.LastModified,
                        Owner = "",
                        Size = file.Size.ToString(),
                        StorageClass = file.StorageClass
                    };
                    returnList.Add(returnObj);
                }

                result.Data = returnList;
                result.IsSuccessful = true;
            } catch (Exception ex) {
                result.IsSuccessful = false;
                result.Error = new ProcessingError("An error occurred while retrieving the list of files.", "Files could not be found. If the problem persists, contact support.", false);
            }

            return result;
        }

        public DataAccessResult<Stream> Get(string fileName) {
            var result = new DataAccessResult<Stream>();
            try {
                var request = new GetObjectRequest {
                    BucketName = _bucketName,
                    Key = fileName
                };

                var response = _client.GetObject(request);
                result.Data = response.ResponseStream;
                result.IsSuccessful = true;
            } catch (Exception ex) {
                ex.ToExceptionless()
                   .SetMessage("Get failed in external storge repository.")
                   .MarkAsCritical()
                   .Submit();
                result.IsSuccessful = false;
                // TODO: pull into error values
                result.Error = new ProcessingError("An error occurred while retrieving the file.",
                    "File could not be found. If the problem persists, contact support.", false);
            }

            return result;
        }

        public DataAccessResult<string> GetAsBase64(string fileName) {
            var result = new DataAccessResult<string>();
            try {
                var request = new GetObjectRequest {
                    BucketName = _bucketName,
                    Key = fileName
                };

                var response = _client.GetObject(request);

                try {
                    using (Stream stream = response.ResponseStream) {
                        long length = stream.Length;
                        byte[] bytes = new byte[length];
                        int bytesToRead = (int)length;
                        int numBytesRead = 0;
                        do {
                            int chunkSize = 1000;
                            if (chunkSize > bytesToRead) {
                                chunkSize = bytesToRead;
                            }
                            int n = stream.Read(bytes, numBytesRead, chunkSize);
                            numBytesRead += n;
                            bytesToRead -= n;
                        }
                        while (bytesToRead > 0);
                        String contents = Encoding.UTF8.GetString(bytes);
                        result.Data = Convert.ToBase64String(bytes);
                    }
                } catch (Exception ex) {
                    ex.ToExceptionless()
                  .SetMessage("GetAsBase64 failed in external storge repository.")
                  .MarkAsCritical()
                  .Submit();
                }
                result.IsSuccessful = true;
            } catch (Exception ex) {
                ex.ToExceptionless()
                   .SetMessage("GetAsBase64 failed in external storge repository.")
                   .MarkAsCritical()
                   .Submit();
                result.IsSuccessful = false;
                // TODO: pull into error values
                result.Error = new ProcessingError("An error occurred while retrieving the file.",
                    "File could not be found. If the problem persists, contact support.", false);
            }

            return result;
        }

        public DataAccessResult<string> GeneratePreSignedUrl(string fileName) {
            var result = new DataAccessResult<string> { IsSuccessful = true };
            GetPreSignedUrlRequest request1 = new GetPreSignedUrlRequest {
                BucketName = _bucketName,
                Key =fileName,
                Expires = DateTime.UtcNow.AddMinutes(5)
            };

            try {
                var urlString = _client.GetPreSignedURL(request1);
            
                result.Data = urlString;
            } catch (AmazonS3Exception amazonS3Exception) {
                amazonS3Exception.ToExceptionless()
                    .SetMessage("GeneratePreSignedUrl failed")
                    .MarkAsCritical()
                    .Submit();
                result.IsSuccessful = false;
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity"))) {
                    result.Error = new ProcessingError("Check the provided AWS Credentials.", "Could not generate temporary URL for " + fileName, true);
                } else {
                    string message = string.Format("Error occurred. Message:'{0}' when listing objects", amazonS3Exception.Message);
                    result.Error = new ProcessingError(message, message, true);
                }
            } catch (Exception e) {
                e.ToExceptionless()
                    .SetMessage("")
                    .MarkAsCritical()
                    .Submit();
                result.IsSuccessful = false;
                result.Error = new ProcessingError(e.Message, e.Message, true);
            }

            return result;
        }
    }
}