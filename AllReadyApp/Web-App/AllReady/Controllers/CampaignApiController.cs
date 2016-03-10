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
        private readonly IAllReadyDataAccess _allReadyDataAccess;
        
        public CampaignApiController(IAllReadyDataAccess allReadyDataAccess)
        {
            _allReadyDataAccess = allReadyDataAccess;
        }

        [Route("search")]
        public IEnumerable<ActivityViewModel> GetCampaignsByPostalCode(string zip, int miles)
        {
            var model = new List<ActivityViewModel>();

            var campaigns = _allReadyDataAccess.ActivitiesByPostalCode(zip, miles)
                .Select(x => x.Campaign)
                .Distinct();

            var activities = campaigns
                .SelectMany(x => x.Activities)
                .ToList();

           activities.ForEach(activity => model.Add(new ActivityViewModel(activity)));

           return model;
        }
    }
}
