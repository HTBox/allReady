using AllReady.Models;
using Auth0;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Authentication.OpenIdConnect;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.OptionsModel;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AllReady.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private IConfiguration _config;
        private readonly IAllReadyDataAccess _dataAccess;

        public AccountController(IConfiguration config, IAllReadyDataAccess dataAccess)
        {
            _config = config;
            _dataAccess = dataAccess;
        }

        /// <summary>
        /// When authenticating using the OpenID Connect middleware the nonce is required by default (but this can be turned off) 
        /// and the state is always required. These values are always generated server side and the following action exposes this
        /// logic to your javascript code.
        /// 
        /// This is useful when the authentication flow needs to start from the auth0 Lock.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Prepare()
        {
            var middlewareOptions = (IOptions<OpenIdConnectAuthenticationOptions>)Context.ApplicationServices.GetService(typeof(IOptions<OpenIdConnectAuthenticationOptions>));

            // Generate the nonce.
            var nonce = middlewareOptions.Options.ProtocolValidator.GenerateNonce();

            // Store it in the cache or in a cookie.
            if (middlewareOptions.Options.NonceCache != null)
            {
                middlewareOptions.Options.NonceCache.TryAddNonce(nonce);
            }
            else
            {
                Response.Cookies.Append(
                    ".AspNet.OpenIdConnect.Nonce." + middlewareOptions.Options.StringDataFormat.Protect(nonce), "N",
                        new CookieOptions { HttpOnly = true, Secure = Request.IsHttps, Expires = DateTime.Now.AddMinutes(10) });
            }

            // Generate the state.
            var state = "OpenIdConnect.AuthenticationProperties=" +
                        Uri.EscapeDataString(middlewareOptions.Options.StateDataFormat.Protect(new AuthenticationProperties(
                            Request.Form.ToDictionary(i => i.Key, i => i.Value?.FirstOrDefault()))));

            // Return nonce to the Lock.
            return Json(new { nonce, state });
        }

        // GET: /Account/Login
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (Context.User == null || !Context.User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
            }

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Callback(string access_token, string id_token, string state)
        {
            if (Context.User.IsSignedIn())
            {
                Context.Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            var client = new Auth0.Client(
                _config.Get("Authentication:Auth0:ClientId"),
                _config.Get("Authentication:Auth0:ClientSecret"),
                _config.Get("Authentication:Auth0:Domain"));

            var profile = client.GetUserInfo(new TokenResult { AccessToken = access_token, IdToken = id_token });

            //var externalIdentity = AuthenticationManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
            //if (externalIdentity == null)
            //{
            //    throw new Exception("Could not get the external identity. Please check your Auth0 configuration settings and ensure that " +
            //                        "you configured UseCookieAuthentication and UseExternalSignInCookie in the OWIN Startup class. " +
            //                        "Also make sure you are not calling setting the callbackOnLocationHash option on the JavaScript login widget.");
            //}
            //AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, CreateIdentity(externalIdentity));

            var userCP = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] {
                        new Claim(ClaimTypes.Name, profile.Name),
                        new Claim("UserType", profile.ExtraProperties.First(x => x.Key == "allReadyUserType").Value.ToString())
                    },
                    CookieAuthenticationDefaults.AuthenticationScheme));
            var userManager = (UserManager<ApplicationUser>)Context.ApplicationServices.GetService(typeof(UserManager<ApplicationUser>));
            var user = await userManager.FindByIdAsync(profile.UserId);
            if (user == null)
            {
                user = new ApplicationUser { UserName = profile.UserId, Email = profile.Email };
                user.EmailConfirmed = true;

                await userManager.CreateAsync(user);
                await _dataAccess.AddUser(user); // CreateAsync doesn't seem to be persisting the user
                if (profile.ExtraProperties.Any(x => x.Key == "allReadyUserType"))
                {
                    await userManager.AddClaimAsync(user, new Claim("UserType", profile.ExtraProperties.First(x => x.Key == "allReadyUserType").Value.ToString()));
                }
            }

            Context.Authentication.SignIn(CookieAuthenticationDefaults.AuthenticationScheme, userCP);

            return Redirect("/");
        }

        // POST: /Account/LogOff
        [AllowAnonymous]
        [HttpPost]
        public IActionResult LogOff()
        {
            // TODO: why doesn't the redirect work until the second time this method is hit?
            if (Context.User.Identity.IsAuthenticated)
            {
                Context.Response.HttpContext.Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return RedirectToAction("Index", "Home");
        }

    }
}