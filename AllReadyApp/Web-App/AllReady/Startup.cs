using System;
using System.Collections.Generic;
using System.Globalization;
using AllReady.Models;
using AllReady.Services;
using Autofac;
using Autofac.Features.Variance;
using Autofac.Framework.DependencyInjection;
using MediatR;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Localization;
using Microsoft.Data.Entity;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using AllReady.Security;

namespace AllReady
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            // Setup configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile("version.json")
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add Application Insights data collection services to the services container.
            services.AddApplicationInsightsTelemetry(Configuration);

            // Add Entity Framework services to the services container.
            var ef = services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<AllReadyContext>(options => options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            services.Configure<AzureStorageSettings>(Configuration.GetSection("Data:Storage"));
            services.Configure<DatabaseSettings>(Configuration.GetSection("Data:DefaultConnection"));
            services.Configure<EmailSettings>(Configuration.GetSection("Email"));
            services.Configure<SampleDataSettings>(Configuration.GetSection("SampleData"));
            services.Configure<GeneralSettings>(Configuration.GetSection("General"));

            // Add CORS support
            services.AddCors(options =>
            {
                options.AddPolicy("allReady",
              builder => builder.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()
              );
            });

            // Add Identity services to the services container.
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                    {
                        options.Password.RequiredLength = PasswordRequirements.PASSWORD_LENGTH;
                        options.Password.RequireNonLetterOrDigit = PasswordRequirements.REQUIRE_NON_ALPHA_OR_DIGIT;
                        options.Password.RequireDigit = PasswordRequirements.REQUIRE_DIGIT;
                        options.Password.RequireUppercase = PasswordRequirements.REQUIRE_UPPERCASE;
                    })
                     .AddEntityFrameworkStores<AllReadyContext>()
                     .AddDefaultTokenProviders();

            // Add Authorization rules for the app
            services.AddAuthorization(options =>
            {
                options.AddPolicy("TenantAdmin", b => b.RequireClaim(Security.ClaimTypes.UserType, "TenantAdmin", "SiteAdmin"));
                options.AddPolicy("SiteAdmin", b => b.RequireClaim(Security.ClaimTypes.UserType, "SiteAdmin"));
            });

            services.AddCookieAuthentication(options =>
             {
                 options.AccessDeniedPath = new PathString("/Home/AccessDenied");
             });

            // Add MVC services to the services container.
            services.AddMvc();

            // configure IoC support
            var container = CreateIoCContainer(services);
            return container.Resolve<IServiceProvider>();

        }

        private IContainer CreateIoCContainer(IServiceCollection services)
        {
            // todo: move these to a proper autofac module
            // Register application services.
            services.AddSingleton((x) => Configuration);
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddSingleton<IClosestLocations, SqlClosestLocations>();
            services.AddTransient<IAllReadyDataAccess, AllReadyDataAccessEF7>();
            services.AddSingleton<IImageService, ImageService>();
            //services.AddSingleton<GeoService>();
            services.AddTransient<SampleDataGenerator>();

            if (Configuration["Data:Storage:EnableAzureQueueService"] == "true")
            {
                // This setting is false by default. To enable queue processing you will 
                // need to override the setting in your user secrets or env vars.
                services.AddTransient<IQueueStorageService, QueueStorageService>();
            }
            else
            {
                // this writer service will just write to the default logger
                services.AddTransient<IQueueStorageService, FakeQueueWriterService>();
            }

            var containerBuilder = new ContainerBuilder();
            
            containerBuilder.RegisterSource(new ContravariantRegistrationSource());
            containerBuilder.RegisterAssemblyTypes(typeof(IMediator).Assembly).AsImplementedInterfaces();
            containerBuilder.RegisterAssemblyTypes(typeof(Startup).Assembly).AsImplementedInterfaces();
            containerBuilder.Register<SingleInstanceFactory>(ctx =>
          {
              var c = ctx.Resolve<IComponentContext>();
              return t => c.Resolve(t);
          });
            containerBuilder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            });

            //Populate the container with services that were previously registered
            containerBuilder.Populate(services);

            var container = containerBuilder.Build();
            return container;
        }

        // Configure is called after ConfigureServices is called.
        public async void Configure(IApplicationBuilder app,
          IHostingEnvironment env,
          ILoggerFactory loggerFactory,
          SampleDataGenerator sampleData,
          AllReadyContext context,
          IConfiguration configuration)
        {
            loggerFactory.MinimumLevel = LogLevel.Verbose;

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

            // CORS support
            app.UseCors("allReady");

            // Configure the HTTP request pipeline.

            var usCultureInfo = new CultureInfo("en-US");
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(usCultureInfo),
                SupportedCultures = new List<CultureInfo>(new[] { usCultureInfo }),
                SupportedUICultures = new List<CultureInfo>(new[] { usCultureInfo })
            });

            // Add Application Insights to the request pipeline to track HTTP request telemetry data.
            app.UseApplicationInsightsRequestTelemetry();

            // Add the following to the request pipeline only in development environment.
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // sends the request to the following path or controller action.
                app.UseExceptionHandler("/Home/Error");
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
                app.UseFacebookAuthentication(options =>
                {
                    options.AppId = Configuration["Authentication:Facebook:AppId"];
                    options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                    options.Scope.Add("email");
                    options.BackchannelHttpHandler = new FacebookBackChannelHandler();
                    options.UserInformationEndpoint = "https://graph.facebook.com/v2.5/me?fields=id,name,email,first_name,last_name";
                });
            }
            // app.UseGoogleAuthentication();

            if (Configuration["Authentication:MicrosoftAccount:ClientId"] != null)
            {
                app.UseMicrosoftAccountAuthentication(options =>
                {
                    options.ClientId = Configuration["Authentication:MicrosoftAccount:ClientId"];
                    options.ClientSecret = Configuration["Authentication:MicrosoftAccount:ClientSecret"];
                    options.Scope.Add("wl.basic");
                    options.Scope.Add("wl.signin");
                });
            }

            if (Configuration["Authentication:Twitter:ConsumerKey"] != null)
            {
                app.UseTwitterAuthentication(options =>
                {
                    options.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
                    options.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
                });
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
            if (env.IsDevelopment() || env.IsEnvironment("Staging"))
            {
                context.Database.Migrate();
            }
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
