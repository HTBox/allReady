using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;

using AllReady.Security;
using AllReady.Models;
using AllReady.Services;
using AllReady.ViewModels;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using MediatR;
using AllReady.Areas.Admin.ViewModels;
using System;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.Features.Activities;
using AllReady.Areas.Admin.Features.Campaigns;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("TenantAdmin")]
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
                return new HttpStatusCodeResult(404);
            }

            if (!User.IsTenantAdmin(activity.TenantId))
            {
                return new HttpUnauthorizedResult();
            }

            return View(activity);
        }

        // GET: Activity/Create
        [Route("Admin/Activity/Create/{campaignId}")]
        public IActionResult Create(int campaignId)
        {
            CampaignSummaryViewModel campaign = _bus.Send(new CampaignSummaryQuery { CampaignId = campaignId });
            if (campaign == null || !User.IsTenantAdmin(campaign.TenantId))
            {
                return new HttpUnauthorizedResult();
            }

            var activity = new ActivityDetailViewModel
            {
                CampaignId = campaign.Id,
                CampaignName = campaign.Name,
                TenantId = campaign.TenantId,
                TenantName = campaign.TenantName,                
                StartDateTime = DateTime.Today.Date,
                EndDateTime = DateTime.Today.Date.AddMonths(1)
            };
            return View("Edit", activity);
        }

        // POST: Activity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Activity/Create/{campaignId}")]
        public IActionResult Create(int campaignId, ActivityDetailViewModel activity)
        {
            if (activity.EndDateTime < activity.StartDateTime)
            {
                ModelState.AddModelError("EndDateTime", "End date cannot be earlier than the start date");
            }

            if (ModelState.IsValid)
            {
                CampaignSummaryViewModel campaign = _bus.Send(new CampaignSummaryQuery { CampaignId = campaignId });
                if (campaign == null || 
                    !User.IsTenantAdmin(campaign.TenantId))
                {
                    return HttpUnauthorized();
                }                
                activity.TenantId = campaign.TenantId;
                var id = _bus.Send(new EditActivityCommand { Activity = activity });
                return RedirectToAction("Details", "Activity", new { area = "Admin", id = id });
            }
            return View("Edit", activity);
        }

        // GET: Activity/Edit/5
        public IActionResult Edit(int id)
        {
            ActivityDetailViewModel activity = _bus.Send(new ActivityDetailQuery{ ActivityId = id });
            if (activity == null)
            {
                return new HttpStatusCodeResult(404);
            }

            if (!User.IsTenantAdmin(activity.TenantId))
            {
                return new HttpUnauthorizedResult();
            }

            return View(activity);
        }

        // POST: Activity/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ActivityDetailViewModel activity)
        {
            if (activity == null)
            {
                return HttpBadRequest();
            }
            //TODO: Use the query pattern here
            int campaignId = _dataAccess.GetManagingTenantId(activity.Id);
            if (!User.IsTenantAdmin(campaignId))
            {
                return HttpUnauthorized();
            }

            if (activity.EndDateTime < activity.StartDateTime)
            {
                ModelState.AddModelError("EndDateTime", "End date cannot be earlier than the start date");
            }

            if (ModelState.IsValid)
            {
                var id = _bus.Send(new EditActivityCommand {Activity = activity });
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
                return new HttpStatusCodeResult(404);
            }

            if (!User.IsTenantAdmin(activity.TenantId))
            {
                return new HttpUnauthorizedResult();
            }

            return View(activity);
        }

        // POST: Activity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(System.Int32 id)
        {
            //TODO: Should be using an ActivitySummaryQuery here
            ActivityDetailViewModel activity = _bus.Send(new ActivityDetailQuery { ActivityId = id });
            if (activity == null)
            {
                return HttpNotFound();
            }
            if (!User.IsTenantAdmin(activity.TenantId))
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
            if (!User.IsTenantAdmin(activity.TenantId))
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
        public async Task<IActionResult> Assign(int id, List<TaskViewModel> tasks)
        {
            if (!UserIsTenantAdminOfActivity(id))
            {
                return new HttpUnauthorizedResult();
            }

            var updates = tasks.ToModel(_dataAccess).ToList();
            //TODO: Replacement for API like Tasks.UpdateRange(updates);
            foreach (var item in updates)
            {
                await _dataAccess.UpdateTaskAsync(item);
            }

            // send all notifications to the queue
            var smsRecipients = new List<string>();
            var emailRecipients = new List<string>();

            foreach (var allReadyTask in updates)
            {
                // get all confirmed contact points for the broadcast
                smsRecipients.AddRange(allReadyTask.AssignedVolunteers.Where(u => u.User.PhoneNumberConfirmed).Select(v => v.User.PhoneNumber));
                emailRecipients.AddRange(allReadyTask.AssignedVolunteers.Where(u => u.User.EmailConfirmed).Select(v => v.User.Email));
            }

            var command = new NotifyVolunteersCommand
            {
                // todo: what information do we add about the task?
                // todo: should we use a template from the email service provider?
                // todo: what about non-English volunteers?
                ViewModel = new NotifyVolunteersViewModel
                {
                    SmsMessage = "You've been assigned a task from AllReady.",
                    SmsRecipients = smsRecipients,
                    EmailMessage = "You've been assigned a task from AllReady.",
                    EmailRecipients = emailRecipients
                }
            };

            _bus.Send(command);

            return RedirectToRoute(new { controller = "Activity", Area = "Admin", action = "Details", id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostActivityFile(int id, IFormFile file)
        {
            Activity a = _dataAccess.GetActivity(id);

            a.ImageUrl = await _imageService.UploadActivityImageAsync(a.Id, a.Tenant.Id, file);
            await _dataAccess.UpdateActivity(a);

            return RedirectToRoute(new { controller = "Activity", Area = "Admin", action = "Edit", id = id });

        }

        private bool UserIsTenantAdminOfActivity(Activity activity)
        {
            return User.IsTenantAdmin(activity.TenantId);
        }
        
        private bool UserIsTenantAdminOfActivity(int activityId)
        {
            return UserIsTenantAdminOfActivity(_dataAccess.GetActivity(activityId));
        }

    }
}