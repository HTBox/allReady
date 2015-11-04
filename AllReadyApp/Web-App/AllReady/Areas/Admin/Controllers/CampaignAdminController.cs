using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using AllReady.Security;
using AllReady.Models;

namespace AllReady.Controllers
{
    [Area("Admin")]
    [Authorize("TenantAdmin")]
    public class CampaignController : Controller
    {
        private IAllReadyDataAccess _dataAccess;

        public CampaignController(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        private ViewResult WithTenants(ViewResult view)
        {
            view.ViewData["Tenants"] = _dataAccess.Tenants.Select(t => new SelectListItem() { Value = t.Id.ToString(), Text = t.Name });
            return view;
        }

        // GET: Campaign
        public IActionResult Index()
        {
            return View(_dataAccess.Campaigns);
        }

        public IActionResult Details(int id)
        {
            Campaign campaign = _dataAccess.GetCampaign(id);

            if (campaign == null)
            {
                return HttpNotFound();
            }

            if (!User.IsTenantAdmin(campaign.ManagingTenantId))
            {
                return HttpUnauthorized();
            }

            return View(campaign);
        }

        // GET: Campaign/Create
        public IActionResult Create()
        {
            return WithTenants(View("Edit"));
        }

        // POST: Campaign/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Campaign campaign)
        {
            if (campaign == null)
            {
                return HttpBadRequest();
            }

            if (!User.IsTenantAdmin(campaign.ManagingTenantId))
            {
                return HttpUnauthorized();
            }

            if (ModelState.IsValid)
            {
                await _dataAccess.AddCampaign(campaign);
                return RedirectToAction("Index", new { area = "Admin" });
            }

            return WithTenants(View("Edit", campaign));
        }

        // GET: Campaign/Edit/5
        public IActionResult Edit(int id)
        {
            Campaign campaign = _dataAccess.GetCampaign(id);

            if (campaign == null)
            {
                return HttpNotFound();
            }

            if (!User.IsTenantAdmin(campaign.ManagingTenantId))
            {
                return HttpUnauthorized();
            }

            return WithTenants(View(campaign));
        }

        // POST: Campaign/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Campaign campaign)
        {
            if (campaign == null)
            {
                return HttpBadRequest();
            }

            if (!User.IsTenantAdmin(campaign.ManagingTenantId))
            {
                return HttpUnauthorized();
            }

            if (ModelState.IsValid)
            {
                await _dataAccess.UpdateCampaign(campaign);
                return RedirectToAction("Index", new { area = "Admin" });
            }
            return View(campaign);
        }        

        // GET: Campaign/Delete/5
        public IActionResult Delete(int id)
        {
            Campaign campaign = _dataAccess.GetCampaign(id);

            if (campaign == null)
            {
                return HttpNotFound();
            }
            if (!User.IsTenantAdmin(campaign.ManagingTenantId))
            {
                return HttpUnauthorized();
            }

            return View(campaign);
        }

        // POST: Campaign/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var campaign = _dataAccess.GetCampaign(id);

            if (!User.IsTenantAdmin(campaign.ManagingTenantId))
            {
                return HttpUnauthorized();
            }   

            await _dataAccess.DeleteCampaign(id);
            return RedirectToAction("Index", new { area = "Admin" });
        }
    }
}
