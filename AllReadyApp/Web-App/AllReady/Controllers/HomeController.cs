using AllReady.Features.Campaigns;
using MediatR;
using Microsoft.AspNet.Mvc;

namespace AllReady.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator mediator;

        public HomeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public IActionResult Index()
        {
            var results = mediator.Send(new CampaignQuery());
            return View(results);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Aesop()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

        public IActionResult AccessDenied()
        {
            return View("~/Views/Shared/AccessDenied.cshtml");
        }
    }
}
