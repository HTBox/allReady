using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace AllReady.Security.Middleware
{
    public static class AssociateUserExtensions
    {
        public static IApplicationBuilder UseAssociateUser(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AssociateUserMiddleware>();
        }
    }

    public class AssociateUserMiddleware
    {
        private readonly RequestDelegate _next;

        public AssociateUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserAuthorizationService userAuthorizationService)
        {
            await userAuthorizationService.AssociateUser(context.User);

            await _next(context);
        }
    }
}
