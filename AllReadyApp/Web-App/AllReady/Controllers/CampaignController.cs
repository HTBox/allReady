using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using AllReady.Models;
using AllReady.ViewModels;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    public class CampaignController : Controller
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public CampaignController(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [HttpGet]
        [Route("~/[controller]")]
        public IActionResult Index()
        {
            return View(_dataAccess.Campaigns.ToViewModel().ToList());
        }

        [HttpGet]
        [Route("~/[controller]/{id}")]
        public IActionResult Details(int id)
        {
            var campaign = _dataAccess.GetCampaign(id);

            if (campaign == null)
                HttpNotFound();

            return View("Details", new CampaignViewModel(campaign));
        }

        [HttpGet]
        [Route("~/[controller]/map/{id}")]
        public IActionResult LocationMap(int id)
        {
            var campaign = _dataAccess.GetCampaign(id);

            if (campaign == null)
                HttpNotFound();

            return View("Map", new CampaignViewModel(campaign));
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<CampaignViewModel> Get()
        {
            return _dataAccess.Campaigns
                .Select(x => new CampaignViewModel(x));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public CampaignViewModel Get(int id)
        {
            var campaign = _dataAccess.GetCampaign(id);

            if (campaign == null)
                HttpNotFound();

            return campaign.ToViewModel();
        }
    }
}
