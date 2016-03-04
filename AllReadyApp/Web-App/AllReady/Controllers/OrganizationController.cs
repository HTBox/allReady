using System.Threading.Tasks;
using AllReady.Features.Organizations;
using MediatR;
using Microsoft.AspNet.Mvc;

namespace AllReady.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly IMediator _mediator;

        public OrganizationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("Organizations/")]
        public IActionResult Index()
        {
            return View(_mediator.Send(new OrganizationsQuery()));
        }

        [Route("Organization/{id}/")]
        public async Task<IActionResult> ShowOrganization(int id)
        {
            var model = await _mediator.SendAsync(new OrganizationDetailsQueryAsync { Id = id });

            if (model == null)
                return HttpNotFound();

            return View("Organization", model);
        }
    }
}