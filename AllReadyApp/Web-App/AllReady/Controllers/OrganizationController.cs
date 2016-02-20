using Microsoft.AspNet.Mvc;
using AllReady.Models;
using AllReady.ViewModels;
using System.Linq;
using MediatR;
using System;
using AllReady.Features.Organizations;
using System.Threading.Tasks;

namespace AllReady.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly IMediator _bus;
        IAllReadyDataAccess _dataAccess;

        public OrganizationController(IMediator bus, IAllReadyDataAccess dataAccess)
        {
            if (bus == null)
                throw new ArgumentNullException(nameof(bus));

            _bus = bus;
            _dataAccess = dataAccess;
        }

        [Route("Organizations/")]
        public IActionResult Index()
        {
            return View(_dataAccess.Organziations.Select(t => new OrganizationViewModel(t)).ToList());
        }

        [Route("Organization/{id}/")]
        public async Task<IActionResult> ShowOrganization(int id)
        {
            if (id <= 0)
            { 
                return HttpNotFound();
            }            

            OrganizationViewModel model = await _bus.SendAsync(new OrganizationDetailsQueryAsync { Id = id });

            if (model == null)
            {
                return HttpNotFound();
            }

            return View("Organization", model);
        }
    }
}