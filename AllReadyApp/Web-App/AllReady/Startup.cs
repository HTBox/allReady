using System;
using System.Collections.Generic;
using System.Globalization;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.Areas.Admin.ViewModels.Validators.Task;
using AllReady.Controllers;
using AllReady.DataAccess;
using AllReady.Hangfire;
using AllReady.Hangfire.Jobs;
using AllReady.Models;
using AllReady.Providers;
using AllReady.Providers.ExternalUserInformationProviders;
using AllReady.Providers.ExternalUserInformationProviders.Providers;
using AllReady.Security;
using AllReady.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.Variance;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using AllReady.Security.Middleware;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Geocoding;
using Geocoding.Google;
using Hangfire;
using Hangfire.SqlServer;
using AllReady.ModelBinding;

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
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                //builder.AddApplicationInsightsSettings(developerMode: true);
                builder.AddApplicationInsightsSettings(developerMode: false);
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
            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin();
            corsBuilder.AllowCredentials();
            services.AddCors(options =>
            {
                options.AddPolicy("allReady", corsBuilder.Build());
            });

            // Add Application Insights data collection services to the services container.
            services.AddApplicationInsightsTelemetry(Configuration);

            // Add Entity Framework services to the services container.
            services.AddDbContext<AllReadyContext>(options => options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            services.Configure<AzureStorageSettings>(Configuration.GetSection("Data:Storage"));
            services.Configure<DatabaseSettings>(Configuration.GetSection("Data:DefaultConnection"));
            services.Configure<EmailSettings>(Configuration.GetSection("Email"));
            services.Configure<SampleDataSettings>(Configuration.GetSection("SampleData"));
            services.Configure<GeneralSettings>(Configuration.GetSection("General"));
            services.Configure<TwitterAuthenticationSettings>(Configuration.GetSection("Authentication:Twitter"));

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

            // Add MVC services to the services container.
            // config add to get passed Angular failing on Options request when logging in.
            services.AddMvc(config =>
            {
                config.ModelBinderProviders.Insert(0, new AdjustToTimezoneModelBinderProvider());
            })
                .AddJsonOptions(options =>
                options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //Hangfire
            services.AddHangfire(configuration => configuration.UseSqlServerStorage(Configuration["Data:HangfireConnection:ConnectionString"]));

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
            services.AddTransient<IDetermineIfATaskIsEditable, DetermineIfATaskIsEditable>();
            services.AddTransient<IValidateEventEditViewModels, EventEditViewModelValidator>();
            services.AddTransient<ITaskEditViewModelValidator, TaskEditViewModelValidator>();
            services.AddTransient<IItineraryEditModelValidator, ItineraryEditModelValidator>();
            services.AddTransient<IOrganizationEditModelValidator, OrganizationEditModelValidator>();
            services.AddTransient<IRedirectAccountControllerRequests, RedirectAccountControllerRequests>();
            services.AddSingleton<IImageService, ImageService>();
            services.AddTransient<ISendRequestConfirmationMessagesAWeekBeforeAnItineraryDate, SendRequestConfirmationMessagesAWeekBeforeAnItineraryDate>();
            services.AddTransient<ISendRequestConfirmationMessagesADayBeforeAnItineraryDate, SendRequestConfirmationMessagesADayBeforeAnItineraryDate>();
            services.AddTransient<ISendRequestConfirmationMessagesTheDayOfAnItineraryDate, SendRequestConfirmationMessagesTheDayOfAnItineraryDate>();

            services.AddTransient<SampleDataGenerator>();

            if (Configuration["Geocoding:EnableGoogleGeocodingService"] == "true")
            {
                //This setting is false by default. To enable Google geocoding you will
                //need to override this setting in your user secrets or env vars.
                //Visit https://developers.google.com/maps/documentation/geocoding/get-api-key to get a free standard usage API key
                services.AddSingleton<IGeocoder>(new GoogleGeocoder(Configuration["Geocoding:GoogleGeocodingApiKey"]));
            }
            else
            {
                //implement null object pattern to protect against IGeocoder not being passed a legit reference at runtime b/c of the above conditional
                services.AddSingleton<IGeocoder>(new NullObjectGeocoder());
            }

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
                //services.AddTransient<IQueueStorageService, SmtpEmailSender>();
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
           
            //ExternalUserInformationProviderFactory registration
            containerBuilder.RegisterType<TwitterExternalUserInformationProvider>().Named<IProvideExternalUserInformation>("Twitter");
            containerBuilder.RegisterType<GoogleExternalUserInformationProvider>().Named<IProvideExternalUserInformation>("Google");
            containerBuilder.RegisterType<MicrosoftAndFacebookExternalUserInformationProvider>().Named<IProvideExternalUserInformation>("Microsoft");
            containerBuilder.RegisterType<MicrosoftAndFacebookExternalUserInformationProvider>().Named<IProvideExternalUserInformation>("Facebook");
            containerBuilder.RegisterType<ExternalUserInformationProviderFactory>().As<IExternalUserInformationProviderFactory>();

            //Hangfire
            containerBuilder.Register(icomponentcontext => new BackgroundJobClient(new SqlServerStorage(Configuration["Data:HangfireConnection:ConnectionString"])))
                .As<IBackgroundJobClient>();

            //Populate the container with services that were previously registered
            containerBuilder.Populate(services);

            var container = containerBuilder.Build();
            return container;
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

            // Configure the HTTP request pipeline.
            var usCultureInfo = new CultureInfo("en-US");
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
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
            else if (env.IsStaging())
            {
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

            // Add token-based protection to the request inject pipeline
            app.UseTokenProtection(new TokenProtectedResourceOptions
            {
                Path = "/api/request",
                PolicyName = "api-request-injest"
            });

            // Add authentication middleware to the request pipeline. You can configure options such as Id and Secret in the ConfigureServices method.
            // For more information see http://go.microsoft.com/fwlink/?LinkID=532715
            if (Configuration["Authentication:Facebook:AppId"] != null)
            {
                var options = new FacebookOptions
                {
                    AppId = Configuration["Authentication:Facebook:AppId"],
                    AppSecret = Configuration["Authentication:Facebook:AppSecret"],
                    BackchannelHttpHandler = new FacebookBackChannelHandler(),
                    UserInformationEndpoint = "https://graph.facebook.com/v2.5/me?fields=id,name,email,first_name,last_name"
                };
                options.Scope.Add("email");

                app.UseFacebookAuthentication(options);
            }

            if (Configuration["Authentication:MicrosoftAccount:ClientId"] != null)
            {
                var options = new MicrosoftAccountOptions
                {
                    ClientId = Configuration["Authentication:MicrosoftAccount:ClientId"],
                    ClientSecret = Configuration["Authentication:MicrosoftAccount:ClientSecret"]
                };

                app.UseMicrosoftAccountAuthentication(options);
            }

            //http://www.bigbrainintelligence.com/Post/get-users-email-address-from-twitter-oauth-ap
            if (Configuration["Authentication:Twitter:ConsumerKey"] != null)
            {
                var options = new TwitterOptions
                {
                    ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"],
                    ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"]
                };

                app.UseTwitterAuthentication(options);
            }

            if (Configuration["Authentication:Google:ClientId"] != null)
            {
                var options = new GoogleOptions
                {
                    ClientId = Configuration["Authentication:Google:ClientId"],
                    ClientSecret = Configuration["Authentication:Google:ClientSecret"]
                };

                app.UseGoogleAuthentication(options);
            }

            //call Migrate here to force the creation of the AllReady database so Hangfire can create its schema under it
            if (!env.IsProduction())
            {
                context.Database.Migrate();
            }

            //Hangfire
            app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = new [] { new HangireDashboardAuthorizationFilter() }});
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