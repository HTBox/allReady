using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace AllReady.Hangfire.Jobs
{
    public class SendRequestStatusToGetASmokeAlarm : ISendRequestStatusToGetASmokeAlarm
    {
        private static HttpClient httpClient;

        public SendRequestStatusToGetASmokeAlarm(IOptions<GetASmokeAlarmApiSettings> getASmokeAlarmApiSettings)
        {
            CreateStaticHttpClient(getASmokeAlarmApiSettings);
        }

        public void Send(string serial, string status, bool acceptance)
        {
            var updateRequestMessage = new { acceptance, status };
            var response = httpClient.PostAsJsonAsync($"admin/requests/status/{serial}", updateRequestMessage).Result;

            //throw HttpRequestException if response is not a success code.
            response.EnsureSuccessStatusCode();
        }

        private void CreateStaticHttpClient(IOptions<GetASmokeAlarmApiSettings> getASmokeAlarmApiSettings)
        {
            if (httpClient == null)
            {
                httpClient = new HttpClient { BaseAddress = new Uri(getASmokeAlarmApiSettings.Value.BaseAddress)};
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //TODO mgmccarthy: the one drawback here is if GASA were to give us a new authorization token, we'd have to reload the web app to get it to update b/c this is now static
                httpClient.DefaultRequestHeaders.Add("Authorization", getASmokeAlarmApiSettings.Value.Token);
            }
        }
    }

    public interface ISendRequestStatusToGetASmokeAlarm
    {
        void Send(string serial, string status, bool acceptance);
    }
}