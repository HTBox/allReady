using System.Net;
using Microsoft.AspNet.Mvc;
using AllReady.Security;
using AllReady.Models;
using AllReady.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Extensions;
using AllReady.Features.Tasks;
using AllReady.ViewModels.Shared;
using AllReady.ViewModels.Task;
using MediatR;
using Microsoft.AspNet.Authorization;
using DeleteTaskCommandAsync = AllReady.Features.Tasks.DeleteTaskCommandAsync;

namespace AllReady.Controllers
{
    [Route("api/task")]
    [Produces("application/json")]
    public class TaskApiController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IDetermineIfATaskIsEditable _determineIfATaskIsEditable;

        public const string FAILED_SIGNUP_TASK_CLOSED = "Signup failed - Task is closed";
        public const string FAILED_SIGNUP_EVENT_NOT_FOUND = "Signup failed - The event could not be found";
        public const string FAILED_SIGNUP_TASK_NOT_FOUND = "Signup failed - The task could not be found";
        public const string FAILED_SIGNUP_UNKOWN_ERROR = "Unkown error";

        public TaskApiController(IMediator mediator, IDetermineIfATaskIsEditable determineIfATaskIsEditable)
        {
            _mediator = mediator;
            _determineIfATaskIsEditable = determineIfATaskIsEditable;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody]TaskViewModel task)
        {
            var allReadyTask = task.ToModel(_mediator);
            if (allReadyTask == null)
            {
                return HttpBadRequest("Should have found a matching event Id");
            }

            var hasPermissions = _determineIfATaskIsEditable.For(User, allReadyTask);
            if (!hasPermissions)
            {
                return HttpUnauthorized();
            }

            if (IfTaskExists(task))
            {
                return HttpBadRequest();
            }

            await _mediator.SendAsync(new AddTaskCommandAsync { AllReadyTask = allReadyTask });

            //http://stackoverflow.com/questions/1860645/create-request-with-post-which-response-codes-200-or-201-and-content
            return new HttpStatusCodeResult((int)HttpStatusCode.Created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]TaskViewModel value)
        {
            var allReadyTask = GetTaskBy(id);

            if (allReadyTask == null)
                return HttpBadRequest();

            var hasPermissions = _determineIfATaskIsEditable.For(User, allReadyTask);
            if (!hasPermissions)
                return HttpUnauthorized();

            // Changing all the potential properties that the VM could have modified.
            allReadyTask.Name = value.Name;
            allReadyTask.Description = value.Description;
            allReadyTask.StartDateTime = value.StartDateTime.Value.UtcDateTime;
            allReadyTask.EndDateTime = value.EndDateTime.Value.UtcDateTime;

            await _mediator.SendAsync(new UpdateTaskCommandAsync { AllReadyTask = allReadyTask });

            //http://stackoverflow.com/questions/2342579/http-status-code-for-update-and-delete
            return new HttpStatusCodeResult((int)HttpStatusCode.NoContent);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var allReadyTask = GetTaskBy(id);

            if (allReadyTask == null)
                return HttpBadRequest();

            var hasPermissions = _determineIfATaskIsEditable.For(User, allReadyTask);
            if (!hasPermissions)
                return HttpUnauthorized();

            await _mediator.SendAsync(new DeleteTaskCommandAsync { TaskId = allReadyTask.Id });

            //http://stackoverflow.com/questions/2342579/http-status-code-for-update-and-delete
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }

        [ValidateAntiForgeryToken]
        [HttpPost("signup")]
        [Authorize]
        [Produces("application/json")]
        public async Task<ActionResult> RegisterTask(EventSignupViewModel signupModel)
        {
            if (signupModel == null)
            {
                return HttpBadRequest();
            }
            
            if (!ModelState.IsValid)
            {
                // this condition should never be hit because client side validation is being performed
                // but just to cover the bases, if this does happen send the erros to the client
                return Json(new { errors = ModelState.GetErrorMessages() });
            }

            var result = await _mediator.SendAsync(new TaskSignupCommandAsync { TaskSignupModel = signupModel });

            switch (result.Status)
            {
                case TaskSignupResult.SUCCESS:
                    return Json(new
                    {
                        isSuccess = true,
                        task = result.Task == null ? null : new TaskViewModel(result.Task, signupModel.UserId)
                    });

                case TaskSignupResult.FAILURE_CLOSEDTASK:
                    return Json(new {
                        isSuccess = false,
                        errors = new string[] { FAILED_SIGNUP_TASK_CLOSED },
                    });

                case TaskSignupResult.FAILURE_EVENTNOTFOUND:
                    return Json(new
                    {
                        isSuccess = false,
                        errors = new string[] { FAILED_SIGNUP_EVENT_NOT_FOUND },
                    });

                case TaskSignupResult.FAILURE_TASKNOTFOUND:
                    return Json(new
                    {
                        isSuccess = false,
                        errors = new string[] { FAILED_SIGNUP_TASK_NOT_FOUND },
                    });

                default:
                    return Json(new {
                        isSuccess = false,
                        errors = new string[] { FAILED_SIGNUP_UNKOWN_ERROR },
                    });
            }
        }

        [HttpDelete("{id}/signup")]
        [Authorize]
        public async Task<JsonResult> UnregisterTask(int id)
        {
            var userId = User.GetUserId();

            var result = await _mediator.SendAsync(new TaskUnenrollCommand { TaskId = id, UserId = userId });

            return Json(new
            {
                result.Status,
                Task = result.Task == null ? null : new TaskViewModel(result.Task, userId)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("changestatus")]
        [Authorize]
        public async Task<JsonResult> ChangeStatus(TaskChangeModel model)
        {
            var result = await _mediator.SendAsync(new TaskStatusChangeCommandAsync { TaskStatus = model.Status, TaskId = model.TaskId, UserId = model.UserId, TaskStatusDescription = model.StatusDescription });
            return Json(new { result.Status, Task = result.Task == null ? null : new TaskViewModel(result.Task, model.UserId) });
        }

        private bool IfTaskExists(TaskViewModel task)
        {
            return GetTaskBy(task.Id) != null;
        }

        private AllReadyTask GetTaskBy(int taskId)
        {
            return _mediator.Send(new TaskByTaskIdQuery { TaskId = taskId });
        }
    }

    public interface IDetermineIfATaskIsEditable
    {
        bool For(ClaimsPrincipal user, AllReadyTask task);
    }

    public class DetermineIfATaskIsEditable : IDetermineIfATaskIsEditable
    {
        public bool For(ClaimsPrincipal user, AllReadyTask task)
        {
            var userId = user.GetUserId();

            if (user.IsUserType(UserType.SiteAdmin))
            {
                return true;
            }

            if (user.IsUserType(UserType.OrgAdmin))
            {
                //TODO: Modify to check that user is organization admin for organization of task
                return true;
            }

            if (task.Event?.Organizer != null && task.Event.Organizer.Id == userId)
            {
                return true;
            }

            if (task.Event?.Campaign?.Organizer != null && task.Event.Campaign.Organizer.Id == userId)
            {
                return true;
            }

            return false;
        }
    }
}