using System.Threading.Tasks;
using AllReady.Features.Event;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class AddEventSignupCommandHandlerAsyncTests
    {
        [Fact]
        public async Task InvokesAddEventSignupAsyncWithCorrectEventSignup()
        {
            var message = new AddEventSignupCommandAsync { EventSignup = new EventSignup() };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new AddEventSignupCommandHandlerAsync(dataAccess.Object);
            await sut.Handle(message);

            dataAccess.Verify(x => x.AddEventSignupAsync(message.EventSignup));
        }
    }
}
