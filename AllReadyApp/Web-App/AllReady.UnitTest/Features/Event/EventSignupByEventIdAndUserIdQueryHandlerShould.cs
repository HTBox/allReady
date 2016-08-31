using System.Threading.Tasks;
using AllReady.Features.Event;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    using Event = AllReady.Models.Event;

    public class EventSignupByEventIdAndUserIdQueryHandlerShould : InMemoryContextTest {
        [Fact]
        public async Task InvokeGetEventSignupWithTheCorrectParameters()
        {
            var options = this.CreateNewContextOptions();
            
            const int eventId = 1;
            const string userId = "1";
            var message = new EventSignupByEventIdAndUserIdQuery {
                EventId = eventId,
                UserId = userId
            };

            using (var context = new AllReadyContext(options)) {
                // TODO: load stuff
                var e = new Event {
                    Id = eventId
                };
                var u = new ApplicationUser {
                    Id = userId
                };
                context.Events.Add(e);
                context.Users.Add(u);
                context.EventSignup.Add(new EventSignup {
                    Event = e,
                    User = u
                });
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options)) {
                var sut = new EventSignupByEventIdAndUserIdQueryHandler(context);
                var signup = sut.Handle(message);

                Assert.Equal(signup.User.Id, userId);
                Assert.Equal(signup.Event.Id, eventId);
            }
        }
    }
}
