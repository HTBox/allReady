using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection;
using AllReady.Models;
using AllReady.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.Controllers
{
    public class ActivityApiControllerTest
    {
        private static IServiceProvider _serviceProvider;
        private static bool populatedData = false;


        public ActivityApiControllerTest()
        {
            if (_serviceProvider == null)
            {
                var services = new ServiceCollection();

                services.AddEntityFramework()
                          .AddInMemoryStore()
                          .AddDbContext<AllReadyContext>(options => options.UseInMemoryStore());
                _serviceProvider = services.BuildServiceProvider();
            }
        }

        [Fact]
        public void GetAllActivities()
        {
            // Arrange
            var AllReadyContext = _serviceProvider.GetService<AllReadyContext>();
            var controller = new ActivityApiController(AllReadyContext, null, null);
            PopulateData(AllReadyContext);

            // Act
            var result = new List<ActivityViewModel>(controller.Get().AsEnumerable());

            // Assert
            var viewResult = Assert.IsType<List<ActivityViewModel>>(result);
            Assert.Equal(result.Count(), 10);
        }

        [Fact]
        public void GetSingleActivity()
        {
            // Arrange
            var AllReadyContext = _serviceProvider.GetService<AllReadyContext>();
            var controller = new ActivityApiController(AllReadyContext, null, null);
            PopulateData(AllReadyContext);

            // Act
            var result = controller.Get(5);

            // Assert
            var viewResult = Assert.IsType<ActivityViewModel>(result);
            Assert.Equal(viewResult.Id, 5);
            Assert.Equal(viewResult.CampaignName, "Campaign 4");
            Assert.Equal(viewResult.CampaignId, 4);
            Assert.Equal(viewResult.Description, "ActivityViewModel.cs: Needs a Description");
            Assert.Equal(viewResult.EndDateTime, DateTime.MaxValue.ToUniversalTime());
            Assert.Equal(viewResult.StartDateTime, DateTime.MinValue.ToUniversalTime());
        }

        [Fact]
        public void PostSingleActivity()
        {
            // Arrange
            var AllReadyContext = _serviceProvider.GetService<AllReadyContext>();
            var controller = new ActivityApiController(AllReadyContext, null, null);
            PopulateData(AllReadyContext);

            // Act
            //ActivityViewModel toPost = new ActivityViewModel()
            //{


            //}

            var result = controller.Get(5);

            // Assert
            var viewResult = Assert.IsType<ActivityViewModel>(result);
            Assert.Equal(viewResult.Id, 5);
            Assert.Equal(viewResult.CampaignName, "Campaign 4");
            Assert.Equal(viewResult.CampaignId, 4);
            Assert.Equal(viewResult.Description, "ActivityViewModel.cs: Needs a Description");
            Assert.Equal(viewResult.EndDateTime, DateTime.MaxValue.ToUniversalTime());
            Assert.Equal(viewResult.StartDateTime, DateTime.MinValue.ToUniversalTime());
        }

        #region Helper Methods
        private void PopulateData(DbContext context)
        {
            if (!populatedData)
            {
                var activities = TestActivityModelProvider.GetActivities();

                foreach (var activity in activities)
                {
                    context.Add(activity);
                }
                context.SaveChanges();
                populatedData = true;
            }
        }

        private class TestActivityModelProvider
        {
            private static int id = 1;
            public static Activity []  GetActivities()
            {
                var campaigns = Enumerable.Range(0, 10).Select(n =>
                    new Campaign()
                    {
                        Description = string.Format("Description # {0}", n),
                        Name = string.Format("Campaign {0}", n),
                        Id = n
                    }).ToArray();

                var tenants = Enumerable.Range(0, 10).Select(n =>
                    new Tenant()
                    {
                        Name = string.Format("Test Tenant {0}", n),
                        Campaigns = new List<Campaign> (new [] { campaigns[n]})
                    }).ToArray();

                var activities = Enumerable.Range(0, 10).Select(n =>
                    new Activity()
                    {
                        Campaign = campaigns[n],
                        EndDateTimeUtc = DateTime.MaxValue.ToUniversalTime(),
                        StartDateTimeUtc = DateTime.MinValue.ToUniversalTime(),
                        Name = string.Format("Activity # {0}", n),
                        Tenant = tenants[n],
                        Id = id++
                    }).ToArray();
                return activities;
            }
        }
        #endregion
    }
}

