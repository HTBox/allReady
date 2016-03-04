using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class ActivityApiControllerTest : TestBase
    {
        private static IServiceProvider _serviceProvider;
        private static bool populatedData;
        private static int activitiesAdded;
        private Mock<IMediator> _mediator;

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
            var controller = GetActivityController();

            // Act
            var activities = new List<ActivityViewModel>(controller.Get());

            // Assert
            Assert.Equal(activitiesAdded, activities.Count());
        }

        [Fact]
        public void GetSingleActivity()
        {
            // Arrange
            var controller = GetActivityController();

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
            var controller = GetActivityController();

            // Act
            const int recordId = 1;
            var activityViewModel = controller.Get(recordId);

            Assert.NotNull(activityViewModel);
        }

        [Fact]
        public void HandlesInvalidActivityId()
        {
            // Arrange
            var controller = GetActivityController();

            // Act
            const int recordId = -1;
            var activityViewModel = controller.Get(recordId);

            Assert.Null(activityViewModel);
        }

        [Fact]
        public async Task UnregisterActivityShouldRemoveActivitySignup()
        {
            // Arrange
            const int recordId = 5;
            var controller = GetActivityController()
                .SetFakeUser(recordId.ToString());

            // Act
            var result = await controller.UnregisterActivity(recordId);

            // Assert
            Assert.NotNull(result);
            var context = _serviceProvider.GetService<AllReadyContext>();
            var numOfUsersSignedUp = context.Activities
                .First(e => e.Id == recordId)
                .UsersSignedUp.Count;
            Assert.Equal(0, numOfUsersSignedUp);
        }

        [Fact]
        public async Task UnregisterActivityShouldRemoveTaskSignup()
        {
            // Arrange
            const int recordId = 5;
            var controller = GetActivityController()
                .SetFakeUser(recordId.ToString());
            
            // Act
            var result = await controller.UnregisterActivity(recordId);

            // Assert
            Assert.NotNull(result);
            var context = _serviceProvider.GetService<AllReadyContext>();
            var numOfTasksSignedUpFor = context.TaskSignups.Count(e => e.Task.Activity.Id == recordId);
            Assert.Equal(0, numOfTasksSignedUpFor);
        }

        #region Helper Methods

        private ActivityApiController GetActivityController()
        {
            var allReadyContext = _serviceProvider.GetService<AllReadyContext>();
            var allReadyDataAccess = new AllReadyDataAccessEF7(allReadyContext);

            _mediator = new Mock<IMediator>();
            var controller = new ActivityApiController(allReadyDataAccess, _mediator.Object);

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
            public const string UserNameFormat = "User {0}";
            public const string TaskDescriptionFormat = "Task {0}";

            public static Activity[] GetActivities()
            {
                var users = Enumerable.Range(1, 10).Select(n =>
                    new ApplicationUser()
                    {
                        Id = n.ToString(),
                        Name = string.Format(UserNameFormat, n)
                    }).ToArray();
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
                        Id = n,
                        UsersSignedUp = new List<ActivitySignup>()
                        {
                            new ActivitySignup()
                            {
                                User = users[n - 1],
                                SignupDateTime = DateTime.Now.ToUniversalTime(),
                                PreferredEmail = "foo@foo.com",
                                PreferredPhoneNumber = "(555) 555-5555"
                            }
                        },
                        Tasks = new List<AllReadyTask>()
                        {
                            new AllReadyTask()
                            {
                                Id = n,
                                Name = string.Format(TaskDescriptionFormat,n),
                                NumberOfVolunteersRequired = 1,
                                Organization = organizations[n - 1],
                                AssignedVolunteers = new List<TaskSignup>()
                                {
                                    new TaskSignup()
                                    {
                                        User = users[n - 1],
                                        StatusDateTimeUtc = DateTime.Now.ToUniversalTime(),
                                        Status = "Ready To Rock And Roll"
                                    }
                                }
                            }
                        }
                    }).ToArray();
                return activities;
            }
        }
        #endregion
    }
}

