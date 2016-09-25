using System.Threading.Tasks;
using AllReady.Features.Events;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.Controllers
{
    public class EventController : Controller
    {
        private readonly IMediator _mediator;

        public EventController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // note: sgordon: These are currently not used as we've simplified the my events page. Note that My Events is moved to VolunteerController and now known as dashboard.
        // once we review functionality needed for that page we may reinstate these in some form so leaving for now.

        //[Route("~/MyEvents/{id}/tasks")]
        //[Authorize]
        //public IActionResult GetMyTasks(int id)
        //{
        //    var view = _mediator.Send(new GetMyTasksQuery { EventId = id, UserId = User.GetUserId() });
        //    return Json(view);
        //}

        //[HttpPost]
        //[Authorize]
        //[ValidateAntiForgeryToken]
        //[Route("~/MyEvents/{id}/tasks")]
        //public async Task<IActionResult> UpdateMyTasks(int id, [FromBody] List<TaskSignupViewModel> model)
        //{
        //    await _mediator.SendAsync(new UpdateMyTasksCommandAsync { TaskSignups = model, UserId = User.GetUserId() });
        //    return Json(new { success = true });
        //}

        [HttpGet]
        public IActionResult Index()
        {
            return View("Events");
        }

        [Route("[controller]/{id}/")]
        [AllowAnonymous]
        public async Task<IActionResult> ShowEvent(int id)
        {
            var viewModel = await _mediator.SendAsync(new ShowEventQueryAsync { EventId = id, User = User });
            if (viewModel == null)
            {
                return NotFound();
            }

            return View("EventWithTasks", viewModel);
        }
    }
}
