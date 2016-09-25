using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using AllReady.Models;
using Microsoft.AspNetCore.Identity;
using Moq;
using Shouldly;
using Xunit;
using System.Threading.Tasks;
using AllReady.Features.Events;
using Helpers;

namespace AllReady.UnitTest.Features.Event
{
    // Can't use this.Context because we're updating data and we can't have two items in Entity Framework's change tracking, see https://docs.efproject.net/en/latest/miscellaneous/testing.html#writing-tests
    public class ShowEventQueryHandlerShould : InMemoryContextTest
    {
        //happy path test. set up data to get all possible properties populated when EventViewModel is returned from handler
        [Fact]
        public async Task SetsTaskSignupViewModel_WithTheCorrectData()
        {
            var options = this.CreateNewContextOptions();

            var appUser = new ApplicationUser
            {
                Id = "asdfasasdfaf",
                Email = "foo@bar.com",
                FirstName = "Foo",
                LastName = "Bar",
                PhoneNumber = "555-555-5555",
            };
            var message = new ShowEventQueryAsync { EventId = 1, User = new ClaimsPrincipal() };
            var userManager = TestHelpers.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser { Id = appUser.Id });

            using (var context = new AllReadyContext(options))
            {
                context.Users.Add(appUser);
                context.Events.Add(CreateAllReadyEventWithTasks(message.EventId, appUser));
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new ShowEventQueryHandlerAsync(context, userManager.Object);
                var eventViewModel = await sut.Handle(message);

                Assert.Equal(message.EventId, eventViewModel.SignupModel.EventId);
                Assert.Equal(appUser.Id, eventViewModel.SignupModel.UserId);
                Assert.Equal($"{appUser.FirstName} {appUser.LastName}", eventViewModel.SignupModel.Name);
            }
        }

        [Fact]
        public async Task ReturnNullWhenEventIsNotFound()
        {
            var options = this.CreateNewContextOptions();

            var message = new ShowEventQueryAsync { EventId = 1 };

            using (var context = new AllReadyContext(options))
            {
                // add nothing
                //await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new ShowEventQueryHandlerAsync(context, null);
                var result = await sut.Handle(message);

                result.ShouldBeNull();
            }
        }

        [Fact]
        public async Task ReturnNullWhenEventsCampaignIslocked()
        {
            var options = this.CreateNewContextOptions();

            const int eventId = 1;
            var message = new ShowEventQueryAsync { EventId = eventId };

            using (var context = new AllReadyContext(options))
            {
                context.Events.Add(new Models.Event
                {
                    Id = eventId,
                    Campaign = new Campaign { Locked = true }
                });
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new ShowEventQueryHandlerAsync(context, null);
                var result = await sut.Handle(message);

                result.ShouldBeNull();
            }
        }

        [Fact]
        public async Task InvokeGetUserIdWithTheCorrectUser_WhenEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            var options = this.CreateNewContextOptions();

            const int eventId = 1;
            const string userId = "1";
            var message = new ShowEventQueryAsync { EventId = eventId, User = new ClaimsPrincipal() };

