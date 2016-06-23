using AllReady.Features.Volunteers;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AllReady.Controllers
{
    [Authorize]
    public class VolunteerController : Controller
    {
        private readonly IMediator _mediator;

        public VolunteerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("~/v")]
        public async Task<IActionResult> DashboardShortUrl()
        {
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [Route("~/v")]
        public IActionResult DashboardShortUrl()
        {
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [Route("~/Volunteers/Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var viewModel = await _mediator.SendAsync(new GetMyEventsQuery { UserId = User.GetUserId() });

            return View("Dashboard", viewModel);
        }
    }
}
