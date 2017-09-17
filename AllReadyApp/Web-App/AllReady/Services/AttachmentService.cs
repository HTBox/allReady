using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace AllReady.Services
{
    public class AttachmentService : IAttachmentService
    {
        private const string ContainerName = "attachments";
        private readonly IBlockBlob blockBlob;

        public AttachmentService(IBlockBlob blockBlob)
        {
            this.blockBlob = blockBlob;
        }

        public async Task<string> UploadTaskAttachmentAsync(int taskId, IFormFile attachment)
        {
            var blobPath = "task/" + taskId;
            return await UploadAttachmentAsync(blobPath, attachment);
        }

        public async Task DeleteAttachmentAsync(string attachmentUrl)
        {
            await blockBlob.DeleteAsync(ContainerName, attachmentUrl);
        }

        private async Task<string> UploadAttachmentAsync(string blobPath, IFormFile attachment)
        {
            var fileName = ContentDispositionHeaderValue.Parse(attachment.ContentDisposition).FileName.ToString().Trim('"').ToLower();
            var blobName = blobPath + "/" + fileName;
            return await blockBlob.UploadFromStreamAsync(ContainerName, blobName, attachment);
        }
    }
}
