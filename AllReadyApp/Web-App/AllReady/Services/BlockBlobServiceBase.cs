using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace AllReady.Services
{
    public abstract class BlockBlobServiceBase
    {
        private readonly IBlockBlob blockBlob;

        protected BlockBlobServiceBase(IBlockBlob blockBlob)
        {
            this.blockBlob = blockBlob;
        }

        protected abstract string ContainerName { get; }
        protected string BlobName { get; private set; }
        protected virtual List<string> AllowableFileExtensions { get; set; } = new List<string>();
        protected virtual List<string> DisallowableFileExtensions { get; set; } = new List<string>();

        protected async Task<string> UploadAsync(string blobPath, IFormFile file)
        {
            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"').ToLower();

            if (FileIsAllowed(fileName) && !FileIsDisallowed(fileName))
            //if (fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") || fileName.EndsWith(".png") || fileName.EndsWith(".gif"))
            {
                BlobName = blobPath + "/" + fileName;
                return await blockBlob.UploadFromStreamAsync(ContainerName, BlobName, file);
            }

            throw new Exception("Invalid file extension: " + fileName + "You can only upload images with the extension: jpg, jpeg, gif, or png");
        }

        protected async Task DeleteAsync(string imageUrl)
        {
            await blockBlob.DeleteAsync(ContainerName, imageUrl);
        }

        private bool FileIsAllowed(string fileName)
        {
            if (AllowableFileExtensions.Count == 0)
                return true;

            //whitelist
            if (AllowableFileExtensions.Contains(fileName))
                return true;

            return false;
        }

        private bool FileIsDisallowed(string fileName)
        {
            if (DisallowableFileExtensions.Count == 0)
                return false;

            //blacklist
            if (DisallowableFileExtensions.Contains(fileName))
                return true;

            return false;
        }
    }
}