using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Campaigns;
using Microsoft.AspNetCore.Mvc;
using AllReady.Models;
using AllReady.Security;
using AllReady.ViewModels.Campaign;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AllReady.Controllers
{
    [Route("api/[controller]")]
    public class CampaignController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUserAuthorizationService _userAuthorizationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CampaignController(IMediator mediator, IUserAuthorizationService userAuthorizationService, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userAuthorizationService = userAuthorizationService;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("~/[controller]")]
        public async Task<IActionResult> Index()
        {
            var unlockedCampaigns = await GetUnlockedCampaigns();

            var isCampaignManager = await _userAuthorizationService.IsCampaignManager();

            if (isCampaignManager && unlockedCampaigns.Any()) unlockedCampaigns.First().IsCampaignManager = true;

            return View(unlockedCampaigns);
        }

        [HttpGet]
        [Route("~/[controller]/{id}", Name = "CampaignDetails")]
        public async Task<IActionResult> Details(int id)
        {
            var campaign = await GetCampaign(id);
            if (campaign == null || campaign.Locked)
            {
                return NotFound();
            }

            ViewBag.AbsoluteUrl = UrlEncode(Url.Action(new UrlActionContext { Action = nameof(CampaignController.Details), Controller = "Campaign", Values = null, Protocol = Request.Scheme }));

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

        [HttpGet]
        [Route("~/[controller]/ManageCampaign")]
        public async Task<IActionResult> ManageCampaign()
        {
            var userIsNotCampaignManager = await _userAuthorizationService.IsCampaignManager() == false;

            if (userIsNotCampaignManager) return Unauthorized();

            var user = await _userManager.GetUserAsync(User);

            var authorizedCampaign = await _mediator.SendAsync(new AuthorizedCampaignsQuery { UserId = user.Id });

            return View(authorizedCampaign);
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
