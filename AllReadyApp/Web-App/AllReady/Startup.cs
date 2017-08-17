using System;
using System.Globalization;
using AllReady.DataAccess;
using AllReady.Hangfire;
using AllReady.Models;
using AllReady.Security;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Hangfire;
using AllReady.ModelBinding;
using Microsoft.AspNetCore.Localization;
using AllReady.Configuration;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;

namespace AllReady
{
    public class Startup
    {
        private static Task HandleRemoteLoginFailure(RemoteFailureContext ctx)
        {
            ctx.Response.Redirect("/Account/Login");
            ctx.HandleResponse();
            return Task.CompletedTask;
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Configuration["version"] = new ApplicationEnvironment().ApplicationVersion;
        }
        
        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //Add CORS support.
            // Must be first to avoid OPTIONS issues when calling from Angular/Browser
            services.AddCors(options =>
            {
                options.AddPolicy("allReady", AllReadyCorsPolicyFactory.BuildAllReadyOpenCorsPolicy());
            });

            // Add Entity Framework services to the services container.
            services.AddDbContext<AllReadyContext>(options => options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            Options.LoadConfigurationOptions(services, Configuration);

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 10;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<AllReadyContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/LogIn";
                options.AccessDeniedPath = "/Home/AccessDenied";
            });

            services.AddAuthentication()
                .AddFacebook(options => {
                    options.AppId = Configuration["authentication:facebook:appid"];
                    options.AppSecret = Configuration["authentication:facebook:appsecret"];
                    options.BackchannelHttpHandler = new FacebookBackChannelHandler();
                    options.UserInformationEndpoint = "https://graph.facebook.com/v2.5/me?fields=id,name,email,first_name,last_name";
                    options.Events = new OAuthEvents()
                    {
                        OnRemoteFailure = ctx => HandleRemoteLoginFailure(ctx)
                    };
                });


            //if (configuration["Authentication:MicrosoftAccount:ClientId"] != null)
            //{
            //    var options = new MicrosoftAccountOptions
            //    {
            //        ClientId = configuration["Authentication:MicrosoftAccount:ClientId"],
            //        ClientSecret = configuration["Authentication:MicrosoftAccount:ClientSecret"],
            //        Events = new OAuthEvents()
            //        {
            //            OnRemoteFailure = ctx => HandleRemoteLoginFailure(ctx)
            //        }
            //    };

            //    app.UseMicrosoftAccountAuthentication(options);
            //}

            ////Twitter doesn't automatically include email addresses, has to be enabled as a special permission
            ////once enabled then RetrieveUserDetails property includes email in claims returned by twitter middleware
            //if (configuration["Authentication:Twitter:ConsumerKey"] != null)
            //{
            //    var options = new TwitterOptions
            //    {
            //        ConsumerKey = configuration["Authentication:Twitter:ConsumerKey"],
            //        ConsumerSecret = configuration["Authentication:Twitter:ConsumerSecret"],
            //        RetrieveUserDetails = true,
            //        Events = new TwitterEvents()
            //        {

            //            OnRemoteFailure = ctx => HandleRemoteLoginFailure(ctx)
            //        }
            //    };

            //    app.UseTwitterAuthentication(options);
            //}

            //if (configuration["Authentication:Google:ClientId"] != null)
            //{
            //    var options = new GoogleOptions
            //    {
            //        ClientId = configuration["Authentication:Google:ClientId"],
            //        ClientSecret = configuration["Authentication:Google:ClientSecret"],
            //        Events = new OAuthEvents()
            //        {
            //            OnRemoteFailure = ctx => HandleRemoteLoginFailure(ctx)
            //        }
            //    };

            //    app.UseGoogleAuthentication(options);
            //}

            // Add Authorization rules for the app
            services.AddAuthorization(options =>
            {
                options.AddPolicy(nameof(UserType.OrgAdmin), b => b.RequireClaim(ClaimTypes.UserType, nameof(UserType.OrgAdmin), nameof(UserType.SiteAdmin)));
                options.AddPolicy(nameof(UserType.SiteAdmin), b => b.RequireClaim(ClaimTypes.UserType, nameof(UserType.SiteAdmin)));
            });

            services.AddLocalization();

            //Currently AllReady only supports en-US culture. This forces datetime and number formats to the en-US culture regardless of local culture
            var usCulture = new CultureInfo("en-US");
            var supportedCultures = new[] { usCulture };
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(usCulture, usCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddMemoryCache();

            // Add MVC services to the services container.
            // config add to get passed Angular failing on Options request when logging in.
            services.AddMvc(config =>
                {
                    config.ModelBinderProviders.Insert(0, new AdjustToTimezoneModelBinderProvider());
                })
                .AddJsonOptions(options =>
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
            });

            //Hangfire
            services.AddHangfire(configuration => configuration.UseSqlServerStorage(Configuration["Data:HangfireConnection:ConnectionString"]));

            services.AddScoped<IAllReadyUserManager, AllReadyUserManager>();
            services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
            services.AddScoped<IAuthorizableEventBuilder, AuthorizableEventBuilder>();

            // configure IoC support
            var container = AllReady.Configuration.Services.CreateIoCContainer(services, Configuration);
            return container.Resolve<IServiceProvider>();
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, AllReadyContext context, SampleDataGenerator sampleData)
        {
            // Put first to avoid issues with OPTIONS when calling from Angular/Browser.
            app.UseCors("allReady");

            app.UseSession();

            // Add the following to the request pipeline only in development environment.
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else if (env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // sends the request to the following path or controller action.
                app.UseExceptionHandler("/Error/500");
            }

            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            // Add static files to the request pipeline.
            app.UseStaticFiles();

            app.UseRequestLocalization();

            Authentication.ConfigureAuthentication(app, Configuration);

            //call Migrate here to force the creation of the AllReady database so Hangfire can create its schema under it
            if (!env.IsProduction())
            {
                context.Database.Migrate();
            }

            ////Hangfire
            app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = new[] { new HangireDashboardAuthorizationFilter() } });
            app.UseHangfireServer();

            // Add MVC to the request pipeline.
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "areaRoute", template: "{area:exists}/{controller}/{action=Index}/{id?}");
                routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Add sample data and test admin accounts if specified in Config.Json.
            // for production applications, this should either be set to false or deleted.
            if (Configuration["SampleData:InsertSampleData"] == "true")
            {
                sampleData.InsertTestData();
            }

            if (Configuration["SampleData:InsertTestUsers"] == "true")
            {
                sampleData.CreateAdminUser().GetAwaiter().GetResult();
            }
        }
    }
}
