using AllReady.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AllReady.Areas.SiteAdmin.Controllers
{
    [Area("Admin")]
    [Authorize("SiteAdmin")]
    public class SiteController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public SiteController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            // Get the list of Users and whether they are TenantAdmins or not
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SearchViewModel model)
        {
            // Get the user and the UserType claim
            var user = await _userManager.FindByEmailAsync(model.Email);
            var claims = await _userManager.GetClaimsAsync(user);
            if (claims.Count <= 0)
            {
                model.TenantAdmin = false;
                return View("MakeUserTenantAdmin", model);
            }
            var claimValue = claims.FirstOrDefault(c => c.Type.Equals("UserType")).Value;
            if (!claimValue.Equals("TenantAdmin"))
            {
                model.TenantAdmin = false;
            }
            return View("MakeUserTenantAdmin", model);
        }

        [HttpGet]
        public IActionResult MakeUserTenantAdmin(SearchViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeUserTenantAdmin(SearchViewModel model, bool diff)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (model.TenantAdmin)
            {
                var result = await _userManager.AddClaimAsync(user, new Claim("UserType", "TenantAdmin"));
                if (result.Succeeded)
                {
                    ViewData["result"] = "Successfully approved user";
                    return View();
                }
                else
                {
                    ViewData["result"] = "Successfully approved user";
                    return Redirect("Error");
                }

            }
            return RedirectToAction("Index");
        }
    }
}
