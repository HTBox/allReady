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
        [Fact(Skip = "NotImplemented")]
        public void SetsEventSignupViewModel_WithTheCorrectData()
        {
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

        [Fact(Skip = "NotImplemented")]
        public void SetUserSkillsToNull_WhenAppUserIsNull_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void SetUserSkillsToNull_WhenAppUserIsNotNull_AndAppUserseAssociatedSkillsIsNull_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        { 
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

        [Fact(Skip = "NotImplemented")]
        public void SetIsUserVolunteerdForEventToTrue_WhenThereAreEventSignupsForTheEventAndTheUser_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void SetIsUserVolunteerdForEventToFalse_WhenThereAreNoEventSignupsForTheEventAndTheUser_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void SetUserTasksToListOfTaskViewModelForAnyTasksWhereTheUserHasVolunteeredInAscendingOrderByTaskStartDateTime_WhenThereAreNoEventSignupsForTheEventAndTheUser_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void SetUserTasksToListOfTaskViewModelForAnyTasksWhereTheUserHasNotVolunteeredInAscendingOrderByTaskStartDateTime_WhenThereAreNoEventSignupsForTheEventAndTheUser_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
        }

        private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            return new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        }
    }
}