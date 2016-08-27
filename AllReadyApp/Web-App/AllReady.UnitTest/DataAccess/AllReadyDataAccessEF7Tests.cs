using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.Features.Event;
using Microsoft.EntityFrameworkCore;
using Xunit;
using MediatR;
using Moq;

namespace AllReady.UnitTest.DataAccess
{

    public class AllReadyDataAccessEF7Tests : InMemoryContextTest
    {

        #region Event
        [Fact]
        public void GetResourcesByCategoryReturnsOnlyThoseResourcesWithMatchingCategory()
        {
            const string categoryToMatch = "category1";

            var context = Context;
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

            var unregisterEventHandler = new UnregisterEventHandler(Context, new Mock<IMediator>().Object);

            var dataContext = Context;
            CreateSutAndPopulateTestDataForEventSignup(dataContext);

            var countOfEventSignupsBeforeMethodInvocation = dataContext.EventSignup.Count();
            var countOfTaskSignupsBeforeMethodInvocation = dataContext.TaskSignups.Count();

            await unregisterEventHandler.DeleteEventAndTaskSignupsAsync(anEventSignupIdThatDoesNotExist);

            var countOfEventSignsupsAfterMethodInvocation = dataContext.EventSignup.Count();
            var countOfTaskSignsupsAfterMethodInvocation = dataContext.TaskSignups.Count();

            Assert.Equal(countOfEventSignupsBeforeMethodInvocation, countOfEventSignsupsAfterMethodInvocation);
            Assert.Equal(countOfTaskSignupsBeforeMethodInvocation, countOfTaskSignsupsAfterMethodInvocation);
        }

        [Fact]
        public async Task DeleteEventAndTaskSignupsAsyncRemovesEventSignup()
        {
            const int eventSignupId = 5;

            var unregisterEventHandler = new UnregisterEventHandler(Context, new Mock<IMediator>().Object);

            CreateSutAndPopulateTestDataForEventSignup(Context);
            await unregisterEventHandler.DeleteEventAndTaskSignupsAsync(eventSignupId);

            var numOfUsersSignedUp = Context.Events.First(e => e.Id == eventSignupId).UsersSignedUp.Count;
            Assert.Equal(0, numOfUsersSignedUp);
        }

        [Fact]
        public async Task DeleteEventAndTaskSignupsAsyncRemovesTaskSignup()
        {
            const int eventSignupId = 5;

            var unregisterEventHandler = new UnregisterEventHandler(Context, new Mock<IMediator>().Object);

            CreateSutAndPopulateTestDataForEventSignup(Context);
            await unregisterEventHandler.DeleteEventAndTaskSignupsAsync(eventSignupId);

            var numOfTasksSignedUpFor = Context.TaskSignups.Count(e => e.Task.Event.Id == eventSignupId);
            Assert.Equal(0, numOfTasksSignedUpFor);
        }

        public void CreateSutAndPopulateTestDataForEventSignup(AllReadyContext allReadyContext) {
            PopulateDataForEventSignup(allReadyContext);
        }

        private static void PopulateDataForEventSignup(DbContext context)
        {
                var campaignEvents = TestEventModelProvider.GetEvents();

                foreach (var campaignEvent in campaignEvents)
                {
                    context.Add(campaignEvent);
                    context.Add(campaignEvent.Campaign);
                }
                context.SaveChanges();
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
                    FirstName = string.Format(UserNameFormat, n),
                    LastName = string.Format(UserNameFormat, n)
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
                    EndDateTime = new DateTime(2200, 12, 31).ToUniversalTime(),
                    StartDateTime = new DateTime(1753, 1, 1).ToUniversalTime(),
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

