using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AllReady.UnitTest.DataAccess
{
    public class AllReadyDataAccessEF7Tests : TestBase
    {
        private static IServiceProvider _serviceProvider;

        public AllReadyDataAccessEF7Tests()
        {
            if (_serviceProvider == null)
            {
                var services = new ServiceCollection();

                // Add EF (Full DB, not In-Memory)
                services.AddEntityFramework()
                    .AddInMemoryDatabase()
                    .AddDbContext<AllReadyContext>(options => options.UseInMemoryDatabase());

                // Setup hosting environment
                IHostingEnvironment hostingEnvironment = new HostingEnvironment();
                hostingEnvironment.EnvironmentName = "Development";
                services.AddSingleton(x => hostingEnvironment);
                _serviceProvider = services.BuildServiceProvider();
            }
        }

        #region Event
        [Fact]
        public void GetResourcesByCategoryReturnsOnlyThoseResourcesWithMatchingCategory()
        {
            const string categoryToMatch = "category1";

            var context = _serviceProvider.GetService<AllReadyContext>();
            context.Resources.Add(new Resource { CategoryTag = categoryToMatch });
            context.Resources.Add(new Resource { CategoryTag = "shouldNotMatchThisCategory" });
            context.SaveChanges();

            var sut = (IAllReadyDataAccess)new AllReadyDataAccessEF7(context);
            var results = sut.GetResourcesByCategory(categoryToMatch).ToList();

            Assert.Equal(results.Single().CategoryTag, categoryToMatch);
        }
        #endregion
    }
}
