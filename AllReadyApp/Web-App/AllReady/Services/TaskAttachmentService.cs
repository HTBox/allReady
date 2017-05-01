using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AllReady.Services
{
    public class TaskAttachmentService : BlockBlobServiceBase, ITaskAttachmentService
    {
        public TaskAttachmentService(IBlockBlob blockBlob) : base(blockBlob)
        {
        }

        protected override string ContainerName => "attachments";

        public async Task<string> UploadAsync(int taskId, IFormFile attachment)
        {
            var blobPath = "task/" + taskId;
            return await UploadAsync(blobPath, attachment);
        }

        public new async Task DeleteAsync(string attachmentUrl)
        {
            await base.DeleteAsync(attachmentUrl);
        }

    }
}