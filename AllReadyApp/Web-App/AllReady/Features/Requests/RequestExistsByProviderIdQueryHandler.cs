using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Requests
{
    public class RequestExistsByProviderIdQueryHandler : IAsyncRequestHandler<RequestExistsByProviderIdQuery, bool>
    {
        private readonly AllReadyContext context;

        public RequestExistsByProviderIdQueryHandler(AllReadyContext context)
        {
            this.context = context;
        }

        public async Task<bool> Handle(RequestExistsByProviderIdQuery message)
        {
            return await context.Requests.AnyAsync(x => x.ProviderRequestId == message.ProviderRequestId);
        }
    }
}
