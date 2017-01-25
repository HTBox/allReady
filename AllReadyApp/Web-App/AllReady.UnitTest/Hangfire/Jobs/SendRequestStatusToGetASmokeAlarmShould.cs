using AllReady.Hangfire.Jobs;
using AllReady.Services;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Hangfire.Jobs
{
    public class SendRequestStatusToGetASmokeAlarmShould
    {
        private readonly IOptions<GetASmokeAlarmApiSettings> _getASmokeAlarmApiSettings;
        public SendRequestStatusToGetASmokeAlarmShould()
        {
            var settings = new GetASmokeAlarmApiSettings()
            {
                BaseAddress = "https://demo.getasmokealarm.org/",
                Token = "someToken"
            };
            _getASmokeAlarmApiSettings = Options.Create(settings);

        }

        [Fact]
        public void SendJsonService()
        {
            HttpRequestMessage requestMsg = null;
            var mockedHttpClient = new Mock<IHttpClient>();
            mockedHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(new HttpResponseMessage())
                .Callback<HttpRequestMessage>(request => requestMsg = request);
            var sendRequestStatusToGetASmokeAlarm = new SendRequestStatusToGetASmokeAlarm(_getASmokeAlarmApiSettings, mockedHttpClient.Object);
            try
            {
                sendRequestStatusToGetASmokeAlarm.Send("request1", "new", false);
            }
            catch { }

            //mockedHttpClient.Verify(x => x.SendAsync(
            //    It.Is<HttpRequestMessage>(request =>
            //        request.RequestUri == new Uri("https://demo.getasmokealarm.org/admin/requests/status/request1")
            //)));

        }
    }
}
