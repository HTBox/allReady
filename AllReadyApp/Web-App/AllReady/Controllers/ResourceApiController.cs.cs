using System.Collections.Generic;
using System.Linq;
using AllReady.Features.Resource;
using AllReady.ViewModels.Resource;
using MediatR;
using Microsoft.AspNet.Mvc;

namespace AllReady.Controllers
{
    [Route("api/resource")]
    [Produces("application/json")]
    public class ResourceApiController : Controller
    {
        private readonly IMediator mediator;

        public ResourceApiController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Route("search")]
        public IEnumerable<ResourceViewModel> GetResourcesByCategory(string category)
        {
            var resources = mediator.Send(new ResourcesByCategoryQuery { Category = category });
            return resources.Select(resource => new ResourceViewModel(resource)).ToList();
        }
    }
}
