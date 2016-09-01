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
using AllReady.Features.Event;
using AllReady.ViewModels.Event;
using AllReady.Areas.Admin.ViewModels.Request;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class EventController : Controller
    {
        private readonly IImageService _imageService;
        private readonly IMediator _mediator;
        private readonly IValidateEventDetailModels _eventDetailModelValidator;

        public EventController(IImageService imageService, IMediator mediator, IValidateEventDetailModels eventDetailModelValidator)
        {
            _imageService = imageService;
            _mediator = mediator;
            _eventDetailModelValidator = eventDetailModelValidator;
        }

        // GET: Event/Details/5
        [HttpGet]
        [Route("Admin/Event/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var campaignEvent = await _mediator.SendAsync(new EventDetailQuery { EventId = id });
            if (campaignEvent == null)
            {
                return NotFound();
            }

            if (!User.IsOrganizationAdmin(campaignEvent.OrganizationId))
            {
                return Unauthorized();
            }

            campaignEvent.ItinerariesDetailsUrl = GenerateItineraryDetailsTemplateUrl();

            return View(campaignEvent);
        }

        private string GenerateItineraryDetailsTemplateUrl()
        {
            var url = Url.Action("Details", "Itinerary", new { Area = "Admin", id = 0 }).TrimEnd('0');

            return string.Concat(url, "{id}");
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

            var campaignEvent = new EventEditViewModel
            {
                CampaignId = campaign.Id,
                CampaignName = campaign.Name,
                TimeZoneId = campaign.TimeZoneId,
                OrganizationId = campaign.OrganizationId,
                OrganizationName = campaign.OrganizationName,
                StartDateTime = DateTime.Today.Date,
                EndDateTime = DateTime.Today.Date
            };

            return View("Edit", campaignEvent);
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Event/Create/{campaignId}")]
        public async Task<IActionResult> Create(int campaignId, EventEditViewModel campaignEvent, IFormFile fileUpload)
        {
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = campaignId });
            if (campaign == null || !User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return Unauthorized();
            }

            var errors = _eventDetailModelValidator.Validate(campaignEvent, campaign);
            errors.ToList().ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            ModelState.Remove("NewItinerary");

            //TryValidateModel is called explictly because of MVC 6 behavior that supresses model state validation during model binding when binding to an IFormFile.
            //See #619.
            if (ModelState.IsValid && TryValidateModel(campaignEvent))
            {
                if (fileUpload != null)
                {
                    if (!fileUpload.IsAcceptableImageContentType())
                    {
                        ModelState.AddModelError("ImageUrl", "You must upload a valid image file for the logo (.jpg, .png, .gif)");
                        return View("Edit", campaignEvent);
                    }
                }

                campaignEvent.OrganizationId = campaign.OrganizationId;
                var id = await _mediator.SendAsync(new EditEventCommand { Event = campaignEvent });

                if (fileUpload != null)
                {
                    // resave now that event has the ImageUrl
                    campaignEvent.Id = id;
                    campaignEvent.ImageUrl = await _imageService.UploadEventImageAsync(campaign.OrganizationId, id, fileUpload);
                    await _mediator.SendAsync(new EditEventCommand { Event = campaignEvent });
                }

                return RedirectToAction(nameof(Details), new { area = "Admin", id = id });
            }

            return View("Edit", campaignEvent);
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
        public async Task<IActionResult> Edit(EventEditViewModel campaignEvent, IFormFile fileUpload)
        {
            if (campaignEvent == null)
            {
                return BadRequest();
            }

            var organizationId = await _mediator.SendAsync(new ManagingOrganizationIdByEventIdQuery { EventId = campaignEvent.Id });
            if (!User.IsOrganizationAdmin(organizationId))
            {
                return Unauthorized();
            }

            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = campaignEvent.CampaignId });

            var errors = _eventDetailModelValidator.Validate(campaignEvent, campaign);
            errors.ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (ModelState.IsValid)
            {
                if (fileUpload != null)
                {
                    if (fileUpload.IsAcceptableImageContentType())
                    {
                        var existingImageUrl = campaignEvent.ImageUrl;
                        campaignEvent.ImageUrl = await _imageService.UploadEventImageAsync(campaign.OrganizationId, campaignEvent.Id, fileUpload);
                        if (campaignEvent.ImageUrl != null && existingImageUrl != null)
                        {
                            await _imageService.DeleteImageAsync(existingImageUrl);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ImageUrl", "You must upload a valid image file for the logo (.jpg, .png, .gif)");
                        return View(campaignEvent);
                    }
                }

                var id = await _mediator.SendAsync(new EditEventCommand { Event = campaignEvent });

                return RedirectToAction(nameof(Details), new { area = "Admin", id = id });
            }

            return View(campaignEvent);
        }

        // GET: Event/Duplicate/5
        [HttpGet]
        public async Task<IActionResult> Duplicate(int id)
        {
            var campaignEvent = await _mediator.SendAsync(new DuplicateEventQuery { EventId = id });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duplicate(DuplicateEventViewModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }
        
            var organizationId = await _mediator.SendAsync(new ManagingOrganizationIdByEventIdQuery { EventId = model.Id });
            if (!User.IsOrganizationAdmin(organizationId))
                return Unauthorized();

            var existingEvent = await _mediator.SendAsync(new EventEditQuery { EventId = model.Id });
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = existingEvent.CampaignId });
            var newEvent = BuildNewEventDetailsModel(existingEvent, model);

            var errors = _eventDetailModelValidator.Validate(newEvent, campaign);
            errors.ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (ModelState.IsValid)
            {
                var id = await _mediator.SendAsync(new DuplicateEventCommand { DuplicateEventModel = model });
                return RedirectToAction(nameof(Details), new { area = "Admin", id });
            }

            return View(model);
        }

        // GET: Event/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var campaignEvent = await _mediator.SendAsync(new EventDetailQuery { EventId = id });
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

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //TODO: Should be using an EventSummaryQuery here
            var campaignEvent = await _mediator.SendAsync(new EventDetailQuery { EventId = id });
            if (campaignEvent == null)
            {
                return NotFound();
            }

            if (!User.IsOrganizationAdmin(campaignEvent.OrganizationId))
            {
                return Unauthorized();
            }

            await _mediator.SendAsync(new DeleteEventCommand { EventId = id });

            return RedirectToAction(nameof(CampaignController.Details), "Campaign", new { area = "Admin", id = campaignEvent.CampaignId });
        }

        [HttpGet]
        public IActionResult Assign(int id)
        {
            var campaignEvent = GetEventBy(id);
            if (campaignEvent == null)
            {
                return NotFound();
            }

            if (!User.IsOrganizationAdmin(campaignEvent.Campaign.ManagingOrganizationId))
            {
                return Unauthorized();
            }

            var model = new EventViewModel(campaignEvent);
            model.Tasks = model.Tasks.OrderBy(t => t.StartDateTime).ThenBy(t => t.Name).ToList();
            model.Volunteers = campaignEvent.UsersSignedUp.Select(u => u.User).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MessageAllVolunteers(MessageEventVolunteersViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //TODO: Query only for the organization Id rather than the whole event detail
            var campaignEvent = await _mediator.SendAsync(new EventDetailQuery { EventId = viewModel.EventId });
            if (campaignEvent == null)
            {
                return NotFound();
            }

            if (!User.IsOrganizationAdmin(campaignEvent.OrganizationId))
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
            var campaignEvent = GetEventBy(id);

            campaignEvent.ImageUrl = await _imageService.UploadEventImageAsync(campaignEvent.Id, campaignEvent.Campaign.ManagingOrganizationId, file);
            await _mediator.SendAsync(new UpdateEvent { Event = campaignEvent });

            return RedirectToRoute(new { controller = "Event", Area = "Admin", action = nameof(Edit), id = id });
        }

        [HttpGet]
        [Route("Admin/Event/[action]/{id}/{status?}")]
        public async Task<IActionResult> Requests(int id, string status)
        {
            var campaignEvent = GetEventBy(id);
            if (campaignEvent == null)
            {
                return NotFound();
            }

            if (!User.IsOrganizationAdmin(campaignEvent.Campaign.ManagingOrganizationId))
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

            var model = await _mediator.SendAsync(new EventRequestsQuery { EventId = id });

            model.PageTitle = pageTitle;
            model.CurrentPage = currentPage;

            model.Requests = await _mediator.SendAsync(new RequestListItemsQuery {Criteria = criteria});
 
            return View(model);
        }

        private Event GetEventBy(int eventId)
        {
            //TODO: refactor message to async when IAllReadyDataAccess read ops are made async
            return _mediator.Send(new EventByIdQuery { EventId = eventId });
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