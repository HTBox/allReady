using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace AllReady.Hangfire.Jobs
{
    public class SendRequestStatusToGetASmokeAlarm : ISendRequestStatusToGetASmokeAlarm
    {
        private readonly IOptions<GetASmokeAlarmApiSettings> getASmokeAlarmApiSettings;
        private static HttpClient httpClient;

        public SendRequestStatusToGetASmokeAlarm(IOptions<GetASmokeAlarmApiSettings> getASmokeAlarmApiSettings)
        {
            this.getASmokeAlarmApiSettings = getASmokeAlarmApiSettings;
            CreateStaticHttpClient(getASmokeAlarmApiSettings);
        }

        public void Send(string serial, string status, bool acceptance)
        {
            var updateRequestMessage = new { acceptance, status };
            var response = httpClient.PostAsJsonAsync($"{getASmokeAlarmApiSettings.Value.BaseAddress}admin/requests/status/{serial}", updateRequestMessage).Result;

            //throw HttpRequestException if response is not a success code.
            response.EnsureSuccessStatusCode();
        }

        private void CreateStaticHttpClient(IOptions<GetASmokeAlarmApiSettings> getASmokeAlarmApiSettings)
        {
            if (httpClient == null)
            {
                //TODO mgmccarthy: the one drawback to setting HttpClient to static is when the authentication token changes. I THINK we would need to reload the web appliation for the changes to take place?
                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", getASmokeAlarmApiSettings.Value.Token);
            }
        }
    }

    public interface ISendRequestStatusToGetASmokeAlarm
    {
        void Send(string serial, string status, bool acceptance);
    }
}