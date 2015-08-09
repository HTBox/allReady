using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using AllReady.Models;
using AllReady.Services;
using AllReady.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AllReady.Controllers
{
    [Route("api/campaign")]
    [Produces("application/json")]
    public class CampaignApiController : Controller
    {        
        private const double MILES_PER_METER = 0.00062137;
        private readonly IAllReadyDataAccess _allReadyDataAccess;
        private readonly UserManager<ApplicationUser> _userManager;
        private IClosestLocations _closestLocations;


        public CampaignApiController(IAllReadyDataAccess allReadyDataAccess,
            UserManager<ApplicationUser> userManager,
            //GeoService geoService,
            IClosestLocations closestLocations)
        {
            _allReadyDataAccess = allReadyDataAccess;
            _userManager = userManager;
            //_geoService = geoService;
            _closestLocations = closestLocations;
        }

        [Route("search")]
        public IEnumerable<CampaignViewModel> GetCampaignsByZip(string zip, int miles)
        {
            List<CampaignViewModel> ret = new List<CampaignViewModel>();

            var campaigns = (from c in _allReadyDataAccess.ActivitiesByPostalCode(zip, miles)
                            select c.Campaign).Distinct().ToList();

            foreach (Campaign campaign in campaigns)
            {
                ret.Add(new CampaignViewModel(campaign));
            }

            return ret;
        }
    }
}
