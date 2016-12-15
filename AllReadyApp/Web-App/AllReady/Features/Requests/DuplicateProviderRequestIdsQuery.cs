using System.Collections.Generic;
using MediatR;

namespace AllReady.Features.Requests
{
    public class DuplicateProviderRequestIdsQuery : IRequest<List<string>>
    {
        public List<string> ProviderRequestIds { get; set; }
    }
}
