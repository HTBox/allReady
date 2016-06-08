using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Security;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Models.ItineraryModels;
using AllReady.Areas.Admin.Models.Validators;
using System.Linq;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class ItineraryController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IItineraryEditModelValidator _itineraryValidator;

        public ItineraryController(IMediator mediator, IItineraryEditModelValidator itineraryValidator)
        {
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }

            if (itineraryValidator == null)
            {
                throw new ArgumentNullException(nameof(itineraryValidator));
            }

            _mediator = mediator;
            _itineraryValidator = itineraryValidator;
        }

        [HttpGet]
        [Route("Admin/Itinerary/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var itinerary = await _mediator.SendAsync(new ItineraryDetailQuery { ItineraryId = id });

            if (itinerary == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(itinerary.OrganizationId))
            {
                return HttpUnauthorized();
            }

            return View("Details", itinerary);
        }

        [HttpPost]
        [Route("Admin/Itinerary/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItineraryEditModel model)
        {
            if (model == null)
            {
                return HttpBadRequest();
            }

            var campaignEvent = await _mediator.SendAsync(new EventSummaryQuery { EventId = model.EventId });

            if (campaignEvent == null)
            {
                return HttpBadRequest();
            }

            if (!User.IsOrganizationAdmin(campaignEvent.OrganizationId))
            {
                return HttpUnauthorized();
            }

            var errors = _itineraryValidator.Validate(model, campaignEvent);

            errors.ToList().ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var result = await _mediator.SendAsync(new EditItineraryCommand() { Itinerary = model });

            if (result > 0)
            {
                return Ok(result);
            }

            return HttpBadRequest();
        }
    }
}
