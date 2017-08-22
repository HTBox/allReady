using AllReady.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace AllReady.Security.Middleware
{
    public static class TokenProtectedResourceExtensions
    {
        // extension method for easy wiring of middleware
        public static IApplicationBuilder UseTokenProtection(this IApplicationBuilder builder, TokenProtectedResourceOptions options)
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
                    await httpContext.ChallengeAsync();
                    return;
                }

                var apiUser = headers.FirstOrDefault(h => h.Key == "ApiUser").Value;
                var token = headers.FirstOrDefault(h => h.Key == "ApiToken").Value;

                var user = await manager.FindByNameAsync(apiUser);
                var authorized = await manager.VerifyUserTokenAsync(user, "Default", TokenTypes.ApiKey, token);

                if (!authorized)
                {
                    await httpContext.ChallengeAsync();
                    return;
                }
            }

            await _next(httpContext);
        }
    }
}
