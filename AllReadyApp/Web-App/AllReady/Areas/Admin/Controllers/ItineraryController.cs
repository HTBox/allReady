using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Areas.Admin.Features.TaskSignups;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Areas.Admin.ViewModels.Request;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.Caching;
using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Constants;

namespace AllReady.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    [Authorize]
    public class ItineraryController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IItineraryEditModelValidator _itineraryValidator;
        private readonly UserManager<ApplicationUser> _userManager;

        public ItineraryController(IMediator mediator, IItineraryEditModelValidator itineraryValidator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _itineraryValidator = itineraryValidator;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("Admin/Itinerary/Details/{id}")]
        public async Task<IActionResult> Details(int id, bool? teamLeadSuccess = null)
        {
            var itinerary = await _mediator.SendAsync(new ItineraryDetailQuery { ItineraryId = id });
            if (itinerary == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);

            itinerary.OptimizeRouteStatus = _mediator.Send(new OptimizeRouteResultStatusQuery(user?.Id, id));

            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(id));
            if (!await authorizableItinerary.UserCanView())
            {
                return new ForbidResult();
            }

            itinerary.TeamLeadChangedSuccess = teamLeadSuccess;
            itinerary.TeamManagementEnabled = await authorizableItinerary.UserCanManageTeamMembers();

            return View("Details", itinerary);
        }

        [HttpPost]
        [Route("Admin/Itinerary/Details/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id, string requestKeywords, RequestStatus? requestStatus)
        {
            var itinerary = await _mediator.SendAsync(new ItineraryDetailQuery { ItineraryId = id });
            if (itinerary == null)
            {
                return NotFound();
            }

            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(id));
            if (!await authorizableItinerary.UserCanEdit())
            {
                return new ForbidResult();
            }

            var filteredRequests = await _mediator.SendAsync(new RequestListItemsQuery
            {
                Criteria = new RequestSearchCriteria
                {
                    ItineraryId = itinerary.Id,
                    Keywords = requestKeywords,
                    Status = requestStatus
                }
            });

            itinerary.Requests = filteredRequests;

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

            var authorizableEvent = await _mediator.SendAsync(new AuthorizableEventQuery(campaignEvent.Id));
            if (!await authorizableEvent.UserCanEdit())
            {
                return new ForbidResult();
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

        [Route("Admin/Itinerary/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var itinerary = await _mediator.SendAsync(new EditItineraryQuery { ItineraryId = id });
            if (itinerary == null)
            {
                return BadRequest();
            }

            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(id));
            if (!await authorizableItinerary.UserCanView())
            {
                return new ForbidResult();
            }

            ViewBag.Title = "Edit " + itinerary.Name;
            return View(itinerary);
        }

        [HttpPost]
        [Route("Admin/Itinerary/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ItineraryEditViewModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var itinerary = await _mediator.SendAsync(new ItinerarySummaryQuery() { ItineraryId = model.Id });
            if (itinerary == null)
            {
                return BadRequest();
            }

            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(model.Id));
            if (!await authorizableItinerary.UserCanEdit())
            {
                return new ForbidResult();
            }

            model.Date = itinerary.Date; // we currently don't allow the date to be changed via edit

            // todo - sgordon: add additional validation for address scenarios - enhance this later
            var errors = _itineraryValidator.Validate(model, itinerary.EventSummary);
            errors.ToList().ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _mediator.SendAsync(new EditItineraryCommand { Itinerary = model });

            return RedirectToAction(nameof(Details), new { area = AreaNames.Admin, id = model.Id });
        }

        [HttpPost]
        [Route("Admin/Itinerary/AddTeamMember")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTeamMember(int itineraryId, int selectedTeamMember)
        {
            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(itineraryId));
            if (!await authorizableItinerary.UserCanManageTeamMembers())
            {
                return new ForbidResult();
            }

            var result = await _mediator.SendAsync(new AddTeamMemberCommand { ItineraryId = itineraryId, VolunteerTaskSignupId = selectedTeamMember });

            // TODO - see comment in RemoveTeamLead method regarding passing message between requests

            return RedirectToAction(nameof(Details), new { id = itineraryId });
        }

        [HttpGet]
        [Route("Admin/Itinerary/{id}/[Action]")]
        public async Task<IActionResult> SelectRequests(int id)
        {
            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(id));
            if (!await authorizableItinerary.UserCanView())
            {
                return new ForbidResult();
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
        public async Task<IActionResult> AddRequests(int id, List<string> selectedRequests)
        {
            // todo - error handling
            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(id));
            if (!await authorizableItinerary.UserCanManageRequests())
            {
                return new ForbidResult();
            }

            if (selectedRequests.Any())
            {
                await _mediator.SendAsync(new AddRequestsToItineraryCommand { ItineraryId = id, RequestIdsToAdd = selectedRequests });
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{volunteerTaskSignupId}")]
        public async Task<IActionResult> ConfirmRemoveTeamMember(int itineraryId, int volunteerTaskSignupId)
        {
            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(itineraryId));
            if (!await authorizableItinerary.UserCanView())
            {
                return new ForbidResult();
            }

            var viewModel = await _mediator.SendAsync(new VolunteerTaskSignupSummaryQuery { VolunteerTaskSignupId = volunteerTaskSignupId });
            if (viewModel == null)
            {
                return NotFound();
            }

            viewModel.ItineraryId = itineraryId;
            viewModel.Title = $"Remove team member: {viewModel.VolunteerName}";
            viewModel.UserIsOrgAdmin = true;

            return View("ConfirmRemoveTeamMember", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{taskSignupId}")]
        public async Task<IActionResult> RemoveTeamMember(VolunteerTaskSignupSummaryViewModel viewModel)
        {
            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(viewModel.ItineraryId));
            if (!await authorizableItinerary.UserCanManageTeamMembers())
            {
                return new ForbidResult();
            }

            await _mediator.SendAsync(new RemoveTeamMemberCommand { VolunteerTaskSignupId = viewModel.VolunteerTaskSignupId });

            return RedirectToAction(nameof(Details), new { id = viewModel.ItineraryId });
        }

        [HttpGet]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{requestId}")]
        public async Task<IActionResult> ConfirmRemoveRequest(int itineraryId, Guid requestId)
        {
            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(itineraryId));
            if (!await authorizableItinerary.UserCanView())
            {
                return new ForbidResult();
            }

            var viewModel = await _mediator.SendAsync(new RequestSummaryQuery { RequestId = requestId });
            if (viewModel == null)
            {
                return NotFound();
            }

            viewModel.Title = $"Remove request: {viewModel.Name}";
            viewModel.UserIsOrgAdmin = true;

            return View("ConfirmRemoveRequest", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{requestId}")]
        public async Task<IActionResult> RemoveRequest(RequestSummaryViewModel viewModel)
        {
            if (!viewModel.UserIsOrgAdmin)
            {
                return new ForbidResult();
            }

            await _mediator.SendAsync(new RemoveRequestCommand { RequestId = viewModel.Id, ItineraryId = viewModel.ItineraryId });

            return RedirectToAction(nameof(Details), new { id = viewModel.ItineraryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{requestId}")]
        public async Task<IActionResult> MoveRequestUp(int itineraryId, Guid requestId)
        {
            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(itineraryId));
            if (!await authorizableItinerary.UserCanManageRequests())
            {
                return new ForbidResult();
            }

            await _mediator.SendAsync(new ReorderRequestCommand { RequestId = requestId, ItineraryId = itineraryId, ReOrderDirection = ReorderRequestCommand.Direction.Up });

            return RedirectToAction(nameof(Details), new { id = itineraryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{requestId}")]
        public async Task<IActionResult> MoveRequestDown(int itineraryId, Guid requestId)
        {
            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(itineraryId));
            if (!await authorizableItinerary.UserCanManageRequests())
            {
                return new ForbidResult();
            }

            await _mediator.SendAsync(new ReorderRequestCommand { RequestId = requestId, ItineraryId = itineraryId, ReOrderDirection = ReorderRequestCommand.Direction.Down });

            return RedirectToAction(nameof(Details), new { id = itineraryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{requestId}")]
        public async Task<IActionResult> MarkComplete(int itineraryId, Guid requestId)
        {
            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(itineraryId));
            if (!await authorizableItinerary.UserCanManageRequests())
            {
                return new ForbidResult();
            }

            await _mediator.SendAsync(new ChangeRequestStatusCommand { RequestId = requestId, NewStatus = RequestStatus.Completed });

            return RedirectToAction(nameof(Details), new { id = itineraryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{requestId}")]
        public async Task<IActionResult> MarkIncomplete(int itineraryId, Guid requestId)
        {
            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(itineraryId));
            if (!await authorizableItinerary.UserCanManageRequests())
            {
                return new ForbidResult();
            }

            await _mediator.SendAsync(new ChangeRequestStatusCommand { RequestId = requestId, NewStatus = RequestStatus.Assigned });

            return RedirectToAction(nameof(Details), new { id = itineraryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{requestId}")]
        public async Task<IActionResult> MarkConfirmed(int itineraryId, Guid requestId)
        {
            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(itineraryId));
            if (!await authorizableItinerary.UserCanManageRequests())
            {
                return new ForbidResult();
            }

            await _mediator.SendAsync(new ChangeRequestStatusCommand { RequestId = requestId, NewStatus = RequestStatus.Confirmed });
            return RedirectToAction(nameof(Details), new { id = itineraryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]/{requestId}")]
        public async Task<IActionResult> MarkUnassigned(int itineraryId, Guid requestId)
        {
            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(itineraryId));
            if (!await authorizableItinerary.UserCanManageRequests())
            {
                return new ForbidResult();
            }

            await _mediator.SendAsync(new ChangeRequestStatusCommand { RequestId = requestId, NewStatus = RequestStatus.Unassigned });
            return RedirectToAction(nameof(Details), new { id = itineraryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]")]
        public async Task<IActionResult> OptimizeRoute(int itineraryId, OptimizeRouteInputModel model)
        {
            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(itineraryId));
            if (!await authorizableItinerary.UserCanEdit())
            {
                return new ForbidResult();
            }

            var user = await _userManager.GetUserAsync(User);

            await _mediator.SendAsync(new OptimizeRouteCommand { ItineraryId = itineraryId, UserId = user?.Id });

            return RedirectToAction(nameof(Details), new { id = itineraryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]")]
        public async Task<IActionResult> SetTeamLead(int itineraryId, [FromForm] int volunteerTaskSignupId)
        {
           
            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(itineraryId));
            if (!await authorizableItinerary.UserCanManageTeamMembers())
            {
                return new ForbidResult();
            }

            var itinUrl = Url.Action(nameof(this.Details), "Itinerary", new { area = AreaNames.Admin, id = itineraryId },
                HttpContext.Request.Scheme);

            var result = await _mediator.SendAsync(new SetTeamLeadCommand(itineraryId, volunteerTaskSignupId, itinUrl));

            // todo - this method of sending messages between requests is not great - should look at properly implementing session and using TempData where possible

            var isSuccess = result == SetTeamLeadResult.Success;

            return RedirectToAction(nameof(Details), new { id = itineraryId, teamLeadSuccess = isSuccess });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Itinerary/{itineraryId}/[Action]")]
        public async Task<IActionResult> RemoveTeamLead(int itineraryId)
        {
            var authorizableItinerary = await _mediator.SendAsync(new AuthorizableItineraryQuery(itineraryId));
            if (!await authorizableItinerary.UserCanManageTeamMembers())
            {
                return new ForbidResult();
            }

            var result = await _mediator.SendAsync(new RemoveTeamLeadCommand(itineraryId));

            // todo - this method of sending messages between requests is not great - should look at properly implementing session and using TempData where possible

            return RedirectToAction(nameof(Details), new { id = itineraryId });
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
                    PostalCode = request.PostalCode
                };

                model.Requests.Add(selectItem);
            }

            return model;
        }
    }
}
