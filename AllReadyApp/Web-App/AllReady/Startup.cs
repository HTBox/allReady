using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authentication.Facebook;
using Microsoft.AspNet.Authentication.Google;
using Microsoft.AspNet.Authentication.MicrosoftAccount;
using Microsoft.AspNet.Authentication.Twitter;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Diagnostics.Entity;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Routing;
using Microsoft.Data.Entity;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Console;
using Microsoft.Framework.Runtime;
using AllReady.Models;
using AllReady.Services;
using Microsoft.AspNet.Authorization;

namespace AllReady
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            // Setup configuration sources.

            var builder = new ConfigurationBuilder(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // This reads the configuration keys from the secret store.
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);

            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Application Insights data collection services to the services container.
            services.AddApplicationInsightsTelemetry(Configuration);

            // Add Entity Framework services to the services container.
            if (Configuration["Data:DefaultConnection:UseInMemory"] == "true")
            {
                services.AddEntityFramework()
                .AddInMemoryStore()
                .AddDbContext<AllReadyContext>();
            }
            else
            {
                //string connectionStringPath = "Data:DefaultConnection:LocalConnectionString";
                string connectionStringPath = "Data:DefaultConnection:AzureConnectionString";
                services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<AllReadyContext>(options =>
                    options.UseSqlServer(Configuration[connectionStringPath]));
            }

            // Add CORS support
            services.AddCors();
            services.ConfigureCors(options => {
                options.AddPolicy("allReady",
                    builder => builder.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials()
                    );
            });

            // Add Identity services to the services container.
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AllReadyContext>()
                .AddDefaultTokenProviders();

            // Add Authorization rules for the app
            services.Configure<AuthorizationOptions>(options =>
            {
                options.AddPolicy("TenantAdmin", new AuthorizationPolicyBuilder().RequireClaim("UserType", new string[] { "TenantAdmin", "SiteAdmin" }).Build());
                options.AddPolicy("SiteAdmin", new AuthorizationPolicyBuilder().RequireClaim("UserType", "SiteAdmin").Build());
            });

            services.ConfigureCookieAuthentication(options =>
             {
                 options.AccessDeniedPath = new PathString("/Home/AccessDenied");
             });

            // Configure the options for the authentication middleware.
            // You can add options for Google, Twitter and other middleware as shown below.
            // For more information see http://go.microsoft.com/fwlink/?LinkID=532715
            if (Configuration["Authentication:Facebook:AppId"] != null)
            {
                services.Configure<FacebookAuthenticationOptions>(options =>
                {
                    options.AppId = Configuration["Authentication:Facebook:AppId"];
                    options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                });
            }

            //Enable Twitter Auth, only id key set
            if (Configuration["Authentication:Twitter:ConsumerKey"] != null)
            {
                services.Configure<TwitterAuthenticationOptions>(options =>
                {
                    options.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
                    options.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
                });
            }

            if (Configuration["Authentication:MicrosoftAccount:ClientId"] != null)
            {
                services.Configure<MicrosoftAccountAuthenticationOptions>(options =>
                {
                    options.ClientId = Configuration["Authentication:MicrosoftAccount:ClientId"];
                    options.ClientSecret = Configuration["Authentication:MicrosoftAccount:ClientSecret"];
                });
            }

            // Add MVC services to the services container.
            services.AddMvc();

            // Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
            // You will also need to add the Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
            // services.AddWebApiConventions();

            // Register application services.
            services.AddSingleton((x) => this.Configuration);
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddSingleton<IClosestLocations, SqlClosestLocations>();
            services.AddTransient<IAllReadyDataAccess, AllReadyDataAccessEF7>();
            services.AddSingleton<IImageService, ImageService>();
            //services.AddSingleton<GeoService>();
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, AllReadyContext dbContext, IServiceProvider serviceProvider)
        {

            loggerFactory.MinimumLevel = LogLevel.Information;
            loggerFactory.AddConsole();

            // CORS support
            app.UseCors("allReady");

            // Configure the HTTP request pipeline.

            // Add Application Insights to the request pipeline to track HTTP request telemetry data.
            app.UseApplicationInsightsRequestTelemetry();

            // Add the following to the request pipeline only in development environment.
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseErrorPage(ErrorPageOptions.ShowAll);
                app.UseDatabaseErrorPage(DatabaseErrorPageOptions.ShowAll);
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // sends the request to the following path or controller action.
                app.UseErrorHandler("/Home/Error");
            }

            // Track data about exceptions from the application. Should be configured after all error handling middleware in the request pipeline.
            app.UseApplicationInsightsExceptionTelemetry();

            // Add static files to the request pipeline.
            app.UseStaticFiles();

            // Add cookie-based authentication to the request pipeline.
            app.UseIdentity();

            // Add authentication middleware to the request pipeline. You can configure options such as Id and Secret in the ConfigureServices method.
            // For more information see http://go.microsoft.com/fwlink/?LinkID=532715
            if (Configuration["Authentication:Facebook:AppId"] != null)
            {
                app.UseFacebookAuthentication();
            }
            // app.UseGoogleAuthentication();

            if (Configuration["Authentication:MicrosoftAccount:ClientId"] != null)
            {
                app.UseMicrosoftAccountAuthentication();
            }

            if (Configuration["Authentication:Twitter:ConsumerKey"] != null)
            {
                app.UseTwitterAuthentication();
            }
            // Add MVC to the request pipeline.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                     name: "areaRoute",
                     template: "{area:exists}/{controller}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
             });

            // Add sample data and test admin accounts if specified in Config.Json.
            // for production applications, this should either be set to false or deleted.
            if (Configuration["Data:InsertSampleData"] == "true")
            {
                SampleData.InsertTestData(dbContext);
            }
            if (Configuration["Data:InsertTestUsers"] == "true")
            {
                SampleData.CreateAdminUser(serviceProvider, dbContext).Wait();
            }
        }

    }
}
