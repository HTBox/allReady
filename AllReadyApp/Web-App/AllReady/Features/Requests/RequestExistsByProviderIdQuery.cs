using MediatR;

namespace AllReady.Features.Requests
{
    public class RequestExistsByProviderIdQuery : IAsyncRequest<bool>
    {
        public string ProviderRequestId { get; set; }
    }
}
