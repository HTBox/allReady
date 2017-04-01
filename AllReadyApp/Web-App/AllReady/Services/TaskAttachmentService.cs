using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using AllReady.Configuration;

namespace AllReady.Services
{
    public class TaskAttachmentService : ITaskAttachmentService
    {
        private const string ContainerName = "attachments";

        private readonly AzureStorageSettings _options;

        public TaskAttachmentService(IOptions<AzureStorageSettings> options)
        {
            _options = options.Value;
        }

        /*
        Blob path conventions
        attachments/task/taskId/attachmentname
        */

        /// <summary>Uploads an attachment given a task ID.</summary>
        /// <param name="taskId">int ID</param>
        /// <param name="attachment">a attachment from Microsoft.AspNet.Http</param>
        /// <returns>URL to the uploaded file</returns>
        public async Task<string> UploadAsync(int taskId, IFormFile attachment)
        {
            var blobPath = "task/" + taskId;
            return await UploadAsync(blobPath, attachment);
        }

        /// <summary>Deletes an attachment permanently</summary>
        /// <param name="attachmentUrl">The URL to the attachment</param>
        /// <returns>The deletion task</returns>
        public async Task DeleteAsync(string attachmentUrl)
        {
            var blobContainer = CloudStorageAccount.Parse(_options.AzureStorage)
                .CreateCloudBlobClient()
                .GetContainerReference(ContainerName);

            var blobName = attachmentUrl.Replace($"{blobContainer.Uri.AbsoluteUri}/", string.Empty);
            var blockBlob = blobContainer.GetBlockBlobReference(blobName);

            await blockBlob.DeleteAsync();
        }

        private async Task<string> UploadAsync(string blobPath, IFormFile attachment)
        {
            //Get filename
            var fileName = ContentDispositionHeaderValue.Parse(attachment.ContentDisposition).FileName.Trim('"').ToLower();
            Debug.WriteLine($"BlobPath={blobPath}, fileName={fileName}, attachment length={attachment.Length}");
          
            var account = CloudStorageAccount.Parse(_options.AzureStorage);
            var container = account.CreateCloudBlobClient().GetContainerReference(ContainerName);

            //Create container if it doesn't exist wiht public access
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Container, new BlobRequestOptions(), new OperationContext());

            var blob = blobPath + "/" + fileName;
            Debug.WriteLine("blob" + blob);

            var blockBlob = container.GetBlockBlobReference(blob);

            blockBlob.Properties.ContentType = attachment.ContentType;

            using (var attachmentStream = attachment.OpenReadStream())
            {
                var contents = new byte[attachment.Length];
                for (var i = 0; i < attachment.Length; i++)
                {
                    contents[i] = (byte)attachmentStream.ReadByte();
                }

                await blockBlob.UploadFromByteArrayAsync(contents, 0, (int)attachment.Length);
            }

            Debug.WriteLine("Attachment uploaded to URI: " + blockBlob.Uri);
            return blockBlob.Uri.ToString();
        }
    }
}