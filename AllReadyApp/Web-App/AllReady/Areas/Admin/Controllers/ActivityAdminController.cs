using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

using AllReady.Security;
using AllReady.Models;
using AllReady.Services;
using AllReady.ViewModels;

using System.Linq;
using System.Threading.Tasks;
using MediatR;
using AllReady.Areas.Admin.Models;
using System;
using AllReady.Areas.Admin.Features.Activities;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Extensions;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class ActivityController : Controller
    {
        private readonly IAllReadyDataAccess _dataAccess;
        private readonly IImageService _imageService;
        private readonly IMediator _bus;

        public ActivityController(IAllReadyDataAccess dataAccess, IImageService imageService, IMediator bus)
        {
            _dataAccess = dataAccess;
            _imageService = imageService;
            _bus = bus;
        }

        // GET: Activity/Details/5
        [HttpGet]
        [Route("Admin/Activity/Details/{id}")]
        public IActionResult Details(int id)
        {
            var activity = _bus.Send(new ActivityDetailQuery { ActivityId = id });

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
        public IActionResult Create(int campaignId)
        {
            CampaignSummaryModel campaign = _bus.Send(new CampaignSummaryQuery { CampaignId = campaignId });
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
            if (activity.EndDateTime < activity.StartDateTime)
            {
                ModelState.AddModelError(nameof(activity.EndDateTime), "End date cannot be earlier than the start date");
            }

                CampaignSummaryModel campaign = _bus.Send(new CampaignSummaryQuery { CampaignId = campaignId });
                if (campaign == null ||
                    !User.IsOrganizationAdmin(campaign.OrganizationId))
                {
                    return HttpUnauthorized();
                }

            if (activity.StartDateTime < campaign.StartDate)
            {
                ModelState.AddModelError(nameof(activity.StartDateTime), "Start date cannot be earlier than the campaign start date " + campaign.StartDate.ToString("d"));
            }

            if (activity.EndDateTime > campaign.EndDate)
            {
                ModelState.AddModelError(nameof(activity.EndDateTime), "End date cannot be later than the campaign end date " + campaign.EndDate.ToString("d"));
            }

            if (ModelState.IsValid)
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
                var id = _bus.Send(new EditActivityCommand { Activity = activity });

                if (fileUpload != null)
                {
                    // resave now that activty has the ImageUrl
                    activity.Id = id;
                    activity.ImageUrl = await _imageService.UploadActivityImageAsync(campaign.OrganizationId, id, fileUpload);
                    _bus.Send(new EditActivityCommand { Activity = activity });
                }

                return RedirectToAction("Details", "Activity", new { area = "Admin", id = id });
            }
            return View("Edit", activity);
        }

        // GET: Activity/Edit/5
        public IActionResult Edit(int id)
        {
            ActivityDetailModel activity = _bus.Send(new ActivityDetailQuery { ActivityId = id });

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
            int organizationId = _dataAccess.GetManagingOrganizationId(activity.Id);
            if (!User.IsOrganizationAdmin(organizationId))
            {
                return HttpUnauthorized();
            }

            if (activity.EndDateTime < activity.StartDateTime)
            {
                ModelState.AddModelError(nameof(activity.EndDateTime), "End date cannot be earlier than the start date");
            }

            CampaignSummaryModel campaign = _bus.Send(new CampaignSummaryQuery { CampaignId = activity.CampaignId });

            if (activity.StartDateTime < campaign.StartDate)
            {
                ModelState.AddModelError(nameof(activity.StartDateTime), "Start date cannot be earlier than the campaign start date " + campaign.StartDate.ToString("d"));
            }

            if (activity.EndDateTime > campaign.EndDate)
            {
                ModelState.AddModelError(nameof(activity.EndDateTime), "End date cannot be later than the campaign end date " + campaign.EndDate.ToString("d"));
            }

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
                
                var id = _bus.Send(new EditActivityCommand { Activity = activity });
                return RedirectToAction("Details", "Activity", new { area = "Admin", id = id });
            }
            return View(activity);
        }

        // GET: Activity/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(int id)
        {
            var activity = _bus.Send(new ActivityDetailQuery { ActivityId = id });
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
            ActivityDetailModel activity = _bus.Send(new ActivityDetailQuery { ActivityId = id });
            if (activity == null)
            {
                return HttpNotFound();
            }
            if (!User.IsOrganizationAdmin(activity.OrganizationId))
            {
                return HttpUnauthorized();
            }
            _bus.Send(new DeleteActivityCommand { ActivityId = id });
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

            var activity = _bus.Send(new ActivityDetailQuery { ActivityId = model.ActivityId });
            if (activity == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(activity.OrganizationId))
            {
                return HttpUnauthorized();
            }

            await _bus.SendAsync(new MessageActivityVolunteersCommand { Model = model });
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

        private bool UserIsOrganizationAdminOfActivity(Activity activity)
        {
            return User.IsOrganizationAdmin(activity.Campaign.ManagingOrganizationId);
        }

        private bool UserIsOrganizationAdminOfActivity(int activityId)
        {
            return UserIsOrganizationAdminOfActivity(_dataAccess.GetActivity(activityId));
        }

    }
}