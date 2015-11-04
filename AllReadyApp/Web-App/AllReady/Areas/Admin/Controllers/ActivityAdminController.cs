﻿using Microsoft.AspNet.Authorization;
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
            view.ViewData["Skills"] = _dataAccess.Skills
                .Select(s => new { Name = s.HierarchicalName, Id = s.Id })
                .OrderBy(a => a.Name)
                .ToList();

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
            if (campaign == null || !User.IsTenantAdmin(campaign.ManagingTenantId))
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
            if (campaign == null || !User.IsTenantAdmin(campaign.ManagingTenantId))
            {
                return new HttpUnauthorizedResult();
            }

            ActivityCreateEditPostParentViewModel activityParentViewModel = new ActivityCreateEditPostParentViewModel
            {
                CampaignId = campaign.Id,
                CampaignName = campaign.Name
            };

            activityParentViewModel.CreateEditViewModel = new ActivityCreateEditViewModel
            {
                RequiredSkills = new List<ActivitySkill>(),
                StartDateTime = DateTime.Today.Date,
                EndDateTime = DateTime.Today.Date.AddMonths(1)
            };

            return View("Edit", activityParentViewModel);
        }

        // POST: Activity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Activity/Create/{campaignId}")]
        public async Task<IActionResult> Create(int campaignId, ActivityCreateEditPostParentViewModel activityParentViewModel)
        {
            if (activityParentViewModel.CreateEditViewModel.EndDateTime < activityParentViewModel.CreateEditViewModel.StartDateTime)
            {
                ModelState.AddModelError("EndDateTime", "End date cannot be earlier than the start date");
            }

            Campaign campaign = _dataAccess.GetCampaign(campaignId);

            if (ModelState.IsValid)
            {                
                if (campaign == null || 
                    !User.IsTenantAdmin(campaign.ManagingTenantId))
                {
                    return HttpUnauthorized();
                }

                Activity activity = new Activity
                {
                    Campaign = campaign,
                    CampaignId = campaignId,
                    TenantId = campaign.ManagingTenantId,
                    Name = activityParentViewModel.CreateEditViewModel.Name,
                    Description = activityParentViewModel.CreateEditViewModel.Description,
                    StartDateTimeUtc = activityParentViewModel.CreateEditViewModel.StartDateTime.ToUniversalTime(),
                    EndDateTimeUtc = activityParentViewModel.CreateEditViewModel.EndDateTime.ToUniversalTime(),
                    RequiredSkills = activityParentViewModel.CreateEditViewModel.RequiredSkills
                };

                await _dataAccess.AddActivity(activity);
                return RedirectToAction("Index", new { campaignId = activity.CampaignId });
            }

            // repopulate non-editable ViewModel values
            activityParentViewModel.CampaignId = campaign.Id;
            activityParentViewModel.CampaignName = campaign.Name;
            
            return View("Edit", activityParentViewModel);
        }

        // GET: Activity/Edit/5
        [Route("Admin/Activity/Edit/{id}")]
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

            ActivityCreateEditPostParentViewModel activityParentViewModel = new ActivityCreateEditPostParentViewModel
            {
                PageTitle = "Activity: " + activity.Name,
                IsCreateView = false,

                ActivityId = activity.Id,
                ActivityName = activity.Name,
                CampaignId = activity.Campaign.Id,
                CampaignName = activity.Campaign.Name
            };

            activityParentViewModel.CreateEditViewModel = new ActivityCreateEditViewModel //(activityCreateEditPostVM)
            {
                Name = activity.Name,
                Description = activity.Description,
                StartDateTime = activity.StartDateTimeUtc.Date,
                EndDateTime = activity.EndDateTimeUtc.Date,
                RequiredSkills = activity.RequiredSkills
            };

            activityParentViewModel.UploadFileViewModel = new ActivityFileUploadViewModel
            {
                Name = activity.Name,
                ImageUrl = activity.ImageUrl
            };

            return View(activityParentViewModel);
        }

        // POST: Activity/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Activity/Edit/{id}")]
        public async Task<IActionResult> Edit(int id, ActivityCreateEditPostParentViewModel activityParentViewModel)
        {
            Activity activity = _dataAccess.GetActivity(id);

            if (activity == null)
            {
                return HttpBadRequest();
            }

            if (!User.IsTenantAdmin(activity.Campaign.ManagingTenantId))
            {
                return HttpUnauthorized();
            }

            if (activityParentViewModel.CreateEditViewModel.EndDateTime < activityParentViewModel.CreateEditViewModel.StartDateTime)
            {
                ModelState.AddModelError("EndDateTime", "End date cannot be earlier than the start date");
            }

            if (ModelState.IsValid)
            {
                if (activityParentViewModel.CreateEditViewModel.RequiredSkills != null && activityParentViewModel.CreateEditViewModel.RequiredSkills.Count > 0)
                {
                    activityParentViewModel.CreateEditViewModel.RequiredSkills.ForEach(acsk => acsk.ActivityId = activityParentViewModel.ActivityId);
                }

                activity.Name = activityParentViewModel.CreateEditViewModel.Name;
                activity.Description = activityParentViewModel.CreateEditViewModel.Description;
                activity.StartDateTimeUtc = activityParentViewModel.CreateEditViewModel.StartDateTime.ToUniversalTime();
                activity.EndDateTimeUtc = activityParentViewModel.CreateEditViewModel.EndDateTime.ToUniversalTime();
                activity.RequiredSkills = activityParentViewModel.CreateEditViewModel.RequiredSkills;

                await _dataAccess.UpdateActivity(activity);

                return RedirectToAction("Index", new { campaignId = activity.CampaignId });
            }

            // repopulate ViewModels from trusted source
            activityParentViewModel.PageTitle = "Activity: " + activity.Name;
            activityParentViewModel.IsCreateView = false;

            // needed for titles and links
            activityParentViewModel.ActivityId = activity.Id;
            activityParentViewModel.ActivityName = activity.Name;
            activityParentViewModel.CampaignId = activity.Campaign.Id;
            activityParentViewModel.CampaignName = activity.Campaign.Name;

            activityParentViewModel.UploadFileViewModel = new ActivityFileUploadViewModel
            {
                Name = activity.Name,
                ImageUrl = activity.ImageUrl
            };

            return View(activityParentViewModel);
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
        [Route("Admin/Activity/PostActivityFile/{id}")]
        public async Task<IActionResult> PostActivityFile(int id, ActivityCreateEditPostParentViewModel activityParentViewModel)
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

            if (ModelState.IsValid) { 
                activity.ImageUrl = await _imageService.UploadActivityImageAsync(activity.Id, activity.Tenant.Id, activityParentViewModel.UploadFileViewModel.UploadedFile);

                await _dataAccess.UpdateActivity(activity);

                return RedirectToAction("Index", new { campaignId = activity.CampaignId });
            }

            // repopulate ViewModels from trusted source
            activityParentViewModel.PageTitle = "Activity: " + activity.Name;
            activityParentViewModel.IsCreateView = false;

            // needed for titles and links
            activityParentViewModel.ActivityId = activity.Id;
            activityParentViewModel.ActivityName = activity.Name;
            activityParentViewModel.CampaignId = activity.Campaign.Id;
            activityParentViewModel.CampaignName = activity.Campaign.Name;

            // valdiation failed, so we need to repopulate the CreateEditViewModel so that both forms are populated
            activityParentViewModel.CreateEditViewModel = new ActivityCreateEditViewModel
            {
                Name = activity.Name,
                Description = activity.Description,
                StartDateTime = activity.StartDateTimeUtc,
                EndDateTime = activity.EndDateTimeUtc,
                RequiredSkills = activity.RequiredSkills,
            };

            return View("Edit", activityParentViewModel);
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