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
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View(_dataAccess.Campaigns));
        }

        // GET: Campaign/Create
        public async Task<IActionResult> Create()
        {
            return await Task.FromResult(WithTenants(View("Edit")));
        }

        // POST: Campaign/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Campaign campaign)
        {
            if (!UserIsTenantAdminOfCampaign(campaign))
            {
                return new HttpUnauthorizedResult();
            }

            if (ModelState.IsValid)
            {
                await _dataAccess.AddCampaign(campaign);
                return await Task.FromResult(RedirectToAction("Index", new { area = "Admin" }));
            }

            return await Task.FromResult(WithTenants(View("Edit", campaign)));
        }

        // GET: Campaign/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(404);
            }
            Campaign campaign = _dataAccess.GetCampaign((int)id);

            if (!UserIsTenantAdminOfCampaign(campaign))
            {
                return new HttpUnauthorizedResult();
            }
            if (campaign == null)
            {
                return new HttpStatusCodeResult(404);
            }

            return await Task.FromResult(WithTenants(View(campaign)));
        }

        // POST: Campaign/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Campaign campaign)
        {
            if (!UserIsTenantAdminOfCampaign(campaign))
            {
                return new HttpUnauthorizedResult();
            }

            if (ModelState.IsValid)
            {
                await _dataAccess.UpdateCampaign(campaign);
                return await Task.FromResult(RedirectToAction("Index", new { area = "Admin" }));
            }
            return await Task.FromResult(View(campaign));
        }

        // GET: Campaign/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(404);
            }

            Campaign campaign = _dataAccess.GetCampaign((int)id);

            if (campaign == null)
            {
                return new HttpStatusCodeResult(404);
            }
            if (!UserIsTenantAdminOfCampaign(campaign))
            {
                return new HttpUnauthorizedResult();
            }

            return await Task.FromResult(View(campaign));
        }

        // POST: Campaign/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!UserIsTenantAdminOfCampaign(id))
            {
                return new HttpUnauthorizedResult();
            }   

            await _dataAccess.DeleteCampaign(id);
            return await Task.FromResult(RedirectToAction("Index", new { area = "Admin" }));
        }

        private bool UserIsTenantAdminOfCampaign(Campaign campaignToCheck)
        {
            return User.IsUserType(UserType.SiteAdmin) ||
                campaignToCheck.ManagingTenantId == User.GetTenantId();
        }

        private bool UserIsTenantAdminOfCampaign(int campaignId)
        {
            return UserIsTenantAdminOfCampaign(_dataAccess.GetCampaign(campaignId));
        }
    }
}
