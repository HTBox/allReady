﻿using System;
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

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("SiteAdmin")]
    public class SiteController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SiteController> _logger;
        private readonly IMediator _mediator;

        public SiteController(UserManager<ApplicationUser> userManager, ILogger<SiteController> logger, IMediator mediator)
        {
            _userManager = userManager;
            _logger = logger;
            _mediator = mediator;
        }

        public IActionResult Index()
        {
            var viewModel = new SiteAdminModel
            {
                Users = _mediator.Send(new AllUsersQuery()).OrderBy(u => u.UserName).ToList()
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _mediator.SendAsync(new UserQuery { UserId = userId });
            var viewModel = new DeleteUserModel
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
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult EditUser(string userId)
        {
            var user = GetUser(userId);
            var organizationId = user.GetOrganizationId();

            var viewModel = new EditUserModel
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
        public async Task<IActionResult> EditUser(EditUserModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            //Skill associations
            var user = GetUser(viewModel.UserId);
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

            var organizationAdminClaim = new Claim(Security.ClaimTypes.UserType, "OrgAdmin");
            if (viewModel.IsOrganizationAdmin)
            {
                //add organization admin claim
                var result = await _userManager.AddClaimAsync(user, organizationAdminClaim);
                if (result.Succeeded)
                {
                    //mgmccarthy: there is no Login action method on the AdminController. The only login method I could find is on the AccountController. Not too sure what to do here
                    var callbackUrl = Url.Action(new UrlActionContext { Action = "Login", Controller = "Admin", Values = new { Email = user.Email }, Protocol = HttpContext.Request.Scheme });
                    await _mediator.SendAsync(new SendAccountApprovalEmail { Email = user.Email, CallbackUrl = callbackUrl });
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
                var user = GetUser(userId);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "User not found.";
                    return View();
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //TODO: mgmccarthy: there is no ResetPassword action methd on the AdminController. Not too sure what to do here. Waiting for feeback via Issue #659
                var callbackUrl = Url.Action(new UrlActionContext { Action = "ResetPassword", Controller = "Admin", Values = new { userId = user.Id, code = code }, Protocol = HttpContext.Request.Scheme });
                await _mediator.SendAsync(new Features.Site.SendResetPasswordEmail { Email = user.Email, CallbackUrl = callbackUrl });

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
                var user = GetUser(userId);
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
        public async Task<IActionResult> AssignApiAccessRole(string userId)
        {
            try
            {
                var user = GetUser(userId);
                await _userManager.AddClaimAsync(user, new Claim(Security.ClaimTypes.UserType, UserType.ApiAccess.ToName()));
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
        public IActionResult ManageApiKeys(string userId)
        {
            var user = GetUser(userId);
            
            if(user.IsUserType(UserType.ApiAccess))
            {
                return View(user);
            }
            else
            {
                return BadRequest("Can't manage keys for a user without the API role.");
            }

        }

        [HttpGet]
        public async Task<IActionResult> GenerateToken(string userId)
        {
            var user = GetUser(userId);
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
        public IActionResult AssignOrganizationAdmin(string userId)
        {
            var user = GetUser(userId);
            if (user.IsUserType(UserType.OrgAdmin) || user.IsUserType(UserType.SiteAdmin))
            {
                return RedirectToAction(nameof(Index));
            }

            var organizations = _mediator.Send(new AllOrganizationsQuery())
                .OrderBy(t => t.Name)
                .Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString() })
                .ToList();

            ViewBag.Organizations = new [] { new SelectListItem { Selected = true, Text = "<Select One>", Value = "0" }}
                .Union(organizations);

            return View(new AssignOrganizationAdminModel { UserId = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignOrganizationAdmin(AssignOrganizationAdminModel model)
        {
            var user = GetUser(model.UserId);
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
                if (_mediator.Send(new AllOrganizationsQuery()).Any(t => t.Id == model.OrganizationId))
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
                var user = GetUser(userId);
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
                var user = GetUser(userId);
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

        private ApplicationUser GetUser(string userId)
        {
            return _mediator.Send(new UserByUserIdQuery { UserId = userId });
        }
    }
}