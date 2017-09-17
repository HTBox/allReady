using System;
using System.Linq;
using System.Collections.Generic;
using AllReady.Models;
using Shouldly;
using Xunit;
using System.Threading.Tasks;
using AllReady.Features.Events;

namespace AllReady.UnitTest.Features.Event
{
    using Event = AllReady.Models.Event;

    // Can't use this.Context because we're updating data and we can't have two items in Entity Framework's change tracking, see https://docs.efproject.net/en/latest/miscellaneous/testing.html#writing-tests
    public class ShowEventQueryHandlerShould : InMemoryContextTest
    {
        //happy path test. set up data to get all possible properties populated when EventViewModel is returned from handler
        [Fact]
        public async Task SetsTaskSignupViewModel_WithTheCorrectData()
        {
            var options = CreateNewContextOptions();

            var appUser = new ApplicationUser
            {
                Id = "asdfasasdfaf",
                Email = "foo@bar.com",
                FirstName = "Foo",
                LastName = "Bar",
                PhoneNumber = "555-555-5555",
            };

            var message = new ShowEventQuery { EventId = 1, UserId = appUser.Id };

            using (var context = new AllReadyContext(options))
            {
                context.Users.Add(appUser);
                context.Events.Add(CreateAllReadyEventWithTasks(message.EventId, appUser));
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new ShowEventQueryHandler(context);
                var eventViewModel = await sut.Handle(message);

                Assert.Equal(message.EventId, eventViewModel.SignupModel.EventId);
                Assert.Equal(appUser.Id, eventViewModel.SignupModel.UserId);
                Assert.Equal($"{appUser.FirstName} {appUser.LastName}", eventViewModel.SignupModel.Name);
            }
        }

        [Fact]
        public async Task ReturnNullWhenEventIsNotFound()
        {
            var options = CreateNewContextOptions();

            var message = new ShowEventQuery { EventId = 1 };

            using (var context = new AllReadyContext(options))
            {
                // add nothing
                //await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new ShowEventQueryHandler(context);
                var result = await sut.Handle(message);

                result.ShouldBeNull();
            }
        }

        [Fact]
        public async Task ReturnNullWhenEventsCampaignIsLocked()
        {
            var options = CreateNewContextOptions();

            const int eventId = 1;
            var message = new ShowEventQuery { EventId = eventId };

            using (var context = new AllReadyContext(options))
            {
                context.Events.Add(new Event
                {
                    Id = eventId,
                    Campaign = new Campaign { Locked = true }
                });
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new ShowEventQueryHandler(context);
                var result = await sut.Handle(message);

                result.ShouldBeNull();
            }
        }

        [Fact]
        public async Task SetUserSkillsToNull_WhenAppUserIsNull_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            var options = CreateNewContextOptions();

            const int eventId = 1;
            const string userId = "asdfasdf";

            var appUser = new ApplicationUser { Id = userId };
            var message = new ShowEventQuery { EventId = eventId, UserId = userId };

            using (var context = new AllReadyContext(options))
            {
                context.Users.Add(appUser);
                context.Events.Add(CreateAllReadyEventWithTasks(message.EventId, appUser));
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new ShowEventQueryHandler(context);
                var eventViewModel = await sut.Handle(message);

                eventViewModel.UserSkills.ShouldBeEmpty();
            }
        }

        [Fact]
        public async Task SetUserSkillsToNull_WhenAppUserIsNotNull_AndAppUserAssociatedSkillsIsNull_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            var options = CreateNewContextOptions();

            const int eventId = 1;
            const string userId = "asdfasdf";

            var appUser = new ApplicationUser { Id = userId, AssociatedSkills = null };
            var message = new ShowEventQuery { EventId = eventId, UserId = userId };

