using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels;
using Microsoft.AspNet.Mvc;

namespace AllReady.Controllers
{
    [Route("api/resource")]
    [Produces("application/json")]
    public class ResourceApiController :Controller
    {
        private readonly IAllReadyDataAccess _allReadyDataAccess;

        public ResourceApiController(IAllReadyDataAccess allReadyDataAccess)
        {
            _allReadyDataAccess = allReadyDataAccess;
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
