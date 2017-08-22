using AllReady.Security.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace AllReady.Configuration
{
    internal static class Authentication
    {
        internal static void ConfigureAuthentication(IApplicationBuilder app, IConfiguration configuration)
        {
            // Add cookie-based authentication to the request pipeline.
            app.UseAuthentication();

            app.UseAssociateUser();

            // Add token-based protection to the request inject pipeline
            app.UseTokenProtection(new TokenProtectedResourceOptions
            {
                Path = "/api/request",
                PolicyName = "api-request-injest"
            });
        }
    }
}
