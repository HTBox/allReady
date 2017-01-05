using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AllReady.Security;
using AllReady.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Extensions;
using AllReady.Features.Manage;
using AllReady.Features.Tasks;
using AllReady.ViewModels.Shared;
using AllReady.ViewModels.Task;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using DeleteTaskCommand = AllReady.Features.Tasks.DeleteTaskCommand;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using AllReady.Features.Events;

namespace AllReady.Controllers
{
    [Route("api/task")]
    [Produces("application/json")]
    public class TaskApiController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IDetermineIfATaskIsEditable _determineIfATaskIsEditable;
        private readonly UserManager<ApplicationUser> _userManager;

        public TaskApiController(IMediator mediator, IDetermineIfATaskIsEditable determineIfATaskIsEditable, UserManager<ApplicationUser> userManager)
        {
          _mediator = mediator;
          _determineIfATaskIsEditable = determineIfATaskIsEditable;
          _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody]TaskViewModel task)
        {
            var allReadyTask = await ToModel(task, _mediator);
            if (allReadyTask == null)
            {
                return BadRequest("Should have found a matching event Id");
            }

            var hasPermissions = _determineIfATaskIsEditable.For(User, allReadyTask, _userManager);
            if (!hasPermissions)
            {
                return Unauthorized();
            }

            if (await TaskExists(task.Id))
            {
                return BadRequest();
            }

            await _mediator.SendAsync(new AddTaskCommand { AllReadyTask = allReadyTask });

            //http://stackoverflow.com/questions/1860645/create-request-with-post-which-response-codes-200-or-201-and-content
            return Created("", allReadyTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]TaskViewModel value)
        {
            var allReadyTask = await GetTaskBy(id);
            if (allReadyTask == null)
            {
                return BadRequest();
            }
            
            var hasPermissions = _determineIfATaskIsEditable.For(User, allReadyTask, _userManager);
            if (!hasPermissions)
            {
                return Unauthorized();
            }
            
            // Changing all the potential properties that the VM could have modified.
            allReadyTask.Name = value.Name;
            allReadyTask.Description = value.Description;
            allReadyTask.StartDateTime = value.StartDateTime.UtcDateTime;
            allReadyTask.EndDateTime = value.EndDateTime.UtcDateTime;

            await _mediator.SendAsync(new UpdateTaskCommand { AllReadyTask = allReadyTask });

            //http://stackoverflow.com/questions/2342579/http-status-code-for-update-and-delete
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var allReadyTask = await GetTaskBy(id);
            if (allReadyTask == null)
            {
                return BadRequest();
            }
            
            var hasPermissions = _determineIfATaskIsEditable.For(User, allReadyTask, _userManager);
            if (!hasPermissions)
            {
                return Unauthorized();
            }
            
            await _mediator.SendAsync(new DeleteTaskCommand { TaskId = allReadyTask.Id });

            //http://stackoverflow.com/questions/2342579/http-status-code-for-update-and-delete
            return Ok();
        }

        //called from AllReady\wwwroot\js\event.js
        [ValidateAntiForgeryToken]
        [HttpPost("signup")]
        [Authorize]
        [Produces("application/json")]
        public async Task<ActionResult> RegisterTask(TaskSignupViewModel signupModel)
        {
            if (signupModel == null)
            {
                return BadRequest();
            }
            
            if (!ModelState.IsValid)
            {
                // this condition should never be hit because client side validation is being performed
                // but just to cover the bases, if this does happen send the erros to the client
                return Json(new { errors = ModelState.GetErrorMessages() });
            }

            var result = await _mediator.SendAsync(new TaskSignupCommand { TaskSignupModel = signupModel });

            switch (result.Status)
            {
            case TaskSignupResult.SUCCESS:
              return Json(new
              {
                isSuccess = true,
                task = result.Task == null ? null : new TaskViewModel(result.Task, signupModel.UserId)
              });

            case TaskSignupResult.FAILURE_CLOSEDTASK:
              return Json(new
              {
                isSuccess = false,
                errors = new[] { "Signup failed - Task is closed" },
              });

            case TaskSignupResult.FAILURE_EVENTNOTFOUND:
              return Json(new
              {
                isSuccess = false,
                errors = new[] { "Signup failed - The event could not be found" },
              });

            case TaskSignupResult.FAILURE_TASKNOTFOUND:
              return Json(new
              {
                isSuccess = false,
                errors = new[] { "Signup failed - The task could not be found" },
              });

            default:
              return Json(new
              {
                isSuccess = false,
                errors = new[] { "Unkown error" },
              });
            }
        }

        //called from AllReady\wwwroot\js\event.js
        [HttpDelete("{id}/signup")]
        [Authorize]
        public async Task<JsonResult> UnregisterTask(int id)
        {
            var userId = _userManager.GetUserId(User);

            var result = await _mediator.SendAsync(new TaskUnenrollCommand { TaskId = id, UserId = userId });

            return Json(new
            {
                result.Status,
                Task = result.Task == null ? null : new TaskViewModel(result.Task, userId)
            });
        }

        //called from AllReady\wwwroot\js\event.js
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("changestatus")]
        [Authorize]
        public async Task<JsonResult> ChangeStatus(TaskChangeModel model)
        {
            var result = await _mediator.SendAsync(new ChangeTaskStatusCommand { TaskStatus = model.Status, TaskId = model.TaskId, UserId = model.UserId, TaskStatusDescription = model.StatusDescription });
            return Json(new { result.Status, Task = result.Task == null ? null : new TaskViewModel(result.Task, model.UserId) });
        }

        private async Task<bool> TaskExists(int taskId)
        {
            return await GetTaskBy(taskId) != null;
        }

        private async Task<AllReadyTask> GetTaskBy(int taskId)
        {
            return await _mediator.SendAsync(new TaskByTaskIdQuery { TaskId = taskId });
        }

        private async Task<AllReadyTask> ToModel(TaskViewModel taskViewModel, IMediator mediator)
        {
            var @event = await mediator.SendAsync(new EventByEventIdQuery { EventId = taskViewModel.EventId });
            if (@event == null)
            {
                return null;
            }

            var newTask = true;
            AllReadyTask allReadyTask;
            if (taskViewModel.Id == 0)
            {
                allReadyTask = new AllReadyTask();
            }
            else
            {
                allReadyTask = await mediator.SendAsync(new TaskByTaskIdQuery { TaskId = taskViewModel.Id });
                newTask = false;
            }

            allReadyTask.Id = taskViewModel.Id;
            allReadyTask.Description = taskViewModel.Description;
            allReadyTask.Event = @event;
            allReadyTask.EndDateTime = taskViewModel.EndDateTime.UtcDateTime;
            allReadyTask.StartDateTime = taskViewModel.StartDateTime.UtcDateTime;
            allReadyTask.Name = taskViewModel.Name;
            allReadyTask.RequiredSkills = allReadyTask.RequiredSkills ?? new List<TaskSkill>();
            taskViewModel.RequiredSkills = taskViewModel.RequiredSkills ?? new List<int>();
            ////Remove old skills
            //dbtask.RequiredSkills.RemoveAll(ts => !taskViewModel.RequiredSkills.Any(s => ts.SkillId == s));
            ////Add new skills
            //dbtask.RequiredSkills.AddRange(taskViewModel.RequiredSkills
            //    .Where(rs => !dbtask.RequiredSkills.Any(ts => ts.SkillId == rs))
            //    .Select(rs => new TaskSkill() { SkillId = rs, TaskId = taskViewModel.Id }));

            // Workaround:  POST is bringing in empty AssignedVolunteers.  Clean this up. Discussing w/ Kiran Challa.
            // Workaround: the if statement is superflous, and should go away once we have the proper fix referenced above.
            if (taskViewModel.AssignedVolunteers != null)
            {
                var bogusAssignedVolunteers = (from assignedVolunteer in taskViewModel.AssignedVolunteers
                                               where string.IsNullOrEmpty(assignedVolunteer.UserId)
                                               select assignedVolunteer).ToList();
                foreach (var bogus in bogusAssignedVolunteers)
                {
                    taskViewModel.AssignedVolunteers.Remove(bogus);
                }
            }
            // end workaround

            if (taskViewModel.AssignedVolunteers != null && taskViewModel.AssignedVolunteers.Count > 0)
            {
                var assignedVolunteers = taskViewModel.AssignedVolunteers.ToList();

                var taskUsersList = await assignedVolunteers.ToTaskSignups(allReadyTask, _mediator);

                // We may be updating an existing task
                if (newTask || allReadyTask.AssignedVolunteers.Count == 0)
                {
                    allReadyTask.AssignedVolunteers = taskUsersList;
                }
                else
                {
                    // Can probably rewrite this more efficiently.
                    foreach (var taskUsers in taskUsersList)
                    {
                        if (!(from entry in allReadyTask.AssignedVolunteers
                              where entry.User.Id == taskUsers.User.Id
                              select entry).Any())
                        {
                            allReadyTask.AssignedVolunteers.Add(taskUsers);
                        }
                    }
                }
            }
            return allReadyTask;
        }
    }

    public static class TaskSignupViewModelExtensions
    {
        public static async Task<List<TaskSignup>> ToTaskSignups(this List<ViewModels.Event.TaskSignupViewModel> viewModels, AllReadyTask task, IMediator mediator)
        {
            var taskSignups = new List<TaskSignup>();
            foreach (var viewModel in viewModels)
            {
                taskSignups.Add(new TaskSignup
                {
                    Task = task,
                    User = await mediator.SendAsync(new UserByUserIdQuery { UserId = viewModel.UserId })
                });
            }

            return taskSignups;
        }
    }

    public interface IDetermineIfATaskIsEditable
    {
        bool For(ClaimsPrincipal user, AllReadyTask task, UserManager<ApplicationUser> userManager);
    }

    public class DetermineIfATaskIsEditable : IDetermineIfATaskIsEditable
    {
        public bool For(ClaimsPrincipal user, AllReadyTask task, UserManager<ApplicationUser> userManager)
        {
            var userId = userManager.GetUserId(user);

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