using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using AllReady.Services;
using Newtonsoft.Json;
using System.Text;

namespace AllReady.Hangfire.Jobs
{
    public class SendRequestStatusToGetASmokeAlarm : ISendRequestStatusToGetASmokeAlarm
    {
        private readonly GetASmokeAlarmApiSettings _getASmokeAlarmApiSettings;
        private readonly IHttpClient _httpClient;

        public SendRequestStatusToGetASmokeAlarm(IOptions<GetASmokeAlarmApiSettings> getASmokeAlarmApiSettings, IHttpClient httpClient)
        {
            _getASmokeAlarmApiSettings = getASmokeAlarmApiSettings.Value;
        }

        public void Send(string serial, string status, bool acceptance)
        {
            var updateRequestMessage = new { acceptance, status };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_getASmokeAlarmApiSettings.BaseAddress}admin/requests/status/{serial}");
            request.Content = new StringContent(JsonConvert.SerializeObject(updateRequestMessage), Encoding.UTF8, "application/json");
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(_getASmokeAlarmApiSettings.Token);
            var response = _httpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
        }
    }

    public interface ISendRequestStatusToGetASmokeAlarm
    {
        void Send(string serial, string status, bool acceptance);
    }
}