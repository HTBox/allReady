using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Activities;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Models;
using AllReady.Extensions;
using AllReady.Models;
using AllReady.Security;
using AllReady.Services;
using AllReady.ViewModels;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using AllReady.Areas.Admin.Models.Validators;
using AllReady.Features.Activity;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class ActivityController : Controller
    {
        private readonly IImageService _imageService;
        private readonly IMediator _mediator;

        public ActivityController(IImageService imageService, IMediator mediator)
        {
            _imageService = imageService;
            _mediator = mediator;
        }

        // GET: Activity/Details/5
        [HttpGet]
        [Route("Admin/Activity/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var activity = await _mediator.SendAsync(new ActivityDetailQuery { ActivityId = id });
            if (activity == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(activity.OrganizationId))
            {
                return HttpUnauthorized();
            }

            return View(activity);
        }

        // GET: Activity/Create
        [Route("Admin/Activity/Create/{campaignId}")]
        public async Task<IActionResult> Create(int campaignId)
        {
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = campaignId });
            if (campaign == null || !User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return HttpUnauthorized();
            }

            var activity = new ActivityDetailModel
            {
                CampaignId = campaign.Id,
                CampaignName = campaign.Name,
                TimeZoneId = campaign.TimeZoneId,
                OrganizationId = campaign.OrganizationId,
                OrganizationName = campaign.OrganizationName,
                StartDateTime = DateTime.Today.Date,
                EndDateTime = DateTime.Today.Date.AddMonths(1)
            };

            return View("Edit", activity);
        }

        // POST: Activity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Activity/Create/{campaignId}")]
        public async Task<IActionResult> Create(int campaignId, ActivityDetailModel activity, IFormFile fileUpload)
        {
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = campaignId });
            if (campaign == null || !User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return HttpUnauthorized();
            }

            var validator = new ActivityDetailModelValidator(_mediator);
            var errors = await validator.Validate(activity, campaign);
            errors.ToList().ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            //TryValidateModel is called explictly because of MVC 6 behavior that supresses model state validation during model binding when binding to an IFormFile.
            //See #619.
            if (ModelState.IsValid && TryValidateModel(activity))
            {
                if (fileUpload != null)
                {
                    if (!fileUpload.IsAcceptableImageContentType())
                    {
                        ModelState.AddModelError("ImageUrl", "You must upload a valid image file for the logo (.jpg, .png, .gif)");
                        return View("Edit", activity);
                    }
                }

                activity.OrganizationId = campaign.OrganizationId;
                var id = await _mediator.SendAsync(new EditActivityCommand { Activity = activity });

                if (fileUpload != null)
                {
                    // resave now that activity has the ImageUrl
                    activity.Id = id;
                    activity.ImageUrl = await _imageService.UploadActivityImageAsync(campaign.OrganizationId, id, fileUpload);
                    await _mediator.SendAsync(new EditActivityCommand { Activity = activity });
                }

                return RedirectToAction(nameof(Details), new { area = "Admin", id = id });
            }

            return View("Edit", activity);
        }

        // GET: Activity/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var activity = await _mediator.SendAsync(new ActivityDetailQuery { ActivityId = id });
            if (activity == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(activity.OrganizationId))
            {
                return HttpUnauthorized();
            }

            return View(activity);
        }

        // POST: Activity/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ActivityDetailModel activity, IFormFile fileUpload)
        {
            if (activity == null)
            {
                return HttpBadRequest();
            }
            
            var organizationId = _mediator.Send(new ManagingOrganizationIdByActivityIdQuery { ActivityId = activity.Id });
            if (!User.IsOrganizationAdmin(organizationId))
            {
                return HttpUnauthorized();
            }

            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = activity.CampaignId });

            var validator = new ActivityDetailModelValidator(_mediator);
            var errors = await validator.Validate(activity, campaign);
            errors.ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (ModelState.IsValid)
            {
                if (fileUpload != null)
                {
                    if (fileUpload.IsAcceptableImageContentType())
                    {
                        activity.ImageUrl = await _imageService.UploadActivityImageAsync(campaign.OrganizationId, activity.Id, fileUpload);
                    }
                    else
                    {
                        ModelState.AddModelError("ImageUrl", "You must upload a valid image file for the logo (.jpg, .png, .gif)");
                        return View(activity);
                    }
                }
                
                var id = await _mediator.SendAsync(new EditActivityCommand { Activity = activity });

                return RedirectToAction(nameof(Details), new { area = "Admin", id = id });
            }

            return View(activity);
        }

        // GET: Activity/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var activity = await _mediator.SendAsync(new ActivityDetailQuery { ActivityId = id });
            if (activity == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(activity.OrganizationId))
            {
                return HttpUnauthorized();
            }

            return View(activity);
        }

        // POST: Activity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //TODO: Should be using an ActivitySummaryQuery here
            var activity = await _mediator.SendAsync(new ActivityDetailQuery { ActivityId = id });
            if (activity == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(activity.OrganizationId))
            {
                return HttpUnauthorized();
            }

            await _mediator.SendAsync(new DeleteActivityCommand { ActivityId = id });

            return RedirectToAction(nameof(CampaignController.Details), "Campaign", new { area = "Admin", id = activity.CampaignId });
        }

        [HttpGet]
        public IActionResult Assign(int id)
        {
            var activity = GetActivityBy(id);
            if (activity == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(activity.Campaign.ManagingOrganizationId))
            {
                return HttpUnauthorized();
            }

            var model = new ActivityViewModel(activity);
            model.Tasks = model.Tasks.OrderBy(t => t.StartDateTime).ThenBy(t => t.Name).ToList();
            model.Volunteers = activity.UsersSignedUp.Select(u => u.User).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MessageAllVolunteers(MessageActivityVolunteersModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }
            
            //TODO: Query only for the organization Id rather than the whole activity detail
            var activity = await _mediator.SendAsync(new ActivityDetailQuery { ActivityId = model.ActivityId });
            if (activity == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(activity.OrganizationId))
            {
                return HttpUnauthorized();
            }

            await _mediator.SendAsync(new MessageActivityVolunteersCommand { Model = model });

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostActivityFile(int id, IFormFile file)
        {
            var activity = GetActivityBy(id);

            activity.ImageUrl = await _imageService.UploadActivityImageAsync(activity.Id, activity.Campaign.ManagingOrganizationId, file);
            await _mediator.SendAsync(new UpdateActivity { Activity = activity });

            return RedirectToRoute(new { controller = "Activity", Area = "Admin", action = nameof(Edit), id = id });
        }

        private Activity GetActivityBy(int activityId)
        {
            //TODO: refactor message to async when IAllReadyDataAccess read ops are made async
            return _mediator.Send(new ActivityByActivityIdQuery { ActivityId = activityId });
        }
    }
}