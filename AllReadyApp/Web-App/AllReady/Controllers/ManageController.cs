using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using AllReady.Models;
using AllReady.Services;

namespace AllReady.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly IAllReadyDataAccess _dataAccess;

        public ManageController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            IAllReadyDataAccess dataAccess)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _dataAccess = dataAccess;
        }

        // GET: /Manage/Index
        [HttpGet]
        public async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var user = GetCurrentUser();
            return View(await user.ToViewModel(_userManager, _signInManager));
        }

        // POST: /Manage/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            var user = GetCurrentUser();

            if (!ModelState.IsValid)
            {
                var viewModelWithInputs = await user.ToViewModel(_userManager, _signInManager);
                viewModelWithInputs.Name = model.Name;
                viewModelWithInputs.TimeZoneId = model.TimeZoneId;
                viewModelWithInputs.AssociatedSkills = model.AssociatedSkills;
                return View(viewModelWithInputs);
            }
            var shouldRefreshSignin = false;
            if (!string.IsNullOrEmpty(model.Name))
            {
                user.Name = model.Name;
                await _userManager.RemoveClaimsAsync(user, User.Claims.Where(c => c.Type == Security.ClaimTypes.DisplayName));
                await _userManager.AddClaimAsync(user, new Claim(Security.ClaimTypes.DisplayName, user.Name));
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

            await _dataAccess.UpdateUser(user);
            if (shouldRefreshSignin)
            {
                await _signInManager.RefreshSignInAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmailConfirmation()
        {
            var user = await _userManager.FindByIdAsync(User.GetUserId());
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email, "Confirm your allReady account",
                "Please confirm your allReady account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");

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
            var user = GetCurrentUser();
            var linkedAccounts = await _userManager.GetLoginsAsync(user);
            ViewData["ShowRemoveButton"] = await _userManager.HasPasswordAsync(user) || linkedAccounts.Count > 1;
            return View(linkedAccounts);
        }

        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message = ManageMessageId.Error;
            var user = GetCurrentUser();
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
            var user = GetCurrentUser();
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
            await _smsSender.SendSmsAsync(model.PhoneNumber, "Your allReady account security code is: " + code);
            return RedirectToAction(nameof(VerifyPhoneNumber), new { PhoneNumber = model.PhoneNumber });
        }

        // POST: /Account/ResendPhoneNumberConfirmation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendPhoneNumberConfirmation(string phoneNumber)
        {
            var user = GetCurrentUser();
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);

            await _smsSender.SendSmsAsync(phoneNumber, "Your allReady account security code is: " + code);

            return RedirectToAction(nameof(VerifyPhoneNumber), new { PhoneNumber = phoneNumber });
        }

        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var user = GetCurrentUser();
            if (user != null)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction(nameof(Index), "Manage");
        }

        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var user = GetCurrentUser();
            if (user != null)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, false);
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction(nameof(Index), "Manage");
        }

        // GET: /Account/VerifyPhoneNumber
        [HttpGet]
        public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(GetCurrentUser(), phoneNumber);
            // Send an SMS to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
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
            var user = GetCurrentUser();
            if (user != null)
            {
                var result = await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.AddPhoneSuccess });
                }
            }
            // If we got this far, something failed, redisplay the form
            ModelState.AddModelError(string.Empty, "Failed to verify phone number");
            return View(model);
        }

        // GET: /Account/RemovePhoneNumber
        [HttpGet]
        public async Task<IActionResult> RemovePhoneNumber()
        {
            var user = GetCurrentUser();
            if (user != null)
            {
                var result = await _userManager.SetPhoneNumberAsync(user, null);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
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
            var user = GetCurrentUser();
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
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

            var user = GetCurrentUser();
            if (user != null)
            {
                if(!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    ModelState.AddModelError("Password", "The password supplied is not correct");
                    return View(model);
                }

                var existingUser = await _userManager.FindByEmailAsync(model.NewEmail.Normalize());
                if(existingUser != null)
                {
                    // The username/email is already registered
                    ModelState.AddModelError("NewEmail", "The email supplied is already registered");
                    return View(model);
                }

                user.PendingNewEmail = model.NewEmail;
                await _userManager.UpdateAsync(user);

                var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
                var callbackUrl = Url.Action("ConfirmNewEmail", "Manage", new { token = token }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(user.Email, "Confirm your allReady account",
                    "Please confirm your new email address for your allReady account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>. Note that once confirmed your original email address will cease to be valid as your username.");

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
                return View("Error");
            }

            var user = GetCurrentUser();
            if (user == null)
            {
                return View("Error");
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
            var user = GetCurrentUser();

            if(string.IsNullOrEmpty(user.PendingNewEmail))
            {
                return View("Error");
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, user.PendingNewEmail);
            var callbackUrl = Url.Action("ConfirmNewEmail", "Manage", new { token = token }, protocol: HttpContext.Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email, "Confirm your allReady account",
                "Please confirm your new email address for your allReady account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>. Note that once confirmed your original email address will cease to be valid as your username.");

            return RedirectToAction(nameof(EmailConfirmationSent));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelChangeEmail()
        {
            var user = GetCurrentUser();

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

            var user = GetCurrentUser();
            if (user != null)
            {
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        //GET: /Account/Manage
        [HttpGet]
        public async Task<IActionResult> ManageLogins(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.AddLoginSuccess ? "The external login was added."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = GetCurrentUser();
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await _userManager.GetLoginsAsync(user);
            var otherLogins = _signInManager.GetExternalAuthenticationSchemes().Where(auth => userLogins.All(ul => auth.AuthenticationScheme != ul.LoginProvider)).ToList();
            ViewData["ShowRemoveButton"] = user.PasswordHash != null || userLogins.Count > 1;
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
            var redirectUrl = Url.Action("LinkLoginCallback", "Manage");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, User.GetUserId());
            return new ChallengeResult(provider, properties);
        }

        // GET: /Manage/LinkLoginCallback
        [HttpGet]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return View("Error");
            }
            var info = await _signInManager.GetExternalLoginInfoAsync(User.GetUserId());
            if (info == null)
            {
                return RedirectToAction(nameof(ManageLogins), new { Message = ManageMessageId.Error });
            }
            var result = await _userManager.AddLoginAsync(user, info);
            var message = result.Succeeded ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;
            return RedirectToAction(nameof(ManageLogins), new { Message = message });
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task<bool> HasPhoneNumber()
        {
            var user = await _userManager.FindByIdAsync(User.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
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

        private ApplicationUser GetCurrentUser()
        {
            return _dataAccess.GetUser(User.GetUserId());
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), nameof(HomeController));
            }
        }

        #endregion
    }
}
