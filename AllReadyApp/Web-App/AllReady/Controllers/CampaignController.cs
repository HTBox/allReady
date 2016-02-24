using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels;
using Microsoft.AspNet.Mvc;

namespace AllReady.Controllers
{
    [Route("api/[controller]")]
    public class CampaignController : Controller
    {
        private readonly IAllReadyDataAccess _dataAccess;

        //TODO: delete this before commiting
        [Key]
        public Guid Foo { get; set; }

        public CampaignController(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [HttpGet]
        [Route("~/[controller]")]
        public IActionResult Index()
        {
            return View(_dataAccess.Campaigns.Where(c => !c.Locked).ToViewModel().ToList());
        }

        [HttpGet]
        [Route("~/[controller]/{id}")]
        public IActionResult Details(int id)
        {
            var campaign = _dataAccess.GetCampaign(id);

            if (campaign == null || campaign.Locked)
                return HttpNotFound();

            ViewBag.AbsoluteUrl = System.Net.WebUtility.UrlEncode(Url.Action("Details", "Campaign", null, protocol: Request.Scheme));

            return View("Details", new CampaignViewModel(campaign));
        }

        [HttpGet]
        [Route("~/[controller]/map/{id}")]
        public IActionResult LocationMap(int id)
        {
            var campaign = _dataAccess.GetCampaign(id);

            if (campaign == null)
                return HttpNotFound();

            return View("Map", new CampaignViewModel(campaign));
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<CampaignViewModel> Get()
        {
            return _dataAccess.Campaigns
                .Where(c => !c.Locked)
                .Select(x => new CampaignViewModel(x));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var campaign = _dataAccess.GetCampaign(id);

            if (campaign == null || campaign.Locked)
            {
                return HttpNotFound();
            }

            return Json(campaign.ToViewModel());
        }
    }
}
