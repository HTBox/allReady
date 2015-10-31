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

        ViewResult AddDropdownData(ViewResult view)
        {           
            view.ViewData["Skills"] = _dataAccess.Skills.Select(s => new { Name = s.HierarchicalName, Id = s.Id }).ToList();
            return view;
        }

        public override ViewResult View()
        {
            return AddDropdownData(base.View());
        }
        public override ViewResult View(object model)
        {
            return AddDropdownData(base.View(model));
        }
        public override ViewResult View(string viewName)
        {
            return AddDropdownData(base.View(viewName));
        }
        public override ViewResult View(string viewName, object model)
        {
            return AddDropdownData(base.View(viewName, model));
        }

        // GET: Activity
        [Route("Admin/Activity/{campaignId}")]
        public IActionResult Index(int campaignId)
        {
            Campaign campaign = _dataAccess.GetCampaign(campaignId);
            if (campaign == null || !UserIsTenantAdmin(campaign.ManagingTenantId))
            {
                return HttpUnauthorized();
            }
            var viewModel = new CampaignActivitiesViewModel
            {
                CampaignId = campaign.Id,
                CampaignName = campaign.Name,
                Activities = campaign.Activities
            };
            return View(viewModel);
        }

        // GET: Activity/Details/5
        [HttpGet]
        [Route("Admin/Activity/Details/{id}")]
        public IActionResult Details(int id)
        {
            var activity = _dataAccess.GetActivity(id);

            if (activity == null)
            {
                return new HttpStatusCodeResult(404);
            }

            var avm = new AdminActivityViewModel
            {
                Id = activity.Id,
                CampaignName = activity.Campaign.Name,
                CampaignId = activity.Campaign.Id,
                Title = activity.Name,
                Description = activity.Description,
                StartDateTime = activity.StartDateTimeUtc,
                EndDateTime = activity.EndDateTimeUtc,
                Volunteers = _dataAccess.ActivitySignups.Where(asup => asup.Activity.Id == id).Select(u => u.User.UserName).ToList(),
                Tasks = activity.Tasks.Select(t => new TaskViewModel
                { Id = t.Id,
                    ActivityId =id,
                    Name = t.Name,
                    Description = t.Description })
                    .OrderBy(t => t.StartDateTime).ThenBy(t=> t.Name).ToList(),
                ImageUrl = activity.ImageUrl
            };

            return View(avm);
        }

        // GET: Activity/Create
        [Route("Admin/Activity/Create/{campaignId}")]
        public IActionResult Create(int campaignId)
        {
            Campaign campaign = _dataAccess.GetCampaign(campaignId);
            if (campaign == null || !UserIsTenantAdmin(campaign.ManagingTenantId))
            {
                return new HttpUnauthorizedResult();
            }

            Activity activity = new Activity
            {
                CampaignId = campaign.Id,
                Campaign = campaign,
                TenantId = campaign.ManagingTenantId,
                RequiredSkills = new List<ActivitySkill>(),
                StartDateTimeUtc = DateTime.Today,
                EndDateTimeUtc = DateTime.Today.AddMonths(1)
            };
            return View("Edit", activity);
        }

        // POST: Activity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Activity/Create/{campaignId}")]
        public async Task<IActionResult> Create(int campaignId, Activity activity)
        {
            Campaign campaign = _dataAccess.GetCampaign(campaignId);
            activity.Campaign = campaign;
            activity.CampaignId = campaignId;
            if (ModelState.IsValid)
            {                
                if (campaign == null || 
                    !UserIsTenantAdmin(campaign.ManagingTenantId))
                {
                    return HttpUnauthorized();
                }                
                activity.TenantId = campaign.ManagingTenantId;
                await _dataAccess.AddActivity(activity);
                return RedirectToAction("Index", new { campaignId = activity.CampaignId });
            }
            return View("Edit", activity);
        }

        // GET: Activity/Edit/5
        public IActionResult Edit(int id)
        {
            Activity activity = _dataAccess.GetActivity(id);
            if (activity == null)
            {
                return new HttpStatusCodeResult(404);
            }

            if (!UserIsTenantAdminOfActivity(activity))
            {
                return new HttpUnauthorizedResult();
            }

            return View(activity);
        }

        // POST: Activity/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Activity activity)
        {
            if (ModelState.IsValid)
            {
                if (activity.RequiredSkills != null && activity.RequiredSkills.Count > 0)
                {
                    activity.RequiredSkills.ForEach(acsk => acsk.ActivityId = activity.Id);
                }
                await _dataAccess.UpdateActivity(activity);
                return RedirectToAction("Index", new { campaignId = activity.CampaignId });
            }

            return View(activity);
        }

        // GET: Activity/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(404);
            }

            Activity activity = _dataAccess.GetActivity((int)id);
            if (activity == null)
            {
                return new HttpStatusCodeResult(404);
            }

            if (!UserIsTenantAdminOfActivity(activity))
            {
                return new HttpUnauthorizedResult();
            }

            return View(activity);
        }

        // POST: Activity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(System.Int32 id)
        {
            Activity activity = _dataAccess.GetActivity(id);
            if (!UserIsTenantAdminOfActivity(activity))
            {
                return new HttpUnauthorizedResult();
            }

            await _dataAccess.DeleteActivity(id);

            return RedirectToAction("Index", new { campaignId = activity.CampaignId });
        }

        [HttpGet]
        public IActionResult Assign(int id)
        {
            var activity = _dataAccess.GetActivity(id);

            if (activity == null)
            {
                return new HttpStatusCodeResult(404);
            }
            if (!UserIsTenantAdminOfActivity(activity))
            {
                return new HttpUnauthorizedResult();
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
            return UserIsTenantAdmin(activity.TenantId);
        }

        private bool UserIsTenantAdmin(int tenantId) {
            int? userTenantId = User.GetTenantId();

            return User.IsUserType(UserType.SiteAdmin) ||
                  (userTenantId.HasValue && userTenantId.Value == tenantId);
        }
        
        private bool UserIsTenantAdminOfActivity(int activityId)
        {
            return UserIsTenantAdminOfActivity(_dataAccess.GetActivity(activityId));
        }

    }
}