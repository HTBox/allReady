using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Features.Event;
using AllReady.ViewModels.Event;
using AllReady.ViewModels.Shared;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using TaskStatus = AllReady.Areas.Admin.Features.Tasks.TaskStatus;

namespace AllReady.Controllers
{
    public class EventController : Controller
    {
        private readonly IMediator _mediator;

        public EventController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("~/MyEvents")]
        [Authorize]
        public IActionResult GetMyEvents()
        {
            var viewModel = _mediator.Send(new GetMyEventsQuery { UserId = User.GetUserId() });
            return View("MyEvents", viewModel);
        }

        [Route("~/MyEvents/{id}/tasks")]
        [Authorize]
        public IActionResult GetMyTasks(int id)
        {
            var view = _mediator.Send(new GetMyTasksQuery { EventId = id, UserId = User.GetUserId() });
            return Json(view);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Route("~/MyEvents/{id}/tasks")]
        public async Task<IActionResult> UpdateMyTasks(int id, [FromBody] List<TaskSignupViewModel> model)
        {
            await _mediator.SendAsync(new UpdateMyTasksCommandAsync { TaskSignups = model, UserId = User.GetUserId() });
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Events");
        }

        [Route("[controller]/{id}/")]
        [AllowAnonymous]
        public IActionResult ShowEvent(int id)
        {
            var viewModel = _mediator.Send(new ShowEventQuery { EventId = id, User = User });
            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View("EventWithTasks", viewModel);
        }

        [HttpPost]
        [Route("/Event/Signup")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(EventSignupViewModel signupModel)
        {
            if (signupModel == null)
            {
                return HttpBadRequest();
            }

            if (ModelState.IsValid)
            {
                await _mediator.SendAsync(new EventSignupCommand { EventSignup = signupModel });
            }

            //TODO: handle invalid event signup info (phone, email) in a useful way
            //  would be best to handle it in KO on the client side (prevent clicking Volunteer)

            return RedirectToAction(nameof(ShowEvent), new { id = signupModel.EventId });
        }

        [HttpGet]
        [Route("/Event/ChangeStatus")]
        [Authorize]
        public async Task<IActionResult> ChangeStatus(int eventId, int taskId, string userId, TaskStatus status, string statusDesc)
        {
            if (userId == null)
            {
                return HttpBadRequest();
            }

            await _mediator.SendAsync(new TaskStatusChangeCommandAsync { TaskStatus = status, TaskId = taskId, UserId = userId, TaskStatusDescription = statusDesc });

            return RedirectToAction(nameof(ShowEvent), new { id = eventId });
        }
    }
}
