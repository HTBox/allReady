using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Resource
{
    public class ResourcesByCategoryQueryHandler : IRequestHandler<ResourcesByCategoryQuery, List<Models.Resource>>
    {
        private readonly AllReadyContext dataContext;

        public ResourcesByCategoryQueryHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public List<Models.Resource> Handle(ResourcesByCategoryQuery message)
        {
            return dataContext.Resources.Where(x => x.CategoryTag == message.Category).ToList();
        }
    }
}
