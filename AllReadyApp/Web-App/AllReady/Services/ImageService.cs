using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace AllReady.Services
{
    public class ImageService : IImageService
    {
        private const string ContainerName = "images";
        private readonly IBlockBlob blockBlob;

        public ImageService(IBlockBlob blockBlob)
        {
            this.blockBlob = blockBlob;
        }

        public async Task<string> UploadOrganizationImageAsync(int organizationId, IFormFile image)
        {
            var blobPath = organizationId.ToString();
            return await UploadAsync(blobPath, image);
        }

        public async Task<string> UploadCampaignImageAsync(int organizationId, int campaignId, IFormFile image)
        {
            var blobPath = organizationId + @"/campaigns/" + campaignId;
            return await UploadAsync(blobPath, image);
        }

        public async Task<string> UploadEventImageAsync(int organizationId, int eventId, IFormFile image)
        {
            var blobPath = organizationId + @"/events/" + eventId;
            return await UploadAsync(blobPath, image);
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            await blockBlob.DeleteAsync(ContainerName, imageUrl);
        }

        private async Task<string> UploadAsync(string blobPath, IFormFile image)
        {
            var fileName = ContentDispositionHeaderValue.Parse(image.ContentDisposition).FileName.ToString().Trim('"').ToLower();
            if (fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") || fileName.EndsWith(".png") || fileName.EndsWith(".gif"))
            {    
                var blobName = blobPath + "/" + fileName;
                return await blockBlob.UploadFromStreamAsync(ContainerName, blobName, image);
            }

            throw new Exception("Invalid file extension: " + fileName + "You can only upload images with the extension: jpg, jpeg, gif, or png");
        }
    }
}
