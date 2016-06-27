using AllReady.Models;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Security.Middleware
{


    public class TokenProtectedResource
    {
        private RequestDelegate _next;
        private TokenProtectedResourceOptions _options;

        public TokenProtectedResource(RequestDelegate next, TokenProtectedResourceOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(HttpContext httpContext, UserManager<ApplicationUser> manager)
        {
            if (httpContext.Request.Path.StartsWithSegments(_options.Path))
            {
                var headers = httpContext.Request.Headers;
                if (!(headers.ContainsKey("ApiUser") && headers.ContainsKey("ApiToken")))
                {
                    await httpContext.Authentication.ChallengeAsync();
                    return;
                }

                var apiUser = headers.FirstOrDefault(h => h.Key == "ApiUser").Value;
                var token = headers.FirstOrDefault(h => h.Key == "ApiToken").Value;

                var user = manager.FindByNameAsync(apiUser).ConfigureAwait(false).GetAwaiter().GetResult();
                var authorized = manager.VerifyUserTokenAsync(user, "Default", "api-access", token).ConfigureAwait(false).GetAwaiter().GetResult();

                if (!authorized)
                {
                    await httpContext.Authentication.ChallengeAsync();
                    return;
                }
            }

            await _next(httpContext);
        }

    }
}
