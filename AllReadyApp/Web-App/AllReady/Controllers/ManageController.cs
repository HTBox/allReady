using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Features.Login;
using AllReady.Features.Manage;
using AllReady.Models;
using AllReady.Security;
using AllReady.ViewModels.Manage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AllReady.Controllers
{
    [Authorize]
    public class ManageController : Controller
    { 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMediator _mediator;

        public ManageController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMediator mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mediator = mediator;
        }

        // GET: /Manage/Index
        [HttpGet]
        public async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            ViewData[STATUS_MESSAGE] =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? ERROR_OCCURRED
                : message == ManageMessageId.AddPhoneSuccess ? "Your mobile phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your mobile phone number was removed."
                : "";

            var user = await GetCurrentUser();
            return View(await user.ToViewModel(_userManager, _signInManager));
        }

        // POST: /Manage/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            var shouldRefreshSignin = false;

            var user = await GetCurrentUser();

            if (!ModelState.IsValid)
            {
                var viewModelWithInputs = await user.ToViewModel(_userManager, _signInManager);
                viewModelWithInputs.FirstName= model.FirstName;
                viewModelWithInputs.LastName = model.LastName;
                viewModelWithInputs.TimeZoneId = model.TimeZoneId;
                viewModelWithInputs.AssociatedSkills = model.AssociatedSkills;
                return View(viewModelWithInputs);
            }

            if (!string.IsNullOrEmpty(model.FirstName))
            {
                user.FirstName= model.FirstName;
                shouldRefreshSignin = true;
            }

            if (!string.IsNullOrEmpty(model.LastName))
            {
                user.LastName= model.LastName;
                shouldRefreshSignin = true;
            }

            if (user.TimeZoneId != model.TimeZoneId)
            {
                user.TimeZoneId = model.TimeZoneId;
                await _userManager.RemoveClaimsAsync(user, User.Claims.Where(c => c.Type == Security.ClaimTypes.TimeZoneId));
                await _userManager.AddClaimAsync(user, new Claim(Security.ClaimTypes.TimeZoneId, user.TimeZoneId));
                shouldRefreshSignin = true;
            }

            user.AssociatedSkills.RemoveAll(usk => model.AssociatedSkills == null || !model.AssociatedSkills.Any(msk => msk.SkillId == usk.SkillId));
            if (model.AssociatedSkills != null)
            {
                user.AssociatedSkills.AddRange(model.AssociatedSkills.Where(msk => !user.AssociatedSkills.Any(usk => usk.SkillId == msk.SkillId)));
            }

            user.AssociatedSkills?.ForEach(usk => usk.UserId = user.Id);

            await _mediator.SendAsync(new UpdateUser { User = user });

            if (shouldRefreshSignin)
            {
                await _signInManager.RefreshSignInAsync(user);
            }

            await UpdateUserProfileCompleteness(user);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmailConfirmation()
        {
            var user = await _userManager.GetUserAsync(User);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action(new UrlActionContext { Action = nameof(AccountController.ConfirmEmail), Controller = "Account", Values = new { userId = user.Id, token },
                Protocol = HttpContext.Request.Scheme });

            await _mediator.SendAsync(new SendConfirmAccountEmail { Email = user.Email, CallbackUrl = callbackUrl });

            return RedirectToAction(nameof(EmailConfirmationSent));
        }

        [HttpGet]
        public IActionResult EmailConfirmationSent()
        {
            return View();
        }

        // GET: /Account/RemoveLogin
        [HttpGet]
        public async Task<IActionResult> RemoveLogin()
        {
            var user = await GetCurrentUser();
            var linkedAccounts = await _userManager.GetLoginsAsync(user);
            ViewData[SHOW_REMOVE_BUTTON] = await _userManager.HasPasswordAsync(user) || linkedAccounts.Count > 1;
            return View(linkedAccounts);
        }

        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message = ManageMessageId.Error;

            var user = await GetCurrentUser();

            if (user != null)
            {
                var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    message = ManageMessageId.RemoveLoginSuccess;
                }
            }

            return RedirectToAction(nameof(ManageLogins), new { Message = message });
        }

        // GET: /Account/AddPhoneNumber
        public IActionResult AddPhoneNumber()
        {
            return View();
        }

        // POST: /Account/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Generate the token and send it
            await GenerateChangePhoneNumberTokenAndSendAccountSecurityTokenSms(await GetCurrentUser(), model.PhoneNumber);
            return RedirectToAction(nameof(VerifyPhoneNumber), new { model.PhoneNumber });
        }

        // POST: /Account/ResendPhoneNumberConfirmation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendPhoneNumberConfirmation(string phoneNumber)
        {
            await GenerateChangePhoneNumberTokenAndSendAccountSecurityTokenSms(await GetCurrentUser(), phoneNumber);
            return RedirectToAction(nameof(VerifyPhoneNumber), new { PhoneNumber = phoneNumber });
        }

        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var user = await GetCurrentUser();
            if (user != null)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var user = await GetCurrentUser();
            if (user != null)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, false);
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Account/VerifyPhoneNumber
        [HttpGet]
        public IActionResult VerifyPhoneNumber(string phoneNumber)
        {
            // Send an SMS to verify the mobile phone number
            return phoneNumber == null ? View(ERROR_VIEW) : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        // POST: /Account/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUser();
            if (user != null)
            {
                var result = await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
                if (result.Succeeded)
                {
                    await UpdateUserProfileCompleteness(user);
                    await _signInManager.SignInAsync(user, isPersistent: false);                    
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.AddPhoneSuccess });
                }
            }

            // If we got this far, something failed, redisplay the form
            ModelState.AddModelError(string.Empty, "Failed to verify mobile phone number");
            return View(model);
        }

        // GET: /Account/RemovePhoneNumber
        [HttpGet]
        public async Task<IActionResult> RemovePhoneNumber()
        {
            var user = await GetCurrentUser();
            if (user != null)
            {
                var result = await _userManager.SetPhoneNumberAsync(user, null);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    await UpdateUserProfileCompleteness(user);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.RemovePhoneSuccess });
                }
            }

            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        // GET: /Manage/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUser();
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
                }

                AddErrorsToModelState(result);

                return View(model);
            }

            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        // GET: /Manage/ChangeEmail
        [HttpGet]
        public IActionResult ChangeEmail()
        {
            return View();
        }

        // POST: /Account/ChangeEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUser();
            if (user != null)
            {
                if(!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    ModelState.AddModelError(nameof(model.Password), "The password supplied is not correct");
                    return View(model);
                }

                var existingUser = await _userManager.FindByEmailAsync(model.NewEmail.Normalize());
                if(existingUser != null)
                {
                    // The username/email is already registered
                    ModelState.AddModelError(nameof(model.NewEmail), "The email supplied is already registered");
                    return View(model);
                }

                user.PendingNewEmail = model.NewEmail;
                await _userManager.UpdateAsync(user);

                await BuildCallbackUrlAndSendNewEmailAddressConfirmationEmail(user, model.NewEmail);

                return RedirectToAction(nameof(EmailConfirmationSent));                
            }

            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        // GET: /Manage/ConfirmNewEmail
        [HttpGet]
        public async Task<IActionResult> ConfirmNewEmail(string token)
        {
            if (token == null)
            {
                return View(ERROR_VIEW);
            }

            var user = await GetCurrentUser();
            if (user == null)
            {
                return View(ERROR_VIEW);
            }

            var result = await _userManager.ChangeEmailAsync(user, user.PendingNewEmail, token);

            if(result.Succeeded)
            {
                await _userManager.SetUserNameAsync(user, user.PendingNewEmail);

                user.PendingNewEmail = null;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangeEmailSuccess });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendChangeEmailConfirmation()
        {
            var user = await GetCurrentUser();

            if(string.IsNullOrEmpty(user?.PendingNewEmail))
            {
                return View(ERROR_VIEW);
            }

            await BuildCallbackUrlAndSendNewEmailAddressConfirmationEmail(user, user.PendingNewEmail);
            
            return RedirectToAction(nameof(EmailConfirmationSent));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelChangeEmail()
        {
            var user = await GetCurrentUser();

            user.PendingNewEmail = null;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

        // GET: /Manage/SetPassword
        [HttpGet]
        public IActionResult SetPassword()
        {
            return View();
        }

        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUser();
            if (user != null)
            {
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrorsToModelState(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        //GET: /Account/Manage
        [HttpGet]
        public async Task<IActionResult> ManageLogins(ManageMessageId? message = null)
        {
            ViewData[STATUS_MESSAGE] =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.AddLoginSuccess ? "The external login was added."
                : message == ManageMessageId.Error ? ERROR_OCCURRED
                : "";

            var user = await GetCurrentUser();
            if (user == null)
            {
                return View(ERROR_VIEW);
            }

            var userLogins = await _userManager.GetLoginsAsync(user);
            var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
            var otherLogins = schemes.Where(auth => userLogins.All(ul => auth.Name != ul.LoginProvider)).ToList();

            ViewData[SHOW_REMOVE_BUTTON] = user.PasswordHash != null || userLogins.Count > 1;

            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(new UrlActionContext { Action = nameof(LinkLoginCallback), Controller = MANAGE_CONTROLLER });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        // GET: /Manage/LinkLoginCallback
        [HttpGet]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                return View(ERROR_VIEW);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(_userManager.GetUserId(User));
            if (info == null)
            {
                return RedirectToAction(nameof(ManageLogins), new { Message = ManageMessageId.Error });
            }

            var result = await _userManager.AddLoginAsync(user, info);
            var message = result.Succeeded ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;

            return RedirectToAction(nameof(ManageLogins), new { Message = message });
        }

        private async Task BuildCallbackUrlAndSendNewEmailAddressConfirmationEmail(ApplicationUser applicationUser, string userEmail)
        {
            var token = await _userManager.GenerateChangeEmailTokenAsync(applicationUser, userEmail);
            var callbackUrl = Url.Action(new UrlActionContext { Action = nameof(ConfirmNewEmail), Controller = MANAGE_CONTROLLER, Values = new { token = token },
                Protocol = HttpContext.Request.Scheme
            });
            await _mediator.SendAsync(new SendNewEmailAddressConfirmationEmail { Email = userEmail, CallbackUrl = callbackUrl });
        }

        private async Task GenerateChangePhoneNumberTokenAndSendAccountSecurityTokenSms(ApplicationUser applicationUser, string phoneNumber)
        {
            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(applicationUser, phoneNumber);
            await _mediator.SendAsync(new SendAccountSecurityTokenSms { PhoneNumber = phoneNumber, Token = token });
        }

        private async Task UpdateUserProfileCompleteness(ApplicationUser user)
        {
            if (user.IsProfileComplete())
            {
                await _mediator.SendAsync(new RemoveUserProfileIncompleteClaimCommand { UserId = user.Id });
                await _signInManager.RefreshSignInAsync(user);
            }
        }

        private void AddErrorsToModelState(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            AddLoginSuccess,
            ChangePasswordSuccess,
            ChangeEmailSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _mediator.SendAsync(new UserByUserIdQuery { UserId = _userManager.GetUserId(User) });
        }

        //ViewData Constants
        private const string SHOW_REMOVE_BUTTON = "ShowRemoveButton";
        private const string STATUS_MESSAGE = "StatusMessage";

        //Message Constants
        private const string ERROR_OCCURRED = "An error has occurred.";

        //Controller Names
        private const string MANAGE_CONTROLLER = "Manage";

        //View Names
        private const string ERROR_VIEW = "Error";
    }
}
