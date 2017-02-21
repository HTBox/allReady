using System.Net.Http;
using System.Net.Http.Headers;
using AllReady.Configuration;
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
            _httpClient = httpClient;
        }

        public void Send(string serial, string status, bool acceptance)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_getASmokeAlarmApiSettings.BaseAddress}admin/requests/status/{serial}")
            {
                Content = new StringContent( JsonConvert.SerializeObject(new { acceptance, status }), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(_getASmokeAlarmApiSettings.Token);
            var response = _httpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
        }
    }
    
    public static class GasaStatus
    {
        public static string New = "new";
        public static string InProgress = "in progress";
        public static string Installed = "";
        public static string Canceled = "canceled";
    }

    public interface ISendRequestStatusToGetASmokeAlarm
    {
        void Send(string serial, string status, bool acceptance);
    }
}