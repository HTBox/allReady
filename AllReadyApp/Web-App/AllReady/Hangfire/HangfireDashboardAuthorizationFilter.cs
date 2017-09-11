using AllReady.Models;
using AllReady.Security;
using Hangfire.Dashboard;

namespace AllReady.Hangfire
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.IsUserType(UserType.SiteAdmin);
        }
    }
}
