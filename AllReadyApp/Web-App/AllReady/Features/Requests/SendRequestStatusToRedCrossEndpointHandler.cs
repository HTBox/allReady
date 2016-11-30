using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Polly;

namespace AllReady.Features.Requests
{
    //http://stackoverflow.com/questions/30304365/using-httpclient-in-asp-net-5-app
    //https://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client

    public class SendRequestStatusToRedCrossEndpointHandler : AsyncRequestHandler<SendRequestStatusToRedCrossEndpoint>
    {
        protected override async Task HandleCore(SendRequestStatusToRedCrossEndpoint message)
        {
            try
            {
                //TODO: move the Polly code to async
                Policy.Handle<HttpRequestException>()
                    //.WaitAndRetry(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15)})
                    .WaitAndRetry(5, retryCount => TimeSpan.FromSeconds(retryCount * retryCount))
                    .Execute(() =>
                    {
                        using (var httpClient = new HttpClient())
                        {
                            var updateRequestMessage = new { message.Acceptance, message.Status };
                            var json = JsonConvert.SerializeObject(updateRequestMessage);

                            httpClient.BaseAddress = new Uri("http://yourapidomain.com/");
                            httpClient.DefaultRequestHeaders.Add("Authorization", "[TOKEN]");

                            var response = httpClient.PostAsJsonAsync($"api/products/{message.SerialNumber}", json).Result;

                            response.EnsureSuccessStatusCode();    // Throw if not a success code.
                            //if (!response.IsSuccessStatusCode) { throw new Exception("error invoking getasmokealarm's enndpoint"); }
                        }

                    });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"something went very wrong:{ex.Message}");
            }
        }
    }
}