using Microsoft.AspNet.Mvc;

using PrepOps.Models;
using PrepOps.ViewModels;

using System.Linq;

namespace PrepOps.Controllers
{
    public class TenantController : Controller
    {
        IPrepOpsDataAccess _dataAccess;

        public TenantController(IPrepOpsDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [Route("Tenants/")]
        public IActionResult Index()
        {
            return View(_dataAccess.Tenants.Select(t => new TenantViewModel(t)).ToList());
        }

        [Route("Tenant/{id}/")]
        public IActionResult ShowTenant(int id)
        {
            var tenant = _dataAccess.GetTenant(id);

            if (tenant == null)
            {
                return HttpNotFound();
            }

            return View("Tenant", new TenantViewModel(tenant));
        }
    }
}
