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

        public TaskApiController(IAllReadyDataAccess allReadyDataAccess, IMediator mediator)
        {
            _allReadyDataAccess = allReadyDataAccess;
            _mediator = mediator;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async void Post([FromBody]TaskViewModel task)
        {
            var hasPermissions = HasTaskEditPermissions(task.ToModel(_allReadyDataAccess));
            if (!hasPermissions)
                HttpUnauthorized();

            var alreadyExists = _allReadyDataAccess.GetTask(task.Id) != null;
            if (alreadyExists)
                HttpBadRequest();

            var model = task.ToModel(_allReadyDataAccess);
            if (model == null)
                HttpBadRequest("Should have found a matching activity Id");

            await _allReadyDataAccess.AddTaskAsync(model);
        }

        [HttpPut("{id}")]
        public async void Put(int id, [FromBody]TaskViewModel value)
        {
            var task = _allReadyDataAccess.GetTask(id);

            var hasPermissions = HasTaskEditPermissions(task);
            if (!hasPermissions)
                HttpUnauthorized();

            if (task == null)
                HttpBadRequest();

            // Changing all the potential properties that the VM could have modified.
            task.Name = value.Name;
            task.Description = value.Description;
            task.StartDateTime = value.StartDateTime.Value.UtcDateTime;
            task.EndDateTime = value.EndDateTime.Value.UtcDateTime;

            await _allReadyDataAccess.UpdateTaskAsync(task);
        }

        [HttpDelete("{id}")]
        public async void Delete(int id)
        {
            var matchingTask = _allReadyDataAccess.GetTask(id);

            if (matchingTask != null)
            {
                var hasPermissions = HasTaskEditPermissions(matchingTask);
                if (!hasPermissions)
                    HttpUnauthorized();

                await _allReadyDataAccess.DeleteTaskAsync(matchingTask.Id);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost("signup")]
        [Authorize]
        [Produces("application/json")]
        public async Task<object> RegisterTask(ActivitySignupViewModel signupModel)
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

            return new { result.Status, Task = result.Task == null ? 
                null : 
                new TaskViewModel(result.Task, signupModel.UserId) };
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
        public async Task<object> ChangeStatus(TaskChangeModel model)
        {
            var result = await _mediator.SendAsync(new TaskStatusChangeCommand { TaskStatus = model.Status, TaskId = model.TaskId, UserId = model.UserId, TaskStatusDescription = model.StatusDescription });
            return new { Status = result.Status, Task = (result.Task == null) ? null : new TaskViewModel(result.Task, model.UserId) };
        }
    }
}
