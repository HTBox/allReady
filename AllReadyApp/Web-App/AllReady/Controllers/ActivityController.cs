﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using AllReady.Models;
using AllReady.ViewModels;
using Microsoft.AspNet.Authorization;
using System.Security.Claims;
using AllReady.Services;

namespace AllReady.Controllers
{
    public class ActivityController : Controller
    {
        private readonly IAllReadyDataAccess _allReadyDataAccess;

        public ActivityController(
            IAllReadyDataAccess allReadyDataAccess,
            IClosestLocations closestLocations)
        {
            _allReadyDataAccess = allReadyDataAccess;
        }

        [Route("~/MyActivities")]
        [Authorize]
        public IActionResult GetMyActivities()
        {
            var myActivities = _allReadyDataAccess.GetActivitySignups(User.GetUserId());
            var signedUp = myActivities.Select(a => new ActivityViewModel(a.Activity));
            var viewModel = new MyActivitiesResultsScreenViewModel("My Activities", signedUp);
            return View("MyActivities", viewModel);
        }

        [Route("~/MyActivities/{id}/tasks")]
        [Authorize]
        public IActionResult GetMyTasks(int id)
        {
            var tasks = _allReadyDataAccess.GetTasksAssignedToUser(id, User.GetUserId());

            var taskView = tasks.Select(t => new TaskSignupViewModel(t));

            return Json(taskView);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Route("~/MyActivities/{id}/tasks")]
        public async Task<IActionResult> UpdateMyTasks(int id, [FromBody]List<TaskSignupViewModel> model)
        {
            var currentUser = _allReadyDataAccess.GetUser(User.GetUserId());
            foreach (var taskSignup in model)
            {
                await _allReadyDataAccess.UpdateTaskSignupAsync(new TaskSignup
                {
                    Id = taskSignup.Id,
                    StatusDateTimeUtc = DateTime.UtcNow,
                    StatusDescription = taskSignup.StatusDescription,
                    Status = taskSignup.Status,
                    Task = new AllReadyTask { Id = taskSignup.TaskId },
                    User = currentUser
                });
            }
            var result = new
            {
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
        public IActionResult ShowActivity(int id)
        {
            var activity = _allReadyDataAccess.GetActivity(id);

            if (activity == null)
            {
                return HttpNotFound();
            }

            return View("Activity", new ActivityViewModel(activity).WithUserInfo(activity, User, _allReadyDataAccess));
        }

        
        [HttpGet]
        [Route("/Activity/Signup/{id}")]
        [AllowAnonymous]
        public IActionResult Signup(int id)
        {
            var returnUrl = $"/Activity/Signup/{id}";

            if (!User.IsSignedIn())
            {
                return RedirectToAction(nameof(AccountController.Login), "Account", new { ReturnUrl = returnUrl });
            }

            var user = _allReadyDataAccess.GetUser(User.GetUserId());

            // Maybe it wasn't logged in properly.
            if (user == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account", new { ReturnUrl = returnUrl });
            }

            //Otherwise, user is actually logged in, but shouldn't have been able to get here without signup info (punt back to activity)
            return RedirectToAction(nameof(ShowActivity), new { id = id });

        }

        [HttpPost]
        [Route("/Activity/Signup")]
        [Authorize]
        public async Task<IActionResult> Signup(ActivitySignupViewModel signupModel)
        {
            //TODO: do as command
            var user = _allReadyDataAccess.GetUser(User.GetUserId());
            var activity = _allReadyDataAccess.GetActivity(signupModel.ActivityId);

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

            return RedirectToAction(nameof(ShowActivity), new { id = signupModel.ActivityId });
        }

    }
}
