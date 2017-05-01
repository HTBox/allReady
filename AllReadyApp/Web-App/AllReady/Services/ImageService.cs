using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AllReady.Services
{
    public class ImageService : BlockBlobServiceBase, IImageService
    {
        public ImageService(IBlockBlob blockBlob) : base(blockBlob)
        {
        }

        protected override string ContainerName => "images";

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
            await DeleteAsync(imageUrl);
        }
    }
}