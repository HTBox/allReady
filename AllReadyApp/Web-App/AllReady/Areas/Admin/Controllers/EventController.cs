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
using AllReady.Constants;
using DeleteQuery = AllReady.Areas.Admin.Features.Events.DeleteQuery;

namespace AllReady.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    [Authorize]
    public class EventController : Controller
    {
        public Func<DateTime> DateTimeTodayDate = () => DateTime.Today.Date;

        private readonly IImageService _imageService;
        private readonly IMediator _mediator;
        private readonly IValidateEventEditViewModels _eventEditViewModelValidator;
        private readonly IUserAuthorizationService _userAuthorizationService;
        private readonly IImageSizeValidator _imageSizeValidator;

        public EventController(IImageService imageService,
            IMediator mediator,
            IValidateEventEditViewModels eventEditViewModelValidator,
            IUserAuthorizationService userAuthorizationService,
            IImageSizeValidator imageSizeValidator)
        {
            _imageService = imageService;
            _mediator = mediator;
            _eventEditViewModelValidator = eventEditViewModelValidator;
            _userAuthorizationService = userAuthorizationService;
            _imageSizeValidator = imageSizeValidator;
        }

        [HttpGet]
        [Route("Admin/Event/ListAll")]
        public async Task<IActionResult> Lister()
        {
            var viewModel =
                await _mediator.SendAsync(new EventListerQuery { UserId = _userAuthorizationService.AssociatedUserId });

            if (viewModel.Count == 1 && viewModel.First().Events.Count == 1)
            {
                return Redirect(Url.Action("Details", "Event", new { id = viewModel.First().Events.First().EventId }));
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("Admin/Event/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await _mediator.SendAsync(new EventDetailQuery { EventId = id });

            if (viewModel == null)
            {
                return NotFound();
            }

            var authorizableEvent = await _mediator.SendAsync(new AuthorizableEventQuery(viewModel.Id, viewModel.CampaignId, viewModel.OrganizationId));

            if (!await authorizableEvent.UserCanView())
            {
                return new ForbidResult();
            }

            // todo - check if the user can duplicate (e.g. create events) against the campaign as well - depends on having an authorizable campaign class first

            if (await authorizableEvent.UserCanDelete())
            {
                viewModel.ShowDeleteButton = true;
            }

            if (await authorizableEvent.UserCanManageChildObjects())
            {
                viewModel.ShowCreateChildObjectButtons = true;
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("Admin/Event/Create/{campaignId}")]
        public async Task<IActionResult> Create(int campaignId)
        {
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = campaignId });

            var authorizableCampaign = await _mediator.SendAsync(new AuthorizableCampaignQuery(campaign.Id, campaign.OrganizationId));
            if (!await authorizableCampaign.UserCanManageChildObjects())
            {
                return new ForbidResult();
            }

            var viewModel = new EventEditViewModel
            {
                CampaignId = campaign.Id,
                CampaignName = campaign.Name,
                TimeZoneId = campaign.TimeZoneId,
                OrganizationId = campaign.OrganizationId,
                OrganizationName = campaign.OrganizationName,
                StartDateTime = DateTimeTodayDate(),
                EndDateTime = DateTimeTodayDate(),
                CampaignStartDateTime = campaign.StartDate,
                CampaignEndDateTime = campaign.EndDate
            };

            return View("Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Event/Create/{campaignId}")]
        public async Task<IActionResult> Create(int campaignId, EventEditViewModel eventEditViewModel, IFormFile fileUpload)
        {
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = campaignId });

            var authorizableCampaign = await _mediator.SendAsync(new AuthorizableCampaignQuery(campaign.Id, campaign.OrganizationId));
            if (!await authorizableCampaign.UserCanManageChildObjects())
            {
                return new ForbidResult();
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
                        ModelState.AddModelError(nameof(eventEditViewModel.ImageUrl), "You must upload a valid image file for the logo (.jpg, .png, .gif)");
                        return View("Edit", eventEditViewModel);
                    }


                    if (_imageSizeValidator != null && fileUpload.Length > _imageSizeValidator.FileSizeInBytes)
                    {
                        ModelState.AddModelError(nameof(eventEditViewModel.ImageUrl), $"File size must be less than {_imageSizeValidator.BytesToMb():#,##0.00}MB!");
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

                return RedirectToAction(nameof(Details), new { area = AreaNames.Admin, id = id });
            }

            return View("Edit", eventEditViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var campaignEvent = await _mediator.SendAsync(new EventEditQuery { EventId = id });
            if (campaignEvent == null)
            {
                return NotFound();
            }

            var authorizableEvent = await _mediator.SendAsync(new AuthorizableEventQuery(id, campaignEvent.CampaignId, campaignEvent.OrganizationId));

            if (!await authorizableEvent.UserCanEdit())
            {
                return new ForbidResult();
            }

            return View(campaignEvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventEditViewModel eventEditViewModel, IFormFile fileUpload)
        {
            if (eventEditViewModel == null)
            {
                return BadRequest();
            }

            var authorizableEvent = await _mediator.SendAsync(new AuthorizableEventQuery(eventEditViewModel.Id));
            if (!await authorizableEvent.UserCanEdit())
            {
                return new ForbidResult();
            }

            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = eventEditViewModel.CampaignId });

            var errors = _eventEditViewModelValidator.Validate(eventEditViewModel, campaign);
            errors.ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (ModelState.IsValid)
            {
                if (fileUpload != null)
                {
                    if (!fileUpload.IsAcceptableImageContentType())
                    {
                        ModelState.AddModelError(nameof(eventEditViewModel.ImageUrl), "You must upload a valid image file for the logo (.jpg, .png, .gif)");
                        return View(eventEditViewModel);
                    }

                    if (_imageSizeValidator != null && fileUpload.Length > _imageSizeValidator.FileSizeInBytes)
                    {
                        ModelState.AddModelError(nameof(eventEditViewModel.ImageUrl), $"File size must be less than {_imageSizeValidator.BytesToMb():#,##0.00}MB!");
                        return View("Edit", eventEditViewModel);
                    }

                    var existingImageUrl = eventEditViewModel.ImageUrl;
                    var newImageUrl = await _imageService.UploadEventImageAsync(campaign.OrganizationId, eventEditViewModel.Id, fileUpload);
                    if (!string.IsNullOrEmpty(newImageUrl))
                    {
                        eventEditViewModel.ImageUrl = newImageUrl;
                        if (existingImageUrl != null && existingImageUrl != newImageUrl)
                        {
                            await _imageService.DeleteImageAsync(existingImageUrl);
                        }
                    }
                }

                var id = await _mediator.SendAsync(new EditEventCommand { Event = eventEditViewModel });

                return RedirectToAction(nameof(Details), new { area = AreaNames.Admin, id = id });
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
                return RedirectToAction(nameof(Details), new { area = AreaNames.Admin, id });
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var viewModel = await _mediator.SendAsync(new DeleteQuery { EventId = id });

            if (viewModel == null)
            {
                return NotFound();
            }

            var authorizableEvent = await _mediator.SendAsync(new AuthorizableEventQuery(viewModel.Id, viewModel.CampaignId, viewModel.OrganizationId));

            if (!await authorizableEvent.UserCanDelete())
            {
                return new ForbidResult();
            }

            ViewData["Title"] = $"Delete event {viewModel.Name}";

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var authorizableEvent = await _mediator.SendAsync(new AuthorizableEventQuery(id));

            if (!await authorizableEvent.UserCanDelete())
            {
                return new ForbidResult();
            }

            await _mediator.SendAsync(new DeleteEventCommand { EventId = id });

            return RedirectToAction(nameof(CampaignController.Details), "Campaign", new { area = AreaNames.Admin, id = authorizableEvent.CampaignId });
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

            var authorizableEvent = await _mediator.SendAsync(new AuthorizableEventQuery(campaignEvent.Id, campaignEvent.CampaignId, campaignEvent.OrganizationId));
            if (!await authorizableEvent.UserCanEdit())
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

            var authorizableEvent = await _mediator.SendAsync(new AuthorizableEventQuery(viewModel.EventId));
            if (!await authorizableEvent.UserCanEdit())
            {
                return new ForbidResult();
            }

            await _mediator.SendAsync(new MessageEventVolunteersCommand { ViewModel = viewModel });

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostEventFile(int id, IFormFile file)
        {
            var organizationId = await _mediator.SendAsync(new OrganizationIdByEventIdQuery { EventId = id });
            var imageUrl = await _imageService.UploadEventImageAsync(organizationId, id, file);

            await _mediator.SendAsync(new UpdateEventImageUrl { EventId = id, ImageUrl = imageUrl });

            return RedirectToRoute(new { controller = "Event", area = AreaNames.Admin, action = nameof(Edit), id });
        }

        [HttpGet]
        [Route("Admin/Event/[action]/{id}/{status?}")]
        public async Task<IActionResult> Requests(int id, string status)
        {
            var authorizableEvent = await _mediator.SendAsync(new AuthorizableEventQuery(id));
            if (!await authorizableEvent.UserCanView())
            {
                return new ForbidResult();
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
                    return RedirectToAction(nameof(Requests), new { id });
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
