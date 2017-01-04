using System.Net.Http;
using System.Threading.Tasks;

namespace AllReady.Services.Routing
{
    public class StaticHttpClient : IHttpClient
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await HttpClient.GetAsync(requestUri);
        }
    }
}