            var userManager = TestHelpers.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser { Id = userId });

            using (var context = new AllReadyContext(options))
            {
                context.Events.Add(new Models.Event
                {
                    Id = eventId,
                    Campaign = new Campaign { Locked = false }
                });
                context.Users.Add(new ApplicationUser { Id = userId });
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new ShowEventQueryHandlerAsync(context, userManager.Object);
                await sut.Handle(message);

                userManager.Verify(x => x.GetUserAsync(message.User), Times.Once);
            }
        }

        [Fact]
        public async Task SetUserSkillsToNull_WhenAppUserIsNull_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            var options = this.CreateNewContextOptions();

            const int eventId = 1;
            const string userId = "asdfasdf";

            var appUser = new ApplicationUser { Id = userId };
            var message = new ShowEventQueryAsync { EventId = eventId, User = new ClaimsPrincipal() };
            var userManager = TestHelpers.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser { Id = userId });

            using (var context = new AllReadyContext(options))
            {
                context.Users.Add(appUser);
                context.Events.Add(CreateAllReadyEventWithTasks(message.EventId, appUser));
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new ShowEventQueryHandlerAsync(context, userManager.Object);
                var eventViewModel = await sut.Handle(message);

                eventViewModel.UserSkills.ShouldBeEmpty();
            }
        }

        [Fact]
        public async Task SetUserSkillsToNull_WhenAppUserIsNotNull_AndAppUserseAssociatedSkillsIsNull_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            var options = this.CreateNewContextOptions();

            const int eventId = 1;
            const string userId = "asdfasdf";

            var appUser = new ApplicationUser
            {
                Id = userId,
                AssociatedSkills = null
            };
            var message = new ShowEventQueryAsync { EventId = eventId, User = new ClaimsPrincipal() };

            var userManager = TestHelpers.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser { Id = appUser.Id });

            using (var context = new AllReadyContext(options))
            {
                context.Users.Add(appUser);
                context.Events.Add(CreateAllReadyEventWithTasks(message.EventId, appUser));
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new ShowEventQueryHandlerAsync(context, userManager.Object);
                var eventViewModel = await sut.Handle(message);
                
                eventViewModel.UserSkills.ShouldBeEmpty();
            }
        }

        [Fact]
        public async Task SetUserTasksToListOfTaskViewModelForAnyTasksWhereTheUserHasVolunteeredInAscendingOrderByTaskStartDateTime_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            var options = this.CreateNewContextOptions();

            const int eventId = 1;
            const string userId = "asdfasdf";

            var appUser = new ApplicationUser { Id = userId };
            var message = new ShowEventQueryAsync { EventId = eventId, User = new ClaimsPrincipal() };
            var allReadyEvent = CreateAllReadyEventWithTasks(message.EventId, appUser);

            var userManager = TestHelpers.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser { Id = appUser.Id });

            using (var context = new AllReadyContext(options))
            {
                context.Users.Add(appUser);
                context.Events.Add(allReadyEvent);
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new ShowEventQueryHandlerAsync(context, userManager.Object);
                var eventViewModel = await sut.Handle(message);

                Assert.Equal(allReadyEvent.Tasks.Where(x => x.AssignedVolunteers.Any(y => y.User.Id.Equals(appUser.Id))).Count(),
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
            var options = this.CreateNewContextOptions();

            const int eventId = 1;
            const string userId = "asdfasdf";

            var appUser = new ApplicationUser { Id = userId };
            var message = new ShowEventQueryAsync { EventId = eventId, User = new ClaimsPrincipal() };
            var allReadyEvent = CreateAllReadyEventWithTasks(message.EventId, appUser);

            var userManager = TestHelpers.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser { Id = appUser.Id });

            using (var context = new AllReadyContext(options))
            {
                context.Users.Add(appUser);
                context.Events.Add(allReadyEvent);
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new ShowEventQueryHandlerAsync(context, userManager.Object);
                var eventViewModel = await sut.Handle(message);

                Assert.Equal(allReadyEvent.Tasks.Where(x => !x.AssignedVolunteers.Any(v => v.User.Id.Equals(appUser.Id))).Count(),
                    eventViewModel.Tasks.Count);
                var previousDateTime = DateTimeOffset.MinValue;
                foreach (var userTask in eventViewModel.Tasks)
                {
                    Assert.True(userTask.StartDateTime > previousDateTime);
                    previousDateTime = userTask.StartDateTime;
                }
            }
        }



        private static Models.Event CreateAllReadyEventWithTasks(int eventId, ApplicationUser appUser)
        {
            return new Models.Event
            {
                Id = eventId,
                Campaign = new Campaign { Locked = false },
                Tasks = new List<AllReadyTask>
                {
                    new AllReadyTask { StartDateTime = new DateTimeOffset(2015, 8, 6, 12, 58, 05, new TimeSpan()), AssignedVolunteers = new List<TaskSignup> { new TaskSignup { User = appUser } } },
                    new AllReadyTask { StartDateTime = new DateTimeOffset(2016, 7, 31, 1, 15, 28, new TimeSpan()), AssignedVolunteers = new List<TaskSignup> { new TaskSignup { User = appUser } }},
                    new AllReadyTask { StartDateTime = new DateTimeOffset(2014, 2, 1, 5, 18, 27, new TimeSpan()), AssignedVolunteers = new List<TaskSignup> { new TaskSignup { User = appUser } }},
                    new AllReadyTask { StartDateTime = new DateTimeOffset(2014, 12, 15, 17, 2, 18, new TimeSpan())},
                    new AllReadyTask { StartDateTime = new DateTimeOffset(2016, 12, 15, 17, 2, 18, new TimeSpan())},
                    new AllReadyTask { StartDateTime = new DateTimeOffset(2013, 12, 15, 17, 2, 18, new TimeSpan())},
                }
            };
        }
    }
}