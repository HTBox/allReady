using System.Threading.Tasks;
using AllReady.Features.Organizations;
using MediatR;
using System;
using AllReady.Features.Organizations;
using System.Threading.Tasks;

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
            if (id <= 0)
                return HttpBadRequest();

            var model = await _mediator.SendAsync(new OrganizationDetailsQueryAsync { Id = id });

            if (model == null)
                return HttpNotFound();

            return View("Organization", model);
        }

        [Route("Organization/{id}/PrivacyPolicy")]
        public async Task<IActionResult> OrganizationPrivacyPolicy(int id)
        {
            if (id <= 0)
                return HttpBadRequest();

            var model = await _mediator.SendAsync(new OrganziationPrivacyPolicyQueryAsync { Id = id });

            if (model == null || string.IsNullOrEmpty(model.Content))
                return RedirectToAction(nameof(ShowOrganization));

            return View("OrgPrivacyPolicy", model);
        }
    }
}