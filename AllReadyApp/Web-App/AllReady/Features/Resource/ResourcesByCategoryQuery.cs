using System.Collections.Generic;
using MediatR;

namespace AllReady.Features.Resource
{
    public class ResourcesByCategoryQuery : IRequest<List<Models.Resource>>
    {
        public string Category { get; set; }
    }
}
