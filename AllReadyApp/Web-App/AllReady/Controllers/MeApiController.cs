using System.Threading.Tasks;
using AllReady.Features.Login;
using AllReady.Models;
using AllReady.Security;
using AllReady.ViewModels.Account;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserType = AllReady.Models.UserType;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using AllReady.Attributes;

namespace AllReady.Controllers
{
    //[Authorize]
    [Route("api/me")]
    public class MeApiController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMediator _mediator;

        public MeApiController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMediator mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mediator = mediator;
        }

        [ExternalEndpoint]
        [HttpPost]
        [Route("login")]
        public async Task<string> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Unable to valid login information" });
            }
            // Require admin users to have a confirmed email before they can log on.
            var user = await _mediator.SendAsync(new ApplicationUserQuery { UserName = model.Email });
            if (user != null)
            {
                var isAdminUser = user.IsUserType(UserType.OrgAdmin) || user.IsUserType(UserType.SiteAdmin);
                if (isAdminUser && !await _userManager.IsEmailConfirmedAsync(user))
                {
                    //TODO: Showing the error page here makes for a bad experience for the user.
                    //It would be better if we redirected to a specific page prompting the user to check their email for a confirmation email and providing an option to resend the confirmation email.
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "You must have a confirmed email to log on." });

                }
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return Request.Cookies[".AspNet.ApplicationCookie"];
            }

            if (result.RequiresTwoFactor)
            {
                //return RedirectToAction(nameof(AdminController.SendCode), "Admin", new { ReturnUrl = returnUrl, model.RememberMe });
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = "2 factor not supported yet!" });

            }

            if (result.IsLockedOut)
            {
                //return View("Lockout");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Account is locked out.  Please try again later" });

            }

            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Login Failed" });
        }
    }
}