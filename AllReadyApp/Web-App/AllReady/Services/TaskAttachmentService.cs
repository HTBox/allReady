using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace AllReady.Services
{
    public class TaskAttachmentService : ITaskAttachmentService
    {
        private const string ContainerName = "attachments";
        private readonly IBlockBlob blockBlob;

        public TaskAttachmentService(IBlockBlob blockBlob)
        {
            this.blockBlob = blockBlob;
        }

        public async Task<string> UploadAsync(int taskId, IFormFile attachment)
        {
            var blobPath = "task/" + taskId;
            return await UploadAsync(blobPath, attachment);
        }

        public async Task DeleteAsync(string attachmentUrl)
        {
            await blockBlob.DeleteAsync(ContainerName, attachmentUrl);
        }

        private async Task<string> UploadAsync(string blobPath, IFormFile attachment)
        {
            var fileName = ContentDispositionHeaderValue.Parse(attachment.ContentDisposition).FileName.Trim('"').ToLower();
            var blobName = blobPath + "/" + fileName;
            return await blockBlob.UploadFromStreamAsync(ContainerName, blobName, attachment);
        }

    }
}