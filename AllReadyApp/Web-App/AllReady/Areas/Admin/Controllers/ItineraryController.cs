using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Areas.Admin.Features.TaskSignups;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Areas.Admin.ViewModels.Request;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
                return NotFound();
            }

            if (!User.IsOrganizationAdmin(itinerary.OrganizationId))
            {
                return Unauthorized();
            }

            return View("Details", itinerary);
        }

        [HttpPost]
        [Route("Admin/Itinerary/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItineraryEditViewModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var campaignEvent = await _mediator.SendAsync(new EventSummaryQuery { EventId = model.EventId });
            if (campaignEvent == null)
            {
                return BadRequest();
            }

            if (!User.IsOrganizationAdmin(campaignEvent.OrganizationId))
            {
                return Unauthorized();
            }

            var errors = _itineraryValidator.Validate(model, campaignEvent);
            errors.ToList().ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.SendAsync(new EditItineraryCommand { Itinerary = model });
            if (result > 0)
            {
                return Ok(result);
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("Admin/Itinerary/AddTeamMember")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTeamMember(int id, int selectedTeamMember)
        {
            // todo: sgordon: This is not a very elegant at the moment as a failure would redirect without any feedback to the user
            // this flow should be reviews and enhanced in a future PR using knockout to send and handle the error messaging on the details page
            // for the purpose of the upcoming red cross testing I chose to leave this since a failure here would be an edge case

            var orgId = await GetOrganizationIdBy(id);
            if (orgId == 0 || !User.IsOrganizationAdmin(orgId))
            {
                return Unauthorized();
            }

            if (id == 0 || selectedTeamMember == 0)
            {
                return RedirectToAction("Details", new { id });
            }

            var isSuccess = await _mediator.SendAsync(new AddTeamMemberCommand { ItineraryId = id, TaskSignupId = selectedTeamMember });

            return RedirectToAction("Details", new { id });
        }

        [HttpGet]
        [Route("Admin/Itinerary/{id}/[Action]")]
        public async Task<IActionResult> SelectRequests(int id)
        {
            var orgId = await GetOrganizationIdBy(id);
            if (orgId == 0 || !User.IsOrganizationAdmin(orgId))
            {
                return Unauthorized();
            }

            var model = await BuildSelectItineraryRequestsModel(id, new RequestSearchCriteria { Status = RequestStatus.Unassigned });

            return View("SelectRequests", model);
        }

        [HttpPost]
        [Route("Admin/Itinerary/{id}/[Action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectRequests(int id, SelectItineraryRequestsViewModel model)
        {
            var newModel = await BuildSelectItineraryRequestsModel(id, new RequestSearchCriteria { Status = RequestStatus.Unassigned, Keywords = model.KeywordsFilter });

            return View("SelectRequests", newModel);
        }

        [HttpPost]
        [Route("Admin/Itinerary/{id}/[Action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRequests(int id, string[] selectedRequests)
        {
            // todo - error handling
            var orgId = await GetOrganizationIdBy(id);
            if (orgId == 0 || !User.IsOrganizationAdmin(orgId))
            {
                return Unauthorized();
            }

            if (selectedRequests.Any())
            {
                await _mediator.SendAsync(new AddRequestsCommand { ItineraryId = id, RequestIdsToAdd = selectedRequests.ToList() });
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{taskSignupId}")]
        public async Task<IActionResult> ConfirmRemoveTeamMember(int itineraryId, int taskSignupId)
        {
            var orgId = await GetOrganizationIdBy(itineraryId);

            if (orgId == 0 || !User.IsOrganizationAdmin(orgId))
            {
                return Unauthorized();
            }

            var model = await _mediator.SendAsync(new TaskSignupSummaryQuery { TaskSignupId = taskSignupId });

            if (model == null)
            {
                return NotFound();
            }

            return View("ConfirmRemoveTeamMember", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{taskSignupId}")]
        public async Task<IActionResult> RemoveTeamMember(int itineraryId, int taskSignupId)
        {
            var orgId = await GetOrganizationIdBy(itineraryId);

            if (orgId == 0 || !User.IsOrganizationAdmin(orgId))
            {
                return Unauthorized();
            }

            var result = await _mediator.SendAsync(new RemoveTeamMemberCommand { TaskSignupId = taskSignupId });

            return RedirectToAction("Details", new { id = itineraryId });
        }

        [HttpGet]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{requestId}")]
        public async Task<IActionResult> ConfirmRemoveRequest(int itineraryId, Guid requestId)
        {
            var orgId = await GetOrganizationIdBy(itineraryId);

            if (orgId == 0 || !User.IsOrganizationAdmin(orgId))
            {
                return Unauthorized();
            }

            var model = await _mediator.SendAsync(new RequestSummaryQuery { RequestId = requestId });

            if (model == null)
            {
                return NotFound();
            }

            return View("ConfirmRemoveRequest", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{requestId}")]
        public async Task<IActionResult> RemoveRequest(int itineraryId, Guid requestId)
        {
            var orgId = await GetOrganizationIdBy(itineraryId);

            if (orgId == 0 || !User.IsOrganizationAdmin(orgId))
            {
                return Unauthorized();
            }

            var result = await _mediator.SendAsync(new RemoveRequestCommand { RequestId = requestId, ItineraryId = itineraryId });

            return RedirectToAction("Details", new { id = itineraryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{requestId}")]
        public async Task<IActionResult> MoveRequestUp(int itineraryId, Guid requestId)
        {
            var orgId = await GetOrganizationIdBy(itineraryId);

            if (orgId == 0 || !User.IsOrganizationAdmin(orgId))
            {
                return Unauthorized();
            }

            var result = await _mediator.SendAsync(new ReorderRequestCommand { RequestId = requestId, ItineraryId = itineraryId, ReOrderDirection = ReorderRequestCommand.Direction.Up });

            return RedirectToAction("Details", new { id = itineraryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{requestId}")]
        public async Task<IActionResult> MoveRequestDown(int itineraryId, Guid requestId)
        {
            var orgId = await GetOrganizationIdBy(itineraryId);

            if (orgId == 0 || !User.IsOrganizationAdmin(orgId))
            {
                return Unauthorized();
            }

            var result = await _mediator.SendAsync(new ReorderRequestCommand { RequestId = requestId, ItineraryId = itineraryId, ReOrderDirection = ReorderRequestCommand.Direction.Down });

            return RedirectToAction("Details", new { id = itineraryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{requestId}")]
        public async Task<IActionResult> MarkComplete(int itineraryId, Guid requestId)
        {
            var orgId = await GetOrganizationIdBy(itineraryId);

            if (orgId == 0 || !User.IsOrganizationAdmin(orgId))
            {
                return Unauthorized();
            }

            // todo: sgordon - Extend this to return success / failure message to the user

            await _mediator.SendAsync(new RequestStatusChangeCommand() { RequestId = requestId, NewStatus = RequestStatus.Completed});

            return RedirectToAction("Details", new { id = itineraryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{requestId}")]
        public async Task<IActionResult> MarkIncomplete(int itineraryId, Guid requestId)
        {
            var orgId = await GetOrganizationIdBy(itineraryId);

            if (orgId == 0 || !User.IsOrganizationAdmin(orgId))
            {
                return Unauthorized();
            }

            // todo: sgordon - Extend this to return success / failure message to the user

            await _mediator.SendAsync(new RequestStatusChangeCommand() { RequestId = requestId, NewStatus = RequestStatus.Assigned });

            return RedirectToAction("Details", new { id = itineraryId });
        }

        private async Task<SelectItineraryRequestsViewModel> BuildSelectItineraryRequestsModel(int itineraryId, RequestSearchCriteria criteria)
        {
            var model = new SelectItineraryRequestsViewModel();

            var itinerary = await _mediator.SendAsync(new ItineraryDetailQuery { ItineraryId = itineraryId });

            model.CampaignId = itinerary.CampaignId;
            model.CampaignName = itinerary.CampaignName;
            model.EventId = itinerary.EventId;
            model.EventName = itinerary.EventName;
            model.ItineraryName = itinerary.Name;

            criteria.EventId = itinerary.EventId;

            var requests = await _mediator.SendAsync(new RequestListItemsQuery { Criteria = criteria });

            foreach (var request in requests)
            {
                var selectItem = new RequestSelectViewModel
                {
                    Id = request.Id,
                    Name = request.Name,
                    DateAdded = request.DateAdded,
                    City = request.City,
                    Address = request.Address,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Postcode = request.Postcode
                };

                model.Requests.Add(selectItem);
            }

            return model;
        }

        private async Task<int> GetOrganizationIdBy(int intinerayId)
        {
            return await _mediator.SendAsync(new OrganizationIdQuery { ItineraryId = intinerayId });
        }
    }
}