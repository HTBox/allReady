using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.Features.Event;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class TaskController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ITaskEditViewModelValidator _taskDetailModelValidator;

        public TaskController(IMediator mediator, ITaskEditViewModelValidator taskDetailModelValidator)
        {
            _mediator = mediator;
            _taskDetailModelValidator = taskDetailModelValidator;
        }

        [HttpGet]
        [Route("Admin/Task/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var details = await _mediator.SendAsync(new DetailsQueryAsync { TaskId = id });
            if (details == null)
            {
                return NotFound();
            }

            return View(details);
        }

        [HttpGet]
        [Route("Admin/Task/Create/{eventId}")]
        public async Task<IActionResult> Create(int eventId)
        {
            var model = await _mediator.SendAsync(new CreateTaskQueryAsync { EventId = eventId });
            if (!User.IsOrganizationAdmin(model.OrganizationId))
            {
                return Unauthorized();
            }

            model.CancelUrl = Url.Action(new UrlActionContext { Action = nameof(EventController.Details), Controller = "Event", Values = new { id = model.EventId, area = "Admin" } });

            ViewBag.Title = "Create Task";

            return View("Edit", model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Route("Admin/Task/Create/{eventId}")]
        //public async Task<IActionResult> Create(int eventId, TaskSummaryViewModel model)
        //{
        //    var errors = _taskDetailModelValidator.Validate(model);

        //    errors.ToList().ForEach(e => ModelState.AddModelError(e.Key, e.Value));

        //    if (ModelState.IsValid)
        //    {
        //        if (!User.IsOrganizationAdmin(model.OrganizationId))
        //        {
        //            return Unauthorized();
        //        }

        //        await _mediator.SendAsync(new EditTaskCommandAsync { Task = model });

        //        return RedirectToAction(nameof(Details), "Event", new { id = eventId });
        //    }

        //    return View("Edit", model);
        //}

        [HttpGet]
        [Route("Admin/Task/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _mediator.SendAsync(new EditTaskQueryAsync { TaskId = id });
            if (!User.IsOrganizationAdmin(model.OrganizationId))
            {
                return Unauthorized();
            }

            model.CancelUrl = Url.Action(new UrlActionContext { Action = nameof(Details), Controller = "Task", Values = new { eventId = model.EventId, id = model.Id, area = "Admin" } });

            ViewBag.Title = "Edit Task";

            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            var errors = _taskDetailModelValidator.Validate(model);
            errors.ToList().ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (ModelState.IsValid)
            {
                if (!User.IsOrganizationAdmin(model.OrganizationId))
                {
                    return Unauthorized();
                }

                var taskId = await _mediator.SendAsync(new EditTaskCommandAsync { Task = model });

                return model.Id == 0 ?
                    RedirectToAction(nameof(EventController.Details), "Event", new { id = model.EventId }) :
                    RedirectToAction(nameof(Details), "Task", new { id = taskId });
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var model = await _mediator.SendAsync(new DeleteQueryAsync { TaskId = id });
            if (!User.IsOrganizationAdmin(model.OrganizationId))
            {
                return Unauthorized();
            }

            model.UserIsOrgAdmin = true;

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(DeleteViewModel model)
        {
            if (!model.UserIsOrgAdmin)
            {
                return Unauthorized();
            }

            await _mediator.SendAsync(new DeleteTaskCommandAsync { TaskId = model.Id });

            return RedirectToAction(nameof(EventController.Details), "Event", new { id = model.EventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int id, List<string> userIds)
        {
            var tasksOrganizationId = await _mediator.SendAsync(new OrganizationIdByTaskIdQueryAsync { TaskId = id });
            if (!User.IsOrganizationAdmin(tasksOrganizationId))
            {
                return Unauthorized();
            }

            await _mediator.SendAsync(new AssignTaskCommandAsync { TaskId = id, UserIds = userIds });

            return RedirectToRoute(new { controller = "Task", Area = "Admin", action = nameof(Details), id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MessageAllVolunteers(MessageTaskVolunteersViewModel model)
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

        //TODO: mgmccarthy: mediator query and handler need to be changed to async when Issue #622 is addressed
        private Event GetEventBy(int eventId) => 
            _mediator.Send(new EventByIdQuery { EventId = eventId });
    }
}
