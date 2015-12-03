using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using AllReady.Models;
using AllReady.ViewModels;
using AllReady.Security;
using MediatR;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.Models;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("TenantAdmin")]
    public class TaskController : Controller
    {
        private readonly IAllReadyDataAccess _dataAccess;
        private readonly IMediator _bus;

        public TaskController(IAllReadyDataAccess dataAccess, IMediator bus)
        {
            _dataAccess = dataAccess;
            _bus = bus;
        }

        [HttpGet]
        [Route("Admin/Task/Create/{activityId}")]
        public IActionResult Create(int activityId)
        {
            var activity = _dataAccess.GetActivity(activityId);
            if (activity == null || !User.IsTenantAdmin(activity.Campaign.ManagingTenantId))
            {
                return HttpUnauthorized();
            }
            var viewModel = new TaskEditModel()
            {
                ActivityId = activity.Id,
                ActivityName = activity.Name,
                CampaignId = activity.CampaignId,
                CampaignName = activity.Campaign.Name,
                TenantId = activity.Campaign.ManagingTenantId,
                TimeZoneId = activity.Campaign.TimeZoneId
            };
            return View("Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Task/Create/{activityId}")]
        public IActionResult Create(int activityId, TaskEditModel model)
        {
            if (model.EndDateTime < model.StartDateTime)
            {
                ModelState.AddModelError("EndDateTime", "Ending time cannot be earlier than the starting time");
            }

            if (ModelState.IsValid)
            {
                if (!User.IsTenantAdmin(model.TenantId))
                {
                    return HttpUnauthorized();
                }
                _bus.Send(new EditTaskCommand() { Task = model });
                return RedirectToAction("Details", "Activity", new { id = activityId });
            }
            return View("Edit", model);
        }

        [HttpGet]
        [Route("Admin/Task/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var task = _bus.Send(new EditTaskQuery() { TaskId = id });
            if (task == null)
            {
                return HttpNotFound();
            }            
            if (!User.IsTenantAdmin(task.TenantId))
            {
                return HttpUnauthorized();
            }
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TaskEditModel model)
        {
            if (model.EndDateTime < model.StartDateTime)
            {
                ModelState.AddModelError("EndDateTime", "Ending time cannot be earlier than the starting time");
            }

            if (ModelState.IsValid)
            {
                if (!User.IsTenantAdmin(model.TenantId))
                {
                    return HttpUnauthorized();
                }
                _bus.Send(new EditTaskCommand() { Task = model });
                return RedirectToAction("Details", "Task", new { id = model.Id });
            }

            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var task = _bus.Send(new TaskQuery() { TaskId = id });
            if (task == null)
            {
                return HttpNotFound();
            }
            
            if (!User.IsTenantAdmin(task.TenantId))
            {
                return HttpUnauthorized();
            }

            return View(task);
        }

        [HttpGet]
        [Route("Admin/Task/Details/{id}")]
        public IActionResult Details(int id)
        {
            var task = _bus.Send(new TaskQuery() { TaskId = id });
            if (task == null)
            {
                return new HttpNotFoundResult();
            }
            return View(task);
        }
        // POST: Activity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var task = _bus.Send(new TaskQuery() { TaskId = id });
            if (task == null)
            {
                return HttpNotFound();
            }
            if (!User.IsTenantAdmin(task.TenantId))
            {
                return HttpUnauthorized();
            }
            var activityId = task.ActivityId;
            _bus.Send(new DeleteTaskCommand() { TaskId = id });
            return RedirectToAction("Details", "Activity", new { id = activityId });
        }


        private bool UserIsTenantAdminOfActivity(Activity activity)
        {
            return User.IsTenantAdmin(activity.Campaign.ManagingTenantId);
        }

        private bool UserIsTenantAdminOfActivity(int activityId)
        {
            return UserIsTenantAdminOfActivity(_dataAccess.GetActivity(activityId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Assign(int id, List<string> userIds)
        {
            var task = _bus.Send(new TaskQuery() { TaskId = id });
            
            if (!UserIsTenantAdminOfActivity(task.ActivityId))
            {
                return new HttpUnauthorizedResult();
            }
            
            _bus.Send(new AssignTaskCommand { TaskId = id, UserIds = userIds });


            return RedirectToRoute(new { controller = "Task", Area = "Admin", action = "Details", id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MessageAllVolunteers(MessageTaskVolunteersModel model)
        {
            //TODO: Query only for the tenant Id rather than the whole activity detail
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var task = _bus.Send(new TaskQuery { TaskId = model.TaskId });
            if (task == null)
            {
                return HttpNotFound();
            }

            if (!User.IsTenantAdmin(task.TenantId))
            {
                return HttpUnauthorized();
            }

            _bus.Send(new MessageTaskVolunteersCommand { Model = model });
            return Ok();
        }


    }
}
