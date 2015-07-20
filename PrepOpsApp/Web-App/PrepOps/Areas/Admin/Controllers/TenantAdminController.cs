using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using PrepOps.Models;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PrepOps.Areas.Admin.Controllers
{
    //[Area("TenantAdmin")]
    [Area("Admin")]
    [Authorize("TenantAdmin")]
    public class TenantController : Controller
    {
        private readonly IPrepOpsDataAccess _dataAccess;

        public TenantController(IPrepOpsDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        // GET: Tenant
        public async Task<IActionResult> Index()
        {
            return await Task.Run(() => View(_dataAccess.Tenants));
        }

        // GET: Tenant/Details/5
        public async Task<IActionResult> Details(System.Int32? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(404);
            }

            Tenant tenant = _dataAccess.GetTenant((int)id);
            if (tenant == null)
            {
                return new HttpStatusCodeResult(404);
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
        public async Task<IActionResult> Create(Tenant tenant)
        {
            if (ModelState.IsValid)
            {
                await _dataAccess.AddTenant(tenant);
                return RedirectToAction("Index");
            }

            return View(tenant);
        }

        // GET: Tenant/Edit/5
        public async Task<IActionResult> Edit(System.Int32? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(404);
            }

            Tenant tenant = _dataAccess.GetTenant((int)id);
            if (tenant == null)
            {
                return new HttpStatusCodeResult(404);
            }

            return View(tenant);
        }

        // POST: Tenant/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Tenant tenant)
        {
            if (ModelState.IsValid)
            {
                await _dataAccess.UpdateTenant(tenant);
                return RedirectToAction("Index");
            }

            return View(tenant);
        }

        // GET: Tenant/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(System.Int32? id)
        {
            // Needs comments:  This method doesn't delete things.
            if (id == null)
            {
                return new HttpStatusCodeResult(404);
            }
            Tenant tenant = _dataAccess.GetTenant((int)id);
            if (tenant == null)
            {
                return new HttpStatusCodeResult(404);
            }

            return View(tenant);
        }

        // POST: Tenant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(System.Int32 id)
        {
            await _dataAccess.DeleteTenant(id);
            return RedirectToAction("Index");
        }
    }
}
