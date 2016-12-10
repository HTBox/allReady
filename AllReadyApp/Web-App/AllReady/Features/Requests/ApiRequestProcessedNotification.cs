using MediatR;

namespace AllReady.Features.Requests
{
    public class ApiRequestProcessedNotification : INotification
    {
        public string ProviderRequestId { get; set; }
    }
}
