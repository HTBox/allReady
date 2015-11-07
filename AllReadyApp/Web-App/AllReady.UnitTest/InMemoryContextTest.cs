using System.IO;
using AllReady.Models;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Data.Entity;

namespace AllReady.UnitTests
{
    public abstract class InMemoryContextTest
    {
        protected AllReadyContext Context;

        protected InMemoryContextTest()
        {
            var services = new ServiceCollection();

            services.AddEntityFramework()
                .AddInMemoryDatabase()
                .AddDbContext<AllReadyContext>(options => options.UseInMemoryDatabase());

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testConfig.json");
            IConfiguration configuration = builder.Build();
            services.AddSingleton(x => configuration);
            IHostingEnvironment hostingEnvironment = new HostingEnvironment();
            hostingEnvironment.EnvironmentName = "Development";
            services.AddSingleton(x => hostingEnvironment);
            var serviceProvider = services.BuildServiceProvider();
            Context = serviceProvider.GetService<AllReadyContext>();
        }
    }
}