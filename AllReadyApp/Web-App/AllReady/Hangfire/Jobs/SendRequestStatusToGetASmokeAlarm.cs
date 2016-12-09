using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace AllReady.Hangfire.Jobs
{
    public class SendRequestStatusToGetASmokeAlarm : ISendRequestStatusToGetASmokeAlarm
    {
        private readonly IOptions<GetASmokeAlarmApiSettings> getASmokeAlarmApiSettings;
        private static readonly HttpClient HttpClient = new HttpClient();

        public SendRequestStatusToGetASmokeAlarm(IOptions<GetASmokeAlarmApiSettings> getASmokeAlarmApiSettings)
        {
            this.getASmokeAlarmApiSettings = getASmokeAlarmApiSettings;
        }

        public void Send(string serial, string status, bool acceptance)
        {
            var baseAddress = getASmokeAlarmApiSettings.Value.BaseAddress;
            var token = getASmokeAlarmApiSettings.Value.Token;

            var updateRequestMessage = new { acceptance, status };

            HttpClient.BaseAddress = new Uri(baseAddress);
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpClient.DefaultRequestHeaders.Add("Authorization", token);

            var response = HttpClient.PostAsJsonAsync($"admin/requests/status/{serial}", updateRequestMessage).Result;

            //throw HttpRequestException if response is not a success code.
            response.EnsureSuccessStatusCode();
        }
    }

    public interface ISendRequestStatusToGetASmokeAlarm
    {
        void Send(string serial, string status, bool acceptance);
    }
}