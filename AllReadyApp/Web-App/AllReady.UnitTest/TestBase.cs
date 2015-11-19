using AllReady.Models;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

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
