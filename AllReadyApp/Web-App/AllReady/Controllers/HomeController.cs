using AllReady.Features.Campaigns;
using AllReady.Features.Home;
using AllReady.ViewModels.Home;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
            var model = new IndexViewModel
            {
                FeaturedCampaign = await mediator.SendAsync(new FeaturedCampaignQuery()),
                ActiveOrUpcomingCampaigns = await mediator.SendAsync(new ActiveOrUpcomingCampaignsQuery())
            };

            if (model.HasFeaturedCampaign)
            {
                var indexOfFeaturedCampaign = model.ActiveOrUpcomingCampaigns.FindIndex(s => s.Id == model.FeaturedCampaign.Id);
                model.ActiveOrUpcomingCampaigns.RemoveAt(indexOfFeaturedCampaign);
            }

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