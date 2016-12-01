using MediatR;

namespace AllReady.Features.Requests
{
    public class ApiRequestProcessedNotification : IAsyncNotification
    {
        public string ProviderRequestId { get; set; }
    }
}
