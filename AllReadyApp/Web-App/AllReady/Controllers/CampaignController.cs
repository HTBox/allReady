using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using AllReady.Models;
using AllReady.ViewModels;

namespace AllReady.Controllers
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
            return View(_dataAccess.Campaigns.Where(c => c.Locked == false).ToViewModel().ToList());
        }

        [HttpGet]
        [Route("~/[controller]/{id}")]
        public IActionResult Details(int id)
        {
            var campaign = _dataAccess.GetCampaign(id);

            if (campaign == null || campaign.Locked)
                return HttpNotFound();

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
                .Where(c => c.Locked == false)
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
