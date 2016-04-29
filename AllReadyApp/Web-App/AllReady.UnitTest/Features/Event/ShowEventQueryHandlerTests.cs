using System.Security.Claims;
using AllReady.Features.Event;
using AllReady.Models;
using Moq;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class ShowEventQueryHandlerTests
    {
        [Fact]
        public void WhenEventNotFoundReturnsNull()
        {
            var showEventCommand = new ShowEventQuery { EventId = 1 };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetEvent(showEventCommand.EventId))
                      .Returns<Models.Event>(null);
            var sut = new ShowEventQueryHandler(dataAccess.Object);
            var viewModel = sut.Handle(showEventCommand);
            viewModel.ShouldBeNull();
        }

        [Fact]
        public void WhenEventCampaignIsLockedReturnsNull()
        {
            var showEventCommand = new ShowEventQuery { EventId = 1 };
            var mockDbAccess = new Mock<IAllReadyDataAccess>();
            var expectedEvent = new Models.Event { Campaign = new Campaign { Locked = true } };
            mockDbAccess.Setup(x => x.GetEvent(showEventCommand.EventId))
                        .Returns(expectedEvent);
            var sut = new ShowEventQueryHandler(mockDbAccess.Object);
            var viewModel = sut.Handle(showEventCommand);
            viewModel.ShouldBeNull();
        }

        [Fact]
        public void HappyPath()
        {
            var showEventCommand = new ShowEventQuery { EventId = 1, User = ClaimsPrincipal.Current };
            var mockDbAccess = new Mock<IAllReadyDataAccess>();
            var expectedEvent = new Models.Event { Campaign = new Campaign { Locked = false } };
            mockDbAccess.Setup(x => x.GetEvent(showEventCommand.EventId))
                        .Returns(expectedEvent);
            var sut = new ShowEventQueryHandler(mockDbAccess.Object);
            var viewModel = sut.Handle(showEventCommand);
            viewModel.ShouldNotBeNull();
        }
    }
}
