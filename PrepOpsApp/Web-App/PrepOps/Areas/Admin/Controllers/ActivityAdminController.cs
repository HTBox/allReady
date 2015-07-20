using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;

using PrepOps.Extensions;
using PrepOps.Models;
using PrepOps.Services;
using PrepOps.ViewModels;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrepOps.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("TenantAdmin")]
    public class ActivityController : Controller
    {
        private readonly IPrepOpsDataAccess _dataAccess;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImageService _imageService;

        public ActivityController(IPrepOpsDataAccess dataAccess, UserManager<ApplicationUser> userManager, IImageService imageService)
        {
            _dataAccess = dataAccess;
            _userManager = userManager;
            _imageService = imageService;
        }

        ViewResult AddDropdownData(ViewResult view)
        {
            view.ViewData["Campaigns"] = _dataAccess.Campaigns.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }).ToList();
            view.ViewData["Tenants"] = _dataAccess.Tenants.Select(t => new SelectListItem() { Value = t.Id.ToString(), Text = t.Name }).ToList();
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
        public async Task<IActionResult> Index()
        {
            return await Task.Run(() => View(_dataAccess.Activities));
        }

        // GET: Activity/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var activity = await Task.Run(() => _dataAccess.GetActivity(id));

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
                Tasks = activity.Tasks.Select(t => new TaskViewModel { Name = t.Name }).OrderBy(t => t.StartDateTime).ThenBy(t=> t.Name).ToList(),
                ImageUrl = activity.ImageUrl
            };

            return View(avm);
        }

        // GET: Activity/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetCurrentUser(Context);
            if (currentUser == null || !await _userManager.IsTenantAdmin(currentUser))
            {
                return new HttpUnauthorizedResult();
            }
            return View();
        }

        // POST: Activity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Activity activity)
        {
            var currentUser = await _userManager.GetCurrentUser(Context);
            if (currentUser == null || !await _userManager.IsTenantAdmin(currentUser))
            {
                return new HttpUnauthorizedResult();
            }

            if (activity.TenantId != _dataAccess.GetUser(currentUser.Id).AssociatedTenant.Id)
            {
                return new HttpUnauthorizedResult();
            }

            if (ModelState.IsValid)
            {
                await _dataAccess.AddActivity(activity);
                return RedirectToAction("Index");
            }
            return View(activity);
        }

        // GET: Activity/Edit/5
        public async Task<IActionResult> Edit(System.Int32 id)
        {
            var currentUser = await _userManager.GetCurrentUser(Context);
            if (currentUser == null || !await _userManager.IsTenantAdmin(currentUser))
            {
                return new HttpUnauthorizedResult();
            }

            Activity activity = await Task.Run(() => _dataAccess.GetActivity(id));

            if (activity == null)
            {
                return new HttpStatusCodeResult(404);
            }

            if (!await UserIsTenantAdminOfActivity(currentUser, activity))
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
            var currentUser = await _userManager.GetCurrentUser(Context);
            if (currentUser == null || !await _userManager.IsTenantAdmin(currentUser))
            {
                return new HttpUnauthorizedResult();
            }

            if (ModelState.IsValid)
            {
                await _dataAccess.UpdateActivity(activity);
                return RedirectToAction("Index");
            }

            return View(activity);
        }

        // GET: Activity/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(System.Int32? id)
        {
            var currentUser = await _userManager.GetCurrentUser(Context);

            if (currentUser == null || !await _userManager.IsTenantAdmin(currentUser))
            {
                return new HttpUnauthorizedResult();
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(404);
            }

            Activity activity = await Task.Run(() => _dataAccess.GetActivity((int)id));
            if (activity == null)
            {
                return new HttpStatusCodeResult(404);
            }

            if (!await UserIsTenantAdminOfActivity(currentUser, activity))
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
            var currentUser = await _userManager.GetCurrentUser(Context);

            if (currentUser == null || !await _userManager.IsTenantAdmin(currentUser))
            {
                return new HttpUnauthorizedResult();
            }

            if (!await UserIsTenantAdminOfActivity(currentUser, id))
            {
                return new HttpUnauthorizedResult();
            }

            await _dataAccess.DeleteActivity(id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Assign(int id)
        {
            var currentUser = await _userManager.GetCurrentUser(Context);
            if (currentUser == null || !await _userManager.IsTenantAdmin(currentUser))
            {
                return new HttpUnauthorizedResult();
            }

            var activity = _dataAccess.GetActivity(id);

            if (activity == null)
            {
                return new HttpStatusCodeResult(404);
            }
            if (!await UserIsTenantAdminOfActivity(currentUser, activity))
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
            var currentUser = await _userManager.GetCurrentUser(Context);

            if (currentUser == null || !await _userManager.IsTenantAdmin(currentUser))
            {
                return new HttpUnauthorizedResult();
            }

            if (!await UserIsTenantAdminOfActivity(currentUser, id))
            {
                return new HttpUnauthorizedResult();
            }

            var updates = tasks.ToModel(_dataAccess).ToList();
            //TODO: Replacement for API like Tasks.UpdateRange(updates);
            foreach (var item in updates)
            {
                await _dataAccess.UpdateTask(item);
            }
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

        private async Task<bool> UserIsTenantAdminOfActivity(ApplicationUser user, Activity activity)
        {
            return await _userManager.IsSiteAdmin(user) ||
                ((user.AssociatedTenant != null) &&
                    (from campaign in user.AssociatedTenant.Campaigns
                     where campaign.Id == activity.CampaignId
                     select campaign).Any());
        }

        private async Task<bool> UserIsTenantAdminOfActivity(ApplicationUser user, int activityId)
        {
            return await UserIsTenantAdminOfActivity(user, _dataAccess.GetActivity(activityId));
        }

    }
}