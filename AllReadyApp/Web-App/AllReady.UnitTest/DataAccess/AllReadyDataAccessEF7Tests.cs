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
        private static bool populatedData;

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

        #region Activity
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

        #region ActivitySignup
        [Fact]
        public async Task DeleteActivityAndTaskSignupsAsyncDoesNotDeleteActivitySignupsOrTaskSignupsForUnkownActivitySignupId()
        {
            const int anActivitySignupIdThatDoesNotExist = 1000;

            var sut = CreateSutAndPopulateTestDataForActivitySignup();

            var countOfActivitySignupsBeforeMethodInvocation = sut.ActivitySignups.Count();
            var countOfTaskSignupsBeforeMethodInvocation = sut.TaskSignups.Count();

            await sut.DeleteActivityAndTaskSignupsAsync(anActivitySignupIdThatDoesNotExist);

            var countOfActivitySignsupsAfterMethodInvocation = sut.ActivitySignups.Count();
            var countOfTaskSignsupsAfterMethodInvocation = sut.TaskSignups.Count();

            Assert.Equal(countOfActivitySignupsBeforeMethodInvocation, countOfActivitySignsupsAfterMethodInvocation);
            Assert.Equal(countOfTaskSignupsBeforeMethodInvocation, countOfTaskSignsupsAfterMethodInvocation);
        }

        [Fact]
        public async Task DeleteActivityAndTaskSignupsAsyncRemovesActivitySignup()
        {
            const int activitySignupId = 5;

            var sut = CreateSutAndPopulateTestDataForActivitySignup();
            await sut.DeleteActivityAndTaskSignupsAsync(activitySignupId);

            var context = _serviceProvider.GetService<AllReadyContext>();
            var numOfUsersSignedUp = context.Activities.First(e => e.Id == activitySignupId).UsersSignedUp.Count;
            Assert.Equal(0, numOfUsersSignedUp);
        }

        [Fact]
        public async Task DeleteActivityAndTaskSignupsAsyncRemovesTaskSignup()
        {
            const int activitySignupId = 5;

            var sut = CreateSutAndPopulateTestDataForActivitySignup();
            await sut.DeleteActivityAndTaskSignupsAsync(activitySignupId);

            var context = _serviceProvider.GetService<AllReadyContext>();
            var numOfTasksSignedUpFor = context.TaskSignups.Count(e => e.Task.Activity.Id == activitySignupId);
            Assert.Equal(0, numOfTasksSignedUpFor);
        }

        public IAllReadyDataAccess CreateSutAndPopulateTestDataForActivitySignup()
        {
            var allReadyContext = _serviceProvider.GetService<AllReadyContext>();
            var allReadyDataAccess = new AllReadyDataAccessEF7(allReadyContext);
            PopulateDataForActivitySignup(allReadyContext);
            return allReadyDataAccess;
        }

        private static void PopulateDataForActivitySignup(DbContext context)
        {
            if (!populatedData)
            {
                var activities = TestActivityModelProvider.GetActivities();

                foreach (var activity in activities)
                {
                    context.Add(activity);
                    context.Add(activity.Campaign);
                }
                context.SaveChanges();
                populatedData = true;
            }
        }

        private static class TestActivityModelProvider
        {
            private const string CampaignNameFormat = "Campaign {0}";
            private const string CampaignDescriptionFormat = "Description for campaign {0}";
            private const string OrganizationNameFormat = "Test Organization {0}";
            private const string ActivityNameFormat = "Activity {0}";
            private const string ActivityDescriptionFormat = "Description for activity {0}";
            private const string UserNameFormat = "User {0}";
            private const string TaskDescriptionFormat = "Task {0}";

            public static IEnumerable<Activity> GetActivities()
            {
                var users = Enumerable.Range(1, 10).Select(n => new ApplicationUser
                {
                    Id = n.ToString(),
                    Name = string.Format(UserNameFormat, n)
                })
                .ToArray();

                var campaigns = Enumerable.Range(1, 10).Select(n => new Campaign
                {
                    Description = string.Format(CampaignDescriptionFormat, n),
                    Name = string.Format(CampaignNameFormat, n),
                    Id = n
                })
                .ToArray();

                var organizations = Enumerable.Range(1, 10).Select(n => new Organization
                {
                    Name = string.Format(OrganizationNameFormat, n),
                    Campaigns = new List<Campaign>(new[] { campaigns[n - 1] })
                })
                .ToArray();

                var activities = Enumerable.Range(1, 10).Select(n => new Activity
                {
                    Campaign = campaigns[n - 1],
                    EndDateTime = DateTime.MaxValue.ToUniversalTime(),
                    StartDateTime = DateTime.MinValue.ToUniversalTime(),
                    Name = string.Format(ActivityNameFormat, n),
                    Description = string.Format(ActivityDescriptionFormat, n),
                    Id = n,
                    UsersSignedUp = new List<ActivitySignup>
                    {
                        new ActivitySignup
                        {
                            User = users[n - 1],
                            SignupDateTime = DateTime.Now.ToUniversalTime(),
                            PreferredEmail = "foo@foo.com",
                            PreferredPhoneNumber = "(555) 555-5555"
                        }
                    },
                    Tasks = new List<AllReadyTask>
                    {
                        new AllReadyTask
                        {
                            Id = n,
                            Name = string.Format(TaskDescriptionFormat,n),
                            NumberOfVolunteersRequired = 1,
                            Organization = organizations[n - 1],
                            AssignedVolunteers = new List<TaskSignup>
                            {
                                new TaskSignup
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
