using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Polly;

namespace AllReady.Features.Requests
{
    public class SendRequestStatusToGetASmokeAlarmEndpointHandler : AsyncRequestHandler<SendRequestStatusToGetASmokeAlarmEndpoint>
    {
        //http://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/ BUT,
        //setting HttpClient to singleton/static instance is NOT a true fix. Other work needs to be done in order for a static HttpClient to honor DNS changes
        //this article explicitly mentinos deployments with Azure and the blue/green swap that happens when you move from Staging to Productoin. If the change in this blog
        //is not made and HttpClient is run as singleton, you could potentially be posting requests to the wrong environment/server(s)
        //http://byterot.blogspot.co.uk/2016/07/singleton-httpclient-dns.html
        //private static readonly HttpClient HttpClient = new HttpClient();

        private readonly ILogger<SendRequestStatusToGetASmokeAlarmEndpointHandler> logger;

        public SendRequestStatusToGetASmokeAlarmEndpointHandler(ILogger<SendRequestStatusToGetASmokeAlarmEndpointHandler> logger)
        {
            this.logger = logger;
        }

        protected override async Task HandleCore(SendRequestStatusToGetASmokeAlarmEndpoint message)
        {
            //test serial numbers: 
            //CHNI-16-00030
            //CHNI-16-00031

            //TODO: move to config. This value will change between "demo" and live functionality
            const string baseAddress = "https://demo.getasmokealarm.org/";
            //TODO: move to user secrets. This token will change between "demo" and live functionality
            const string token = "Oht2Fug2";
            //TODO: move to config. This we can change the number of retries per environment
            const int retry = 5;

            var updateRequestMessage = new { message.Acceptance, message.Status };

            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(retry, retryCount => TimeSpan.FromSeconds(retryCount * retryCount), onRetry: (exception, timeSpan, retryCount) =>
                {
                    logger.LogWarning($"Call to getasmokealarm's {baseAddress} failed for serial {message.Serial} : Retry {retryCount} out of {retry} retries : TimeSpan {timeSpan} : Ex {exception.Message}");
                });

            try
            {
                var response = await retryPolicy.ExecuteAsync(async () =>
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.BaseAddress = new Uri(baseAddress);
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Add("Authorization", token);

                        //https://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client
                        //"the PostAsJsonAsync method serializes an object to JSON and then sends the JSON payload in a POST request."
                        return await httpClient.PostAsJsonAsync($"admin/requests/status/{message.Serial}", updateRequestMessage);
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