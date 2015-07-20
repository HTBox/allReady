using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;

using PrepOps.Extensions;
using PrepOps.Models;

namespace PrepOps.Controllers
{
    [Area("Admin")]
    [Authorize("TenantAdmin")]
    public class CampaignController : Controller
    {
        private IPrepOpsDataAccess _dataAccess;
        private readonly UserManager<ApplicationUser> _userManager;

        public CampaignController(IPrepOpsDataAccess dataAccess, UserManager<ApplicationUser> userManager)
        {
            _dataAccess = dataAccess;
            _userManager = userManager;
        }

        private ViewResult WithTenants(ViewResult view)
        {
            view.ViewData["Tenants"] = _dataAccess.Tenants.Select(t => new SelectListItem() { Value = t.Id.ToString(), Text = t.Name });
            return view;
        }

        // GET: Campaign
        public async Task<IActionResult> Index()
        {
            return await Task.Run(() => View(_dataAccess.Campaigns));
        }

        // GET: Campaign/Details/5
        public async Task<IActionResult> Details(System.Int32? id)
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

            return View(campaign);
        }

        // GET: Campaign/Create
        public async Task<IActionResult> Create()
        {
            return WithTenants(View());
        }

        // POST: Campaign/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Campaign campaign)
        {
            var currentUser = await _userManager.GetCurrentUser(Context);
            if (currentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            // Workaround: Sometimes AssociatedTenant is missing when using UserManager
            var currentUserWithAssociatedTenant = _dataAccess.GetUser(currentUser.Id);
            if (!await UserIsTenantAdminOfCampaign(currentUserWithAssociatedTenant, campaign))
            {
                return new HttpUnauthorizedResult();
            }

            if (ModelState.IsValid)
            {
                await _dataAccess.AddCampaign(campaign);
                return RedirectToAction("Index");
            }

            return WithTenants(View(campaign));
        }

        // GET: Campaign/Edit/5
        public async Task<IActionResult> Edit(System.Int32? id)
        {
            var currentUser = await _userManager.GetCurrentUser(Context);
            if (currentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(404);
            }
            Campaign campaign = _dataAccess.GetCampaign((int)id);

            if (!await UserIsTenantAdminOfCampaign(currentUser, campaign))
            {
                return new HttpUnauthorizedResult();
            }
            if (campaign == null)
            {
                return new HttpStatusCodeResult(404);
            }

            return WithTenants(View(campaign));
        }

        // POST: Campaign/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Campaign campaign)
        {
            var currentUser = await _userManager.GetCurrentUser(Context);
            if (currentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            if (_dataAccess.GetUser(currentUser.Id).AssociatedTenant.Id != campaign.ManagingTenantId)
            {
                return new HttpUnauthorizedResult();
            }

            if (ModelState.IsValid)
            {
                await _dataAccess.UpdateCampaign(campaign);
                return RedirectToAction("Index");
            }
            return View(campaign);
        }

        // GET: Campaign/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(System.Int32? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(404);
            }

            var currentUser = await _userManager.GetCurrentUser(Context);
            if (currentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            Campaign campaign = _dataAccess.GetCampaign((int)id);

            if (campaign == null)
            {
                return new HttpStatusCodeResult(404);
            }
            if (!await UserIsTenantAdminOfCampaign(currentUser, campaign))
            {
                return new HttpUnauthorizedResult();
            }

            return View(campaign);
        }

        // POST: Campaign/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(System.Int32 id)
        {
            var currentUser = await _userManager.GetCurrentUser(Context);
            if (currentUser == null || !await UserIsTenantAdminOfCampaign(currentUser, id))
            {
                return new HttpUnauthorizedResult();
            }   

            await _dataAccess.DeleteCampaign(id);
            return RedirectToAction("Index");
        }

        private async Task<bool> UserIsTenantAdminOfCampaign(ApplicationUser user, Campaign campaignToCheck)
        {
            return await _userManager.IsSiteAdmin(user) ||
                ((user.AssociatedTenant != null) && (campaignToCheck.ManagingTenantId == user.AssociatedTenant.Id));
        }

        private async Task<bool> UserIsTenantAdminOfCampaign(ApplicationUser user, int campaignId)
        {
            return await UserIsTenantAdminOfCampaign(user, _dataAccess.GetCampaign(campaignId));
        }
    }
}
