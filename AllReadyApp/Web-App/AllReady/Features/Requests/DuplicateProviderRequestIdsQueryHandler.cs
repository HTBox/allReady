using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Requests
{
    public class DuplicateProviderRequestIdsQueryHandler : IRequestHandler<DuplicateProviderRequestIdsQuery, List<string>>
    {
        private readonly AllReadyContext context;

        public DuplicateProviderRequestIdsQueryHandler(AllReadyContext context)
        {
            this.context = context;
        }

        public List<string> Handle(DuplicateProviderRequestIdsQuery message)
        {
            return context.Requests.Where(x => message.ProviderRequestIds.Contains(x.ProviderRequestId)).Select(x => x.ProviderRequestId).ToList();
        }
    }
}
