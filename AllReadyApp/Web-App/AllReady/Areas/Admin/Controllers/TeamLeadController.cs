using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.TeamLead;
using AllReady.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.Areas.Admin.Controllers
{
    [Authorize]
    [Area(AreaNames.Admin)]
    public class TeamLeadController : Controller
    {
        private readonly IMediator _mediator;

        public TeamLeadController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _mediator.SendAsync(new TeamLeadItineraryListViewModelQuery(User));

            return View(viewModel);
        }
    }
}
