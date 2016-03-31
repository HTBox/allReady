using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Features.Login;
using AllReady.Models;
using AllReady.Services;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;

namespace AllReady.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        //Email Constants
        private const string EMAIL_CONFIRMATION_SUBJECT = "Confirm your allReady account";
        private const string NEW_EMAIL_CONFIRM = 
            "Please confirm your new email address for your allReady account by clicking this link: <a href=\"{0}\">link</a>. Note that once confirmed your original email address will cease to be valid as your username.";
        private const string RESEND_EMAIL_CONFIRM = "Please confirm your allReady account by clicking this link: <a href=\"{0}\">link</a>";
        //Error Constants
        private const string ACCOUNT_SECURITY_CODE = "Your allReady account security code is: ";
        private const string FAILED_TO_VERIFY_PHONE_NUMBER = "Failed to verify phone number";
        private const string PASSWORD_INCORRECT = "The password supplied is not correct";
        private const string EMAIL_ALREADY_REGISTERED = "The email supplied is already registered";
        //ViewData Constants
        private const string SHOW_REMOVE_BUTTON = "ShowRemoveButton";
        private const string STATUS_MESSAGE = "StatusMessage";
        //Message Constants
        private const string PASSWORD_CHANGED = "Your password has been changed.";
        private const string PASSWORD_SET = "Your password has been set.";
        private const string TWO_FACTOR_SET = "Your two-factor authentication provider has been set.";
        private const string ERROR_OCCURRED = "An error has occurred.";
        private const string PHONE_NUMBER_ADDED = "Your phone number was added.";
        private const string PHONE_NUMBER_REMOVED = "Your phone number was removed.";
        private const string EXTERNAL_LOGIN_REMOVED = "The external login was removed.";
        private const string EXTERNAL_LOGIN_ADDED = "The external login was added.";
        //Controller Names
        private const string ACCOUNT_CONTROLLER = "Account";
        private const string MANAGE_CONTROLLER = "Manage";
        //View Names
        private const string ERROR_VIEW = "Error";
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly IAllReadyDataAccess _dataAccess;
        private readonly IMediator _mediator;

        public ManageController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            IAllReadyDataAccess dataAccess,
            IMediator mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _dataAccess = dataAccess;
            _mediator = mediator;
        }

        // GET: /Manage/Index
        [HttpGet]
        public async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            ViewData[STATUS_MESSAGE] =
                message == ManageMessageId.ChangePasswordSuccess ? PASSWORD_CHANGED
                : message == ManageMessageId.SetPasswordSuccess ? PASSWORD_SET
                : message == ManageMessageId.SetTwoFactorSuccess ? TWO_FACTOR_SET
                : message == ManageMessageId.Error ? ERROR_OCCURRED
                : message == ManageMessageId.AddPhoneSuccess ? PHONE_NUMBER_ADDED
                : message == ManageMessageId.RemovePhoneSuccess ? PHONE_NUMBER_REMOVED
                : "";

            var user = GetCurrentUser();
            return View(await user.ToViewModel(_userManager, _signInManager));
        }

        // POST: /Manage/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            var shouldRefreshSignin = false;

            var user = GetCurrentUser();

            if (!ModelState.IsValid)
            {
                var viewModelWithInputs = await user.ToViewModel(_userManager, _signInManager);
                viewModelWithInputs.Name = model.Name;
                viewModelWithInputs.TimeZoneId = model.TimeZoneId;
                viewModelWithInputs.AssociatedSkills = model.AssociatedSkills;
                return View(viewModelWithInputs);
            }

            if (!string.IsNullOrEmpty(model.Name))
            {
                user.Name = model.Name;
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

            await UpdateUserProfileCompleteness(user);

            return RedirectToAction(nameof(Microsoft.Data.Entity.Metadata.Internal.Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmailConfirmation()
        {
            var user = await _userManager.FindByIdAsync(User.GetUserId());
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //var callbackUrl = Url.Action(nameof(ConfirmNewEmail), ACCOUNT_CONTROLLER, new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
            var callbackUrl = Url.Action(new UrlActionContext { Action = nameof(ConfirmNewEmail), Controller = ACCOUNT_CONTROLLER, Values = new { userId = user.Id, code = code },
                Protocol = HttpContext.Request.Scheme });
            await _emailSender.SendEmailAsync(user.Email, EMAIL_CONFIRMATION_SUBJECT, string.Format(RESEND_EMAIL_CONFIRM, callbackUrl));

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
            ViewData[SHOW_REMOVE_BUTTON] = await _userManager.HasPasswordAsync(user) || linkedAccounts.Count > 1;
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
            await _smsSender.SendSmsAsync(model.PhoneNumber, ACCOUNT_SECURITY_CODE + code);
            return RedirectToAction(nameof(VerifyPhoneNumber), new { PhoneNumber = model.PhoneNumber });
        }

        // POST: /Account/ResendPhoneNumberConfirmation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendPhoneNumberConfirmation(string phoneNumber)
        {
            var user = GetCurrentUser();

            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            await _smsSender.SendSmsAsync(phoneNumber, ACCOUNT_SECURITY_CODE + code);

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

            return RedirectToAction(nameof(Microsoft.Data.Entity.Metadata.Internal.Index));
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

            return RedirectToAction(nameof(Microsoft.Data.Entity.Metadata.Internal.Index));
        }

        // GET: /Account/VerifyPhoneNumber
        [HttpGet]
        public IActionResult VerifyPhoneNumber(string phoneNumber)
        {
            // Send an SMS to verify the phone number
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

            var user = GetCurrentUser();
            if (user != null)
            {
                var result = await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
                if (result.Succeeded)
                {
                    await UpdateUserProfileCompleteness(user);
                    await _signInManager.SignInAsync(user, isPersistent: false);                    
                    return RedirectToAction(nameof(Microsoft.Data.Entity.Metadata.Internal.Index), new { Message = ManageMessageId.AddPhoneSuccess });
                }
            }

            // If we got this far, something failed, redisplay the form
            ModelState.AddModelError(string.Empty, FAILED_TO_VERIFY_PHONE_NUMBER);
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
                    await UpdateUserProfileCompleteness(user);
                    return RedirectToAction(nameof(Microsoft.Data.Entity.Metadata.Internal.Index), new { Message = ManageMessageId.RemovePhoneSuccess });
                }
            }

            return RedirectToAction(nameof(Microsoft.Data.Entity.Metadata.Internal.Index), new { Message = ManageMessageId.Error });
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
                    return RedirectToAction(nameof(Microsoft.Data.Entity.Metadata.Internal.Index), new { Message = ManageMessageId.ChangePasswordSuccess });
                }

                AddErrorsToModelState(result);

                return View(model);
            }

            return RedirectToAction(nameof(Microsoft.Data.Entity.Metadata.Internal.Index), new { Message = ManageMessageId.Error });
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
                    ModelState.AddModelError(nameof(model.Password), PASSWORD_INCORRECT);
                    return View(model);
                }

                var existingUser = await _userManager.FindByEmailAsync(model.NewEmail.Normalize());
                if(existingUser != null)
                {
                    // The username/email is already registered
                    ModelState.AddModelError(nameof(model.NewEmail), EMAIL_ALREADY_REGISTERED);
                    return View(model);
                }

                user.PendingNewEmail = model.NewEmail;
                await _userManager.UpdateAsync(user);

                var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
                //var callbackUrl = Url.Action(nameof(ConfirmNewEmail), MANAGE_CONTROLLER, new { token = token }, protocol: HttpContext.Request.Scheme);
                var callbackUrl = Url.Action(new UrlActionContext { Action = nameof(ConfirmNewEmail), Controller = MANAGE_CONTROLLER, Values = new { token = token },
                    Protocol = HttpContext.Request.Scheme });
                await _emailSender.SendEmailAsync(user.Email, EMAIL_CONFIRMATION_SUBJECT, string.Format(NEW_EMAIL_CONFIRM, callbackUrl));

                return RedirectToAction(nameof(EmailConfirmationSent));                
            }

            return RedirectToAction(nameof(Microsoft.Data.Entity.Metadata.Internal.Index), new { Message = ManageMessageId.Error });
        }

        // GET: /Manage/ConfirmNewEmail
        [HttpGet]
        public async Task<IActionResult> ConfirmNewEmail(string token)
        {
            if (token == null)
            {
                return View(ERROR_VIEW);
            }

            var user = GetCurrentUser();
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

            return RedirectToAction(nameof(Microsoft.Data.Entity.Metadata.Internal.Index), new { Message = ManageMessageId.ChangeEmailSuccess });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendChangeEmailConfirmation()
        {
            var user = GetCurrentUser();

            if(string.IsNullOrEmpty(user.PendingNewEmail))
            {
                return View(ERROR_VIEW);
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, user.PendingNewEmail);
            //var callbackUrl = Url.Action(nameof(ConfirmNewEmail), MANAGE_CONTROLLER, new { token = token }, protocol: HttpContext.Request.Scheme);
            var callbackUrl = Url.Action(new UrlActionContext { Action = nameof(ConfirmNewEmail), Controller = MANAGE_CONTROLLER, Values = new { token = token },
                Protocol = HttpContext.Request.Scheme });
            await _emailSender.SendEmailAsync(user.Email, EMAIL_CONFIRMATION_SUBJECT, string.Format(NEW_EMAIL_CONFIRM, callbackUrl));

            return RedirectToAction(nameof(EmailConfirmationSent));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelChangeEmail()
        {
            var user = GetCurrentUser();

            user.PendingNewEmail = null;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Microsoft.Data.Entity.Metadata.Internal.Index));
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
                    return RedirectToAction(nameof(Microsoft.Data.Entity.Metadata.Internal.Index), new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrorsToModelState(result);
                return View(model);
            }
            return RedirectToAction(nameof(Microsoft.Data.Entity.Metadata.Internal.Index), new { Message = ManageMessageId.Error });
        }

        //GET: /Account/Manage
        [HttpGet]
        public async Task<IActionResult> ManageLogins(ManageMessageId? message = null)
        {
            ViewData[STATUS_MESSAGE] =
                message == ManageMessageId.RemoveLoginSuccess ? EXTERNAL_LOGIN_REMOVED
                : message == ManageMessageId.AddLoginSuccess ? EXTERNAL_LOGIN_ADDED
                : message == ManageMessageId.Error ? ERROR_OCCURRED
                : "";

            var user = GetCurrentUser();
            if (user == null)
            {
                return View(ERROR_VIEW);
            }

            var userLogins = await _userManager.GetLoginsAsync(user);
            var otherLogins = _signInManager.GetExternalAuthenticationSchemes().Where(auth => userLogins.All(ul => auth.AuthenticationScheme != ul.LoginProvider)).ToList();

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
            //var redirectUrl = Url.Action(nameof(LinkLoginCallback), MANAGE_CONTROLLER);
            var redirectUrl = Url.Action(new UrlActionContext { Action = nameof(LinkLoginCallback), Controller = MANAGE_CONTROLLER });
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
                return View(ERROR_VIEW);
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

        private ApplicationUser GetCurrentUser()
        {
            return _dataAccess.GetUser(User.GetUserId());
        }
        #endregion
    }
}
