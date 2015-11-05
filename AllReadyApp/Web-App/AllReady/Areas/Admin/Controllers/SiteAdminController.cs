using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;

using AllReady.Models;

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Services;
using AllReady.Areas.Admin.ViewModels;

namespace AllReady.Areas.SiteAdmin.Controllers
{
    [Area("Admin")]
    [Authorize("SiteAdmin")]
    public class SiteController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IAllReadyDataAccess _dataAccess;

        public SiteController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IAllReadyDataAccess dataAccess)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _dataAccess = dataAccess;
        }
        
        public IActionResult Index()
        {
            var viewModel = new SiteAdminViewModel()
            {
                Users = _dataAccess.Users.OrderBy(u => u.UserName).ToList()
            };
            return View(viewModel);
        }

        public async Task<IActionResult> EditUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var claims = await _userManager.GetClaimsAsync(user);
            var viewModel = new EditUserViewModel()
            {
                UserId = userId,
                UserName = user.UserName,
                AssociatedSkills = user.AssociatedSkills,
                IsTenantAdmin = claims.Any(c => c.Type == Security.ClaimTypes.UserType && c.Value == "TenantAdmin")
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(EditUserViewModel model)
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SearchViewModel model)
        {
            // Get the user and the UserType claim
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "No users matching this email address");
                return View();
            }
            
            //TODO: Simplify this logic
            var claims = await _userManager.GetClaimsAsync(user);
            if (claims.Count <= 0)
            {
                model.TenantAdmin = false;
                return View("MakeUserTenantAdmin", model);
            }
            var claimValue = claims.FirstOrDefault(c => c.Type.Equals(Security.ClaimTypes.UserType)).Value;
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
                var userTypeClaim = new Claim(Security.ClaimTypes.UserType, "TenantAdmin");
                //TODO: Also need to set the TenantId claim
                var result = await _userManager.AddClaimsAsync(user, new[] { userTypeClaim} ); 
                if(result.Succeeded)
                {
                    ViewData["result"] = "Successfully made user a tenant admin";
                    var callbackUrl = Url.Action("Login", "Admin", new { Email = model.Email }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(model.Email, "Account Approval", "Your account has been approved by an administrator. Please <a href=" + callbackUrl + ">Click here to Log in</a>");
                    return View();
                }
                else
                {
                    return Redirect("Error");
                }

            }
            return RedirectToAction("Index");
        }
    }
}