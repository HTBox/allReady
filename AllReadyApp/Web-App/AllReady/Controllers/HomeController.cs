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
            var campaignModel = _bus.Send(new CampaignQuery());
            return View(campaignModel.CampaignViewModels);
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
