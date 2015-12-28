using Microsoft.AspNet.Mvc;
using AllReady.Models;
using AllReady.ViewModels;
using System.Linq;

namespace AllReady.Controllers
{
    public class OrganizationController : Controller
    {
        IAllReadyDataAccess _dataAccess;

        public OrganizationController(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [Route("Organizations/")]
        public IActionResult Index()
        {
            return View(_dataAccess.Organziations.Select(t => new OrganizationViewModel(t)).ToList());
        }

        [Route("Organization/{id}/")]
        public IActionResult ShowOrganization(int id)
        {
            var organization = _dataAccess.GetOrganization(id);

            if (organization == null)
            {
                return HttpNotFound();
            }

            return View("Organization", new OrganizationViewModel(organization));
        }
    }
}