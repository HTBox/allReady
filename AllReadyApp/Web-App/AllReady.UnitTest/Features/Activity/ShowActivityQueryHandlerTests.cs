using System.Security.Claims;
using AllReady.Features.Activity;
using AllReady.Models;
using Moq;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Features.Activity
{
    public class ShowActivityQueryHandlerTests
    {
        [Fact]
        public void WhenActivityNotFoundReturnsNull()
        {
            var showActivityCommand = new ShowActivityQuery { ActivityId = 1 };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetActivity(showActivityCommand.ActivityId))
                      .Returns<Models.Activity>(null);
            var sut = new ShowActivityQueryHandler(dataAccess.Object);
            var viewModel = sut.Handle(showActivityCommand);
            viewModel.ShouldBeNull();
        }

        [Fact]
        public void WhenActivityCampaignIsLockedReturnsNull()
        {
            var showActivityCommand = new ShowActivityQuery { ActivityId = 1 };
            var mockDbAccess = new Mock<IAllReadyDataAccess>();
            var expectedActivity = new Models.Activity { Campaign = new Campaign { Locked = true } };
            mockDbAccess.Setup(x => x.GetActivity(showActivityCommand.ActivityId))
                        .Returns(expectedActivity);
            var sut = new ShowActivityQueryHandler(mockDbAccess.Object);
            var viewModel = sut.Handle(showActivityCommand);
            viewModel.ShouldBeNull();
        }

        [Fact]
        public void HappyPath()
        {
            var showActivityCommand = new ShowActivityQuery { ActivityId = 1, User = ClaimsPrincipal.Current };
            var mockDbAccess = new Mock<IAllReadyDataAccess>();
            var expectedActivity = new Models.Activity { Campaign = new Campaign { Locked = false } };
            mockDbAccess.Setup(x => x.GetActivity(showActivityCommand.ActivityId))
                        .Returns(expectedActivity);
            var sut = new ShowActivityQueryHandler(mockDbAccess.Object);
            var viewModel = sut.Handle(showActivityCommand);
            viewModel.ShouldNotBeNull();
        }
    }
}
