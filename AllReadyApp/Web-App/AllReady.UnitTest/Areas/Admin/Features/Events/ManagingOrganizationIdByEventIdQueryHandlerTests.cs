using AllReady.Areas.Admin.Features.Events;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Events
{
    public class ManagingOrganizationIdByEventIdQueryHandlerTests
    {
        [Fact]
        public void ManagingOrganizationIdByEventIdQueryHandlerInvokesGetManagingOrganizationIdWithCorrectEventId()
        {
            var message = new ManagingOrganizationIdByEventIdQuery { EventId = 1 };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new ManagingOrganizationIdByEventIdQueryHandler(dataAccess.Object);
            sut.Handle(message);

            dataAccess.Verify(x => x.GetManagingOrganizationId(message.EventId), Times.Once);
        }
    }
}
