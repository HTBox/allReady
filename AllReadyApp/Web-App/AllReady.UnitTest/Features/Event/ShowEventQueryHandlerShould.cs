using System.Security.Claims;
using AllReady.Features.Event;
using AllReady.Models;
using Moq;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class ShowEventQueryHandlerShould
    {
        [Fact]
        public void ReturnNullWhenEventIsNotFound()
        {
            var showEventQuery = new ShowEventQuery { EventId = 1 };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetEvent(showEventQuery.EventId)).Returns<Models.Event>(null);

            var sut = new ShowEventQueryHandler(dataAccess.Object);
            var viewModel = sut.Handle(showEventQuery);

            viewModel.ShouldBeNull();
        }

        [Fact]
        public void ReturnNullWhenEventsCampaignIslocked()
        {
            var showEventCommand = new ShowEventQuery { EventId = 1 };
            var mockDbAccess = new Mock<IAllReadyDataAccess>();
            var expectedEvent = new Models.Event { Campaign = new Campaign { Locked = true } };
            mockDbAccess.Setup(x => x.GetEvent(showEventCommand.EventId)).Returns(expectedEvent);

            var sut = new ShowEventQueryHandler(mockDbAccess.Object);
            var viewModel = sut.Handle(showEventCommand);

            viewModel.ShouldBeNull();
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignUserSkillsToNull_WhenAppUserIsNull_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignUserSkillsToAppUsersAssociatedSkills_WhenAppUserIsNotNull_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
        }

        //TODO: more tests stubs need to be written

        [Fact(Skip = "NotImplemented")]
        public void InvokeGetEventSignupsWithCorrectParameters_WhenEventIsNotNullAndEventsCampaignIsUnlocked()
        {
        }
    }
}
