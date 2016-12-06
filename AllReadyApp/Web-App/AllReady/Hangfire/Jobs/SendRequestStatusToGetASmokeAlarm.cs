using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace AllReady.Hangfire.Jobs
{
    //TODO mgmccarthy: see if we can run HttpClient statically
    //http://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/ BUT,
    //setting HttpClient to singleton/static instance is NOT a true fix. Other work needs to be done in order for a static HttpClient to honor DNS changes
    //this article explicitly mentinos deployments with Azure and the blue/green swap that happens when you move from Staging to Productoin. If the change in this blog
    //is not made and HttpClient is run as singleton, you could potentially be posting requests to the wrong environment/server(s)
    //http://byterot.blogspot.co.uk/2016/07/singleton-httpclient-dns.html
    //private static readonly HttpClient HttpClient = new HttpClient();

    public class SendRequestStatusToGetASmokeAlarm : ISendRequestStatusToGetASmokeAlarm
    {
        private readonly IOptions<GetASmokeAlarmApiSettings> getASmokeAlarmApiSettings;

        public SendRequestStatusToGetASmokeAlarm(IOptions<GetASmokeAlarmApiSettings> getASmokeAlarmApiSettings)
        {
            this.getASmokeAlarmApiSettings = getASmokeAlarmApiSettings;
        }

        public void Send(string serial, string status, bool acceptance)
        {
            var baseAddress = getASmokeAlarmApiSettings.Value.BaseAddress;
            var token = getASmokeAlarmApiSettings.Value.Token;

            var updateRequestMessage = new { acceptance, status };

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseAddress);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", token);

                var response = httpClient.PostAsJsonAsync($"admin/requests/status/{serial}", updateRequestMessage).Result;

                //Throw HttpRequestException response is not a success code.
                response.EnsureSuccessStatusCode();
            }
        }
    }

    public interface ISendRequestStatusToGetASmokeAlarm
    {
        void Send(string serial, string status, bool acceptance);
    }
}