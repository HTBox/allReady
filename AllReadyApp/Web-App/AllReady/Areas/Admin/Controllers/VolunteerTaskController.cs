using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels.Validators.VolunteerTask;
using AllReady.Areas.Admin.ViewModels.VolunteerTask;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Features.Users;
using AllReady.Areas.Admin.ViewModels.Event;
using AllReady.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using DeleteQuery = AllReady.Areas.Admin.Features.Tasks.DeleteQuery;
using DeleteViewModel = AllReady.Areas.Admin.ViewModels.VolunteerTask.DeleteViewModel;

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

        [HttpPost, ActionName("RemoveVolunteer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveVolunteer(RemoveVolunteerViewModel viewModel)
        {
            await _mediator.SendAsync(new RemoveVolunteerFromTaskCommand
            {
                TaskId = viewModel.TaskId,
                UserId = viewModel.UserId,
                NotifyUser = viewModel.Notify
            });

            return RedirectToAction(nameof(Details), new { id = viewModel.TaskId});
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

        [Route("Admin/Event/{eventId}/Task/{taskId}/candidates")]
        public async Task<IActionResult> VolunteerCandidates(int eventId, int taskId)
        {
            var viewModel = await _mediator.SendAsync(new DetailsQuery() { VolunteerTaskId = taskId });
            var authorizableTask = await _mediator.SendAsync(new AuthorizableTaskQuery(taskId));
            if (!await authorizableTask.UserCanEdit())
                return new ForbidResult();

            var eventInfo = await _mediator.SendAsync(new EventDetailQuery() { EventId = eventId });

            var query = new ListPossibleVolunteersForTaskQuery
            {
                OrganizationId = eventInfo.OrganizationId,
                TaskId = taskId
            };
            var users = await _mediator.SendAsync(query);

            var model = new VolunteerCandidatesViewModel
            {
                MaxSelectableCount = viewModel.NumberOfVolunteersRequired - viewModel.AssignedVolunteers.Count,
                Volunteers = users
                    .Select(x => new SelectListItem { Value = x.UserId, Text = $"{x.Name} ({x.TaskCount} assigned tasks)" })
                    .ToList(),
            };

            return Json(model);
        }

        [HttpPost]
        [Route("Admin/Task/Assign/Volunteer")]
        public async Task<IActionResult> AssignChosenVolunteers(AssignVolunteersViewModel viewModel)
        {
            var authorizableTask = await _mediator.SendAsync(new AuthorizableTaskQuery(viewModel.TaskId));
            if (!await authorizableTask.UserCanEdit())
                return new ForbidResult();


            foreach (var userId in viewModel.UserId)
            {
                var cmd = new AssignVolunteerToTaskCommand
                {
                    UserId = userId,
                    VolunteerTaskId = viewModel.TaskId,
                    NotifyUser = viewModel.NotifyUsers
                };
                await _mediator.SendAsync(cmd);
            }

            var task = await _mediator.SendAsync(new DetailsQuery() { VolunteerTaskId = viewModel.TaskId });
            return RedirectToAction("Details", "Event", new { id = task.EventId });
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
