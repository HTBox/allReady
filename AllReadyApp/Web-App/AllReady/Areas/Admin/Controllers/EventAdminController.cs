using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Extensions;
using AllReady.Models;
using AllReady.Security;
using AllReady.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AllReady.Areas.Admin.ViewModels.Event;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.Areas.Admin.ViewModels.Request;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class EventController : Controller
    {
        public Func<DateTime> DateTimeTodayDate = () => DateTime.Today.Date;

        private readonly IImageService _imageService;
        private readonly IMediator _mediator;
        private readonly IValidateEventEditViewModels _eventEditViewModelValidator;

        public EventController(IImageService imageService, IMediator mediator, IValidateEventEditViewModels eventEditViewModelValidator)
        {
            _imageService = imageService;
            _mediator = mediator;
            _eventEditViewModelValidator = eventEditViewModelValidator;
        }

        // GET: Event/Details/5
        [HttpGet]
        [Route("Admin/Event/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await _mediator.SendAsync(new EventDetailQuery { EventId = id });
            if (viewModel == null)
            {
                return NotFound();
            }

            if (!User.IsOrganizationAdmin(viewModel.OrganizationId))
            {
                return Unauthorized();
            }

            var url = Url.Action("Details", "Itinerary", new { Area = "Admin", id = 0 }).TrimEnd('0');
            viewModel.ItinerariesDetailsUrl = string.Concat(url, "{id}");

            return View(viewModel);
        }

        // GET: Event/Create
        [Route("Admin/Event/Create/{campaignId}")]
        public async Task<IActionResult> Create(int campaignId)
        {
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = campaignId });
            if (campaign == null || !User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return Unauthorized();
            }

            var viewModel = new EventEditViewModel
            {
                CampaignId = campaign.Id,
                CampaignName = campaign.Name,
                TimeZoneId = campaign.TimeZoneId,
                OrganizationId = campaign.OrganizationId,
                OrganizationName = campaign.OrganizationName,
                StartDateTime = DateTimeTodayDate(),
                EndDateTime = DateTimeTodayDate()
            };

            return View("Edit", viewModel);
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Event/Create/{campaignId}")]
        public async Task<IActionResult> Create(int campaignId, EventEditViewModel eventEditViewModel, IFormFile fileUpload)
        {
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = campaignId });
            if (campaign == null || !User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return Unauthorized();
            }

            var errors = _eventEditViewModelValidator.Validate(eventEditViewModel, campaign);
            errors.ToList().ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            ModelState.Remove("NewItinerary");

            //TryValidateModel is called explictly because of MVC 6 behavior that supresses model state validation during model binding when binding to an IFormFile.
            //See #619.
            if (ModelState.IsValid && TryValidateModel(eventEditViewModel))
            {
                if (fileUpload != null)
                {
                    if (!fileUpload.IsAcceptableImageContentType())
                    {
                        ModelState.AddModelError("ImageUrl", "You must upload a valid image file for the logo (.jpg, .png, .gif)");
                        return View("Edit", eventEditViewModel);
                    }
                }

                eventEditViewModel.OrganizationId = campaign.OrganizationId;
                var id = await _mediator.SendAsync(new EditEventCommand { Event = eventEditViewModel });

                if (fileUpload != null)
                {
                    // resave now that event has the ImageUrl
                    eventEditViewModel.Id = id;
                    eventEditViewModel.ImageUrl = await _imageService.UploadEventImageAsync(campaign.OrganizationId, id, fileUpload);
                    await _mediator.SendAsync(new EditEventCommand { Event = eventEditViewModel });
                }

                return RedirectToAction(nameof(Details), new { area = "Admin", id = id });
            }

            return View("Edit", eventEditViewModel);
        }

        // GET: Event/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var campaignEvent = await _mediator.SendAsync(new EventEditQuery { EventId = id });
            if (campaignEvent == null)
            {
                return NotFound();
            }

            if (!User.IsOrganizationAdmin(campaignEvent.OrganizationId))
            {
                return Unauthorized();
            }

            return View(campaignEvent);
        }

        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventEditViewModel eventEditViewModel, IFormFile fileUpload)
        {
            if (eventEditViewModel == null)
            {
                return BadRequest();
            }

            var organizationId = await _mediator.SendAsync(new ManagingOrganizationIdByEventIdQuery { EventId = eventEditViewModel.Id });
            if (!User.IsOrganizationAdmin(organizationId))
            {
                return Unauthorized();
            }

            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = eventEditViewModel.CampaignId });

            var errors = _eventEditViewModelValidator.Validate(eventEditViewModel, campaign);
            errors.ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (ModelState.IsValid)
            {
                if (fileUpload != null)
                {
                    if (fileUpload.IsAcceptableImageContentType())
                    {
                        var existingImageUrl = eventEditViewModel.ImageUrl;
                        var newImageUrl = await _imageService.UploadEventImageAsync(campaign.OrganizationId, campaign.Id, fileUpload);
                        if (!string.IsNullOrEmpty(newImageUrl))
                        {
                            eventEditViewModel.ImageUrl = newImageUrl;
                            if (existingImageUrl != null)
                            {
                                await _imageService.DeleteImageAsync(existingImageUrl);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ImageUrl", "You must upload a valid image file for the logo (.jpg, .png, .gif)");
                        return View(eventEditViewModel);
                    }
                }

                var id = await _mediator.SendAsync(new EditEventCommand { Event = eventEditViewModel });

                return RedirectToAction(nameof(Details), new { area = "Admin", id = id });
            }

            return View(eventEditViewModel);
        }

        // GET: Event/Duplicate/5
        [HttpGet]
        public async Task<IActionResult> Duplicate(int id)
        {
            var viewModel = await _mediator.SendAsync(new DuplicateEventQuery { EventId = id });
            if (viewModel == null)
            {
                return NotFound();
            }

            if (!User.IsOrganizationAdmin(viewModel.OrganizationId))
            {
                return Unauthorized();
            }

            viewModel.UserIsOrgAdmin = true;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duplicate(DuplicateEventViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest();
            }

            if (!viewModel.UserIsOrgAdmin)
            {
                return Unauthorized();
            }

            var existingEvent = await _mediator.SendAsync(new EventEditQuery { EventId = viewModel.Id });
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = existingEvent.CampaignId });
            var newEvent = BuildNewEventDetailsModel(existingEvent, viewModel);

            //mgmccarthy: why are we validating here? We don't need to validate as the event that is being duplicated was already validated before it was created
            var errors = _eventEditViewModelValidator.Validate(newEvent, campaign);
            errors.ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (ModelState.IsValid)
            {
                var id = await _mediator.SendAsync(new DuplicateEventCommand { DuplicateEventModel = viewModel });
                return RedirectToAction(nameof(Details), new { area = "Admin", id });
            }

            return View(viewModel);
        }

        // GET: Event/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var viewModel = await _mediator.SendAsync(new DeleteQuery { EventId = id });
            if (!User.IsOrganizationAdmin(viewModel.OrganizationId))
            {
                return Unauthorized();
            }

            viewModel.UserIsOrgAdmin = true;
            viewModel.Title = $"Delete event {viewModel.Name}";

            return View(viewModel);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(DeleteViewModel viewModel)
        {
            if (!viewModel.UserIsOrgAdmin)
            {
                return Unauthorized();
            }

            await _mediator.SendAsync(new DeleteEventCommand { EventId = viewModel.Id });

            return RedirectToAction(nameof(CampaignController.Details), "Campaign", new { area = "Admin", id = viewModel.CampaignId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteEventImage(int eventId)
        {
            var campaignEvent = await _mediator.SendAsync(new EventEditQuery { EventId = eventId });

            if (campaignEvent == null)
            {
                return Json(new { status = "NotFound" });
            }

            if (!User.IsOrganizationAdmin(campaignEvent.OrganizationId))
            {
                return Json(new { status = "Unauthorized" });
            }

            if (campaignEvent.ImageUrl != null)
            {
                await _imageService.DeleteImageAsync(campaignEvent.ImageUrl);
                campaignEvent.ImageUrl = null;
                await _mediator.SendAsync(new EditEventCommand { Event = campaignEvent });
                return Json(new { status = "Success" });
            }

            return Json(new { status = "NothingToDelete" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MessageAllVolunteers(MessageEventVolunteersViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var eventsOrganizationId = await _mediator.SendAsync(new OrganizationIdByEventIdQuery { EventId = viewModel.EventId });
            if (!User.IsOrganizationAdmin(eventsOrganizationId))
            {
                return Unauthorized();
            }

            await _mediator.SendAsync(new MessageEventVolunteersCommand { ViewModel = viewModel });

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostEventFile(int id, IFormFile file)
        {
            var organizationId = await _mediator.SendAsync(new OrganizationIdByEventIdQuery { EventId = id });
            var imageUrl = await _imageService.UploadEventImageAsync(id, organizationId, file);

            await _mediator.SendAsync(new UpdateEventImageUrl { EventId = id, ImageUrl = imageUrl });

            return RedirectToRoute(new { controller = "Event", Area = "Admin", action = nameof(Edit), id });
        }

        [HttpGet]
        [Route("Admin/Event/[action]/{id}/{status?}")]
        public async Task<IActionResult> Requests(int id, string status)
        {
            var organizationId = await _mediator.SendAsync(new OrganizationIdByEventIdQuery { EventId = id });
            if (!User.IsOrganizationAdmin(organizationId))
            {
                return Unauthorized();
            }

            var criteria = new RequestSearchCriteria { EventId = id };
            var pageTitle = "All Requests";
            var currentPage = "All";

            if (!string.IsNullOrEmpty(status))
            { 
                RequestStatus requestStatus;
                if (Enum.TryParse(status, out requestStatus))
                {
                    criteria.Status = requestStatus;
                    pageTitle = $"{status} Requests";
                    currentPage = status;
                }
                else
                {
                    return RedirectToAction(nameof(Requests), new {id});
                }
            }

            var viewModel = await _mediator.SendAsync(new EventRequestsQuery { EventId = id });
            viewModel.PageTitle = pageTitle;
            viewModel.CurrentPage = currentPage;

            viewModel.Requests = await _mediator.SendAsync(new RequestListItemsQuery { Criteria = criteria });
 
            return View(viewModel);
        }

        private static EventEditViewModel BuildNewEventDetailsModel(EventEditViewModel existingEvent, DuplicateEventViewModel newEventDetails)
        {
            existingEvent.Id = 0;
            existingEvent.Name = newEventDetails.Name;
            existingEvent.Description = newEventDetails.Description;
            existingEvent.StartDateTime = newEventDetails.StartDateTime;
            existingEvent.EndDateTime = newEventDetails.EndDateTime;
            return existingEvent;
        }
    }
}