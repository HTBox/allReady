using AllReady.Features.Campaigns;
using MediatR;
using Microsoft.AspNet.Mvc;

namespace AllReady.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator _bus;

        public HomeController(IMediator bus)
        {
            _bus = bus;
        }

        public IActionResult Index()
        {
            var results = _bus.Send(new CampaignQuery());
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
