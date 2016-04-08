using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Users;
using AllReady.Areas.Admin.Models;
using AllReady.Extensions;
using AllReady.Models;
using AllReady.Security;
using AllReady.Services;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.Extensions.Logging;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("SiteAdmin")]
    public class SiteController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IAllReadyDataAccess _dataAccess;
        private ILogger<SiteController> _logger;
        private readonly IMediator _mediator;

        public SiteController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IAllReadyDataAccess dataAccess, ILogger<SiteController> logger, IMediator mediator)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _dataAccess = dataAccess;
            _logger = logger;
            _mediator = mediator;
        }

        public IActionResult Index()
        {
            var viewModel = new SiteAdminModel()
            {
                Users = _dataAccess.Users.OrderBy(u => u.UserName).ToList()
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult DeleteUser(string userId)
        {
            var user = _mediator.Send(new UserQuery { UserId = userId });

            var viewModel = new DeleteUserModel()
            {
                UserId = userId,
                UserName = user.UserName,
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDeleteUser(string userId)
        {
            await _mediator.SendAsync(new DeleteUserCommand { UserId = userId });
            return RedirectToAction(nameof(Index));
        }

        public IActionResult EditUser(string userId)
        {
            var user = _dataAccess.GetUser(userId);
            var organizationId = user.GetOrganizationId();

            var viewModel = new EditUserModel
            {
                UserId = userId,
                UserName = user.UserName,
                AssociatedSkills = user.AssociatedSkills,
                IsOrganizationAdmin = user.IsUserType(UserType.OrgAdmin),
                IsSiteAdmin = user.IsUserType(UserType.SiteAdmin),
                Organization = organizationId != null ? _dataAccess.GetOrganization(organizationId.Value) : null
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
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

            var organizationAdminClaim = new Claim(Security.ClaimTypes.UserType, "OrgAdmin");
            if (viewModel.IsOrganizationAdmin)
            {
                //add organization admin claim
                var result = await _userManager.AddClaimAsync(user, organizationAdminClaim);
                if (result.Succeeded)
                {
                    //mgmccarthy: there is no Login action method on the AdminController. The only login method I could find is on the AccountController. Not too sure what to do here
                    var callbackUrl = Url.Action(new UrlActionContext { Action = "Login", Controller = "Admin", Values = new { Email = user.Email }, Protocol = HttpContext.Request.Scheme });
                    await _emailSender.SendEmailAsync(user.Email, "Account Approval", $"Your account has been approved by an administrator. Please <a href=\"{callbackUrl}\">Click here to Log in</a>");
                }
                else
                {
                    return Redirect("Error");
                }
            }
            else if (user.IsUserType(UserType.OrgAdmin))
            {
                //remove organization admin claim
                var result = await _userManager.RemoveClaimAsync(user, organizationAdminClaim);
                if (!result.Succeeded)
                {
                    return Redirect("Error");
                }
            }

            return RedirectToAction(nameof(Index));
        }

        //TODO: This should be an HttpPost but that also requires changes to the view that is calling this
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId)
        {
            try
            {
                var user = _dataAccess.GetUser(userId);
                if (user == null)
                {
                    ViewBag.ErrorMessage = $"User not found.";
                    return View();
                }

                // Send an email with this link
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //mgmccarthy: there is no ResetPassword action methd on the AdminController. Not too sure what to do here.
                var callbackUrl = Url.Action(new UrlActionContext { Action = "ResetPassword", Controller = "Admin", Values = new { userId = user.Id, code = code }, Protocol = HttpContext.Request.Scheme });
                await _emailSender.SendEmailAsync(user.Email, "Reset Password", $"Please reset your password by clicking here: <a href=\"{callbackUrl}\">link</a>");

                ViewBag.SuccessMessage = $"Sent password reset email for {user.UserName}.";

                return View();

            }
            catch (Exception ex)
            {
                _logger.LogError(@"Failed to reset password for {userId}", ex);
                ViewBag.ErrorMessage = $"Failed to reset password for {userId}. Exception thrown.";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> AssignSiteAdmin(string userId)
        {
            try
            {
                var user = _dataAccess.GetUser(userId);
                await _userManager.AddClaimAsync(user, new Claim(Security.ClaimTypes.UserType, UserType.SiteAdmin.ToName()));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(@"Failed to assign site admin for {userId}", ex);
                ViewBag.ErrorMessage = $"Failed to assign site admin for {userId}. Exception thrown.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult AssignOrganizationAdmin(string userId)
        {
            var user = _dataAccess.GetUser(userId);
            if (user.IsUserType(UserType.OrgAdmin) || user.IsUserType(UserType.SiteAdmin))
            {
                return RedirectToAction(nameof(Index));
            }

            var organizations = _dataAccess.Organizations
                .OrderBy(t => t.Name)
                .Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString() })
                .ToList();

            ViewBag.Organizations = new [] 
            {
                new SelectListItem { Selected = true, Text = "<Select One>", Value = "0" }
            }.Union(organizations);

            return View(new AssignOrganizationAdminModel { UserId = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignOrganizationAdmin(AssignOrganizationAdminModel model)
        {
            var user = _dataAccess.GetUser(model.UserId);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            if (model.OrganizationId == 0)
            {
                ModelState.AddModelError(nameof(AssignOrganizationAdminModel.OrganizationId), "You must pick a valid organization.");
            }

            if (ModelState.IsValid)
            {
                if (_dataAccess.Organizations.Any(t => t.Id == model.OrganizationId))
                {
                    await _userManager.AddClaimAsync(user, new Claim(Security.ClaimTypes.UserType, UserType.OrgAdmin.ToName()));
                    await _userManager.AddClaimAsync(user, new Claim(Security.ClaimTypes.Organization, model.OrganizationId.ToString()));
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(nameof(AssignOrganizationAdminModel.OrganizationId), "Invalid Organization. Please contact support.");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RevokeSiteAdmin(string userId)
        {
            try
            {
                var user = _dataAccess.GetUser(userId);
                await _userManager.RemoveClaimAsync(user, new Claim(Security.ClaimTypes.UserType, UserType.SiteAdmin.ToName()));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(@"Failed to assign site admin for {userId}", ex);
                ViewBag.ErrorMessage = $"Failed to assign site admin for {userId}. Exception thrown.";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> RevokeOrganizationAdmin(string userId)
        {
            try
            {
                var user = _dataAccess.GetUser(userId);
                var claims = await _userManager.GetClaimsAsync(user);
                await _userManager.RemoveClaimAsync(user, claims.First(c => c.Type == Security.ClaimTypes.UserType));
                await _userManager.RemoveClaimAsync(user, claims.First(c => c.Type == Security.ClaimTypes.Organization));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(@"Failed to assign site admin for {userId}", ex);
                ViewBag.ErrorMessage = $"Failed to assign site admin for {userId}. Exception thrown.";
                return View();
            }
        }
    }
}