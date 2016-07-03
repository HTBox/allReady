using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Areas.Admin.Models.Validators;
using AllReady.Features.Event;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class TaskController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ITaskSummaryModelValidator _taskDetailModelValidator;

        public TaskController(IMediator mediator, ITaskSummaryModelValidator taskDetailModelValidator)
        {
            _mediator = mediator;
            _taskDetailModelValidator = taskDetailModelValidator;
        }

        [HttpGet]
        [Route("Admin/Task/Create/{eventId}")]
        public IActionResult Create(int eventId)
        {
            var campaignEvent = GetEventBy(eventId);
            if (campaignEvent == null || !User.IsOrganizationAdmin(campaignEvent.Campaign.ManagingOrganizationId))
            {
                return Unauthorized();
            }
            
            var viewModel = new TaskSummaryModel
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
        public async Task<IActionResult> Create(int eventId, TaskSummaryModel model)
        {
            var errors = _taskDetailModelValidator.Validate(model);

            errors.ToList().ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (ModelState.IsValid)
            {
                if (!User.IsOrganizationAdmin(model.OrganizationId))
                {
                    return Unauthorized();
                }
                
                await _mediator.SendAsync(new EditTaskCommandAsync { Task = model });

                //mgmccarthy: I'm assuming this is EventController in the Admin area
                return RedirectToAction(nameof(Details), "Event", new { id = eventId });
            }

            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskSummaryModel model)
        {
            var errors = _taskDetailModelValidator.Validate(model);

            errors.ToList().ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (ModelState.IsValid)
            {
                if (!User.IsOrganizationAdmin(model.OrganizationId))
                {
                    return Unauthorized();
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
                return NotFound();
            }

            if (!User.IsOrganizationAdmin(task.OrganizationId))
            {
                return Unauthorized();
            }
            
            return View(task);
        }

        //mgmccarthy: does anyone know why there are no attributes here on this action method?
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _mediator.SendAsync(new TaskQueryAsync { TaskId = id });
            if (task == null)
            {
                return NotFound();
            }

            if (!User.IsOrganizationAdmin(task.OrganizationId))
            {
                return Unauthorized();
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
                return NotFound();
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
                return NotFound();
            }

            if (!User.IsOrganizationAdmin(taskSummaryModel.OrganizationId))
            {
                return Unauthorized();
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
                return Unauthorized();
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
                return BadRequest(ModelState);
            }
            
            var task = await _mediator.SendAsync(new TaskQueryAsync { TaskId = model.TaskId });
            if (task == null)
            {
                return NotFound();
            }

            if (!User.IsOrganizationAdmin(task.OrganizationId))
            {
                return Unauthorized();
            }
            
            await _mediator.SendAsync(new MessageTaskVolunteersCommandAsync { Model = model });

            return Ok();
        }

        //mgmccarthy: mediator query and handler need to be changed to async when Issue #622 is addressed
        private Event GetEventBy(int eventId) => 
            _mediator.Send(new EventByIdQuery { EventId = eventId });
    }
}
