using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AllReady.Services
{
    public interface IAttachmentService
    {
        Task<string> UploadTaskAttachmentAsync(int taskId, IFormFile attachment);
        Task DeleteAttachmentAsync(string attachmentUrl);
    }
}