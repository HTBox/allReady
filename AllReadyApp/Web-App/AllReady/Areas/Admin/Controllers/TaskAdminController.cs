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

        [Route("Admin/Task/{activityId}")]
        public IActionResult Index(int activityId)
        {
            ViewBag.ActivityId = activityId;
            var activity = _dataAccess.GetActivity(activityId);
            if (activity == null || !User.IsTenantAdmin(activity.TenantId))
            {
                return HttpUnauthorized();
            }
            return View(activity);
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
            if (ModelState.IsValid)
            {
                var activity = _dataAccess.GetActivity(model.ActivityId);
                if (activity == null || !User.IsTenantAdmin(activity.TenantId))
                {
                    return HttpUnauthorized();
                }
                _dataAccess.AddTaskAsync(model.ToModel(_dataAccess));
                return RedirectToAction("Index");
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
            if (ModelState.IsValid)
            {
                var activity = _dataAccess.GetActivity(model.ActivityId);
                if (activity == null || !User.IsTenantAdmin(activity.TenantId))
                {
                    return HttpUnauthorized();
                }
                await _dataAccess.UpdateTaskAsync(model.ToModel(_dataAccess));
                return RedirectToAction("Index", new { activityId = model.ActivityId });
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

            var model = new TaskViewModel
            {
                Id = dbTask.Id,
                Name = dbTask.Name,
                ActivityId = dbTask.Activity.Id,
                StartDateTime = dbTask.StartDateTimeUtc,
                EndDateTime = dbTask.EndDateTimeUtc
            };

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

            var model = new TaskViewModel
            {
                Id = dbTask.Id,
                ActivityId = activityId,
                Description = dbTask.Description,
                Name = dbTask.Name,
                StartDateTime = dbTask.StartDateTimeUtc,
                EndDateTime = dbTask.EndDateTimeUtc
            };

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

            return RedirectToAction("Index", new { activityId = dbTask.Activity.Id});
        }

    }
}
