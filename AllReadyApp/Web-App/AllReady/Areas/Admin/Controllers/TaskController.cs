using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Features.Activity;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class TaskController : Controller
    {
        private readonly IMediator _mediator;

        public TaskController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("Admin/Task/Create/{activityId}")]
        public IActionResult Create(int activityId)
        {
            var activity = GetActivityBy(activityId);
            if (activity == null || !User.IsOrganizationAdmin(activity.Campaign.ManagingOrganizationId))
                return HttpUnauthorized();
            
            var viewModel = new TaskEditModel
            {
                ActivityId = activity.Id,
                ActivityName = activity.Name,
                CampaignId = activity.CampaignId,
                CampaignName = activity.Campaign.Name,
                OrganizationId = activity.Campaign.ManagingOrganizationId,
                TimeZoneId = activity.Campaign.TimeZoneId,
                StartDateTime = activity.StartDateTime,
                EndDateTime = activity.EndDateTime
            };

            return View("Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Task/Create/{activityId}")]
        public async Task<IActionResult> Create(int activityId, TaskEditModel model)
        {
            if (model.EndDateTime < model.StartDateTime)
                ModelState.AddModelError(nameof(model.EndDateTime), "Ending time cannot be earlier than the starting time");

            WarnDateTimeOutOfRange(ref model);

            if (ModelState.IsValid)
            {
                if (!User.IsOrganizationAdmin(model.OrganizationId))
                    return HttpUnauthorized();
                
                await _mediator.SendAsync(new EditTaskCommand { Task = model });

                //mgmccarthy: I'm assuming this is ActivityController in the Admin area
                return RedirectToAction(nameof(ActivityController.Details), "Activity", new { id = activityId });
            }

            return View("Edit", model);
        }

        [HttpGet]
        [Route("Admin/Task/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _mediator.SendAsync(new EditTaskQuery { TaskId = id });
            if (task == null)
                return HttpNotFound();

            if (!User.IsOrganizationAdmin(task.OrganizationId))
                return HttpUnauthorized();

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskEditModel model)
        {
            if (model.EndDateTime < model.StartDateTime)
                ModelState.AddModelError(nameof(model.EndDateTime), "Ending time cannot be earlier than the starting time");

            WarnDateTimeOutOfRange(ref model);

            if (ModelState.IsValid)
            {
                if (!User.IsOrganizationAdmin(model.OrganizationId))
                    return HttpUnauthorized();

                await _mediator.SendAsync(new EditTaskCommand { Task = model });

                return RedirectToAction(nameof(Details), "Task", new { id = model.Id });
            }

            return View(model);
        }

        //mgmccarthy: does anyone know why there are no attributes here on this action method?
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _mediator.SendAsync(new TaskQuery { TaskId = id });
            if (task == null)
                return HttpNotFound();
            
            if (!User.IsOrganizationAdmin(task.OrganizationId))
                return HttpUnauthorized();

            return View(task);
        }

        [HttpGet]
        [Route("Admin/Task/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var task = await _mediator.SendAsync(new TaskQuery { TaskId = id });
            if (task == null)
                return HttpNotFound();

            return View(task);
        }

        // POST: Activity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskSummaryModel = await _mediator.SendAsync(new TaskQuery { TaskId = id });
            if (taskSummaryModel == null)
                return HttpNotFound();

            if (!User.IsOrganizationAdmin(taskSummaryModel.OrganizationId))
                return HttpUnauthorized();

            _mediator.Send(new DeleteTaskCommand { TaskId = id });

            //I'm assuming this is ActivityController in the Admin area
            return RedirectToAction(nameof(ActivityController.Details), "Activity", new { id = taskSummaryModel.ActivityId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int id, List<string> userIds)
        {
            var taskSummaryModel = await _mediator.SendAsync(new TaskQuery { TaskId = id });

            var activity = GetActivityBy(taskSummaryModel.ActivityId);
            if (!User.IsOrganizationAdmin(activity.Campaign.ManagingOrganizationId))
                return HttpUnauthorized();
            
            await _mediator.SendAsync(new AssignTaskCommand { TaskId = id, UserIds = userIds });

            return RedirectToRoute(new { controller = "Task", Area = "Admin", action = nameof(Details), id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MessageAllVolunteers(MessageTaskVolunteersModel model)
        {
            //TODO: Query only for the organization Id rather than the whole activity detail
            if (!ModelState.IsValid)
                return HttpBadRequest(ModelState);

            var task = await _mediator.SendAsync(new TaskQuery { TaskId = model.TaskId });
            if (task == null)
                return HttpNotFound();

            if (!User.IsOrganizationAdmin(task.OrganizationId))
                return HttpUnauthorized();

            await _mediator.SendAsync(new MessageTaskVolunteersCommand { Model = model });

            return Ok();
        }

        private void WarnDateTimeOutOfRange(ref TaskEditModel model)
        {
            if (model.StartDateTime.HasValue || model.EndDateTime.HasValue)
            {
                var activity = GetActivityBy(model.ActivityId);
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(activity.Campaign.TimeZoneId);

                DateTimeOffset? convertedStartDateTime = null;
                if (model.StartDateTime.HasValue)
                {
                    var startDateValue = model.StartDateTime.Value;
                    var startDateTimeOffset = timeZone.GetUtcOffset(startDateValue);
                    convertedStartDateTime = new DateTimeOffset(startDateValue.Year, startDateValue.Month, startDateValue.Day, startDateValue.Hour, startDateValue.Minute, 0, startDateTimeOffset);
                }

                DateTimeOffset? convertedEndDateTime = null;
                if (model.EndDateTime.HasValue)
                {
                    var endDateValue = model.EndDateTime.Value;
                    var endDateTimeOffset = timeZone.GetUtcOffset(endDateValue);
                    convertedEndDateTime = new DateTimeOffset(endDateValue.Year, endDateValue.Month, endDateValue.Day, endDateValue.Hour, endDateValue.Minute, 0, endDateTimeOffset);
                }

                if ((convertedStartDateTime < activity.StartDateTime || convertedEndDateTime > activity.EndDateTime) &&
                    (model.IgnoreTimeRangeWarning == false))
                {
                    ModelState.AddModelError("", "Although valid, task time is out of range for activity time from " +
                        activity.StartDateTime.DateTime.ToString("g") + " to " + activity.EndDateTime.DateTime.ToString("g") + " " + activity.Campaign.TimeZoneId);
                    ModelState.Remove("IgnoreTimeRangeWarning");
                    model.IgnoreTimeRangeWarning = true;
                }
            }
        }

        //TODO: mediator query and handler need to be changed to async
        private Activity GetActivityBy(int activityId) => _mediator.Send(new ActivityByActivityIdQuery { ActivityId = activityId });
    }
}
