using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace LS.AzureStorage
{
    public abstract class BlobStorageBase : AzureStorageBase
    {
        protected BlobStorageBase(CloudStorageAccount cloudStorageAccount) : base(cloudStorageAccount)
        {} 

        public override void CreateStorageContainerIfDoesNotExist()
        {
            var client = GetBlobContainer();
            client.CreateIfNotExists();
        }

        public void SetAccessAs(BlobContainerPublicAccessType accessType)
        {
            var permissions = new BlobContainerPermissions();
            permissions.PublicAccess = accessType;
            var client = GetBlobContainer();
            client.SetPermissions(permissions);
        }

        public override void DeleteStorageContainer()
        {
            var client = GetBlobContainer();
            client.Delete();
        }

        public void Save(Stream stream, string blobName, string contentType = "application/octet-stream")
        {
            var client = GetBlobContainer();
            var blob = client.GetBlockBlobReference(blobName);
            using(stream)
            {
                blob.UploadFromStream(stream);
                blob.Properties.ContentType = MimeContentGenerator.GetMimeTypeFor(contentType);
                blob.SetProperties();
            }
        }

        public void Save(byte[] byteArray, string blobName, string contentType = "application/octet-stream")
        {
            var stream = new MemoryStream(byteArray);

            Save(stream, blobName, contentType);
        }

        public MemoryStream Get(string blobName)
        {
            var client = GetBlobContainer();
            var blob = client.GetBlockBlobReference(blobName);
            using(var blobStream = new MemoryStream(1024*1000))
            {
                blob.DownloadToStream(blobStream);
                return blobStream;
            }
        }

        public void Delete(string blobName)
        {
            var client = GetBlobContainer();
            var blob = client.GetBlockBlobReference(blobName);
            blob.Delete(); 
        }

        public bool DoesBlobExist(string blobName)
        {
            var container = GetBlobContainer();
            var blob = container.GetBlockBlobReference(blobName);
            return blob.Exists();
        }

        private CloudBlobContainer GetBlobContainer()
        {
            var blobClient = CloudStorageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(StorageContainerName);

            return container;
        }

    }
}
