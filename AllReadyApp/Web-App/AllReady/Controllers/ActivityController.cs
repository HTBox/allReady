using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using AllReady.Models;
using AllReady.ViewModels;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using AllReady.Services;

namespace AllReady.Controllers
{
    public class ActivityController : Controller
    {
        private readonly IAllReadyDataAccess _allReadyDataAccess;
        private readonly UserManager<ApplicationUser> _userManager;

        public ActivityController(
            IAllReadyDataAccess allReadyDataAccess,
            UserManager<ApplicationUser> userManager,
            IClosestLocations closestLocations)
        {
            _allReadyDataAccess = allReadyDataAccess;
            _userManager = userManager;
        }

        [Route("~/MyActivities")]
        [Authorize]
        public async Task<IActionResult> GetMyActivities()
        {
            var user = await GetCurrentUserAsync();
            var myActivities = _allReadyDataAccess.GetActivitySignups(user);
            var signedUp = myActivities.Select(a => new ActivityViewModel(a.Activity));
            var viewModel = new MyActivitiesResultsScreenViewModel("My Activities", signedUp);
            return View("MyActivities", viewModel);
        }

        [Route("~/MyActivities/{id}/tasks")]
        [Authorize]
        public async Task<IActionResult> GetMyTasks(int id)
        {
            var user = await GetCurrentUserAsync();

            var tasks = _allReadyDataAccess.GetTasksAssignedToUser(id, user);

            var taskView = tasks.Select(t => new TaskSignupViewModel(t));

            return Json(taskView);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Route("~/MyActivities/{id}/tasks")]
        public async Task<IActionResult> UpdateMyTasks(int id, [FromBody]List<TaskSignupViewModel> model) {
            var currentUser = await GetCurrentUserAsync();
            foreach (var taskSignup in model) {
                await _allReadyDataAccess.UpdateTaskSignupAsync(new TaskUsers {
                    Id = taskSignup.Id,
                    StatusDateTimeUtc = DateTime.UtcNow,
                    StatusDescription = taskSignup.StatusDescription,
                    Status = taskSignup.Status,
                    Task = new AllReadyTask { Id = taskSignup.TaskId },
                    User = currentUser
                });
            }
            var result = new {
                success = true
            };
            return Json(result);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Activities");
        }

        [Route("[controller]/{id}/")]
        [AllowAnonymous]
        public async Task<IActionResult> ShowActivity(int id)
        {
            var activity = _allReadyDataAccess.GetActivity(id);

            if (activity == null)
            {
                return HttpNotFound();
            }

            var isUserSignedUpForActivity = User.IsSignedIn();

            if (!isUserSignedUpForActivity)
            {
                return View("Activity", new ActivityViewModel(activity, false));
            }

            var user = await GetCurrentUserAsync();
            var signedUp = _allReadyDataAccess.GetActivitySignups(id, user);

            isUserSignedUpForActivity = signedUp.Any();

            return View("Activity", new ActivityViewModel(activity, isUserSignedUpForActivity));
        }

        [HttpGet]
        [Route("/Activity/Signup/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Signup(int id)
        {
            var returnUrl = $"/Activity/Signup/{id}";

            if (!User.IsSignedIn())
            {
                return RedirectToAction(nameof(AccountController.Login), "Account", new { ReturnUrl = returnUrl });
            }

            var user = await GetCurrentUserAsync();

            // Maybe it wasn't logged in properly.
            if (user == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account", new { ReturnUrl = returnUrl });
            }

            var activity = _allReadyDataAccess.GetActivity(id);

            if (activity == null)
            {
                return HttpNotFound();
            }

            if (activity.UsersSignedUp == null)
            {
                activity.UsersSignedUp = new List<ActivitySignup>();
            }
            // If the user clicks multiple times, they may already be signed up.
            if (!(from actSignup in activity.UsersSignedUp
                  where actSignup.User.Id == user.Id
                  select actSignup).Any())
            {

                activity.UsersSignedUp.Add(new ActivitySignup
                {
                    Activity = activity,
                    User = user,
                    SignupDateTime = DateTime.UtcNow
                });

                await _allReadyDataAccess.UpdateActivity(activity);
            }

            return RedirectToAction(nameof(GetMyActivities));
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(Context.User.GetUserId());
        }
    }
}
