using Microsoft.AspNet.Mvc;
using AllReady.Models;
using Microsoft.AspNet.Authorization;
using MediatR;
using AllReady.Areas.Admin.Features.Tenants;
using AllReady.Areas.Admin.Models;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("SiteAdmin")]
    public class TenantController : Controller
    {
        private readonly IMediator _bus;

        public TenantController(IMediator bus)
        {
            _bus = bus;
        }

        // GET: Tenant
        public IActionResult Index()
        {
            var list = _bus.Send(new TenantListQuery());
            return View(list);
        }

        // GET: Tenant/Details/5
        public IActionResult Details(int id)
        {
            var tenant = _bus.Send(new TenantDetailQuery { Id = id });
            if (tenant == null)
            {
                return HttpNotFound();
            }

            return View(tenant);
        }

        // GET: Tenant/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tenant/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Create(TenantEditModel tenant)
        {
            if (ModelState.IsValid)
            {
                int id = _bus.Send(new TenantEditCommand { Tenant = tenant });
                return RedirectToAction("Index");
            }

            return View(tenant);
        }

        // GET: Tenant/Edit/5
        public IActionResult Edit(int id)
        {

            var tenant = _bus.Send(new TenantEditQuery { Id = id });
            if (tenant == null)
            {
                return HttpNotFound();
            }

            return View("Edit",tenant);
        }

        // POST: Tenant/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TenantEditModel tenant)
        {
            if (ModelState.IsValid)
            {
                int id = _bus.Send(new TenantEditCommand { Tenant = tenant });
                return RedirectToAction("Details", new { id = id, area = "Admin" });
            }

            return View("Edit", tenant);
        }

        // GET: Tenant/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(int? id)
        {
            // Needs comments:  This method doesn't delete things.
            if (id == null)
            {
                return HttpNotFound();
            }
            var tenant = _bus.Send(new TenantDetailQuery { Id = id.Value });
            if (tenant == null)
            {
                return HttpNotFound();
            }

            return View(tenant);
        }

        // POST: Tenant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _bus.Send(new TenantDeleteCommand { Id= id });
            return RedirectToAction("Index");
        }
    }
}
