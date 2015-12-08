using Microsoft.AspNet.Mvc;

using AllReady.Models;
using AllReady.ViewModels;

using System.Linq;

namespace AllReady.Controllers
{
    public class TenantController : Controller
    {
        IAllReadyDataAccess _dataAccess;

        public TenantController(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [Route("Tenants/")]
        public IActionResult Index()
        {
            return View(_dataAccess.Organziations.Select(t => new TenantViewModel(t)).ToList());
        }

        [Route("Tenant/{id}/")]
        public IActionResult ShowTenant(int id)
        {
            var tenant = _dataAccess.GetOrganization(id);

            if (tenant == null)
            {
                return HttpNotFound();
            }

            return View("Tenant", new TenantViewModel(tenant));
        }
    }
}
