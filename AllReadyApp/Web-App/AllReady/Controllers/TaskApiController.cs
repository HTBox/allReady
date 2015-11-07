using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;

using AllReady.Security;
using AllReady.Models;
using AllReady.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace AllReady.Controllers
{
    [Route("api/task")]
    [Produces("application/json")]
    public class TaskApiController : Controller
    {
        private readonly IAllReadyDataAccess _allReadyDataAccess;

        public TaskApiController(IAllReadyDataAccess allReadyDataAccess)
        {
            _allReadyDataAccess = allReadyDataAccess;
        }

        private bool HasTaskEditPermissions(AllReadyTask task)
        {
            var userId = User.GetUserId();
            if (User.IsUserType(UserType.SiteAdmin))
            {
                return true;
            }

            if (User.IsUserType(UserType.TenantAdmin))
            {
                //TODO: Modify to check that user is tenant admin for tenant of task
                return true;
            }

            if (task.Activity != null && task.Activity.Organizer != null && task.Activity.Organizer.Id == userId)
            {
                return true;
            }

            if (task.Activity != null && task.Activity.Campaign != null && task.Activity.Campaign.Organizer != null && task.Activity.Campaign.Organizer.Id == userId)
            {
                return true;
            }

            return false;
        }

        private bool HasTaskSignupEditPermissions(AllReadyTask task)
        {
            if (HasTaskEditPermissions(task))
            {
                return true;
            }
            else
            {
                var userId = User.GetUserId();                
                if (task.AssignedVolunteers != null && task.AssignedVolunteers.FirstOrDefault(x => x.User.Id == userId) != null)
                {
                    return true;
                }
                else { return false; }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async void Post([FromBody]TaskViewModel task)
        {
            bool hasPermissions = HasTaskEditPermissions(task.ToModel(_allReadyDataAccess));
            if (!hasPermissions)
            {
                HttpUnauthorized();
            }

            bool alreadyExists = _allReadyDataAccess.GetTask(task.Id) != null;

            if (alreadyExists)
            {
                HttpBadRequest();
            }

            var model = task.ToModel(_allReadyDataAccess);
            if (model == null)
            {
                HttpBadRequest("Should have found a matching activity Id");
            }

            await _allReadyDataAccess.AddTaskAsync(model);
        }

        [HttpPut("{id}")]
        public async void Put(int id, [FromBody]TaskViewModel value)
        {
            var task = _allReadyDataAccess.GetTask(id);

            bool hasPermissions = HasTaskEditPermissions(task);
            if (!hasPermissions)
            {
                HttpUnauthorized();
            }

            if (task == null)
            {
                HttpBadRequest();
            }

            // Changing all the potential properties that the VM could have modified.
            task.Name = value.Name;
            task.Description = value.Description;
            task.StartDateTimeUtc = value.StartDateTime.Value.UtcDateTime;
            task.EndDateTimeUtc = value.EndDateTime.Value.UtcDateTime;

            await _allReadyDataAccess.UpdateTaskAsync(task);
        }

        [HttpDelete("{id}")]
        public async void Delete(int id)
        {
            var matchingTask = _allReadyDataAccess.GetTask(id);

            if (matchingTask != null)
            {
                bool hasPermissions = HasTaskEditPermissions(matchingTask);
                if (!hasPermissions)
                {
                    HttpUnauthorized();
                }
                await _allReadyDataAccess.DeleteTaskAsync(matchingTask.Id);
            }
        }

        [HttpGet]
        [Route("/Signup/{taskId}")]
        [Produces("application/json", Type = typeof(TaskViewModel))]
        public async void Signup(int taskId, string userId = "")
        {
            var task = _allReadyDataAccess.GetTask(taskId);

            if (task == null)
            {
                HttpNotFound();
            }

            ApplicationUser user = _allReadyDataAccess.GetUser(User.GetUserId());

            if (task.AssignedVolunteers == null)
            {
                task.AssignedVolunteers = new List<TaskSignup>();
            }

            task.AssignedVolunteers.Add(new TaskSignup
            {
                Task = task,
                User = user,
                StatusDateTimeUtc = DateTime.UtcNow
            });

            await _allReadyDataAccess.UpdateTaskAsync(task);
        }
    }
}
