using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Events
{
    public class UpdateEventHandlerTests
    {
        [Fact]
        public async Task UpdateEventHandlerInvokesUpdateEventWithCorrectEvent()
        {
            var message = new UpdateEvent { Event = new Event() };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new UpdateEventHandler(dataAccess.Object);
            await sut.Handle(message);

            dataAccess.Verify(x => x.UpdateEvent(message.Event), Times.Once);
        }
    }
}
