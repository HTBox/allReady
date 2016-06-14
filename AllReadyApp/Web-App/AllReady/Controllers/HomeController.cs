using AllReady.Features.Campaigns;
using AllReady.ViewModels;
using MediatR;
using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;
using AllReady.ViewModels.Home;

namespace AllReady.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator mediator;

        public HomeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomePageViewModel();

            //todo: per #691 the model may need to be updated as we 
            //      no longer require the list of campaigns
            model.Campaigns = mediator.Send(new CampaignQuery());
            model.FeaturedCampaign = await mediator.SendAsync(new FeaturedCampaignQueryAsync());

            return View(model);
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

        public IActionResult PrivacyPolicy()
        {
            return View(nameof(PrivacyPolicy));
        }
    }
}
