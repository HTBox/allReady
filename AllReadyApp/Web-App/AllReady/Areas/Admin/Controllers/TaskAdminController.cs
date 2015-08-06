using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;

using AllReady.Extensions;
using AllReady.Models;
using AllReady.ViewModels;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("TenantAdmin")]
    public class TaskController : Controller
    {
        private readonly IAllReadyDataAccess _dataAccess;
        private readonly UserManager<ApplicationUser> _userManager;

        public TaskController(IAllReadyDataAccess dataAccess, UserManager<ApplicationUser> usermanager)
        {
            _dataAccess = dataAccess;
            _userManager = usermanager;
        }

        ViewResult AddDropdownData(ViewResult view)
        {
            view.ViewData["Campaigns"] = _dataAccess.Campaigns.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }).ToList();
            view.ViewData["Tenants"] = _dataAccess.Tenants.Select(t => new SelectListItem() { Value = t.Id.ToString(), Text = t.Name }).ToList();
            view.ViewData["Activities"] = _dataAccess.Activities.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }).ToList();
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

        [Route("Admin/Task/{activityId?}")]
        public async Task<IActionResult> Index(int? activityId)
        {
            if (activityId != null)
            {
                ViewBag.ActivityId = activityId;
                return View(new List<Activity>() { _dataAccess.GetActivity((int)activityId) });
            }
            else
            {
                var currentUser = await _userManager.GetCurrentUser(Context);
                if (currentUser == null)
                {
                    return new HttpUnauthorizedResult();
                }
                var thisTenantsActivities = (from activity in _dataAccess.Activities
                                             where activity.Campaign.ManagingTenant == currentUser.AssociatedTenant
                                             select activity).ToList();

                return View(thisTenantsActivities);
            }
        }

        [HttpGet]
        [Route("Admin/Task/Create")]
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Task/Create")]
        public IActionResult Create(TaskViewModel model) {
            if (ModelState.IsValid) {
                _dataAccess.AddTask(model.ToModel(_dataAccess));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        [Route("Admin/Task/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var dbTask = _dataAccess.GetTask(id);
            var model = new TaskViewModel {
                Id = dbTask.Id,
                Name = dbTask.Name,
                Description = dbTask.Description,
                ActivityId = dbTask.Activity.Id,
                ActivityName = dbTask.Activity.Name,
                StartDateTime = dbTask.StartDateTimeUtc,
                EndDateTime = dbTask.EndDateTimeUtc
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _dataAccess.UpdateTask(model.ToModel(_dataAccess));
                if (ViewBag.ActivityId != null)
                {
                    return RedirectToAction("Index", new { activityId = ViewBag.ActivityId });
                }
                else
                {
                    return RedirectToAction("Index");
                }                
            }

            return View(model);
        }

        public IActionResult Delete(int id) {

            var dbTask = _dataAccess.GetTask(id);
            if (dbTask == null) {
                return new HttpStatusCodeResult(404);
            }

            var model = new TaskViewModel {
                Id = dbTask.Id,
                Name = dbTask.Name,
                StartDateTime = dbTask.StartDateTimeUtc,
                EndDateTime = dbTask.EndDateTimeUtc
            };

            return View(model);
        }

        // POST: Activity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            await _dataAccess.DeleteTask(id);

            return RedirectToAction("Index");
        }

    }
}
