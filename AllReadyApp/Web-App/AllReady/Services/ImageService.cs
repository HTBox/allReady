using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.OptionsModel;
using Microsoft.Net.Http.Headers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AllReady.Services
{
    public class ImageService : IImageService
    {
        private const string CONTAINER_NAME = "images";
        private readonly AzureStorageSettings _settings;

        public ImageService(IOptions<AzureStorageSettings> options)
        {
            _settings = options.Value;
        }

        /*
        Blob path conventions
        images/tenantid/imagename
        images/tenant/activityId/imagename
        image/guid/imagename
        */

        /// <summary>
        /// Uploads an image given a unique tenant ID. Passing in the same params will overwrite the existing file.
        /// </summary>
        /// <param name="tenantId">int ID</param>
        /// <param name="image">a image from Microsoft.AspNet.Http</param>
        /// <returns></returns>
        public async Task<string> UploadTenantImageAsync(int tenantId, IFormFile image)
        {
            var blobPath = tenantId.ToString();
            return await UploadImageAsync(blobPath, image);
        }

        public async Task<string> UploadActivityImageAsync(int tenantId, int activityId, IFormFile image)
        {
            var blobPath = tenantId + @"/activities/" + activityId;
            return await UploadImageAsync(blobPath, image);
        }

        public async Task<string> UploadCampaignImageAsync(int tenantId, int campaignId, IFormFile image)
        {
            var blobPath = tenantId + @"/campaigns/" + campaignId;
            return await UploadImageAsync(blobPath, image);
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            var blobPath = Guid.NewGuid().ToString().ToLower();
            return await UploadImageAsync(blobPath, image);
        }

        private async Task<string> UploadImageAsync(string blobPath, IFormFile image)
        {
            //Get filename
            var fileName = (ContentDispositionHeaderValue.Parse(image.ContentDisposition).FileName).Trim('"').ToLower();
            Debug.WriteLine("BlobPath={0}, fileName={1}, image length={2}", blobPath, fileName, image.Length.ToString());

            if (fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") || fileName.EndsWith(".png") ||
                fileName.EndsWith(".gif"))
            {
                var account = CloudStorageAccount.Parse(_settings.AzureStorage);
                var container = account.CreateCloudBlobClient().GetContainerReference(CONTAINER_NAME);

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
