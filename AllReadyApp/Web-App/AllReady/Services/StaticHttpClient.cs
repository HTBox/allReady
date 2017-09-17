using System.Net.Http;
using System.Threading.Tasks;

namespace AllReady.Services
{
    public class StaticHttpClient : IHttpClient
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await HttpClient.GetAsync(requestUri);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await HttpClient.SendAsync(request);
        }
    }
}
