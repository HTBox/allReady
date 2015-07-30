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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Post([FromBody]CampaignViewModel campaign)
        {
            if (campaign == null)
                HttpBadRequest();

            var exists = _dataAccess.GetCampaign(campaign.Id) != null;

            _dataAccess.AddCampaign(campaign.ToModel(_dataAccess));
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]CampaignViewModel campaign)
        {
            if (campaign == null)
                HttpBadRequest();

            var matching = _dataAccess.GetCampaign(campaign.Id);

            if (matching == null)
            {
                _dataAccess.AddCampaign(campaign.ToModel(_dataAccess));
            }
            else
            {
                matching.Activities = campaign.Activities.ToModel(_dataAccess).ToList();
                matching.Description = campaign.Description;
                matching.Name = campaign.Name;
                matching.StartDateTimeUtc = campaign.StartDate.UtcDateTime;
                matching.EndDateTimeUtc = campaign.EndDate.UtcDateTime;

                _dataAccess.UpdateCampaign(matching);
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _dataAccess.DeleteCampaign(id);
        }
    }
}
