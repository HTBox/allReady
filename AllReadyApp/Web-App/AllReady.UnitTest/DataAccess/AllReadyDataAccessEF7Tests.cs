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

        #region EventSignup
        [Fact]
        public async Task DeleteEventAndTaskSignupsAsyncDoesNotDeleteEventSignupsOrTaskSignupsForUnkownEventSignupId()
        {
            const int anEventSignupIdThatDoesNotExist = 1000;

            var sut = CreateSutAndPopulateTestDataForEventSignup();

            var countOfEventSignupsBeforeMethodInvocation = sut.EventSignups.Count();
            var countOfTaskSignupsBeforeMethodInvocation = sut.TaskSignups.Count();

            await sut.DeleteEventAndTaskSignupsAsync(anEventSignupIdThatDoesNotExist);

            var countOfEventSignsupsAfterMethodInvocation = sut.EventSignups.Count();
            var countOfTaskSignsupsAfterMethodInvocation = sut.TaskSignups.Count();

            Assert.Equal(countOfEventSignupsBeforeMethodInvocation, countOfEventSignsupsAfterMethodInvocation);
            Assert.Equal(countOfTaskSignupsBeforeMethodInvocation, countOfTaskSignsupsAfterMethodInvocation);
        }

        [Fact]
        public async Task DeleteEventAndTaskSignupsAsyncRemovesEventSignup()
        {
            const int eventSignupId = 5;

            var sut = CreateSutAndPopulateTestDataForEventSignup();
            await sut.DeleteEventAndTaskSignupsAsync(eventSignupId);

            var context = _serviceProvider.GetService<AllReadyContext>();
            var numOfUsersSignedUp = context.Events.First(e => e.Id == eventSignupId).UsersSignedUp.Count;
            Assert.Equal(0, numOfUsersSignedUp);
        }

        [Fact]
        public async Task DeleteEventAndTaskSignupsAsyncRemovesTaskSignup()
        {
            const int eventSignupId = 5;

            var sut = CreateSutAndPopulateTestDataForEventSignup();
            await sut.DeleteEventAndTaskSignupsAsync(eventSignupId);

            var context = _serviceProvider.GetService<AllReadyContext>();
            var numOfTasksSignedUpFor = context.TaskSignups.Count(e => e.Task.Event.Id == eventSignupId);
            Assert.Equal(0, numOfTasksSignedUpFor);
        }

        public IAllReadyDataAccess CreateSutAndPopulateTestDataForEventSignup()
        {
            var allReadyContext = _serviceProvider.GetService<AllReadyContext>();
            var allReadyDataAccess = new AllReadyDataAccessEF7(allReadyContext);
            PopulateDataForEventSignup(allReadyContext);
            return allReadyDataAccess;
        }

        private static void PopulateDataForEventSignup(DbContext context)
        {
            if (!populatedData)
            {
                var campaignEvents = TestEventModelProvider.GetEvents();

                foreach (var campaignEvent in campaignEvents)
                {
                    context.Add(campaignEvent);
                    context.Add(campaignEvent.Campaign);
                }
                context.SaveChanges();
                populatedData = true;
            }
        }

        private static class TestEventModelProvider
        {
            private const string CampaignNameFormat = "Campaign {0}";
            private const string CampaignDescriptionFormat = "Description for campaign {0}";
            private const string OrganizationNameFormat = "Test Organization {0}";
            private const string EventNameFormat = "Event {0}";
            private const string EventDescriptionFormat = "Description for event {0}";
            private const string UserNameFormat = "User {0}";
            private const string TaskDescriptionFormat = "Task {0}";

            public static IEnumerable<Event> GetEvents()
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

                var campaignEvents = Enumerable.Range(1, 10).Select(n => new Event
                {
                    Campaign = campaigns[n - 1],
                    EndDateTime = DateTime.MaxValue.ToUniversalTime(),
                    StartDateTime = DateTime.MinValue.ToUniversalTime(),
                    Name = string.Format(EventNameFormat, n),
                    Description = string.Format(EventDescriptionFormat, n),
                    Id = n,
                    UsersSignedUp = new List<EventSignup>
                    {
                        new EventSignup
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

                return campaignEvents;
            }
        }

        #endregion

    }
}
