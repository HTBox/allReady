using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Events;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class AddEventSignupCommandHandlerAsyncShould : InMemoryContextTestBase
    {
        [Fact]
        public async Task AddAnEventSignup()
        {
            var message = new AddEventSignupCommandAsync { EventSignup = new EventSignup() };

            var sut = new AddEventSignupCommandHandlerAsync(Context);
            await sut.Handle(message);

            var result = Context.EventSignup.Single(x => x.Id == message.EventSignup.Id);

            Assert.Equal(result, message.EventSignup);
        }
    }
}
