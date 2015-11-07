using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;

using AllReady.Models;
using AllReady.ViewModels;
using System.Security.Claims;
using AllReady.Security;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("TenantAdmin")]
    public class TaskController : Controller
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public TaskController(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public override ViewResult View()
        {
            return base.View().WithSkills(_dataAccess);
        }
        public override ViewResult View(object model)
        {
            return base.View(model).WithSkills(_dataAccess);
        }
        public override ViewResult View(string viewName)
        {
            return base.View(viewName).WithSkills(_dataAccess);
        }
        public override ViewResult View(string viewName, object model)
        {
            return base.View(viewName, model).WithSkills(_dataAccess);
        }

        [HttpGet]
        [Route("Admin/Task/Create/{activityId}")]
        public IActionResult Create(int activityId)
        {
            var activity = _dataAccess.GetActivity(activityId);
            if (activity == null || !User.IsTenantAdmin(activity.TenantId))
            {
                return HttpUnauthorized();
            }
            var viewModel = new TaskViewModel()
            {
                IsNew = true,
                ActivityId = activity.Id,
                ActivityName = activity.Name,
                CampaignId = activity.CampaignId,
                CampaignName = activity.Campaign.Name,
                TenantId = activity.TenantId,
                TenantName = activity.Tenant.Name
            };
            return View("Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Task/Create/{activityId}")]
        public IActionResult Create(int activityId, TaskViewModel model)
        {
            if (model.EndDateTime < model.StartDateTime)
            {
                ModelState.AddModelError("EndDateTime", "Ending time cannot be earlier than the starting time");
            }

            if (ModelState.IsValid)
            {
                var activity = _dataAccess.GetActivity(model.ActivityId);
                if (activity == null || !User.IsTenantAdmin(activity.TenantId))
                {
                    return HttpUnauthorized();
                }
                _dataAccess.AddTaskAsync(model.ToModel(_dataAccess));
                return RedirectToAction("Details", "Activity", new { id = activityId });
            }
            model.IsNew = true;
            return View("Edit", model);
        }

        [HttpGet]
        [Route("Admin/Task/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var dbTask = _dataAccess.GetTask(id);
            if (dbTask == null || dbTask.Activity == null)
            {
                return HttpNotFound();
            }            
            if (!User.IsTenantAdmin(dbTask.Activity.TenantId))
            {
                return HttpUnauthorized();
            }
            var model = new TaskViewModel(dbTask);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskViewModel model)
        {
            if (model.EndDateTime < model.StartDateTime)
            {
                ModelState.AddModelError("EndDateTime", "Ending time cannot be earlier than the starting time");
            }

            if (ModelState.IsValid)
            {
                var activity = _dataAccess.GetActivity(model.ActivityId);
                if (activity == null || !User.IsTenantAdmin(activity.TenantId))
                {
                    return HttpUnauthorized();
                }
                await _dataAccess.UpdateTaskAsync(model.ToModel(_dataAccess));
                return RedirectToAction("Details", "Activity", new { id = model.ActivityId });
            }

            return View(model);
        }

        public IActionResult Delete(int id)
        {

            var dbTask = _dataAccess.GetTask(id);
            if (dbTask == null || dbTask.Activity == null)
            {
                return new HttpNotFoundResult();
            }
            
            if (!User.IsTenantAdmin(dbTask.Activity.TenantId))
            {
                return HttpUnauthorized();
            }

            var model = new TaskViewModel(dbTask);
            return View(model);
        }

        [HttpGet]
        [Route("Admin/Task/Details/{activityId}/{id}")]
        public IActionResult Details(int activityId, int id)
        {

            var dbTask = _dataAccess.GetTask(id);
            if (dbTask == null)
            {
                return new HttpNotFoundResult();
            }

            var model = new TaskViewModel(dbTask);

            return View(model);
        }
        // POST: Activity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dbTask = _dataAccess.GetTask(id);
            if (dbTask == null || dbTask.Activity == null)
            {
                return new HttpNotFoundResult();
            }

            if (!User.IsTenantAdmin(dbTask.Activity.TenantId))
            {
                return HttpUnauthorized();
            }

            await _dataAccess.DeleteTaskAsync(id);

            return RedirectToAction("Details", "Activity", new { id = dbTask.Activity.Id});
        }

    }
}
