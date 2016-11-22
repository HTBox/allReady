using MediatR;

namespace AllReady.Features.Requests
{
    public class RequestExistsByProviderIdQuery : IRequest<bool>
    {
        public string RequestProviderId { get; set; }
    }
}
