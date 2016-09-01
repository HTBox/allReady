using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Areas.Admin.ViewModels.Validators.Task;
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
        private readonly ITaskEditViewModelValidator _taskEditViewModelValidator;

        public TaskController(IMediator mediator, ITaskEditViewModelValidator taskEditViewModelValidator)
        {
            _mediator = mediator;
            _taskEditViewModelValidator = taskEditViewModelValidator;
        }

        [HttpGet]
        [Route("Admin/Task/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await _mediator.SendAsync(new DetailsQueryAsync { TaskId = id });
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("Admin/Task/Create/{eventId}")]
        public async Task<IActionResult> Create(int eventId)
        {
            var viewModel = await _mediator.SendAsync(new CreateTaskQueryAsync { EventId = eventId });
            if (!User.IsOrganizationAdmin(viewModel.OrganizationId))
            {
                return Unauthorized();
            }

            viewModel.CancelUrl = Url.Action(new UrlActionContext { Action = nameof(EventController.Details), Controller = "Event", Values = new { id = viewModel.EventId, area = "Admin" } });

            ViewBag.Title = "Create Task";

            return View("Edit", viewModel);
        }

        [HttpGet]
        [Route("Admin/Task/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var viewModel = await _mediator.SendAsync(new EditTaskQueryAsync { TaskId = id });
            if (!User.IsOrganizationAdmin(viewModel.OrganizationId))
            {
                return Unauthorized();
            }

            viewModel.CancelUrl = Url.Action(new UrlActionContext { Action = nameof(Details), Controller = "Task", Values = new { eventId = viewModel.EventId, id = viewModel.Id, area = "Admin" } });
            ViewBag.Title = "Edit Task";

            return View("Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel viewModel)
        {
            var errors = _taskDetailModelValidator.Validate(viewModel);
            errors.ToList().ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (ModelState.IsValid)
            {
                if (!User.IsOrganizationAdmin(viewModel.OrganizationId))
                {
                    return Unauthorized();
                }

                var taskId = await _mediator.SendAsync(new EditTaskCommandAsync { Task = viewModel });

                return viewModel.Id == 0 ?
                    RedirectToAction(nameof(EventController.Details), "Event", new { id = viewModel.EventId }) :
                    RedirectToAction(nameof(Details), "Task", new { id = taskId });
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var viewModel = await _mediator.SendAsync(new DeleteQueryAsync { TaskId = id });
            if (!User.IsOrganizationAdmin(viewModel.OrganizationId))
            {
                return Unauthorized();
            }

            viewModel.Title = $"Delete task {viewModel.Name}";
            viewModel.UserIsOrgAdmin = true;

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(DeleteViewModel viewModel)
        {
            if (!viewModel.UserIsOrgAdmin)
            {
                return Unauthorized();
            }

            await _mediator.SendAsync(new DeleteTaskCommandAsync { TaskId = viewModel.Id });

            return RedirectToAction(nameof(EventController.Details), "Event", new { id = viewModel.EventId });
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
        public async Task<IActionResult> MessageAllVolunteers(MessageTaskVolunteersViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tasksOrganizationId = await _mediator.SendAsync(new OrganizationIdByTaskIdQueryAsync { TaskId = viewModel.TaskId });
            if (!User.IsOrganizationAdmin(tasksOrganizationId))
            {
                return Unauthorized();
            }

            await _mediator.SendAsync(new MessageTaskVolunteersCommandAsync { Model = viewModel });

            return Ok();
        }
    }
}