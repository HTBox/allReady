using AllReady.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AllReady.Services
{
    public class BlockBlob : IBlockBlob
    {
        private readonly IOptions<AzureStorageSettings> options;

        public BlockBlob(IOptions<AzureStorageSettings> options)
        {
            this.options = options;
        }

        public async Task<string> UploadFromStreamAsync(string containerName, string blobName, IFormFile file)
        {
            var container = CloudStorageAccount.Parse(options.Value.AzureStorage).CreateCloudBlobClient().GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Container, new BlobRequestOptions(), new OperationContext());
            var blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.Properties.ContentType = file.ContentType;

            using (var imageStream = file.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(imageStream);
            }

            return blockBlob.Uri.ToString();
        }

        public async Task DeleteAsync(string containerName, string blobUrl)
        {
            var blobContainer = CloudStorageAccount.Parse(options.Value.AzureStorage).CreateCloudBlobClient().GetContainerReference(containerName);
            var blobName = blobUrl.Replace($"{blobContainer.Uri.AbsoluteUri}/", string.Empty);
            var blockBlob = blobContainer.GetBlockBlobReference(blobName);

            await blockBlob.DeleteAsync();
        }
    }
}
