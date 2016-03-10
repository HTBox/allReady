using System.Threading.Tasks;
using AllReady.Features.Activity;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Activity
{
    public class AddActivitySignupCommandHandlerAsyncTests
    {
        [Fact]
        public async Task InvokesAddActivitySignupAsyncWithCorrectActivitySignup()
        {
            var message = new AddActivitySignupCommandAsync { ActivitySignup = new ActivitySignup() };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new AddActivitySignupCommandHandlerAsync(dataAccess.Object);
            await sut.Handle(message);

            dataAccess.Verify(x => x.AddActivitySignupAsync(message.ActivitySignup));
        }
    }
}
