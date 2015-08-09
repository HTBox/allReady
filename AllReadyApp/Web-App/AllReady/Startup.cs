using AllReady.Models;
using AllReady.Services;
using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Authentication.Notifications;
using Microsoft.AspNet.Authentication.OpenIdConnect;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Diagnostics.Entity;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Runtime;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

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

            //// Add Identity services to the services container.
            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<AllReadyContext>()
            //    .AddDefaultTokenProviders();

            // OpenID Connect Authentication Requires Cookie Auth
            services.Configure<ExternalAuthenticationOptions>(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            // Add Authorization rules for the app
            services.Configure<AuthorizationOptions>(options =>
            {
                options.AddPolicy("TenantAdmin", new AuthorizationPolicyBuilder().RequireClaim("UserType", new string[] { "TenantAdmin", "SiteAdmin" }).Build());
                options.AddPolicy("SiteAdmin", new AuthorizationPolicyBuilder().RequireClaim("UserType", "SiteAdmin").Build());
            });

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

            // Configure the OWIN Pipeline to use OpenID Connect Authentication
            app.UseCookieAuthentication(options => {
                options.AutomaticAuthentication = true;
                options.LoginPath = "/Account/Login";
            });

            app.UseOpenIdConnectAuthentication(options =>
            {
                options.ClientId = Configuration.Get("Auth0:ClientId");
                options.Authority = "https://" + Configuration.Get("Auth0:Domain");
                options.AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = OnAuthenticationFailed,
                    
                    // read/modify the claims that are populated based on the JWT
                    SecurityTokenValidated = context =>
                    {
                        var claimsIdentity = context.AuthenticationTicket.Principal.Identity as ClaimsIdentity;

                        // add Auth0 access_token as claim
                        claimsIdentity.AddClaim(new Claim("access_token", context.ProtocolMessage.AccessToken));

                        // ensure name claim
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, claimsIdentity.FindFirst("name").Value));

                        return Task.FromResult(0);
                    }
                };
            });


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
        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        {
            notification.HandleResponse();
            notification.Response.Redirect("/Home/Error?message=" + notification.Exception.Message);
            return Task.FromResult(0);
        }
    }
}
