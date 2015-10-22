using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;

using AllReady.Extensions;
using AllReady.Models;
using AllReady.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AllReady.Controllers
{
    [Route("api/task")]
    [Produces("application/json")]
    public class TaskApiController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAllReadyDataAccess _allReadyDataAccess;

        public TaskApiController(IAllReadyDataAccess allReadyDataAccess, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _allReadyDataAccess = allReadyDataAccess;
        }

        private async Task<bool> HasTaskEditPermissions(AllReadyTask task)
        {
            ApplicationUser currentUser = await _userManager.GetCurrentUser(HttpContext);
            IList<Claim> claims = await _userManager.GetClaimsForCurrentUser(HttpContext);
            if (claims.IsUserType(UserType.SiteAdmin))
            {
                return true;
            }

            if (claims.IsUserType(UserType.TenantAdmin))
            {
                //TODO: Modify to check that user is tenant admin for tenant of task
                return true;
            }

            if (task.Activity != null && task.Activity.Organizer != null && task.Activity.Organizer.Id == currentUser.Id)
            {
                return true;
            }

            if (task.Activity != null && task.Activity.Campaign != null && task.Activity.Campaign.Organizer != null && task.Activity.Campaign.Organizer.Id == currentUser.Id)
            {
                return true;
            }

            return false;
        }

        private async Task<bool> HasTaskSignupEditPermissions(AllReadyTask task)
        {
            if (await HasTaskEditPermissions(task))
            {
                return true;
            }
            else
            {
                ApplicationUser currentUser = await _userManager.GetCurrentUser(HttpContext);
                if (task.AssignedVolunteers != null && task.AssignedVolunteers.FirstOrDefault(x => x.User.Id == currentUser.Id) != null)
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
            bool hasPermissions = await HasTaskEditPermissions(task.ToModel(_allReadyDataAccess));
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

            bool hasPermissions = await HasTaskEditPermissions(task);
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
                bool hasPermissions = await HasTaskEditPermissions(matchingTask);
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

            ApplicationUser user = await _userManager.GetCurrentUser(HttpContext);

            if (task.AssignedVolunteers == null)
            {
                task.AssignedVolunteers = new List<TaskUsers>();
            }

            task.AssignedVolunteers.Add(new TaskUsers
            {
                Task = task,
                User = user,
                StatusDateTimeUtc = DateTime.UtcNow
            });

            await _allReadyDataAccess.UpdateTaskAsync(task);
        }
    }
}
