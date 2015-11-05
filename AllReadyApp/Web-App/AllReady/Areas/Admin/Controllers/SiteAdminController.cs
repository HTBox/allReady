using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;

using AllReady.Models;

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Services;
using AllReady.Areas.Admin.ViewModels;
using AllReady.Security;

namespace AllReady.Areas.Admin.Controllers
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

        public IActionResult EditUser(string userId)
        {
            var user = _dataAccess.GetUser(userId);
            var tenantId = user.GetTenantId();
            var viewModel = new EditUserViewModel()
            {
                UserId = userId,
                UserName = user.UserName,
                AssociatedSkills = user.AssociatedSkills,
                IsTenantAdmin = user.IsTenantAdmin(),
                Tenant = tenantId != null ? _dataAccess.GetTenant(tenantId.Value) : null
            };
            return View(viewModel).WithSkills(_dataAccess);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel).WithSkills(_dataAccess);
            }

            //Skill associations
            var user = _dataAccess.GetUser(viewModel.UserId);
            user.AssociatedSkills.RemoveAll(usk => viewModel.AssociatedSkills == null || !viewModel.AssociatedSkills.Any(msk => msk.SkillId == usk.SkillId));
            if (viewModel.AssociatedSkills != null)
            {
                user.AssociatedSkills.AddRange(viewModel.AssociatedSkills.Where(msk => !user.AssociatedSkills.Any(usk => usk.SkillId == msk.SkillId)));
            }
            if (user.AssociatedSkills != null && user.AssociatedSkills.Count > 0)
            {
                user.AssociatedSkills.ForEach(usk => usk.UserId = user.Id);
            }
            await _dataAccess.UpdateUser(user);

            //TODO: Make user tenant admin part

            return RedirectToAction(nameof(Index));
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