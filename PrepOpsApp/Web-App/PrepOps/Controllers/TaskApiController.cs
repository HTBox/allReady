using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;

using PrepOps.Extensions;
using PrepOps.Models;
using PrepOps.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrepOps.Controllers
{
    [Route("api/task")]
    [Produces("application/json")]
    public class TaskApiController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPrepOpsDataAccess _prepOpsDataAccess;

        public TaskApiController(IPrepOpsDataAccess prepOpsDataAccess, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _prepOpsDataAccess = prepOpsDataAccess;
        }

        private async Task<bool> HasTaskEditPermissions(PrepOpsTask task)
        {
            ApplicationUser currentUser = await _userManager.GetCurrentUser(Context);
            IList<Claim> claims = await _userManager.GetClaimsForCurrentUser(Context);
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

        private async Task<bool> HasTaskSignupEditPermissions(PrepOpsTask task)
        {
            if (await HasTaskEditPermissions(task))
            {
                return true;
            }
            else
            {
                ApplicationUser currentUser = await _userManager.GetCurrentUser(Context);
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
            bool hasPermissions = await HasTaskEditPermissions(task.ToModel(_prepOpsDataAccess));
            if (!hasPermissions)
            {
                HttpUnauthorized();
            }

            bool alreadyExists = _prepOpsDataAccess.GetTask(task.Id) != null;

            if (alreadyExists)
            {
                HttpBadRequest();
            }

            var model = task.ToModel(_prepOpsDataAccess);
            if (model == null)
            {
                HttpBadRequest("Should have found a matching activity Id");
            }

            await _prepOpsDataAccess.AddTask(model);
        }

        [HttpPut("{id}")]
        public async void Put(int id, [FromBody]TaskViewModel value)
        {
            var task = _prepOpsDataAccess.GetTask(id);

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

            await _prepOpsDataAccess.UpdateTask(task);
        }

        [HttpDelete("{id}")]
        public async void Delete(int id)
        {
            var matchingTask = _prepOpsDataAccess.GetTask(id);

            if (matchingTask != null)
            {
                bool hasPermissions = await HasTaskEditPermissions(matchingTask);
                if (!hasPermissions)
                {
                    HttpUnauthorized();
                }
                await _prepOpsDataAccess.DeleteTask(matchingTask.Id);
            }
        }

        [HttpGet]
        [Route("/Signup/{taskId}")]
        [Produces("application/json", Type = typeof(TaskViewModel))]
        public async void Signup(int taskId, string userId = "")
        {
            var task = _prepOpsDataAccess.GetTask(taskId);

            if (task == null)
            {
                HttpNotFound();
            }

            ApplicationUser user = await _userManager.GetCurrentUser(Context);

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

            await _prepOpsDataAccess.UpdateTask(task);
        }
    }
}
