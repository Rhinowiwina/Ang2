using Microsoft.WindowsAzure.Storage;

namespace LS.AzureStorage
{
    public abstract class AzureStorageBase
    {
        protected CloudStorageAccount CloudStorageAccount { get; set; }

        public abstract string StorageContainerName { get; }

        protected AzureStorageBase(CloudStorageAccount cloudStorageAccount)
        {
            CloudStorageAccount = cloudStorageAccount;
        }

        public abstract void CreateStorageContainerIfDoesNotExist();
        public abstract void DeleteStorageContainer();
    }
}
