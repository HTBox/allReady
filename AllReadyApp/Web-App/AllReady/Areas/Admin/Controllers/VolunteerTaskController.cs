using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels.Validators.VolunteerTask;
using AllReady.Areas.Admin.ViewModels.VolunteerTask;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AllReady.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    [Authorize]
    public class VolunteerTaskController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IValidateVolunteerTaskEditViewModelValidator _volunteerTaskEditViewModelValidator;

        public VolunteerTaskController(IMediator mediator, IValidateVolunteerTaskEditViewModelValidator volunteerTaskEditViewModelValidator)
        {
            _mediator = mediator;
            _volunteerTaskEditViewModelValidator = volunteerTaskEditViewModelValidator;
        }

        [HttpGet]
        [Route("Admin/VolunteerTask/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await _mediator.SendAsync(new DetailsQuery { VolunteerTaskId = id });
            if (viewModel == null)
            {
                return NotFound();
            }

            var authorizableTask = await _mediator.SendAsync(new AuthorizableTaskQuery(id));
            if (!await authorizableTask.UserCanView())
            {
                return new ForbidResult();
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("Admin/VolunteerTask/Create/{eventId}")]
        public async Task<IActionResult> Create(int eventId)
        {
            var viewModel = await _mediator.SendAsync(new CreateVolunteerTaskQuery { EventId = eventId });
            var authorizableEvent = await _mediator.SendAsync(new Features.Events.AuthorizableEventQuery(eventId));
            if (!await authorizableEvent.UserCanManageChildObjects())
            {
                return new ForbidResult();
            }

            viewModel.CancelUrl = Url.Action(new UrlActionContext { Action = nameof(EventController.Details), Controller = "Event", Values = new { id = viewModel.EventId, area = AreaNames.Admin } });

            ViewBag.Title = "Create Task";

            return View("Edit", viewModel);
        }

        [HttpGet]
        [Route("Admin/VolunteerTask/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var viewModel = await _mediator.SendAsync(new EditVolunteerTaskQuery { VolunteerTaskId = id });
            var authorizableTask = await _mediator.SendAsync(new AuthorizableTaskQuery(id));
            if (!await authorizableTask.UserCanView())
            {
                return new ForbidResult();
            }

            viewModel.CancelUrl = Url.Action(new UrlActionContext { Action = nameof(Details), Controller = "VolunteerTask", Values = new { eventId = viewModel.EventId, id = viewModel.Id, area = AreaNames.Admin } });
            ViewBag.Title = "Edit Task";

            return View("Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel viewModel)
        {
            var errors = await _volunteerTaskEditViewModelValidator.Validate(viewModel);
            errors.ToList().ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (ModelState.IsValid)
            {
                if (viewModel.Id == 0)
                {
                    var authorizableEvent = await _mediator.SendAsync(new Features.Events.AuthorizableEventQuery(viewModel.EventId));
                    if (!await authorizableEvent.UserCanManageChildObjects())
                    {
                        return new ForbidResult();
                    }
                }
                else
                {
                    var authorizableTask = await _mediator.SendAsync(new AuthorizableTaskQuery(viewModel.Id));
                    if (!await authorizableTask.UserCanEdit())
                    {
                        return new ForbidResult();
                    }
                }

                var volunteerTaskId = await _mediator.SendAsync(new EditVolunteerTaskCommand { VolunteerTask = viewModel });

                return viewModel.Id == 0 ?
                    RedirectToAction(nameof(EventController.Details), "Event", new { id = viewModel.EventId }) :
                    RedirectToAction(nameof(Details), "VolunteerTask", new { id = volunteerTaskId });
            }
            if (viewModel.Id == 0)
            {
                viewModel.CancelUrl = Url.Action(new UrlActionContext { Action = nameof(EventController.Details), Controller = "Event", Values = new { id = viewModel.EventId, area = AreaNames.Admin } });
            }
            else
            {
                viewModel.CancelUrl = Url.Action(new UrlActionContext { Action = nameof(Details), Controller = "VolunteerTask", Values = new { eventId = viewModel.EventId, id = viewModel.Id, area = AreaNames.Admin } });
            }
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var viewModel = await _mediator.SendAsync(new DeleteQuery { VolunteerTaskId = id });
            var authorizableTask = await _mediator.SendAsync(new AuthorizableTaskQuery(id));
            if (!await authorizableTask.UserCanEdit())
            {
                return new ForbidResult();
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
                return new ForbidResult();
            }

            await _mediator.SendAsync(new DeleteVolunteerTaskCommand { VolunteerTaskId = viewModel.Id });

            return RedirectToAction(nameof(EventController.Details), "Event", new { id = viewModel.EventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int id, List<string> userIds)
        {
            var volunteerTasksOrganizationId = await _mediator.SendAsync(new OrganizationIdByVolunteerTaskIdQuery { VolunteerTaskId = id });
            var authorizableTask = await _mediator.SendAsync(new AuthorizableTaskQuery(id));
            if (!await authorizableTask.UserCanEdit())
            {
                return new ForbidResult();
            }

            await _mediator.SendAsync(new AssignVolunteerTaskCommand { VolunteerTaskId = id, UserIds = userIds });

            return RedirectToRoute(new { controller = "VolunteerTask", area = AreaNames.Admin, action = nameof(Details), id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MessageAllVolunteers(MessageTaskVolunteersViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var volunteerTasksOrganizationId = await _mediator.SendAsync(new OrganizationIdByVolunteerTaskIdQuery { VolunteerTaskId = viewModel.VolunteerTaskId });
            var authorizableTask = await _mediator.SendAsync(new AuthorizableTaskQuery(viewModel.VolunteerTaskId));
            if (!await authorizableTask.UserCanEdit())
            {
                return new ForbidResult();
            }

            await _mediator.SendAsync(new MessageTaskVolunteersCommand { Model = viewModel });

            return Ok();
        }
    }
}
