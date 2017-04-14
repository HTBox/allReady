using System;
using System.Globalization;
using AllReady.DataAccess;
using AllReady.Hangfire;
using AllReady.Models;
using AllReady.Security;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Serialization;
using Hangfire;
using AllReady.ModelBinding;
using Microsoft.AspNetCore.Localization;
using AllReady.Configuration;

namespace AllReady
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Setup configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("version.json")
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This reads the configuration keys from the secret store.
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets("aspnet5-AllReady-468aac76-4430-43e6-848e-f4a3b90d61d0");

                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            else if (env.IsStaging() || env.IsProduction())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: false);
            }

            Configuration = builder.Build();

            Configuration["version"] = new ApplicationEnvironment().ApplicationVersion; // version in project.json
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //Add CORS support.
            // Must be first to avoid OPTIONS issues when calling from Angular/Browser
            services.AddCors(options =>
            {
                options.AddPolicy("allReady", AllReadyCorsPolicyFactory.BuildAllReadyOpenCorsPolicy());
            });

            // Add Application Insights data collection services to the services container.
            services.AddApplicationInsightsTelemetry(Configuration);

            // Add Entity Framework services to the services container.
            services.AddDbContext<AllReadyContext>(options => options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            Options.LoadConfigurationOptions(services, Configuration);

            // Add Identity services to the services container.
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Cookies.ApplicationCookie.AccessDeniedPath = new PathString("/Home/AccessDenied");
            })
            .AddEntityFrameworkStores<AllReadyContext>()
            .AddDefaultTokenProviders();

            // Add Authorization rules for the app
            services.AddAuthorization(options =>
            {
                options.AddPolicy("OrgAdmin", b => b.RequireClaim(Security.ClaimTypes.UserType, "OrgAdmin", "SiteAdmin"));
                options.AddPolicy("SiteAdmin", b => b.RequireClaim(Security.ClaimTypes.UserType, "SiteAdmin"));
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
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, SampleDataGenerator sampleData, AllReadyContext context, IConfiguration configuration)
        {
            // Put first to avoid issues with OPTIONS when calling from Angular/Browser.
            app.UseCors("allReady");

            // todo: in RC update we can read from a logging.json config file
            loggerFactory.AddConsole((category, level) =>
            {
                if (category.StartsWith("Microsoft."))
                {
                    return level >= LogLevel.Information;
                }
                return true;
            });

            if (env.IsDevelopment())
            {
                // this will go to the VS output window
                loggerFactory.AddDebug((category, level) =>
                {
                    if (category.StartsWith("Microsoft."))
                    {
                        return level >= LogLevel.Information;
                    }
                    return true;
                });
            }

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

            //Hangfire
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
                await sampleData.CreateAdminUser();
            }
        }
    }
}
