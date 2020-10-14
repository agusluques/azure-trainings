using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;

namespace TestWebApp.Helpers
{
    public  class StorageHelper
    {
        private static CloudBlobClient cloudBlobClient;
        private static CloudBlobContainer cloudBlobContainer;
        private readonly ILogger<StorageHelper> _logger;
        public StorageHelper(ILogger<StorageHelper> logger)
        {
            _logger = logger;
        }

        public async Task<bool> UploadImage(Stream fileStream, string fileName, IConfiguration Configuration)
        {
            string storageConnection = Configuration.GetConnectionString("StorageAccount");

            cloudBlobClient = CloudStorageAccount.Parse(storageConnection)
                .CreateCloudBlobClient();

            cloudBlobContainer = cloudBlobClient.GetContainerReference(Configuration.GetConnectionString("BlobImages"));

            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

            //cloudBlockBlob.Properties.ContentType = file.ContentType;

            await cloudBlockBlob.UploadFromStreamAsync(fileStream);

            // log the URi
            _logger.LogInformation(cloudBlockBlob.SnapshotQualifiedStorageUri.PrimaryUri.ToString());

            return await Task.FromResult(true);
        }
    }
}
