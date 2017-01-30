using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Features.Campaigns;
using Microsoft.AspNetCore.Mvc;
using AllReady.Models;
using AllReady.ViewModels.Campaign;
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
        public async Task<IActionResult> Index()
        {
            var unlockedCampaigns = await GetUnlockedCampaigns();
            return View(unlockedCampaigns);
        }

        [HttpGet]
        [Route("~/[controller]/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var campaign = await GetCampaign(id);
            if (campaign == null || campaign.Locked)
            {
                return NotFound();
            }
            
            ViewBag.AbsoluteUrl = UrlEncode(Url.Action(new UrlActionContext { Action = "Details", Controller = "Campaign", Values = null, Protocol = Request.Scheme }));

            return View("Details", new CampaignViewModel(campaign));
        }

        [HttpGet]
        [Route("~/[controller]/map/{id}")]
        public async Task<IActionResult> LocationMap(int id)
        {
            var campaign = await GetCampaign(id);
            if (campaign == null)
            {
                return NotFound();
            }
            
            return View("Map", new CampaignViewModel(campaign));
        }

        // GET: api/values
        [HttpGet]
        public async Task<IEnumerable<CampaignViewModel>> Get()
        {
            return await GetUnlockedCampaigns();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var campaign = await GetCampaign(id);
            if (campaign == null || campaign.Locked)
            {
                return NotFound();
            }
            
            return Json(campaign.ToViewModel());
        }

        private async Task<List<CampaignViewModel>> GetUnlockedCampaigns()
        {
            return await _mediator.SendAsync(new UnlockedCampaignsQuery());
        }

        private async Task<Campaign> GetCampaign(int campaignId)
        {
            return await _mediator.SendAsync(new CampaignByCampaignIdQuery { CampaignId = campaignId });
        }

        protected virtual string UrlEncode(string value)
        {
            return System.Net.WebUtility.UrlEncode(value);
        }
    }
}