using AllReady.Areas.Admin.Features.Activities;
using AllReady.Models;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest
{
    public class AdminTests
    {
        private static IServiceProvider _serviceProvider;

        public AdminTests()
        {
            if (_serviceProvider == null)
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
                _serviceProvider = services.BuildServiceProvider();
            }
        }

        [Fact]
        public void GetInvalidActivityDetail()
        {
            var allReadyContext = _serviceProvider.GetService<AllReadyContext>();
            var query = new ActivityDetailQuery();
            var handler = new ActivityDetailQueryHandler(allReadyContext);
            var result = handler.Handle(query);
            Assert.Null(result);
        }

    }
}
