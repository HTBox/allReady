using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AllReady.Services
{
    public interface IVolunteerTaskAttachmentService
    {
        Task<string> UploadAsync(int volunteerTaskId, IFormFile attachment);
        Task DeleteAsync(string attachmentUrl);
    }
}