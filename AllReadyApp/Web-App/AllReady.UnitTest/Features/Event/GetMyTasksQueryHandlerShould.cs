using System.Linq;
using AllReady.Models;
using Xunit;
using System.Threading.Tasks;
using AllReady.Features.Events;

namespace AllReady.UnitTest.Features.Event
{
    using Event = AllReady.Models.Event;

    public class GetMyTasksQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task ReturnsExpectedTasks()
        {
            var options = CreateNewContextOptions();

            const int eventId = 1;
            const string userId = "9D0929AC-BE6A-4A0B-A758-6C6FC31A8C47";
            var message = new GetMyVolunteerTasksQuery { EventId = eventId, UserId = userId };

            using (var context = new AllReadyContext(options))
            {
                context.VolunteerTaskSignups.Add(new VolunteerTaskSignup
                {
                    User = new ApplicationUser { Id = userId },
                    VolunteerTask = new VolunteerTask { Event = new Event { Id = eventId, Campaign = new Campaign { Locked = false }}}
                });
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new GetMyVolunteerTasksQueryHandler(context);
                var response = sut.Handle(message);

                Assert.True(response.Any());
            }
        }
    }
}