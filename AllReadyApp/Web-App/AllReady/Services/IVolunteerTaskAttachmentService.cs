using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AllReady.Services
{
    public interface IVolunteerTaskAttachmentService
    {
        Task<string> UploadAsync(int volunteerTaskId, IFormFile attachment);
        Task DeleteAsync(string attachmentUrl);
    }
}