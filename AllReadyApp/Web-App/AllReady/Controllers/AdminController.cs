using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Configuration;
using AllReady.Constants;
using AllReady.Features.Admin;
using AllReady.Features.Manage;
using AllReady.Models;
using AllReady.ViewModels.Account;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;

namespace AllReady.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMediator _mediator;
        private readonly IOptions<SampleDataSettings> _settings;
        private readonly IOptions<GeneralSettings> _generalSettings;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMediator mediator,
            IOptions<SampleDataSettings> options,
            IOptions<GeneralSettings> generalSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mediator = mediator;
            _settings = options;
            _generalSettings = generalSettings;
        }

        //
        // GET: /Admin/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        //
        // POST: /Admin/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    TimeZoneId = _generalSettings.Value.DefaultTimeZone
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(new UrlActionContext { Action = nameof(ConfirmEmail), Controller = ControllerNames.Admin, Values = new { userId = user.Id, token = token }, Protocol = Request.Scheme });
                    await _mediator.SendAsync(new SendConfirmAccountEmail { Email = model.Email, CallbackUrl = callbackUrl });
                    return RedirectToAction(nameof(DisplayEmail), AreaNames.Admin);
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Admin/DisplayEmail
        [HttpGet]
        [AllowAnonymous]
        public IActionResult DisplayEmail()
        {
            return View();
        }

        // GET: /Admin/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (token == null)
            {
                return View("Error");
            }
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            
            var result = await _userManager.ConfirmEmailAsync(user, token);

            // If the account confirmation was successful, then send the SiteAdmin an email to approve
            // this user as a Organization Admin
            if (result.Succeeded)
            {
                var callbackUrl = Url.Action(new UrlActionContext { Action = nameof(SiteController.EditUser), Controller = "Site", Values = new { area = AreaNames.Admin, userId = user.Id }, Protocol = HttpContext.Request.Scheme });
                await _mediator.SendAsync(new SendApproveOrganizationUserAccountEmail { DefaultAdminUsername = _settings.Value.DefaultAdminUsername, CallbackUrl = callbackUrl });
            }

            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // GET: /Admin/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // POST: /Admin/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            
            // Generate the token and send it
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(token))
            {
                return View("Error");
            }
            
            if (model.SelectedProvider == "Email")
            {
                await _mediator.SendAsync(new SendSecurityCodeEmail { Email = await _userManager.GetEmailAsync(user), Token = token });
            }
            else if (model.SelectedProvider == "Phone")
            {
                await _mediator.SendAsync(new SendAccountSecurityTokenSms { PhoneNumber = await _userManager.GetPhoneNumberAsync(user), Token = token });
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
        }

        // GET: /Admin/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // POST: /Admin/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                if (Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                
                return RedirectToPage("/Index");
            }

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            
            ModelState.AddModelError("", "Invalid code.");

            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}