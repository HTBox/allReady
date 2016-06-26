using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Resource
{
    public class ResourcesByCategoryQueryHandler : IRequestHandler<ResourcesByCategoryQuery, List<Models.Resource>>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public ResourcesByCategoryQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public List<Models.Resource> Handle(ResourcesByCategoryQuery message)
        {
            return dataAccess.GetResourcesByCategory(message.Category).ToList();
        }
    }
}
