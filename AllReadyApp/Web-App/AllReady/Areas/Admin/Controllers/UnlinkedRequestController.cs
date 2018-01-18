using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Features.UnlinkedRequests;
using AllReady.Areas.Admin.ViewModels.UnlinkedRequests;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.Constants;
using AllReady.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AllReady.Models;

namespace AllReady.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    [Authorize(nameof(UserType.OrgAdmin))]
    public class UnlinkedRequestController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUnlinkedRequestViewModelValidator _modelValidator;

        public UnlinkedRequestController(IMediator mediator, IUnlinkedRequestViewModelValidator modelValidator)
        {
            _mediator = mediator;
            _modelValidator = modelValidator;
        }

        // GET: Admin/UnlinkedRequest/List
        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> List()
        {
            if (!User.IsOrganizationAdmin())
            {
                return Unauthorized();
            }

            var orgId = User.GetOrganizationId();

            return View(await _mediator.SendAsync(new UnlinkedRequestListQuery()
            {
                OrganizationId = orgId.GetValueOrDefault()
            }));
        }

        [HttpPost]
        [ActionName("AddRequests")]
        public async Task<IActionResult> AddRequests(UnlinkedRequestViewModel model)
        {
            var organizationId = User.GetOrganizationId();
            if (!organizationId.HasValue || !User.IsOrganizationAdmin(organizationId.Value))
            {
                return Unauthorized();
            }
            
            var errors = _modelValidator.Validate(model);
            errors.ToList().ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (ModelState.IsValid)
            {
                var eventSummary = await _mediator.SendAsync(new EventSummaryQuery()
                {
                    EventId = model.EventId
                });

                if (eventSummary == null || eventSummary.OrganizationId != organizationId.Value)
                {
                    return Unauthorized();
                }

                await _mediator.SendAsync(new AddRequestsToEventCommand
                {
                    EventId = model.EventId,
                    SelectedRequestIds = model.Requests.Where(req => req.IsSelected).Select(req => req.Id).ToList()
                });
                return RedirectToAction(nameof(List));
            }

            var orgId = User.GetOrganizationId();
            var queryResponse = await _mediator.SendAsync(new UnlinkedRequestListQuery()
            {
                OrganizationId = orgId.GetValueOrDefault()
            });

            model.Requests = queryResponse.Requests;        
            model.Events = queryResponse.Events;

            return View(nameof(List), model);
        }
    }
}