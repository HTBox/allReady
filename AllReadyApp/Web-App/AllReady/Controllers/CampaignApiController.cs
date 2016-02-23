using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels;
using Microsoft.AspNet.Mvc;

namespace AllReady.Controllers
{
    [Route("api/campaign")]
    [Produces("application/json")]
    public class CampaignApiController : Controller
    {        
        private const double MILES_PER_METER = 0.00062137;
        private readonly IAllReadyDataAccess _allReadyDataAccess;
        

        public CampaignApiController(IAllReadyDataAccess allReadyDataAccess)
        {
            _allReadyDataAccess = allReadyDataAccess;
        }

        [Route("search")]
        public IEnumerable<ActivityViewModel> GetCampaignsByZip(string zip, int miles)
        {
            List<ActivityViewModel> ret = new List<ActivityViewModel>();

            var campaigns = (from c in _allReadyDataAccess.ActivitiesByPostalCode(zip, miles)
                              select c.Campaign).Distinct();

            var activities = (from c in campaigns
                              from p in c.Activities
                              select p);                           

            foreach (Activity activity in activities)
            {
                ret.Add(new ActivityViewModel(activity));
            }

            return ret;
        }
    }
}
