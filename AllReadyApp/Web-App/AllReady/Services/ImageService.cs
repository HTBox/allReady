using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AllReady.Services
{
    public class ImageService : IImageService
    {
        private const string ContainerName = "images";
        private readonly AzureStorageSettings _options;

        public ImageService(IOptions<AzureStorageSettings> options)
        {
            _options = options.Value;
        }

        /*
        Blob path conventions
        images/organizationId/imagename
        images/organization/eventId/imagename
        image/guid/imagename
        */

        /// <summary>
        /// Uploads an image given a unique organization ID. Passing in the same params will overwrite the existing file.
        /// </summary>
        /// <param name="organizationId">int ID</param>
        /// <param name="image">a image from Microsoft.AspNet.Http</param>
        /// <returns></returns>
        public async Task<string> UploadOrganizationImageAsync(int organizationId, IFormFile image)
        {
            var blobPath = organizationId.ToString();
            return await UploadImageAsync(blobPath, image);
        }

        public async Task<string> UploadEventImageAsync(int organizationId, int eventId, IFormFile image)
        {
            var blobPath = organizationId + @"/events/" + eventId;
            return await UploadImageAsync(blobPath, image);
        }

        public async Task<string> UploadCampaignImageAsync(int organizationId, int campaignId, IFormFile image)
        {
            var blobPath = organizationId + @"/campaigns/" + campaignId;
            return await UploadImageAsync(blobPath, image);
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            var blobPath = Guid.NewGuid().ToString().ToLower();
            return await UploadImageAsync(blobPath, image);
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            var blobContainer = CloudStorageAccount.Parse(_options.AzureStorage)
                .CreateCloudBlobClient()
                .GetContainerReference(ContainerName);

            var blobName = imageUrl.Replace($"{blobContainer.Uri.AbsoluteUri}/", string.Empty);
            var blockBlob = blobContainer.GetBlockBlobReference(blobName);

            await blockBlob.DeleteAsync();
        }

        private async Task<string> UploadImageAsync(string blobPath, IFormFile image)
        {
            //Get filename
            var fileName = ContentDispositionHeaderValue.Parse(image.ContentDisposition).FileName.Trim('"').ToLower();
            Debug.WriteLine($"BlobPath={blobPath}, fileName={fileName}, image length={image.Length}");

            if (fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") || fileName.EndsWith(".png") || fileName.EndsWith(".gif"))
            {
                var account = CloudStorageAccount.Parse(_options.AzureStorage);
                var container = account.CreateCloudBlobClient().GetContainerReference(ContainerName);

                //Create container if it doesn't exist wiht public access
                await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Container, new BlobRequestOptions(), new OperationContext());

                var blob = blobPath + "/" + fileName;
                Debug.WriteLine("blob" + blob);

                var blockBlob = container.GetBlockBlobReference(blob);

                blockBlob.Properties.ContentType = image.ContentType;

                using (var imageStream = image.OpenReadStream())
                {
                    //Option #1
                    var contents = new byte[image.Length];

                    for (var i = 0; i < image.Length; i++)
                    {
                        contents[i] = (byte)imageStream.ReadByte();
                    }

                    await blockBlob.UploadFromByteArrayAsync(contents, 0, (int)image.Length);

                    //Option #2
                    //await blockBlob.UploadFromStreamAsync(imageStream);
                }

                Debug.WriteLine("Image uploaded to URI: " + blockBlob.Uri);
                return blockBlob.Uri.ToString();
            }

            throw new Exception("Invalid file extension: " + fileName + "You can only upload images with the extension: jpg, jpeg, gif, or png");
        }
    }
}