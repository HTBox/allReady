using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace AllReady.Services
{
    public class FileImageService : IImageService
    {
        private const string UploadFolder = "upload";
        private const string WebPath = "/" + UploadFolder + "/";
        private readonly string _uploadPath;

        public FileImageService(IHostingEnvironment environment)
        {
            _uploadPath = Path.Combine(environment.WebRootPath, UploadFolder);
            var uploadDir = new DirectoryInfo(_uploadPath);
            if (!uploadDir.Exists)
            {
                uploadDir.Create();
            }
        }

        public Task<string> UploadEventImageAsync(int organizationId, int eventId, IFormFile image)
        {
            return UploadFile($"EI{organizationId:D6}_{eventId:D6}", image);
        }

        public Task<string> UploadCampaignImageAsync(int organizationId, int campaignId, IFormFile image)
        {
            return UploadFile($"CI{organizationId:D6}_{campaignId:D6}", image);
        }

        public Task<string> UploadImageAsync(IFormFile image)
        {
            return UploadFile(Guid.NewGuid().ToString().ToLower(), image);
        }

        public Task<string> UploadOrganizationImageAsync(int organizationId, IFormFile image)
        {
            return UploadFile($"OI{organizationId:D6}", image);
        }

        private async Task<string> UploadFile(string filename, IFormFile image)
        {
            string filenameWithExt = filename + Path.GetExtension(image.FileName);
            string fullPath = Path.Combine(_uploadPath, filenameWithExt);
            using (FileStream fs = File.OpenWrite(fullPath))
            {
                await image.CopyToAsync(fs);
            }

            return WebPath + filenameWithExt;
        }

        public Task DeleteImageAsync(string imageUrl)
        {
            string filename = Path.GetFileName(imageUrl);
            string fileFullPath = Path.Combine(_uploadPath, filename);
            File.Delete(fileFullPath);
            return Task.CompletedTask;
        }
    }
}
