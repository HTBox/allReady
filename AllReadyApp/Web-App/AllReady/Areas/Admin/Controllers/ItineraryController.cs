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
using AllReady.Areas.Admin.Models.RequestModels;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Features.TaskSignups;

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

            var result = await _mediator.SendAsync(new EditItineraryCommand { Itinerary = model });

            if (result > 0)
            {
                return Ok(result);
            }

            return HttpBadRequest();
        }

        [HttpPost]
        [Route("Admin/Itinerary/AddTeamMember")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTeamMember(int id, int selectedTeamMember)
        {
            // todo: sgordon: This is not a very elegant at the moment as a failure would redirect without any feedback to the user
            // this flow should be reviews and enhanced in a future PR using knockout to send and handle the error messaging on the details page
            // for the purpose of the upcoming red cross testing I chose to leave this since a failure here would be an edge case

            var orgId = await _mediator.SendAsync(new OrganizationIdQuery { ItineraryId = id });

            if(orgId == 0 || !User.IsOrganizationAdmin(orgId))
            {
                return HttpUnauthorized();
            }

            if (id == 0 || selectedTeamMember == 0)
            {
                return RedirectToAction("Details", new { id = id });
            }

            var isSuccess = await _mediator.SendAsync(new AddTeamMemberCommand { ItineraryId = id, TaskSignupId = selectedTeamMember });

            return RedirectToAction("Details", new { id = id });
        }

        [HttpGet]
        [Route("Admin/Itinerary/{id}/[Action]")]
        public async Task<IActionResult> SelectRequests(int id)
        {
            var orgId = await _mediator.SendAsync(new OrganizationIdQuery { ItineraryId = id });

            if (orgId == 0 || !User.IsOrganizationAdmin(orgId))
            {
                return HttpUnauthorized();
            }

            var model = new SelectItineraryRequestsModel();

            var itinerary = await _mediator.SendAsync(new ItineraryDetailQuery { ItineraryId = id });

            model.CampaignId = itinerary.CampaignId;
            model.CampaignName = itinerary.CampaignName;
            model.EventId = itinerary.EventId;
            model.EventName = itinerary.EventName;
            model.ItineraryName = itinerary.Name;

            var requests =  await _mediator.SendAsync(new RequestListItemsQuery { criteria = new RequestSearchCriteria() });

            foreach(var request in requests)
            {
                var selectItem = new RequestSelectModel
                {
                    Id = request.Id,
                    Name = request.Name,
                };

                model.Requests.Add(selectItem);
            }
            
            return View("SelectRequests", model);
        }

        [HttpPost]
        [Route("Admin/Itinerary/{id}/[Action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRequests(int id, string[] selectedRequests)
        {
            // todo - error handling

            var orgId = await _mediator.SendAsync(new OrganizationIdQuery { ItineraryId = id });

            if (orgId == 0 || !User.IsOrganizationAdmin(orgId))
            {
                return HttpUnauthorized();
            }

            if (selectedRequests.Any())
            { 
                var result = await _mediator.SendAsync(new AddRequestsCommand { ItineraryId = id, RequestIdsToAdd = selectedRequests.ToList() });
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpGet]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{taskSignupId}")]
        public async Task<IActionResult> ConfirmRemoveTeamMember(int itineraryId, int taskSignupId)
        {
            var orgId = await _mediator.SendAsync(new OrganizationIdQuery { ItineraryId = itineraryId });

            if (orgId == 0 || !User.IsOrganizationAdmin(orgId))
            {
                return HttpUnauthorized();
            }

            var model = await _mediator.SendAsync(new TaskSignupSummaryQuery {TaskSignupId = taskSignupId});

            if (model == null)
            {
                return HttpNotFound();
            }

            return View("ConfirmRemoveTeamMember", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{taskSignupId}")]
        public async Task<IActionResult> RemoveTeamMember(int itineraryId, int taskSignupId)
        {
            var orgId = await _mediator.SendAsync(new OrganizationIdQuery { ItineraryId = itineraryId });

            if (orgId == 0 || !User.IsOrganizationAdmin(orgId))
            {
                return HttpUnauthorized();
            }

            var result = await _mediator.SendAsync(new RemoveTeamMemberCommand { TaskSignupId = taskSignupId });

            return RedirectToAction("Details", new {id = itineraryId });
        }
    }
}
