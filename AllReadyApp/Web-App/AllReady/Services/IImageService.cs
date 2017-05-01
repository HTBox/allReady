using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AllReady.Services
{
    public interface IImageService
    {
        Task<string> UploadOrganizationImageAsync(int organizationId, IFormFile image);
        Task<string> UploadCampaignImageAsync(int organizationId, int campaignId, IFormFile image);
        Task<string> UploadEventImageAsync(int organizationId, int eventId, IFormFile image);
        Task DeleteImageAsync(string imageUrl);
    }
}