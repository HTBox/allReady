using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Configuration;
using AllReady.Constants;
using AllReady.Features.Login;
using AllReady.Features.Manage;
using AllReady.Models;
using AllReady.Providers.ExternalUserInformationProviders;
using AllReady.Security;
using AllReady.ViewModels.Account;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using UserType = AllReady.Models.UserType;

namespace AllReady.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOptions<GeneralSettings> _generalSettings;
        private readonly IMediator _mediator;
        private readonly IExternalUserInformationProviderFactory _externalUserInformationProviderFactory;
        private readonly IRedirectAccountControllerRequests _redirectAccountControllerRequests;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<GeneralSettings> generalSettings,
            IMediator mediator,
            IExternalUserInformationProviderFactory externalUserInformationProviderFactory,
            IRedirectAccountControllerRequests redirectAccountControllerRequests
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _generalSettings = generalSettings;
            _mediator = mediator;
            _externalUserInformationProviderFactory = externalUserInformationProviderFactory;
            _redirectAccountControllerRequests = redirectAccountControllerRequests;
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // Require admin users to have a confirmed email before they can log on.
                var user = await _mediator.SendAsync(new ApplicationUserQuery { UserName = model.Email });
                if (user != null)
                {
                    var isAdminUser = user.IsUserType(UserType.OrgAdmin) || user.IsUserType(UserType.SiteAdmin);
                    if (isAdminUser && !await _userManager.IsEmailConfirmedAsync(user))
                    {
                        //TODO: Showing the error page here makes for a bad experience for the user.
                        //It would be better if we redirected to a specific page prompting the user to check their email for a confirmation email and providing an option to resend the confirmation email.
                        ViewData["Message"] = "You must have a confirmed email to log on.";
                        return View("Error");
                    }
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    return _redirectAccountControllerRequests.RedirectToLocal(returnUrl, user);
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(AdminController.SendCode), AreaNames.Admin, new { ReturnUrl = returnUrl, model.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                return View(model);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    TimeZoneId = _generalSettings.Value.DefaultTimeZone
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var callbackUrl = Url.Action(new UrlActionContext
                    {
                        Action = nameof(ConfirmEmail),
                        Controller = "Account",
                        Values = new { userId = user.Id, token = emailConfirmationToken },
                        Protocol = HttpContext.Request.Scheme
                    });

                    await _mediator.SendAsync(new SendConfirmAccountEmail { Email = user.Email, CallbackUrl = callbackUrl });

                    var changePhoneNumberToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
                    await _mediator.SendAsync(new SendAccountSecurityTokenSms { PhoneNumber = model.PhoneNumber, Token = changePhoneNumberToken });

                    await _userManager.AddClaimAsync(user, new Claim(Security.ClaimTypes.ProfileIncomplete, "NewUser"));
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    TempData["NewAccount"] = true;
                    
                    return RedirectToPage("/Index");
                }

                AddErrorsToModelState(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded && user.IsProfileComplete())
            {
                await _mediator.SendAsync(new RemoveUserProfileIncompleteClaimCommand { UserId = user.Id });
                if (_signInManager.IsSignedIn(User))
                {
                    await _signInManager.RefreshSignInAsync(user);
                }
            }

            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(new UrlActionContext { Action = nameof(ResetPassword), Controller = "Account", Values = new { userId = user.Id, code },
                    Protocol = HttpContext.Request.Scheme });
                await _mediator.SendAsync(new SendResetPasswordEmail { Email = model.Email, CallbackUrl = callbackUrl });

                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            AddErrorsToModelState(result);

            return View();
        }

        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(new UrlActionContext { Action = nameof(ExternalLoginCallback), Values = new { ReturnUrl = returnUrl } });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var externalLoginSignInAsyncResult = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, isPersistent: false);
            var externalUserInformationProvider = _externalUserInformationProviderFactory.GetExternalUserInformationProvider(externalLoginInfo.LoginProvider);
            var externalUserInformation = await externalUserInformationProvider.GetExternalUserInformation(externalLoginInfo);

            if (externalLoginSignInAsyncResult.Succeeded)
            {
                if (string.IsNullOrEmpty(externalUserInformation.Email))
                   return View("Error");

                var user = await _mediator.SendAsync(new ApplicationUserQuery { UserName = externalUserInformation.Email });
                return _redirectAccountControllerRequests.RedirectToLocal(returnUrl, user);
            }

            // If the user does not have an account, then ask the user to create an account.
            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel
            {
                Email = externalUserInformation.Email,
                FirstName = externalUserInformation.FirstName,
                LastName = externalUserInformation.LastName,
                ReturnUrl = returnUrl,
                LoginProvider = externalLoginInfo.LoginProvider,
                EmailIsVerifiedByExternalLoginProvider = !string.IsNullOrEmpty(externalUserInformation.Email)
            });
        }

        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction(nameof(ManageController.Index), "Manage");
            }

            if (ModelState.IsValid)
            {
                var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
                if (externalLoginInfo == null)
                {
                    return View("ExternalLoginFailure");
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    TimeZoneId = _generalSettings.Value.DefaultTimeZone,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, externalLoginInfo);
                    if (result.Succeeded)
                    {
                        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        if (model.EmailIsVerifiedByExternalLoginProvider)
                        {
                            await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);
                        }
                        else
                        {
                            var callbackUrl = Url.Action(new UrlActionContext
                            {
                                Action = nameof(ConfirmEmail),
                                Controller = "Account",
                                Values = new { userId = user.Id, token = emailConfirmationToken },
                                Protocol = HttpContext.Request.Scheme
                            });
                            await _mediator.SendAsync(new SendConfirmAccountEmail { Email = user.Email, CallbackUrl = callbackUrl });
                        }

                        var changePhoneNumberToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
                        await _mediator.SendAsync(new SendAccountSecurityTokenSms { PhoneNumber = model.PhoneNumber, Token = changePhoneNumberToken });

                        await _userManager.AddClaimAsync(user, new Claim(Security.ClaimTypes.ProfileIncomplete, "NewUser"));
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        return _redirectAccountControllerRequests.RedirectToLocal(returnUrl, user);
                    }
                }

                AddErrorsToModelState(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        private void AddErrorsToModelState(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }

    public interface IRedirectAccountControllerRequests
    {
        IActionResult RedirectToLocal(string returnUrl, ApplicationUser user);
    }

    public class RedirectAccountControllerRequests : IRedirectAccountControllerRequests
    {
        private readonly IUrlHelper _urlHelper;

        public RedirectAccountControllerRequests(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public IActionResult RedirectToLocal(string returnUrl, ApplicationUser user)
        {
            if (_urlHelper.IsLocalUrl(returnUrl))
            {
                return new RedirectResult(returnUrl);
            }

            if (user.IsUserType(UserType.SiteAdmin))
            {
                return new RedirectToActionResult(nameof(SiteController.Index), "Site", new { area = AreaNames.Admin });
            }

            if (user.IsUserType(UserType.OrgAdmin))
            {
                return new RedirectToActionResult(nameof(Areas.Admin.Controllers.CampaignController.Index), "Campaign", new { area = AreaNames.Admin });
            }

            return new RedirectToPageResult("/Index");
        }
    }
}
