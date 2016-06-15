using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Models;
using AllReady.Extensions;
using AllReady.Models;
using AllReady.Security;
using AllReady.Services;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using AllReady.Areas.Admin.Models.Validators;
using AllReady.Features.Event;
using AllReady.ViewModels.Event;

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
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(campaignEvent.OrganizationId))
            {
                return HttpUnauthorized();
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
                return HttpUnauthorized();
            }

            var campaignEvent = new EventEditModel
            {
                CampaignId = campaign.Id,
                CampaignName = campaign.Name,
                TimeZoneId = campaign.TimeZoneId,
                OrganizationId = campaign.OrganizationId,
                OrganizationName = campaign.OrganizationName,
                StartDateTime = DateTime.Today.Date,
                EndDateTime = DateTime.Today.Date.AddMonths(1)
            };

            return View("Edit", campaignEvent);
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Event/Create/{campaignId}")]
        public async Task<IActionResult> Create(int campaignId, EventEditModel campaignEvent, IFormFile fileUpload)
        {
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = campaignId });
            if (campaign == null || !User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return HttpUnauthorized();
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
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(campaignEvent.OrganizationId))
            {
                return HttpUnauthorized();
            }

            return View(campaignEvent);
        }

        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventEditModel campaignEvent, IFormFile fileUpload)
        {
            if (campaignEvent == null)
            {
                return HttpBadRequest();
            }
            
            var organizationId = _mediator.Send(new ManagingOrganizationIdByEventIdQuery { EventId = campaignEvent.Id });
            if (!User.IsOrganizationAdmin(organizationId))
            {
                return HttpUnauthorized();
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
                        campaignEvent.ImageUrl = await _imageService.UploadEventImageAsync(campaign.OrganizationId, campaignEvent.Id, fileUpload);
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
                return HttpNotFound();

            if (!User.IsOrganizationAdmin(campaignEvent.OrganizationId))
                return HttpUnauthorized();

            return View(campaignEvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duplicate(DuplicateEventModel model)
        {
            if (model == null)
                return HttpBadRequest();

            var organizationId = _mediator.Send(new ManagingOrganizationIdByEventIdQuery { EventId = model.Id });
            if (!User.IsOrganizationAdmin(organizationId))
                return HttpUnauthorized();

            var existingEvent = await _mediator.SendAsync(new EventEditQuery() { EventId = model.Id });
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = existingEvent.CampaignId });
            var newEvent = buildNewEventDetailsModel(existingEvent, model);

            var errors = _eventDetailModelValidator.Validate(newEvent, campaign);
            errors.ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (ModelState.IsValid)
            {
                var id = await _mediator.SendAsync(new DuplicateEventCommand { DuplicateEventModel = model });

                return RedirectToAction(nameof(Details), new { area = "Admin", id = id });
            }

            return View(model);
        }

        private EventEditModel buildNewEventDetailsModel(EventEditModel existingEvent, DuplicateEventModel newEventDetails)
        {
            existingEvent.Id = 0;
            existingEvent.Name = newEventDetails.Name;
            existingEvent.Description = newEventDetails.Description;
            existingEvent.StartDateTime = newEventDetails.StartDateTime;
            existingEvent.EndDateTime = newEventDetails.EndDateTime;

            return existingEvent;
        }

        // GET: Event/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var campaignEvent = await _mediator.SendAsync(new EventDetailQuery { EventId = id });
            if (campaignEvent == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(campaignEvent.OrganizationId))
            {
                return HttpUnauthorized();
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
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(campaignEvent.OrganizationId))
            {
                return HttpUnauthorized();
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
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(campaignEvent.Campaign.ManagingOrganizationId))
            {
                return HttpUnauthorized();
            }

            var model = new EventViewModel(campaignEvent);
            model.Tasks = model.Tasks.OrderBy(t => t.StartDateTime).ThenBy(t => t.Name).ToList();
            model.Volunteers = campaignEvent.UsersSignedUp.Select(u => u.User).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MessageAllVolunteers(MessageEventVolunteersModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }
            
            //TODO: Query only for the organization Id rather than the whole event detail
            var campaignEvent = await _mediator.SendAsync(new EventDetailQuery { EventId = model.EventId });
            if (campaignEvent == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(campaignEvent.OrganizationId))
            {
                return HttpUnauthorized();
            }

            await _mediator.SendAsync(new MessageEventVolunteersCommand { Model = model });

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

        private Event GetEventBy(int eventId)
        {
            //TODO: refactor message to async when IAllReadyDataAccess read ops are made async
            return _mediator.Send(new EventByIdQuery { EventId = eventId });
        }
    }
}