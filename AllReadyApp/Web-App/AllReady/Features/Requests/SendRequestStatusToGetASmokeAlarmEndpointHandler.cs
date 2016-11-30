using System;
using System.Net.Http;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;

namespace AllReady.Features.Requests
{
    //http://stackoverflow.com/questions/30304365/using-httpclient-in-asp-net-5-app
    //https://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client
    public class SendRequestStatusToGetASmokeAlarmEndpointHandler : AsyncRequestHandler<SendRequestStatusToGetASmokeAlarmEndpoint>
    {
        private readonly ILogger<SendRequestStatusToGetASmokeAlarmEndpointHandler> logger;

        public SendRequestStatusToGetASmokeAlarmEndpointHandler(ILogger<SendRequestStatusToGetASmokeAlarmEndpointHandler> logger)
        {
            this.logger = logger;
        }

        protected override async Task HandleCore(SendRequestStatusToGetASmokeAlarmEndpoint message)
        {
            const string baseAddress = "http://yourapidomain.com/";
            const string token = "TOKEN";
            const int retry = 5;

            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(retry, retryCount => TimeSpan.FromSeconds(retryCount * retryCount), onRetry: (exception, timeSpan, retryCount) =>
                {
                    logger.LogWarning($"Call to getasmokealarm's {baseAddress} failed for serial {message.Serial} : Retry {retryCount} out of {retry} retries : TimeSpan {timeSpan} : Ex {exception.Message}");
                });

            var updateRequestMessage = new { message.Acceptance, message.Status };
            var json = JsonConvert.SerializeObject(updateRequestMessage);

            try
            {
                var response = await retryPolicy.ExecuteAsync(async () =>
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.BaseAddress = new Uri(baseAddress);
                        httpClient.DefaultRequestHeaders.Add("Authorization", token);
                        return await httpClient.PostAsJsonAsync($"api/products/{message.Serial}", json);
                    }
                });

                //Throw if not a success code. (this will throw HttpRequestException). HttpClient will not throw an exception on a non-200 http status code the way WebClient did
                response.EnsureSuccessStatusCode();
            }
            //if an exception is thrown that is NOT part of the .Handle<TException> configured exception, Polly will throw an exception, and we will end up here
            catch (Exception ex)
            {
                logger.LogCritical($"Call to getasmokealarm's {baseAddress} failed for serial {message.Serial} - with unhandled exception : Ex {ex.Message}");
            }
        }
    }
}