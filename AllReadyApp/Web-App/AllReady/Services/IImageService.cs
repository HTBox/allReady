using Microsoft.AspNet.Http;
using System.Threading.Tasks;

namespace AllReady.Services
{
    public interface IImageService
    {
        Task<string> UploadActivityImageAsync(int tenantId, int activityId, IFormFile image);
        Task<string> UploadCampaignImageAsync(int tenantId, int campaignId, IFormFile image);
        Task<string> UploadImageAsync(IFormFile image);
        Task<string> UploadTenantImageAsync(int tenantId, IFormFile image);
    }
}
