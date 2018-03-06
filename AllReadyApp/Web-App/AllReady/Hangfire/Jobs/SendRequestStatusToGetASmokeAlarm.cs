using System.Net.Http;
using System.Net.Http.Headers;
using AllReady.Configuration;
using Microsoft.Extensions.Options;
using AllReady.Services;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace AllReady.Hangfire.Jobs
{
    public class SendRequestStatusToGetASmokeAlarm : ISendRequestStatusToGetASmokeAlarm
    {
        private readonly GetASmokeAlarmApiSettings _getASmokeAlarmApiSettings;
        private readonly IHttpClient _httpClient;

        public SendRequestStatusToGetASmokeAlarm(IOptions<GetASmokeAlarmApiSettings> getASmokeAlarmApiSettings, IHttpClient httpClient)
        {
            _getASmokeAlarmApiSettings = getASmokeAlarmApiSettings.Value;
            _httpClient = httpClient;
        }

        public async Task Send(string serial, string status, bool acceptance)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_getASmokeAlarmApiSettings.BaseAddress}admin/requests/status/{serial}")
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { acceptance, status }), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(_getASmokeAlarmApiSettings.Token);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }

    public static class GasaStatus
    {
        public const string New = "new";
        public const string InProgress = "in progress";
        public const string Installed = "";
        public const string Canceled = "canceled";
        public const string Requested = "requested";
    }

    public interface ISendRequestStatusToGetASmokeAlarm
    {
        Task Send(string serial, string status, bool acceptance);
    }
}
