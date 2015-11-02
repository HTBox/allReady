using System;
using System.Collections.Generic;
using Microsoft.AspNet.Authentication.Facebook;
using Microsoft.AspNet.Authentication.MicrosoftAccount;
using Microsoft.AspNet.Authentication.Twitter;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Diagnostics.Entity;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using AllReady.Models;
using AllReady.Services;
using Autofac;
using Autofac.Features.Variance;
using Autofac.Framework.DependencyInjection;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.Dnx.Runtime;

namespace AllReady
{
  public class Startup
  {
    public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
    {
      // Setup configuration sources.

      var builder = new ConfigurationBuilder()
          .SetBasePath(appEnv.ApplicationBasePath)
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
    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
      // Add Application Insights data collection services to the services container.
      services.AddApplicationInsightsTelemetry(Configuration);

      // Add Entity Framework services to the services container.
      var ef = services.AddEntityFramework();

      if (Configuration["Data:DefaultConnection:UseInMemory"].ToLowerInvariant() == "true")
      {
        ef = ef.AddInMemoryDatabase();
      }
      else
      {
        ef = ef.AddSqlServer();
      }

      ef.AddDbContext<AllReadyContext>();

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
      services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<AllReadyContext>()
               .AddDefaultTokenProviders();

      // Add Authorization rules for the app
      services.Configure<AuthorizationOptions>(options =>
      {
        options.AddPolicy("TenantAdmin", new AuthorizationPolicyBuilder().RequireClaim(Security.ClaimTypes.UserType, new string[] { "TenantAdmin", "SiteAdmin" }).Build());
        options.AddPolicy("SiteAdmin", new AuthorizationPolicyBuilder().RequireClaim(Security.ClaimTypes.UserType, "SiteAdmin").Build());
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
      services.AddTransient<IQueueStorageService, QueueStorageService>();
      services.AddSingleton<IClosestLocations, SqlClosestLocations>();
      services.AddTransient<IAllReadyDataAccess, AllReadyDataAccessEF7>();
      services.AddSingleton<IImageService, ImageService>();
      //services.AddSingleton<GeoService>();
      services.AddTransient<SampleDataGenerator>();

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
      SampleDataGenerator sampleData)
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
      if (Configuration["Data:InsertSampleData"] == "true")
      {
        sampleData.InsertTestData();
      }
      if (Configuration["Data:InsertTestUsers"] == "true")
      {
        await sampleData.CreateAdminUser();
      }
    }

  }
}
