using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace AllReady.Services
{
    public class FakeImageService : IImageService
    {
        private const string UploadFolder = "upload";
        private const string WebPath = "/" + UploadFolder + "/";
        private readonly string uploadPath;

        public FakeImageService(IHostingEnvironment environment)
        {
            uploadPath = Path.Combine(environment.WebRootPath, UploadFolder);
            var uploadDir = new DirectoryInfo(uploadPath);
            if (!uploadDir.Exists)
            {
                uploadDir.Create();
            }
        }

        public Task<string> UploadEventImageAsync(int organizationId, int eventId, IFormFile image)
        {
            return UploadFile(image);
        }

        public Task<string> UploadCampaignImageAsync(int organizationId, int campaignId, IFormFile image)
        {
            return UploadFile(image);
        }

        public Task<string> UploadImageAsync(IFormFile image)
        {
            return UploadFile(image);
        }

        public Task<string> UploadOrganizationImageAsync(int organizationId, IFormFile image)
        {
            return UploadFile(image);
        }

        public Task DeleteImageAsync(string imageUrl)
        {
            var filename = Path.GetFileName(imageUrl);
            var fileFullPath = Path.Combine(uploadPath, filename);
            if (File.Exists(fileFullPath))
            {
                File.Delete(fileFullPath);
            }

            return Task.CompletedTask;
        }

        private async Task<string> UploadFile(IFormFile image)
        {
            var filename = Guid.NewGuid().ToString().ToLower();
            var filenameWithExt = filename + Path.GetExtension(image.FileName);
            var fullPath = Path.Combine(uploadPath, filenameWithExt);
            using (var fs = File.OpenWrite(fullPath))
            {
                await image.CopyToAsync(fs);
            }

            return WebPath + filenameWithExt;
        }
    }
}
