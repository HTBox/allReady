using Microsoft.AspNetCore.Cors.Infrastructure;

namespace AllReady.Security
{
    public static class AllReadyCorsPolicyFactory
    {
        public static CorsPolicy BuildAllReadyOpenCorsPolicy()
        {
            var corsBuilder = new CorsPolicyBuilder();

            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            // we need to revisit this and build a whitelise of IP addresses
            corsBuilder.AllowAnyOrigin();
            corsBuilder.AllowCredentials();

            return corsBuilder.Build();
        }
    }
}
