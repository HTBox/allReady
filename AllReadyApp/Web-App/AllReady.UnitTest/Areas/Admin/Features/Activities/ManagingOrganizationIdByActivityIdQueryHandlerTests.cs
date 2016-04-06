using AllReady.Areas.Admin.Features.Activities;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Activities
{
    public class ManagingOrganizationIdByActivityIdQueryHandlerTests
    {
        [Fact]
        public void ManagingOrganizationIdByActivityIdQueryHandlerInvokesGetManagingOrganizationIdWithCorrectActivityId()
        {
            var message = new ManagingOrganizationIdByActivityIdQuery { ActivityId = 1 };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new ManagingOrganizationIdByActivityIdQueryHandler(dataAccess.Object);
            sut.Handle(message);

            dataAccess.Verify(x => x.GetManagingOrganizationId(message.ActivityId), Times.Once);
        }
    }
}
