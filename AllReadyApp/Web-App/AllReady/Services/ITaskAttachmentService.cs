using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AllReady.Services
{
    public interface ITaskAttachmentService
    {
        Task<string> UploadAsync(int taskId, IFormFile attachment);
        Task DeleteAsync(string attachmentUrl);
    }
}