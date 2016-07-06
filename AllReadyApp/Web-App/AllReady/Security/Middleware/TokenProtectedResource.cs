using AllReady.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Security.Middleware
{
    public static class TokenProtectedResourceExtensions
    {
        // extension method for easy wiring of middleware
        public static IApplicationBuilder UseTokenProtection(
            this IApplicationBuilder builder, TokenProtectedResourceOptions options)
        {
            return builder.UseMiddleware<TokenProtectedResource>(options);
        }
    }

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

                var user = await manager.FindByNameAsync(apiUser).ConfigureAwait(false);
                var authorized = await manager.VerifyUserTokenAsync(user, "Default", "api-request-injest", token).ConfigureAwait(false);

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
