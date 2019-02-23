using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Features.Site;
using AllReady.Areas.Admin.Features.Users;
using AllReady.Areas.Admin.ViewModels.Site;
using AllReady.Extensions;
using AllReady.Features.Manage;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using AllReady.Constants;
using AllReady.Features.Campaigns;
using AllReady.Features.Events;
using AllReady.Features.Tasks;

namespace AllReady.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    [Authorize(nameof(UserType.SiteAdmin))]
    public class SiteController : Controller
    {
        public Func<DateTime> DateTimeNow = () => DateTime.Now;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SiteController> _logger;
        private readonly IMediator _mediator;

        public SiteController(UserManager<ApplicationUser> userManager, ILogger<SiteController> logger, IMediator mediator)
        {
            _userManager = userManager;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _mediator.SendAsync(new IndexQuery()));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _mediator.SendAsync(new UserQuery { UserId = userId });
            var campaigns = await _mediator.SendAsync(new CampaignByApplicationUserIdQuery() { ApplicationUserId = userId });
            var events = await _mediator.SendAsync(new EventsByApplicationUserIdQuery() { ApplicationUserId = userId });
            var volunteerTasks = await _mediator.SendAsync(new VolunteerTasksByApplicationUserIdQuery() { ApplicationUserId = userId });

            var viewModel = new DeleteUserViewModel
            {
                UserId = userId,
                UserName = user.UserName,
                IsSiteAdmin = user.IsSiteAdmin,
                OrganizationName = user.Organization?.Name,
                IsOrganizationAdmin = user.IsOrganizationAdmin,
                Campaigns = campaigns?.Select(x => x.Name),
                Events = events?.Select(x => x.Name),
                VolunteerTasks = volunteerTasks?.Select(x => x.Name)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditUser(string userId)
        {
            var user = await GetUser(userId);
            var organizationId = user.GetOrganizationId();

            var viewModel = new EditUserViewModel
            {
                UserId = userId,
                UserName = user.UserName,
                AssociatedSkills = user.AssociatedSkills,
                IsOrganizationAdmin = user.IsUserType(UserType.OrgAdmin),
                IsSiteAdmin = user.IsUserType(UserType.SiteAdmin),
                Organization = organizationId != null ? _mediator.Send(new OrganizationByIdQuery { OrganizationId = organizationId.Value }) : null
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            //Skill associations
            var user = await GetUser(viewModel.UserId);
            user.AssociatedSkills.RemoveAll(usk => viewModel.AssociatedSkills == null || !viewModel.AssociatedSkills.Any(msk => msk.SkillId == usk.SkillId));

            if (viewModel.AssociatedSkills != null)
            {
                user.AssociatedSkills.AddRange(viewModel.AssociatedSkills.Where(msk => !user.AssociatedSkills.Any(usk => usk.SkillId == msk.SkillId)));
            }

            if (user.AssociatedSkills != null && user.AssociatedSkills.Count > 0)
            {
                user.AssociatedSkills.ForEach(usk => usk.UserId = user.Id);
            }

            await _mediator.SendAsync(new UpdateUser { User = user });

            var organizationAdminClaim = new Claim(Security.ClaimTypes.UserType, nameof(UserType.OrgAdmin));
            if (viewModel.IsOrganizationAdmin)
            {
                //add organization admin claim
                var result = await _userManager.AddClaimAsync(user, organizationAdminClaim);
                if (result.Succeeded)
                {
                    //mgmccarthy: there is no Login action method on the AdminController. The only login method I could find is on the AccountController. Not too sure what to do here
                    var callbackUrl = Url.Action(new UrlActionContext { Action = nameof(AllReady.Controllers.AccountController.Login), Controller = "Account", Values = new { Email = user.Email }, Protocol = HttpContext.Request.Scheme });
                    await _mediator.SendAsync(new SendAccountApprovalEmailCommand { Email = user.Email, CallbackUrl = callbackUrl });
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


        [HttpGet]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            var user = await GetUser(userId);
            if (user?.LockoutEnd > DateTimeNow())
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeNow().Date.AddDays(-1));
            }
            return RedirectToAction(nameof(Index));
        }


        //TODO: This should be an HttpPost but that also requires changes to the view that is calling this
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId)
        {
            try
            {
                var user = await GetUser(userId);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "User not found.";
                    return View();
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //TODO: mgmccarthy: there is no ResetPassword action methd on the AdminController. Not too sure what to do here. Waiting for feeback via Issue #659
                var callbackUrl = Url.Action(new UrlActionContext { Action = nameof(AllReady.Controllers.AccountController.ResetPassword), Controller = "Account", Values = new { userId = user.Id, code = code }, Protocol = HttpContext.Request.Scheme });
                await _mediator.SendAsync(new Features.Site.SendResetPasswordEmailCommand { Email = user.Email, CallbackUrl = callbackUrl });

                ViewBag.SuccessMessage = $"Sent password reset email for {user.UserName}.";

                return View();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to reset password for {userId}", ex);
                ViewBag.ErrorMessage = $"Failed to reset password for {userId}. Exception thrown.";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> AssignSiteAdmin(string userId)
        {
            try
            {
                var user = await GetUser(userId);
                await _userManager.AddClaimAsync(user, new Claim(Security.ClaimTypes.UserType, nameof(UserType.SiteAdmin)));
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
        public async Task<IActionResult> AssignApiAccessRole(string userId)
        {
            try
            {
                var user = await GetUser(userId);
                await _userManager.AddClaimAsync(user, new Claim(Security.ClaimTypes.UserType, nameof(UserType.ApiAccess)));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to assign API role for {userId}", ex);
                ViewBag.ErrorMessage = $"Failed to assign API role for {userId}. Exception thrown.";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageApiKeys(string userId)
        {
            var user = await GetUser(userId);
            if (user.IsUserType(UserType.ApiAccess))
            {
                return View(user);
            }

            return BadRequest("Can't manage keys for a user without the API role.");
        }

        [HttpGet]
        public async Task<IActionResult> GenerateToken(string userId)
        {
            var user = await GetUser(userId);
            try
            {
                var token = await _userManager.GenerateUserTokenAsync(user, "Default", TokenTypes.ApiKey);
                ViewBag.ApiToken = token;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create API key for {userId}", ex);
                ViewBag.ErrorMessage = $"Failed to assign API role for {userId}. Exception thrown.";
                return View();
            }

        }

        [HttpGet]
        public async Task<IActionResult> AssignOrganizationAdmin(string userId)
        {
            var user = await GetUser(userId);
            if (user.IsUserType(UserType.OrgAdmin) || user.IsUserType(UserType.SiteAdmin))
            {
                return RedirectToAction(nameof(Index));
            }

            var organizations = await _mediator.SendAsync(new AllOrganizationsQuery());
            var selectListItems = organizations
                .OrderBy(t => t.Name)
                .Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString() })
                .ToList();

            ViewBag.Organizations = new[] { new SelectListItem { Selected = true, Text = "<Select One>", Value = "0" } }
                .Union(selectListItems);

            return View(new AssignOrganizationAdminViewModel { UserId = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignOrganizationAdmin(AssignOrganizationAdminViewModel model)
        {
            var user = await GetUser(model.UserId);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (model.OrganizationId == 0)
            {
                ModelState.AddModelError(nameof(AssignOrganizationAdminViewModel.OrganizationId), "You must pick a valid organization.");
            }

            if (ModelState.IsValid)
            {
                var organizations = await _mediator.SendAsync(new AllOrganizationsQuery());
                if (organizations.Any(t => t.Id == model.OrganizationId))
                {
                    await _userManager.AddClaimAsync(user, new Claim(Security.ClaimTypes.UserType, nameof(UserType.OrgAdmin)));
                    await _userManager.AddClaimAsync(user, new Claim(Security.ClaimTypes.Organization, model.OrganizationId.ToString()));
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(nameof(AssignOrganizationAdminViewModel.OrganizationId), "Invalid Organization. Please contact support.");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RevokeSiteAdmin(string userId)
        {
            try
            {
                var user = await GetUser(userId);
                await _userManager.RemoveClaimAsync(user, new Claim(Security.ClaimTypes.UserType, nameof(UserType.SiteAdmin)));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to revoke site admin for {userId}", ex);
                ViewBag.ErrorMessage = $"Failed to revoke site admin for {userId}. Exception thrown.";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> RevokeOrganizationAdmin(string userId)
        {
            try
            {
                var user = await GetUser(userId);
                var claims = await _userManager.GetClaimsAsync(user);
                await _userManager.RemoveClaimAsync(user, claims.First(c => c.Type == Security.ClaimTypes.UserType));
                await _userManager.RemoveClaimAsync(user, claims.First(c => c.Type == Security.ClaimTypes.Organization));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to revoke organization admin for {userId}", ex);
                ViewBag.ErrorMessage = $"Failed to revoke organization admin for {userId}. Exception thrown.";
                return View();
            }
        }

        private async Task<ApplicationUser> GetUser(string userId)
        {
            return await _mediator.SendAsync(new UserByUserIdQuery { UserId = userId });
        }
    }
}
