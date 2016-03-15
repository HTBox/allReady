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
using MediatR;
using Microsoft.AspNet.Authorization;

namespace AllReady.Controllers
{
    [Route("api/task")]
    [Produces("application/json")]
    public class TaskApiController : Controller
    {
        private readonly IAllReadyDataAccess _allReadyDataAccess;
        private readonly IMediator _mediator;
        private readonly IDetermineIfATaskIsEditable _determineIfATaskIsEditable;

        public TaskApiController(IAllReadyDataAccess allReadyDataAccess, IMediator mediator, IDetermineIfATaskIsEditable determineIfATaskIsEditable)
        {
            _allReadyDataAccess = allReadyDataAccess;
            _mediator = mediator;
            _determineIfATaskIsEditable = determineIfATaskIsEditable;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody]TaskViewModel task)
        {
            var allReadyTask = task.ToModel(_allReadyDataAccess);

            var hasPermissions = _determineIfATaskIsEditable.For(User, allReadyTask);
            if (!hasPermissions)
                return HttpUnauthorized();

            if (IfTaskExists(task))
                return HttpBadRequest();

            //@mgmccarthy: this should not be enforced here, it should be enforced as a guard clause on the constructor of TaskViewModel and tested in a unit test for that class
            var model = allReadyTask;
            if (model == null)
                return HttpBadRequest("Should have found a matching activity Id");

            //TODO: refactor to mediator
            await _allReadyDataAccess.AddTaskAsync(model);

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

            //TODO:refactor to mediator
            await _allReadyDataAccess.UpdateTaskAsync(allReadyTask);

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

            //TODO: refactor to mediator
            await _allReadyDataAccess.DeleteTaskAsync(allReadyTask.Id);

            //http://stackoverflow.com/questions/2342579/http-status-code-for-update-and-delete
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }

        [ValidateAntiForgeryToken]
        [HttpPost("signup")]
        [Authorize]
        [Produces("application/json")]
        public async Task<ActionResult> RegisterTask(ActivitySignupViewModel signupModel)
        {
            if (signupModel == null)
                return HttpBadRequest();

            if (!ModelState.IsValid)
            {
                // this condition should never be hit because client side validation is being performed
                // but just to cover the bases, if this does happen send the erros to the client
                return Json(new { errors = ModelState.GetErrorMessages() });
            }

            var result = await _mediator.SendAsync(new TaskSignupCommand { TaskSignupModel = signupModel });

            return Json(new { result.Status, Task = result.Task == null ? null : new TaskViewModel(result.Task, signupModel.UserId) });
        }

        [HttpDelete("{id}/signup")]
        [Authorize]
        public async Task<ActionResult> UnregisterTask(int id)
        {
            var userId = User.GetUserId();
            var result = await _mediator.SendAsync(new TaskUnenrollCommand { TaskId = id, UserId = userId });

            return Json(new { result.Status, Task = result.Task == null ? null : new TaskViewModel(result.Task, userId) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("changestatus")]
        [Authorize]
        public async Task<ActionResult> ChangeStatus(TaskChangeModel model)
        {
            var result = await _mediator.SendAsync(new TaskStatusChangeCommandAsync { TaskStatus = model.Status, TaskId = model.TaskId, UserId = model.UserId, TaskStatusDescription = model.StatusDescription });

            return Json(new { result.Status, Task = result.Task == null ? null : new TaskViewModel(result.Task, model.UserId) });
        }

        private bool IfTaskExists(TaskViewModel task) => GetTaskBy(task.Id) != null;

        private AllReadyTask GetTaskBy(int taskId) => _mediator.Send(new TaskByTaskIdQuery { TaskId = taskId });
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
                return true;

            if (user.IsUserType(UserType.OrgAdmin))
            {
                //TODO: Modify to check that user is organization admin for organization of task
                return true;
            }

            if (task.Activity?.Organizer != null && task.Activity.Organizer.Id == userId)
                return true;

            if (task.Activity?.Campaign?.Organizer != null && task.Activity.Campaign.Organizer.Id == userId)
                return true;

            return false;
        }
    }
}