using AllReady.Models;
using AllReady.Services;
using AllReady.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Controllers
{
    [Route("api/resource")]
    [Produces("application/json")]
    public class ResourceApiController :Controller
    {
        private readonly IAllReadyDataAccess _allReadyDataAccess;
        private readonly UserManager<ApplicationUser> _userManager;

        public ResourceApiController(IAllReadyDataAccess allReadyDataAccess,
               UserManager<ApplicationUser> userManager)
        {
            _allReadyDataAccess = allReadyDataAccess;
            _userManager = userManager;
        }

        [Route("search")]
        public IEnumerable<ResourceViewModel> GetResourcesByCategory(string category)
        {
            List<ResourceViewModel> ret = new List<ResourceViewModel>();

            var resources = (from c in _allReadyDataAccess.GetResourcesByCategory(category)
                             select c).Distinct();

            foreach (Resource resource in resources)
            {
                ret.Add(new ResourceViewModel(resource));
            }
            return ret;
        }

    }
}
