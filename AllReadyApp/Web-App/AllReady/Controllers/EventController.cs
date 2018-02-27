using System.Threading.Tasks;
using AllReady.Features.Events;
using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AllReady.Controllers
{
    public class EventController : Controller
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
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
        //    await _mediator.SendAsync(new UpdateMyTasksCommand { TaskSignups = model, UserId = User.GetUserId() });
        //    return Json(new { success = true });
        //}

        [HttpGet]
        public IActionResult Index()
        {
            return View("Events");
        }

        [Route("[controller]/{id}/", Name = "EventDetails")]
        [AllowAnonymous]
        public async Task<IActionResult> ShowEvent(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var viewModel = await _mediator.SendAsync(new ShowEventQuery { EventId = id, UserId = user?.Id });
            if (viewModel == null)
            {
                return NotFound();
            }

            ViewBag.AbsoluteUrl = System.Net.WebUtility.UrlEncode(Url.Action(new UrlActionContext { Action = nameof(ShowEvent), Controller = "Event", Values = null, Protocol = Request.Scheme }));

            return View("EventWithTasks", viewModel);
        }

        [HttpGet]
        [Route("~/Events/Dashboard")]
        public IActionResult Dashboard()
        {
            return View("Dashboard");
        }
    }
}
