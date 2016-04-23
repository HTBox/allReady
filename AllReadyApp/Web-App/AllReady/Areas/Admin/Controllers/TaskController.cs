using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Features.Event;
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
        [Route("Admin/Task/Create/{eventId}")]
        public IActionResult Create(int eventId)
        {
            var campaignEvent = GetEventBy(eventId);
            if (campaignEvent == null || !User.IsOrganizationAdmin(campaignEvent.Campaign.ManagingOrganizationId))
            {
                return HttpUnauthorized();
            }
            
            var viewModel = new TaskEditModel
            {
                EventId = campaignEvent.Id,
                EventName = campaignEvent.Name,
                CampaignId = campaignEvent.CampaignId,
                CampaignName = campaignEvent.Campaign.Name,
                OrganizationId = campaignEvent.Campaign.ManagingOrganizationId,
                TimeZoneId = campaignEvent.Campaign.TimeZoneId,
                StartDateTime = campaignEvent.StartDateTime,
                EndDateTime = campaignEvent.EndDateTime
            };

            return View("Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Task/Create/{eventId}")]
        public async Task<IActionResult> Create(int eventId, TaskEditModel model)
        {
            if (model.EndDateTime < model.StartDateTime)
            {
                ModelState.AddModelError(nameof(model.EndDateTime), "Ending time cannot be earlier than the starting time");
            }
            
            WarnDateTimeOutOfRange(ref model);

            if (ModelState.IsValid)
            {
                if (!User.IsOrganizationAdmin(model.OrganizationId))
                {
                    return HttpUnauthorized();
                }
                
                await _mediator.SendAsync(new EditTaskCommandAsync { Task = model });

                //mgmccarthy: I'm assuming this is EventController in the Admin area
                return RedirectToAction(nameof(Details), "Event", new { id = eventId });
            }

            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskEditModel model)
        {
            if (model.EndDateTime < model.StartDateTime)
            {
                ModelState.AddModelError(nameof(model.EndDateTime), "Ending time cannot be earlier than the starting time");
            }
            
            WarnDateTimeOutOfRange(ref model);

            if (ModelState.IsValid)
            {
                if (!User.IsOrganizationAdmin(model.OrganizationId))
                {
                    return HttpUnauthorized();
                }
                
                await _mediator.SendAsync(new EditTaskCommandAsync { Task = model });

                return RedirectToAction(nameof(Details), "Task", new { id = model.Id });
            }

            return View(model);
        }

        [HttpGet]
        [Route("Admin/Task/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _mediator.SendAsync(new EditTaskQueryAsync { TaskId = id });
            if (task == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(task.OrganizationId))
            {
                return HttpUnauthorized();
            }
            
            return View(task);
        }

        //mgmccarthy: does anyone know why there are no attributes here on this action method?
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _mediator.SendAsync(new TaskQueryAsync { TaskId = id });
            if (task == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(task.OrganizationId))
            {
                return HttpUnauthorized();
            }
            
            return View(task);
        }

        [HttpGet]
        [Route("Admin/Task/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var task = await _mediator.SendAsync(new TaskQueryAsync { TaskId = id });
            if (task == null)
            {
                return HttpNotFound();
            }
            
            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskSummaryModel = await _mediator.SendAsync(new TaskQueryAsync { TaskId = id });
            if (taskSummaryModel == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(taskSummaryModel.OrganizationId))
            {
                return HttpUnauthorized();
            }
            
            await _mediator.SendAsync(new DeleteTaskCommandAsync { TaskId = id });

            //mgmccarthy: I'm assuming this is EventController in the Admin area
            return RedirectToAction(nameof(EventController.Details), "Event", new { id = taskSummaryModel.EventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int id, List<string> userIds)
        {
            var taskSummaryModel = await _mediator.SendAsync(new TaskQueryAsync { TaskId = id });

            var campaignEvent = GetEventBy(taskSummaryModel.EventId);
            if (!User.IsOrganizationAdmin(campaignEvent.Campaign.ManagingOrganizationId))
            {
                return HttpUnauthorized();
            }
            
            await _mediator.SendAsync(new AssignTaskCommandAsync { TaskId = id, UserIds = userIds });

            return RedirectToRoute(new { controller = "Task", Area = "Admin", action = nameof(Details), id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MessageAllVolunteers(MessageTaskVolunteersModel model)
        {
            //TODO: Query only for the organization Id rather than the whole event detail
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }
            
            var task = await _mediator.SendAsync(new TaskQueryAsync { TaskId = model.TaskId });
            if (task == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(task.OrganizationId))
            {
                return HttpUnauthorized();
            }
            
            await _mediator.SendAsync(new MessageTaskVolunteersCommandAsync { Model = model });

            return Ok();
        }

        private void WarnDateTimeOutOfRange(ref TaskEditModel model)
        {
            if (model.StartDateTime.HasValue || model.EndDateTime.HasValue)
            {
                var campaignEvent = GetEventBy(model.EventId);
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(campaignEvent.Campaign.TimeZoneId);

                DateTimeOffset? convertedStartDateTime = null;
                if (model.StartDateTime.HasValue)
                {
                    var startDateValue = model.StartDateTime.Value;
                    var startDateTimeOffset = timeZoneInfo.GetUtcOffset(startDateValue);
                    convertedStartDateTime = new DateTimeOffset(startDateValue.Year, startDateValue.Month, startDateValue.Day, startDateValue.Hour, startDateValue.Minute, 0, startDateTimeOffset);
                }

                DateTimeOffset? convertedEndDateTime = null;
                if (model.EndDateTime.HasValue)
                {
                    var endDateValue = model.EndDateTime.Value;
                    var endDateTimeOffset = timeZoneInfo.GetUtcOffset(endDateValue);
                    convertedEndDateTime = new DateTimeOffset(endDateValue.Year, endDateValue.Month, endDateValue.Day, endDateValue.Hour, endDateValue.Minute, 0, endDateTimeOffset);
                }

                if ((convertedStartDateTime < campaignEvent.StartDateTime || convertedEndDateTime > campaignEvent.EndDateTime) &&
                    (model.IgnoreTimeRangeWarning == false))
                {
                    ModelState.AddModelError("", "Although valid, task time is out of range for event time from " +
                        campaignEvent.StartDateTime.DateTime.ToString("g") + " to " + campaignEvent.EndDateTime.DateTime.ToString("g") + " " + campaignEvent.Campaign.TimeZoneId);
                    ModelState.Remove("IgnoreTimeRangeWarning");
                    model.IgnoreTimeRangeWarning = true;
                }
            }
        }

        //mgmccarthy: mediator query and handler need to be changed to async when Issue #622 is addressed
        private Event GetEventBy(int eventId) => 
            _mediator.Send(new EventByIdQuery { EventId = eventId });
    }
}
