using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Features.Activity;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using TaskStatus = AllReady.Areas.Admin.Features.Tasks.TaskStatus;

namespace AllReady.Controllers
{
    public class ActivityController : Controller
    {
        private readonly IMediator _mediator;

        public ActivityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("~/MyActivities")]
        [Authorize]
        public IActionResult GetMyActivities()
        {
            var viewModel = _mediator.Send(new GetMyActivitiesQuery { UserId = User.GetUserId() });
            return View("MyActivities", viewModel);
        }

        [Route("~/MyActivities/{id}/tasks")]
        [Authorize]
        public IActionResult GetMyTasks(int id)
        {
            var view = _mediator.Send(new GetMyTasksQuery { ActivityId = id, UserId = User.GetUserId() });
            return Json(view);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Route("~/MyActivities/{id}/tasks")]
        public async Task<IActionResult> UpdateMyTasks(int id, [FromBody] List<TaskSignupViewModel> model)
        {
            await _mediator.SendAsync(new UpdateMyTasksCommandAsync { TaskSignups = model, UserId = User.GetUserId() });
            return Json(new { success = true });
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
            var viewModel = _mediator.Send(new ShowActivityQuery { ActivityId = id, User = User });
            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return viewModel.ActivityType == ActivityTypes.ActivityManaged
                ? View("Activity", viewModel)
                : View("ActivityWithTasks", viewModel);
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
                await _mediator.SendAsync(new ActivitySignupCommand { ActivitySignup = signupModel });
            }
            
                //TODO: handle invalid activity signup info (phone, email) in a useful way
                //  would be best to handle it in KO on the client side (prevent clicking Volunteer)

            return RedirectToAction(nameof(ShowActivity), new { id = signupModel.ActivityId });
        }

        [HttpGet]
        [Route("/Activity/ChangeStatus")]
        [Authorize]
        public async Task<IActionResult> ChangeStatus(int activityId, int taskId, string userId, TaskStatus status, string statusDesc)
        {
            if (userId == null)
                return HttpBadRequest();
        [HttpGet]
        [Route("/Activity/ChangeStatus")]
        [Authorize]
        public async Task<IActionResult> ChangeStatus(int activityId, int taskId, string userId, TaskStatus status, string statusDesc)
        {
            if (userId == null)
            {
                return HttpBadRequest();
            }

            await _mediator.SendAsync(new TaskStatusChangeCommandAsync { TaskStatus = status, TaskId = taskId, UserId = userId, TaskStatusDescription = statusDesc });

            return RedirectToAction(nameof(ShowActivity), new { id = activityId });
        }
    }
}
