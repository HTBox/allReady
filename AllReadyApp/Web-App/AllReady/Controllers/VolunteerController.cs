using AllReady.Features.Volunteers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AllReady.Models;
using System.Collections.Generic;

namespace AllReady.Controllers
{
    [Authorize]
    public class VolunteerController : Controller
    {
        private readonly IMediator _mediator;
        private UserManager<ApplicationUser> _userManager;

        public VolunteerController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("~/v")]
        public IActionResult DashboardShortUrl()
        {
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet]
        [Route("~/Volunteers/Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var viewModel = await _mediator.SendAsync(new GetVolunteerEventsQuery { UserId = _userManager.GetUserId(User) });

            return View("Dashboard", viewModel);
        }
    }
}
