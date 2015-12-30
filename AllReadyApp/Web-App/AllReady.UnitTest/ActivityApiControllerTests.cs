using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Controllers;
using AllReady.Models;
using AllReady.ViewModels;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

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
        [Fact]
        public void ActivityDoesExist()
        {
            // Arrange
            ActivityApiController controller = GetActivityController();

            // Act
            int recordId = 1;
            var activityViewModel = controller.Get(recordId);

            Assert.NotNull(activityViewModel);

        }

        [Fact]
        public void HandlesInvalidActivityId()
        {
            // Arrange
            ActivityApiController controller = GetActivityController();

            // Act
            int recordId = -1;
            var activityViewModel = controller.Get(recordId);

            Assert.Null(activityViewModel);

        }

        #region Helper Methods

        private ActivityApiController GetActivityController()
        {
            var allReadyContext = _serviceProvider.GetService<AllReadyContext>();
            var allReadyDataAccess = new AllReadyDataAccessEF7(allReadyContext);
            var controller = new ActivityApiController(allReadyDataAccess);
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
            public const string OrganizationNameFormat = "Test Organization {0}";
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

                var organizations = Enumerable.Range(1, 10).Select(n =>
                    new Organization()
                    {
                        Name = string.Format(OrganizationNameFormat, n),
                        Campaigns = new List<Campaign>(new[] { campaigns[n - 1] })
                    }).ToArray();

                var activities = Enumerable.Range(1, 10).Select(n =>
                    new Activity()
                    {
                        Campaign = campaigns[n - 1],
                        EndDateTime = DateTime.MaxValue.ToUniversalTime(),
                        StartDateTime = DateTime.MinValue.ToUniversalTime(),
                        Name = string.Format(ActivityNameFormat, n),
                        Description = string.Format(ActivityDescriptionFormat, n),
                        Id = id++
                    }).ToArray();
                return activities;
            }
        }
        #endregion
    }
}

