using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using AllReady.Models;
using AllReady.ViewModels;
using Microsoft.AspNet.Authorization;
using System.Security.Claims;
using MediatR;
using AllReady.Features.Activity;
using AllReady.Areas.Admin.Features.Tasks;
using TaskStatus = AllReady.Areas.Admin.Features.Tasks.TaskStatus;
namespace AllReady.Controllers
{
    public class ActivityController : Controller
    {
        private readonly IAllReadyDataAccess _allReadyDataAccess;
        private readonly IMediator _bus;

        public ActivityController(IAllReadyDataAccess allReadyDataAccess, IMediator bus)
        {
            _allReadyDataAccess = allReadyDataAccess;
            _bus = bus;
        }

        [Route("~/MyActivities")]
        [Authorize]
        public IActionResult GetMyActivities()
        {
            var myActivities = _allReadyDataAccess.GetActivitySignups(User.GetUserId()).Where(a => !a.Activity.Campaign.Locked);
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

            if (activity == null || activity.Campaign.Locked)
            {
                return HttpNotFound();
            }

            return View("Activity", new ActivityViewModel(activity).WithUserInfo(activity, User, _allReadyDataAccess));
        }

        [HttpPost]
        [Route("/Activity/Signup")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(ActivitySignupViewModel signupModel)
        {
            if (signupModel == null)
            {
                return HttpBadRequest();
            }

            if (ModelState.IsValid)
            {
                await _bus.SendAsync(new ActivitySignupCommand() { ActivitySignup = signupModel });
            }
            else
            {
                //TODO: handle invalid activity signup info (phone, email) in a useful way
                //  would be best to handle it in KO on the client side (prevent clicking Volunteer)
            }

            return RedirectToAction(nameof(ShowActivity), new { id = signupModel.ActivityId });
        }

        [HttpGet]
        [Route("/Activity/ChangeStatus")]
        [Authorize]
        public IActionResult ChangeStatus(int activityId, int taskId, string userId, TaskStatus status, string statusDesc)
        {
            if (userId == null)
            {
                return HttpBadRequest();
            }

            _bus.Send(new TaskStatusChangeCommand { TaskStatus = status, TaskId = taskId, UserId = userId, TaskStatusDescription = statusDesc });

            return RedirectToAction(nameof(ShowActivity), new { id = activityId });
        }

    }
}