            using (var context = new AllReadyContext(options))
            {
                context.Users.Add(appUser);
                context.Events.Add(CreateAllReadyEventWithTasks(message.EventId, appUser));
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new ShowEventQueryHandler(context);
                var eventViewModel = await sut.Handle(message);

                eventViewModel.UserSkills.ShouldBeEmpty();
            }
        }

        [Fact]
        public async Task SetUserTasksToListOfTaskViewModelForAnyTasksWhereTheUserHasVolunteeredInAscendingOrderByTaskStartDateTime_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            var options = CreateNewContextOptions();

            const int eventId = 1;
            const string userId = "asdfasdf";

            var appUser = new ApplicationUser { Id = userId };
            var message = new ShowEventQuery { EventId = eventId, UserId = userId };
            var allReadyEvent = CreateAllReadyEventWithTasks(message.EventId, appUser);

            using (var context = new AllReadyContext(options))
            {
                context.Users.Add(appUser);
                context.Events.Add(allReadyEvent);
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new ShowEventQueryHandler(context);
                var eventViewModel = await sut.Handle(message);

                Assert.Equal(allReadyEvent.VolunteerTasks.Count(x => x.AssignedVolunteers.Any(y => y.User.Id.Equals(appUser.Id))),
                    eventViewModel.UserTasks.Count);
                var previousDateTime = DateTimeOffset.MinValue;
                foreach (var userTask in eventViewModel.UserTasks)
                {
                    Assert.True(userTask.StartDateTime > previousDateTime);
                    previousDateTime = userTask.StartDateTime;
                }
            }
        }

        [Fact]
        public async Task SetTasksToListOfTaskViewModelForAnyTasksWhereTheUserHasNotVolunteeredInAscendingOrderByTaskStartDateTime_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            var options = CreateNewContextOptions();

            const int eventId = 1;
            const string userId = "asdfasdf";

            var appUser = new ApplicationUser { Id = userId };
            var message = new ShowEventQuery { EventId = eventId, UserId = userId };
            var allReadyEvent = CreateAllReadyEventWithTasks(message.EventId, appUser);

            using (var context = new AllReadyContext(options))
            {
                context.Users.Add(appUser);
                context.Events.Add(allReadyEvent);
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new ShowEventQueryHandler(context);
                var eventViewModel = await sut.Handle(message);

                Assert.Equal(allReadyEvent.VolunteerTasks.Count(x => !x.AssignedVolunteers.Any(v => v.User.Id.Equals(appUser.Id))),
                    eventViewModel.Tasks.Count);
                var previousDateTime = DateTimeOffset.MinValue;
                foreach (var userTask in eventViewModel.Tasks)
                {
                    Assert.True(userTask.StartDateTime > previousDateTime);
                    previousDateTime = userTask.StartDateTime;
                }
            }
        }

        private static Event CreateAllReadyEventWithTasks(int eventId, ApplicationUser appUser)
        {
            return new Event
            {
                Id = eventId,
                Campaign = new Campaign { Locked = false, ManagingOrganization = new Organization() },
                Location = new Location(),
                VolunteerTasks = new List<VolunteerTask>
                {
                    new VolunteerTask { StartDateTime = new DateTimeOffset(2015, 8, 6, 12, 58, 05, new TimeSpan()), AssignedVolunteers = new List<VolunteerTaskSignup> { new VolunteerTaskSignup { User = appUser } } },
                    new VolunteerTask { StartDateTime = new DateTimeOffset(2016, 7, 31, 1, 15, 28, new TimeSpan()), AssignedVolunteers = new List<VolunteerTaskSignup> { new VolunteerTaskSignup { User = appUser } }},
                    new VolunteerTask { StartDateTime = new DateTimeOffset(2014, 2, 1, 5, 18, 27, new TimeSpan()), AssignedVolunteers = new List<VolunteerTaskSignup> { new VolunteerTaskSignup { User = appUser } }},
                    new VolunteerTask { StartDateTime = new DateTimeOffset(2014, 12, 15, 17, 2, 18, new TimeSpan())},
                    new VolunteerTask { StartDateTime = new DateTimeOffset(2016, 12, 15, 17, 2, 18, new TimeSpan())},
                    new VolunteerTask { StartDateTime = new DateTimeOffset(2013, 12, 15, 17, 2, 18, new TimeSpan())},
                }
            };
        }
    }
}
