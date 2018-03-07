using AllReady.Hangfire.Jobs;
using AllReady.Services;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Net;
using System.Net.Http;
using AllReady.Configuration;
using Xunit;
using System.Threading.Tasks;

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
        public async Task SendsValidHttpRequestMessage()
        {
            var mockedHttpClient = new Mock<IHttpClient>();
            var sendRequestStatusToGetASmokeAlarm = new SendRequestStatusToGetASmokeAlarm(_getASmokeAlarmApiSettings, mockedHttpClient.Object);
            try
            {
                await sendRequestStatusToGetASmokeAlarm.Send("request1", "new", false);
            }
            catch { }

            mockedHttpClient.Verify(x => x.SendAsync(
                It.Is<HttpRequestMessage>(request =>
                    request.RequestUri == new Uri("https://demo.getasmokealarm.org/admin/requests/status/request1")
                    && request.Headers.Authorization.Scheme == "someToken"
                    && request.Content.ReadAsStringAsync().Result == "{\"acceptance\":false,\"status\":\"new\"}"
                    && request.Content.Headers.ContentType.ToString() == "application/json; charset=utf-8"
            )));
        }

        [Fact]
        public async Task ThrowsException_WhenOnNonSuccessHttpStatus()
        {
            var mockedHttpClient = new Mock<IHttpClient>();
            mockedHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));
            var sendRequestStatusToGetASmokeAlarm = new SendRequestStatusToGetASmokeAlarm(_getASmokeAlarmApiSettings, mockedHttpClient.Object);
            
            await Assert.ThrowsAsync<HttpRequestException>(
                async () => await sendRequestStatusToGetASmokeAlarm.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())
            );            
        }
    }
}
