using AllReady.Models;
using AllReady.ViewModels;
using AllReady.Controllers;
using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Microsoft.Framework.Configuration;
using System.IO;
using Microsoft.AspNet.Hosting;

namespace AllReady.UnitTest
{
    public class ActivityApiControllerTest : TestBase
    {
        private static IServiceProvider _serviceProvider;
        private static bool populatedData = false;
        private static int activitiesAdded = 0;

        public ActivityApiControllerTest()
        {
            if (_serviceProvider == null)
            {
                var services = new ServiceCollection();

                // Add Configuration to the Container
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddEnvironmentVariables();
                IConfiguration configuration = builder.Build();
                services.AddSingleton(x => configuration);

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

        [Fact]
        public void GetAllActivities()
        {
            // Arrange
            ActivityApiController controller = GetActivityController();

            // Act
            var activities = new List<ActivityViewModel>(controller.Get());

            // Assert
            Assert.Equal(activitiesAdded, activities.Count());
        }

        [Fact]
        public void GetSingleActivity()
        {
            // Arrange
            ActivityApiController controller = GetActivityController();

            // Act
            int recordId = 5;
            var activityViewModel = controller.Get(recordId);

            // Assert
            Assert.Equal(activityViewModel.Id, recordId);
            Assert.Equal(activityViewModel.CampaignName, string.Format(TestActivityModelProvider.CampaignNameFormat, recordId));
            Assert.Equal(activityViewModel.CampaignId, recordId);
            Assert.Equal(activityViewModel.Description, string.Format(TestActivityModelProvider.ActivityDescriptionFormat, recordId));
            Assert.Equal(activityViewModel.EndDateTime, DateTime.MaxValue.ToUniversalTime());
            Assert.Equal(activityViewModel.StartDateTime, DateTime.MinValue.ToUniversalTime());
        }

        #region Helper Methods

        private ActivityApiController GetActivityController()
        {
            var allReadyContext = _serviceProvider.GetService<AllReadyContext>();
            var allReadyDataAccess = new AllReadyDataAccessEF7(allReadyContext);
            var controller = new ActivityApiController(allReadyDataAccess, null);
            PopulateData(allReadyContext);
            return controller;
        }

        private void PopulateData(DbContext context)
        {
            if (!populatedData)
            {
                var activities = TestActivityModelProvider.GetActivities();

                foreach (var activity in activities)
                {
                    context.Add(activity);
                    context.Add(activity.Campaign);
                    context.Add(activity.Tenant);
                    activitiesAdded++;
                }
                context.SaveChanges();
                populatedData = true;
            }
        }

        private class TestActivityModelProvider
        {
            public const string CampaignNameFormat = "Campaign {0}";
            public const string CampaignDescriptionFormat = "Description for campaign {0}";
            public const string TenantNameFormat = "Test Tenant {0}";
            public const string ActivityNameFormat = "Activity {0}";
            public const string ActivityDescriptionFormat = "Description for activity {0}";

            private static int id = 1;
            public static Activity[] GetActivities()
            {
                var campaigns = Enumerable.Range(1, 10).Select(n =>
                    new Campaign()
                    {
                        Description = string.Format(CampaignDescriptionFormat, n),
                        Name = string.Format(CampaignNameFormat, n),
                        Id = n
                    }).ToArray();

                var tenants = Enumerable.Range(1, 10).Select(n =>
                    new Tenant()
                    {
                        Name = string.Format(TenantNameFormat, n),
                        Campaigns = new List<Campaign>(new[] { campaigns[n - 1] })
                    }).ToArray();

                var activities = Enumerable.Range(1, 10).Select(n =>
                    new Activity()
                    {
                        Campaign = campaigns[n - 1],
                        EndDateTimeUtc = DateTime.MaxValue.ToUniversalTime(),
                        StartDateTimeUtc = DateTime.MinValue.ToUniversalTime(),
                        Name = string.Format(ActivityNameFormat, n),
                        Description = string.Format(ActivityDescriptionFormat, n),
                        Tenant = tenants[n - 1],
                        Id = id++
                    }).ToArray();
                return activities;
            }
        }
        #endregion
    }
}

