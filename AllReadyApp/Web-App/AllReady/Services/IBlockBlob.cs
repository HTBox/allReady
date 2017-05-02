using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AllReady.Services
{
    public interface IBlockBlob
    {
        Task<string> UploadFromStreamAsync(string containerName, string blobName, IFormFile file);
        Task DeleteAsync(string containerName, string blobUrl);
    }
}
