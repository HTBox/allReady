using AllReady.Security;
using AllReady.Security.Middleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace AllReady.Configuration
{
    internal static class Authentication
    {
        //Handles remote login failure typically where user doesn't give consent
        private static Task HandleRemoteLoginFailure(FailureContext ctx)
        {
            ctx.Response.Redirect("/Account/Login");
            ctx.HandleResponse();
            return Task.FromResult(0);
        }

        internal static void ConfigureAuthentication(IApplicationBuilder app, IConfiguration configuration)
        {
            // Add cookie-based authentication to the request pipeline.
            app.UseIdentity();

            // Add token-based protection to the request inject pipeline
            app.UseTokenProtection(new TokenProtectedResourceOptions
            {
                Path = "/api/request",
                PolicyName = "api-request-injest"
            });

            // Add authentication middleware to the request pipeline. You can configure options such as Id and Secret in the ConfigureServices method.
            // For more information see http://go.microsoft.com/fwlink/?LinkID=532715
            if (configuration["Authentication:Facebook:AppId"] != null)
            {
                var options = new FacebookOptions
                {
                    AppId = configuration["Authentication:Facebook:AppId"],
                    AppSecret = configuration["Authentication:Facebook:AppSecret"],
                    BackchannelHttpHandler = new FacebookBackChannelHandler(),
                    UserInformationEndpoint = "https://graph.facebook.com/v2.5/me?fields=id,name,email,first_name,last_name",
                    Events = new OAuthEvents()
                    {
                        OnRemoteFailure = ctx => HandleRemoteLoginFailure(ctx)
                    }
                };
                options.Scope.Add("email");

                app.UseFacebookAuthentication(options);
            }

            if (configuration["Authentication:MicrosoftAccount:ClientId"] != null)
            {
                var options = new MicrosoftAccountOptions
                {
                    ClientId = configuration["Authentication:MicrosoftAccount:ClientId"],
                    ClientSecret = configuration["Authentication:MicrosoftAccount:ClientSecret"],
                    Events = new OAuthEvents()
                    {
                        OnRemoteFailure = ctx => HandleRemoteLoginFailure(ctx)
                    }
                };

                app.UseMicrosoftAccountAuthentication(options);
            }

            //Twitter doesn't automatically include email addresses, has to be enabled as a special permission
            //once enabled then RetrieveUserDetails property includes name and email in claims returned by twitter middleware
            if (configuration["Authentication:Twitter:ConsumerKey"] != null)
            {
                var options = new TwitterOptions
                {
                    ConsumerKey = configuration["Authentication:Twitter:ConsumerKey"],
                    ConsumerSecret = configuration["Authentication:Twitter:ConsumerSecret"],
                    RetrieveUserDetails = true,
                    Events = new TwitterEvents()
                    {

                        OnRemoteFailure = ctx => HandleRemoteLoginFailure(ctx)
                    }
                };

                app.UseTwitterAuthentication(options);
            }

            if (configuration["Authentication:Google:ClientId"] != null)
            {
                var options = new GoogleOptions
                {
                    ClientId = configuration["Authentication:Google:ClientId"],
                    ClientSecret = configuration["Authentication:Google:ClientSecret"],
                    Events = new OAuthEvents()
                    {
                        OnRemoteFailure = ctx => HandleRemoteLoginFailure(ctx)
                    }
                };

                app.UseGoogleAuthentication(options);
            }
        }

    }
}
