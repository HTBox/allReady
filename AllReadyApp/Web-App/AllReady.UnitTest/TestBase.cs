using AllReady.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting.Internal;

namespace AllReady.UnitTest
{
  /// <summary>
  /// Inherit from this type to implement tests that
  /// have access to a service provider, empty in-memory
  /// database, and basic configuration.
  /// </summary>
  public abstract class TestBase
  {
    protected IServiceProvider ServiceProvider { get; private set; }

    public TestBase()
    {
      if (ServiceProvider == null)
      {
        var services = new ServiceCollection();

        // set up empty in-memory test db
        services
          .AddEntityFrameworkInMemoryDatabase()
          .AddDbContext<AllReadyContext>(options => options.UseInMemoryDatabase().UseInternalServiceProvider(services.BuildServiceProvider()));

        // add identity service
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AllReadyContext>();
        var context = new DefaultHttpContext();
        context.Features.Set<IHttpAuthenticationFeature>(new HttpAuthenticationFeature());
        services.AddSingleton<IHttpContextAccessor>(h => new HttpContextAccessor { HttpContext = context });

        // Setup hosting environment
        IHostingEnvironment hostingEnvironment = new HostingEnvironment();
        hostingEnvironment.EnvironmentName = "Development";
        services.AddSingleton(x => hostingEnvironment);

        // set up service provider for tests
        ServiceProvider = services.BuildServiceProvider();
      }
    }
  }
}
