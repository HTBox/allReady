using System.Linq;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Requests
{
    public class RequestExistsByProviderIdQueryHandler : IRequestHandler<RequestExistsByProviderIdQuery, bool>
    {
        private readonly AllReadyContext context;

        public RequestExistsByProviderIdQueryHandler(AllReadyContext context)
        {
            this.context = context;
        }

        public bool Handle(RequestExistsByProviderIdQuery message)
        {
            return context.Requests.Any(x => x.ProviderRequestId == message.ProviderRequestId);
        }
    }
}
