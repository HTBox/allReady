using MediatR;

namespace AllReady.Features.Requests
{
    public class RequestExistsByProviderIdQuery : IRequest<bool>
    {
        public string ProviderRequestId { get; set; }
    }
}
