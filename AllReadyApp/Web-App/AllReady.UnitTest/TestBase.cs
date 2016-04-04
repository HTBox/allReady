using AllReady.Models;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Http.Features.Authentication;
using Microsoft.AspNet.Http.Features.Authentication.Internal;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using System;

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
                services.AddEntityFramework()
                    .AddInMemoryDatabase()
                    .AddDbContext<AllReadyContext>(options => options.UseInMemoryDatabase());

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
