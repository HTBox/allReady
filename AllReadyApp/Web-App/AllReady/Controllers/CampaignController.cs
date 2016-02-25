using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AllReady.Features.Campaigns;
using Microsoft.AspNet.Mvc;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Controllers
{
    [Route("api/[controller]")]
    public class CampaignController : Controller
    {
        private readonly IAllReadyDataAccess _dataAccess;
        private readonly IMediator _mediator;

        //TODO: delete this before commiting
        [Key]
        public Guid Foo { get; set; }

        public CampaignController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("~/[controller]")]
        public IActionResult Index()
        {
            return View(_mediator.Send(new CampaignIndexQuery()));
        }

        [HttpGet]
        [Route("~/[controller]/{id}")]
        public IActionResult Details(int id)
        {
            var campaign = GetCampaign(id);

            if (campaign == null || campaign.Locked)
                return HttpNotFound();

            //"Url.Action("Details", "Campaign", null, protocol: Request.Scheme)" is "http://localhost:48408/Campaign/2"
            //"System.Net.WebUtility.UrlEncode(Url.Action("Details", "Campaign", null, protocol: Request.Scheme));" is "http%3A%2F%2Flocalhost%3A48408%2FCampaign%2F2"
            ViewBag.AbsoluteUrl = System.Net.WebUtility.UrlEncode(Url.Action("Details", "Campaign", null, protocol: Request.Scheme));

            return View("Details", new CampaignViewModel(campaign));
        }

        [HttpGet]
        [Route("~/[controller]/map/{id}")]
        public IActionResult LocationMap(int id)
        {
            var campaign = GetCampaign(id);

            if (campaign == null)
                return HttpNotFound();

            return View("Map", new CampaignViewModel(campaign));
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<CampaignViewModel> Get()
        {
            //TODO: refactor to mediator
            //CampaingGetQuery
            return _dataAccess.Campaigns.Where(c => !c.Locked).Select(x => new CampaignViewModel(x));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var campaign = GetCampaign(id);

            if (campaign == null || campaign.Locked)
                return HttpNotFound();

            return Json(campaign.ToViewModel());
            }

        private Campaign GetCampaign(int campaignId)
        {
            return _mediator.Send(new CampaginByCampaignIdQuery { CampaignId = campaignId });
        }
    }
}
