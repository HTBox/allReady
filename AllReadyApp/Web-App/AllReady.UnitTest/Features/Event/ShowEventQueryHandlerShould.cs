using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using AllReady.Features.Event;
using AllReady.Models;
using Microsoft.AspNetCore.Identity;
using Moq;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class ShowEventQueryHandlerShould
    {
        //happy path test. set up data to get all possible properties populated when EventViewModel is returned from handler
        [Fact]
        public void SetsEventSignupViewModel_WithTheCorrectData()
        {
            var appUser = new ApplicationUser()
            {
                Id = "asdfasasdfaf",
                Email = "foo@bar.com",
                FirstName = "Foo",
                LastName = "Bar",
                PhoneNumber = "555-555-5555",
            };
            var message = new ShowEventQuery() { EventId = 1, User = new ClaimsPrincipal() };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(message.User)).Returns(appUser.Id);
            dataAccess.Setup(x => x.GetUser(appUser.Id)).Returns(appUser);
            dataAccess.Setup(x => x.GetEvent(message.EventId)).Returns(CreateAllReadyEventWithTasks(message.EventId, appUser));

            var sut = new ShowEventQueryHandler(dataAccess.Object, userManager.Object);
            var eventViewModel = sut.Handle(message);

            Assert.Equal(message.EventId, eventViewModel.SignupModel.EventId);
            Assert.Equal(appUser.Id, eventViewModel.SignupModel.UserId);
            Assert.Equal($"{appUser.FirstName} {appUser.LastName}", eventViewModel.SignupModel.Name);
            Assert.Equal(appUser.Email, eventViewModel.SignupModel.PreferredEmail);
            Assert.Equal(appUser.PhoneNumber, eventViewModel.SignupModel.PreferredPhoneNumber);
        }

        [Fact]
        public void InvokeGetEventWithTheCorrectEventId()
        {
            var message = new ShowEventQuery { EventId = 1 };
            var dataAccess = new Mock<IAllReadyDataAccess>();

            var sut = new ShowEventQueryHandler(dataAccess.Object, null);
            sut.Handle(message);

            dataAccess.Verify(x => x.GetEvent(message.EventId), Times.Once);
        }

        [Fact]
        public void ReturnNullWhenEventIsNotFound()
        {
            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetEvent(It.IsAny<int>())).Returns<Models.Event>(null);

            var sut = new ShowEventQueryHandler(dataAccess.Object, null);
            var result = sut.Handle(new ShowEventQuery());

            result.ShouldBeNull();
        }

        [Fact]
        public void ReturnNullWhenEventsCampaignIslocked()
        {
            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetEvent(It.IsAny<int>())).Returns(new Models.Event { Campaign = new Campaign { Locked = true }});

            var sut = new ShowEventQueryHandler(dataAccess.Object, null);
            var result = sut.Handle(new ShowEventQuery());

            result.ShouldBeNull();
        }

        [Fact]
        public void InvokeGetUserIdWithTheCorrectUser_WhenEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            var message = new ShowEventQuery { User = new ClaimsPrincipal() };

            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetEvent(It.IsAny<int>())).Returns(new Models.Event { Campaign = new Campaign { Locked = false }});
            dataAccess.Setup(x => x.GetUser(It.IsAny<string>())).Returns(new ApplicationUser());

            var userManager = CreateUserManagerMock();

            var sut = new ShowEventQueryHandler(dataAccess.Object, userManager.Object);
            sut.Handle(message);

            userManager.Verify(x => x.GetUserId(message.User), Times.Once);
        }

        [Fact]
        public void InvokeGetUserWithTheCorrectUserId_WhenEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            const string userId = "1";
            var message = new ShowEventQuery { User = new ClaimsPrincipal() };
            
            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetEvent(It.IsAny<int>())).Returns(new Models.Event { Campaign = new Campaign { Locked = false }});
            dataAccess.Setup(x => x.GetUser(It.IsAny<string>())).Returns(new ApplicationUser());

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            var sut = new ShowEventQueryHandler(dataAccess.Object, userManager.Object);
            sut.Handle(message);

            dataAccess.Verify(x => x.GetUser(userId), Times.Once);
        }

        [Fact]
        public void SetUserSkillsToNull_WhenAppUserIsNull_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            var appUser = new ApplicationUser() { Id = "asdfasdf" };
            var message = new ShowEventQuery() { EventId = 1, User = new ClaimsPrincipal() };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(message.User)).Returns("adfasdfaf");
            dataAccess.Setup(x => x.GetEvent(message.EventId)).Returns(CreateAllReadyEventWithTasks(message.EventId, appUser));

            var sut = new ShowEventQueryHandler(dataAccess.Object, userManager.Object);
            var eventViewModel = sut.Handle(message);

            Assert.Null(eventViewModel.UserSkills);
        }

        [Fact]
        public void SetUserSkillsToNull_WhenAppUserIsNotNull_AndAppUserseAssociatedSkillsIsNull_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            var appUser = new ApplicationUser()
            {
                Id = "asdfasfd",
                AssociatedSkills = null
            };
            var message = new ShowEventQuery() { EventId = 1, User = new ClaimsPrincipal() };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(message.User)).Returns(appUser.Id);
            dataAccess.Setup(x => x.GetUser(appUser.Id)).Returns(appUser);
            dataAccess.Setup(x => x.GetEvent(message.EventId)).Returns(CreateAllReadyEventWithTasks(message.EventId, appUser));

            var sut = new ShowEventQueryHandler(dataAccess.Object, userManager.Object);
            var eventViewModel = sut.Handle(message);

            Assert.Null(eventViewModel.UserSkills);
        }

        [Fact]
        public void InvokeGetEventSignupsWithCorrectParameters_WhenEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            const string userId = "1";
            var message = new ShowEventQuery { User = new ClaimsPrincipal() };
            var @event = new Models.Event { Campaign = new Campaign { Locked = false }};

            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetEvent(It.IsAny<int>())).Returns(@event);
            dataAccess.Setup(x => x.GetUser(It.IsAny<string>())).Returns(new ApplicationUser());

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            var sut = new ShowEventQueryHandler(dataAccess.Object, userManager.Object);
            sut.Handle(message);

            dataAccess.Verify(x => x.GetEventSignups(@event.Id, userId), Times.Once);
        }

        [Fact]
        public void SetIsUserVolunteerdForEventToTrue_WhenThereAreEventSignupsForTheEventAndTheUser_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            var appUser = new ApplicationUser(){ Id = "asdfasfd" };
            var message = new ShowEventQuery() { EventId = 1, User = new ClaimsPrincipal() };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(message.User)).Returns(appUser.Id);
            dataAccess.Setup(x => x.GetUser(appUser.Id)).Returns(appUser);
            dataAccess.Setup(x => x.GetEvent(message.EventId)).Returns(CreateAllReadyEventWithTasks(message.EventId, appUser));
            dataAccess.Setup(x => x.GetEventSignups(message.EventId, appUser.Id)).Returns(new List<EventSignup>() { new EventSignup() });

            var sut = new ShowEventQueryHandler(dataAccess.Object, userManager.Object);
            var eventViewModel = sut.Handle(message);

            Assert.True(eventViewModel.IsUserVolunteeredForEvent);
        }

        [Fact]
        public void SetIsUserVolunteerdForEventToFalse_WhenThereAreNoEventSignupsForTheEventAndTheUser_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            var appUser = new ApplicationUser() { Id = "asdfasfd" };
            var message = new ShowEventQuery() { EventId = 1, User = new ClaimsPrincipal() };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(message.User)).Returns(appUser.Id);
            dataAccess.Setup(x => x.GetUser(appUser.Id)).Returns(appUser);
            dataAccess.Setup(x => x.GetEvent(message.EventId)).Returns(CreateAllReadyEventWithTasks(message.EventId, appUser));

            var sut = new ShowEventQueryHandler(dataAccess.Object, userManager.Object);
            var eventViewModel = sut.Handle(message);

            Assert.False(eventViewModel.IsUserVolunteeredForEvent);
        }

        [Fact]
        public void SetUserTasksToListOfTaskViewModelForAnyTasksWhereTheUserHasVolunteeredInAscendingOrderByTaskStartDateTime_WhenThereAreNoEventSignupsForTheEventAndTheUser_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            var appUser = new ApplicationUser() { Id = "asdfasfd" };
            var message = new ShowEventQuery() { EventId = 1, User = new ClaimsPrincipal() };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(message.User)).Returns(appUser.Id);
            dataAccess.Setup(x => x.GetUser(appUser.Id)).Returns(appUser);
            var allReadyEvent = CreateAllReadyEventWithTasks(message.EventId, appUser);
            dataAccess.Setup(x => x.GetEvent(message.EventId)).Returns(allReadyEvent);
            dataAccess.Setup(x => x.GetEventSignups(message.EventId, appUser.Id)).Returns(new List<EventSignup>());

            var sut = new ShowEventQueryHandler(dataAccess.Object, userManager.Object);
            var eventViewModel = sut.Handle(message);

            Assert.Equal(allReadyEvent.Tasks.Where(x => x.AssignedVolunteers.Any(y => y.User.Id.Equals(appUser.Id))).Count(), 
                         eventViewModel.UserTasks.Count);
            var previousDateTime = DateTimeOffset.MinValue;
            foreach(var userTask in eventViewModel.UserTasks)
            {
                Assert.True(userTask.StartDateTime > previousDateTime);
                previousDateTime = userTask.StartDateTime;
            }
        }

        [Fact]
        public void SetTasksToListOfTaskViewModelForAnyTasksWhereTheUserHasNotVolunteeredInAscendingOrderByTaskStartDateTime_WhenThereAreNoEventSignupsForTheEventAndTheUser_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            var appUser = new ApplicationUser() { Id = "asdfasfd" };
            var message = new ShowEventQuery() { EventId = 1, User = new ClaimsPrincipal() };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(message.User)).Returns(appUser.Id);
            dataAccess.Setup(x => x.GetUser(appUser.Id)).Returns(appUser);
            var allReadyEvent = CreateAllReadyEventWithTasks(message.EventId, appUser);
            dataAccess.Setup(x => x.GetEvent(message.EventId)).Returns(allReadyEvent);
            dataAccess.Setup(x => x.GetEventSignups(message.EventId, appUser.Id)).Returns(new List<EventSignup>());

            var sut = new ShowEventQueryHandler(dataAccess.Object, userManager.Object);
            var eventViewModel = sut.Handle(message);

            Assert.Equal(allReadyEvent.Tasks.Where(x => !x.AssignedVolunteers.Any(v => v.User.Id.Equals(appUser.Id))).Count(), 
                         eventViewModel.Tasks.Count);
            var previousDateTime = DateTimeOffset.MinValue;
            foreach (var userTask in eventViewModel.Tasks)
            {
                Assert.True(userTask.StartDateTime > previousDateTime);
                previousDateTime = userTask.StartDateTime;
            }
        }

        private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            return new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        }

        private static Models.Event CreateAllReadyEventWithTasks(int eventId, ApplicationUser appUser)
        {
            return new Models.Event()
            {
                Id = eventId,
                Campaign = new Campaign() { Locked = false },
                Tasks = new List<AllReadyTask>()
                {
                    new AllReadyTask() { StartDateTime = new DateTimeOffset(2015, 8, 6, 12, 58, 05, new TimeSpan()), AssignedVolunteers = new List<TaskSignup>() { new TaskSignup() { User = appUser } } },
                    new AllReadyTask() { StartDateTime = new DateTimeOffset(2016, 7, 31, 1, 15, 28, new TimeSpan()), AssignedVolunteers = new List<TaskSignup>() { new TaskSignup() { User = appUser } }},
                    new AllReadyTask() { StartDateTime = new DateTimeOffset(2014, 2, 1, 5, 18, 27, new TimeSpan()), AssignedVolunteers = new List<TaskSignup>() { new TaskSignup() { User = appUser } }},
                    new AllReadyTask() { StartDateTime = new DateTimeOffset(2014, 12, 15, 17, 2, 18, new TimeSpan())},
                    new AllReadyTask() { StartDateTime = new DateTimeOffset(2016, 12, 15, 17, 2, 18, new TimeSpan())},
                    new AllReadyTask() { StartDateTime = new DateTimeOffset(2013, 12, 15, 17, 2, 18, new TimeSpan())},
                }
            };
        }
    }
}