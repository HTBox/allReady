﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Activities;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Features.Shared;
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

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class ActivityController : Controller
    {
        private readonly IAllReadyDataAccess _dataAccess;
        private readonly IImageService _imageService;
        private readonly IMediator _mediator;

        public ActivityController(IAllReadyDataAccess dataAccess, IImageService imageService, IMediator mediator)
        {
            _dataAccess = dataAccess;
            _imageService = imageService;
            _mediator = mediator;
        }

        // GET: Activity/Details/5
        [HttpGet]
        [Route("Admin/Activity/Details/{id}")]
        public IActionResult Details(int id)
        {
            var activity = _mediator.Send(new ActivityDetailQuery { ActivityId = id });

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
            CampaignSummaryModel campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = campaignId });
            if (campaign == null ||
                !User.IsOrganizationAdmin(campaign.OrganizationId))
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
                var id = _mediator.Send(new EditActivityCommand { Activity = activity });

                if (fileUpload != null)
                {
                    // resave now that activity has the ImageUrl
                    activity.Id = id;
                    activity.ImageUrl = await _imageService.UploadActivityImageAsync(campaign.OrganizationId, id, fileUpload);
                    _mediator.Send(new EditActivityCommand { Activity = activity });
                }

                return RedirectToAction("Details", "Activity", new { area = "Admin", id = id });
            }

            return View("Edit", activity);
        }

        // GET: Activity/Edit/5
        public IActionResult Edit(int id)
        {
            ActivityDetailModel activity = _mediator.Send(new ActivityDetailQuery { ActivityId = id });

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
            
            //TODO: Use the query pattern here
            var organizationId = _dataAccess.GetManagingOrganizationId(activity.Id);
            if (!User.IsOrganizationAdmin(organizationId))
            {
                return HttpUnauthorized();
            }

            CampaignSummaryModel campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = activity.CampaignId });

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
                
                var id = _mediator.Send(new EditActivityCommand { Activity = activity });
                return RedirectToAction("Details", "Activity", new { area = "Admin", id = id });
            }
            return View(activity);
        }

        // GET: Activity/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(int id)
        {
            var activity = _mediator.Send(new ActivityDetailQuery { ActivityId = id });
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
        public IActionResult DeleteConfirmed(System.Int32 id)
        {
            //TODO: Should be using an ActivitySummaryQuery here
            ActivityDetailModel activity = _mediator.Send(new ActivityDetailQuery { ActivityId = id });
            if (activity == null)
            {
                return HttpNotFound();
            }
            if (!User.IsOrganizationAdmin(activity.OrganizationId))
            {
                return HttpUnauthorized();
            }
            _mediator.Send(new DeleteActivityCommand { ActivityId = id });
            return RedirectToAction("Details", "Campaign", new { area = "Admin", id = activity.CampaignId });
        }

        [HttpGet]
        public IActionResult Assign(int id)
        {
            //TODO: Update this to use Query pattern
            var activity = _dataAccess.GetActivity(id);

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
            //TODO: Query only for the organization Id rather than the whole activity detail
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var activity = _mediator.Send(new ActivityDetailQuery { ActivityId = model.ActivityId });
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
            //TODO: Use a command here
            Activity a = _dataAccess.GetActivity(id);

            a.ImageUrl = await _imageService.UploadActivityImageAsync(a.Id, a.Campaign.ManagingOrganizationId, file);
            await _dataAccess.UpdateActivity(a);

            return RedirectToRoute(new { controller = "Activity", Area = "Admin", action = "Edit", id = id });
        }
    }
}