using System.Threading.Tasks;
using AllReady.Models;
using AllReady.Security;
using AllReady.Security.Middleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        internal static IServiceCollection AddAppAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(nameof(UserType.OrgAdmin), b => b.RequireClaim(ClaimTypes.UserType, nameof(UserType.OrgAdmin), nameof(UserType.SiteAdmin)));
                options.AddPolicy(nameof(UserType.SiteAdmin), b => b.RequireClaim(ClaimTypes.UserType, nameof(UserType.SiteAdmin)));
            });

            return services;
        }

        internal static IServiceCollection AddExternalAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration["Authentication:Facebook:AppId"] != null)
            {
                services.AddAuthentication()
                    .AddFacebook(options => {
                        options.AppId = configuration["authentication:facebook:appid"];
                        options.AppSecret = configuration["authentication:facebook:appsecret"];
                        options.BackchannelHttpHandler = new FacebookBackChannelHandler();
                        options.UserInformationEndpoint = "https://graph.facebook.com/v2.5/me?fields=id,name,email,first_name,last_name";
                        options.Events = new OAuthEvents
                        {
                            OnRemoteFailure = HandleRemoteLoginFailure
                        };
                    });
            }

            if (configuration["Authentication:MicrosoftAccount:ClientId"] != null)
            {
                services.AddAuthentication()
                    .AddMicrosoftAccount(options => {
                        options.ClientId = configuration["Authentication:MicrosoftAccount:ClientId"];
                        options.ClientSecret = configuration["Authentication:MicrosoftAccount:ClientSecret"];
                        options.Events = new OAuthEvents
                        {
                            OnRemoteFailure = HandleRemoteLoginFailure
                        };
                    });
            }

            if (configuration["Authentication:Twitter:ConsumerKey"] != null)
            {
                services.AddAuthentication()
                    .AddTwitter(options => {
                        options.ConsumerKey = configuration["Authentication:Twitter:ConsumerKey"];
                        options.ConsumerSecret = configuration["Authentication:Twitter:ConsumerSecret"];
                        options.RetrieveUserDetails = true;
                        options.Events = new TwitterEvents
                        {
                            OnRemoteFailure = HandleRemoteLoginFailure
                        };
                    });
            }

            if (configuration["Authentication:Google:ClientId"] != null)
            {
                services.AddAuthentication()
                    .AddGoogle(options => {
                        options.ClientId = configuration["Authentication:Google:ClientId"];
                        options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                        options.Events = new OAuthEvents
                        {
                            OnRemoteFailure = HandleRemoteLoginFailure
                        };
                    });
            }

            return services;
        }

        private static Task HandleRemoteLoginFailure(RemoteFailureContext ctx)
        {
            ctx.Response.Redirect("/Account/Login");
            ctx.HandleResponse();
            return Task.CompletedTask;
        }
    }
}
