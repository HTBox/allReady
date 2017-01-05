using System.Net.Http;
using System.Threading.Tasks;

namespace AllReady.Services.Routing
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);
    }
}
