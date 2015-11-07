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
using MediatR;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("TenantAdmin")]
    public class TaskController : Controller
    {
        private readonly IAllReadyDataAccess _dataAccess;
        private readonly IMediator _bus;

        public TaskController(IAllReadyDataAccess dataAccess, IMediator bus)
        {
            _dataAccess = dataAccess;
            _bus = bus;
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
            var task = _bus.Send(new EditTaskQuery() { TaskId = id });
            if (task == null)
            {
                return HttpNotFound();
            }            
            if (!User.IsTenantAdmin(task.TenantId))
            {
                return HttpUnauthorized();
            }
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TaskEditViewModel model)
        {
            if (model.EndDateTime < model.StartDateTime)
            {
                ModelState.AddModelError("EndDateTime", "Ending time cannot be earlier than the starting time");
            }

            if (ModelState.IsValid)
            {
                if (!User.IsTenantAdmin(model.TenantId))
                {
                    return HttpUnauthorized();
                }
                _bus.Send(new EditTaskCommand() { Task = model });
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
        [Route("Admin/Task/Details/{id}")]
        public IActionResult Details(int id)
        {
            var task = _bus.Send(new TaskQuery() { TaskId = id });
            if (task == null)
            {
                return new HttpNotFoundResult();
            }
            return View(task);
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
