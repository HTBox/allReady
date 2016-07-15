using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Features.Login;
using AllReady.Features.Manage;
using AllReady.Models;
using AllReady.Security;
using AllReady.Services;
using AllReady.ViewModels.Account;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.Extensions.OptionsModel;

namespace AllReady.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IOptions<GeneralSettings> _generalSettings;
        private readonly IMediator _mediator;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IOptions<GeneralSettings> generalSettings,
            IMediator mediator
            )
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _signInManager = signInManager;
            _generalSettings = generalSettings;
            _mediator = mediator;
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData[RETURN_URL] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData[RETURN_URL] = returnUrl;

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
                        //      It would be better if we redirected to a specific page prompting the user to check their email for a confirmation email and providing an option to resend the confirmation email.
                        ViewData[MESSAGE] = "You must have a confirmed email to log on.";
                        return View(ERROR_VIEW);
                    }
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl, user);
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(AdminController.SendCode), ADMIN_CONTROLLER, new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    return View(LOCKOUT_VIEW);
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
                    // Send an email with this link
                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var callbackUrl = Url.Action(new UrlActionContext {
                        Action = nameof(ConfirmEmail),
                        Controller = ACCOUNT_CONTROLLER,
                        Values = new { userId = user.Id, token = emailConfirmationToken },
                        Protocol = HttpContext.Request.Scheme }
                    );

                    await _emailSender.SendEmailAsync(model.Email, "Confirm your allReady account", 
                        $"Please confirm your allReady account by clicking this link: <a href=\"{callbackUrl}\">link</a>");

                    var changePhoneNumberToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
                    await _mediator.SendAsync(new SendAccountSecurityTokenSms { PhoneNumber = model.PhoneNumber, Token = changePhoneNumberToken });

                    await _userManager.AddClaimAsync(user, new Claim(Security.ClaimTypes.ProfileIncomplete, "NewUser"));
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction(nameof(HomeController.Index), HOME_CONTROLLER);
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
            return RedirectToAction(nameof(HomeController.Index), HOME_CONTROLLER);
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return View(ERROR_VIEW);
            }
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View(ERROR_VIEW);
            }
            
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded && user.IsProfileComplete())
            {
                await _mediator.SendAsync(new RemoveUserProfileIncompleteClaimCommand { UserId = user.Id });
                if (User.IsSignedIn())
                {
                    await _signInManager.RefreshSignInAsync(user);
                }
            }

            return View(result.Succeeded ? CONFIRM_EMAIL_VIEW : ERROR_VIEW);
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
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View(FORGOT_PASSWORD_CONFIRMATION_VIEW);
                }

                //Send an email with this link
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(new UrlActionContext { Action = nameof(ResetPassword), Controller = ACCOUNT_CONTROLLER, Values = new { userId = user.Id, code = code },
                    Protocol = HttpContext.Request.Scheme });
                await _emailSender.SendEmailAsync(model.Email, "Reset allReady Password", $"Please reset your allReady password by clicking here: <a href=\"{callbackUrl}\">link</a>");

                return View(FORGOT_PASSWORD_CONFIRMATION_VIEW);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View(ERROR_VIEW) : View();
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
            var redirectUrl = Url.Action(new UrlActionContext { Action = nameof(ExternalLoginCallback), Values = new { ReturnUrl = returnUrl }});
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
            var result = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, isPersistent: false);
            var email = externalLoginInfo.ExternalPrincipal.FindFirstValue(System.Security.Claims.ClaimTypes.Email);

            string firstName;
            string lastName;
            RetrieveFirstAndLastNameFromExternalPrincipal(externalLoginInfo, out firstName, out lastName);
            
            if (result.Succeeded)
            {
                var user = await _mediator.SendAsync(new ApplicationUserQuery { UserName = email });
                return RedirectToLocal(returnUrl, user);
            }
            
            // If the user does not have an account, then ask the user to create an account.
            ViewData[RETURN_URL] = returnUrl;
            ViewData[LOGIN_PROVIDER] = externalLoginInfo.LoginProvider;

            return View(EXTERNAL_LOGIN_CONFIRMATION_VIEW, new ExternalLoginConfirmationViewModel { Email = email, FirstName = firstName, LastName = lastName });
        }

        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (User.IsSignedIn())
            {
                return RedirectToAction(nameof(ManageController.Index), MANAGE_CONTROLLER);
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
                if (externalLoginInfo == null)
                {
                    return View(EXTERNAL_LOGIN_FAILURE_VIEW);
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
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl, user);
                    }
                }

                AddErrorsToModelState(result);
            }

            ViewData[RETURN_URL] = returnUrl;
            return View(model);
        }

        private void AddErrorsToModelState(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl, ApplicationUser user)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (user.IsUserType(UserType.SiteAdmin))
            {
                return RedirectToAction(nameof(SiteController.Index), SITE_CONTROLLER, new { area = ADMIN_AREA });
            }

            if (user.IsUserType(UserType.OrgAdmin))
            {
                return RedirectToAction(nameof(Areas.Admin.Controllers.CampaignController.Index), CAMPAIGN_CONTROLLER, new { area = ADMIN_AREA });
            }

            return RedirectToAction(nameof(HomeController.Index), HOME_CONTROLLER);
        }

        private static void RetrieveFirstAndLastNameFromExternalPrincipal(ExternalLoginInfo externalLoginInfo, out string firstName, out string lastName)
        {
            var name = externalLoginInfo.ExternalPrincipal.FindFirstValue(System.Security.Claims.ClaimTypes.Name);

            firstName = string.Empty;
            lastName = string.Empty;
            if (string.IsNullOrEmpty(name))
                return;

            var array = name.Split(' ');
            firstName = array[0];
            lastName = array[1];
        }
        
        //Area Names
        private const string ADMIN_AREA = "Admin";

        //Controller Names
        private const string HOME_CONTROLLER = "Home";
        private const string MANAGE_CONTROLLER = "Manage";
        private const string SITE_CONTROLLER = "Site";
        private const string CAMPAIGN_CONTROLLER = "Campaign";
        private const string ADMIN_CONTROLLER = "Admin";
        private const string ACCOUNT_CONTROLLER = "Account";

        //View Names
        private const string EXTERNAL_LOGIN_CONFIRMATION_VIEW = "ExternalLoginConfirmation";
        private const string EXTERNAL_LOGIN_FAILURE_VIEW = "ExternalLoginFailure";
        private const string FORGOT_PASSWORD_CONFIRMATION_VIEW = "ForgotPasswordConfirmation";        
        private const string ERROR_VIEW = "Error";
        private const string LOCKOUT_VIEW = "Lockout";
        private const string CONFIRM_EMAIL_VIEW = "ConfirmEmail";

        //View Data
        private const string RETURN_URL = "ReturnUrl";
        private const string LOGIN_PROVIDER = "LoginProvider";
        private const string MESSAGE = "Message";
    }
}