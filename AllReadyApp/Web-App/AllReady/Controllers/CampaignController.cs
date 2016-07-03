using System.Collections.Generic;
using AllReady.Features.Campaigns;
using Microsoft.AspNetCore.Mvc;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AllReady.Controllers
{
    [Route("api/[controller]")]
    public class CampaignController : Controller
    {
        private readonly IMediator _mediator;

        public CampaignController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("~/[controller]")]
        public IActionResult Index()
        {
            var unlockedCampaigns = GetUnlockedCampaigns();
            return View(unlockedCampaigns);
        }

        [HttpGet]
        [Route("~/[controller]/{id}")]
        public IActionResult Details(int id)
        {
            var campaign = GetCampaign(id);

            if (campaign == null || campaign.Locked)
                return NotFound();

            ViewBag.AbsoluteUrl = UrlEncode(Url.Action(new UrlActionContext { Action = "Details", Controller = "Campaign", Values = null, Protocol = Request.Scheme }));

            return View("Details", new CampaignViewModel(campaign));
        }

        [HttpGet]
        [Route("~/[controller]/map/{id}")]
        public IActionResult LocationMap(int id)
        {
            var campaign = GetCampaign(id);

            if (campaign == null)
                return NotFound();

            return View("Map", new CampaignViewModel(campaign));
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<CampaignViewModel> Get()
        {
            return GetUnlockedCampaigns();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var campaign = GetCampaign(id);

            if (campaign == null || campaign.Locked)
                return NotFound();

            return Json(campaign.ToViewModel());
        }

        private List<CampaignViewModel> GetUnlockedCampaigns()
        {
            return _mediator.Send(new UnlockedCampaignsQuery());
        }

        private Campaign GetCampaign(int campaignId)
        {
            return _mediator.Send(new CampaignByCampaignIdQuery { CampaignId = campaignId });
        }

        protected virtual string UrlEncode(string value)
        {
            return System.Net.WebUtility.UrlEncode(value);
        }
    }
}