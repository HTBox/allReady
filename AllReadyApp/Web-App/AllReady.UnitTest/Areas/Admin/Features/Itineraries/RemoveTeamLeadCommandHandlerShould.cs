using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Models;
using Xunit;
using System.Linq;
using Shouldly;

namespace AllReady.UnitTest.Areas.Admin.Features.Itineraries
{
    public class RemoveTeamLeadCommandHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var itinerary1 = new Itinerary
            {
                Id = 1,
                Name = "Test Itinerary 1"
            };

            var task1 = new VolunteerTask
            {
                Id = 1,
                Name = "Test Task"
            };

            var taskSignup1 = new VolunteerTaskSignup
            {
                Id = 1,
                VolunteerTask = task1,
                Itinerary = itinerary1,
                IsTeamLead = true
            };

            Context.Add(itinerary1);
            Context.Add(task1);
            Context.Add(taskSignup1);

            Context.SaveChanges();
        }

        [Fact]
        public async Task RemovesTeamLead_WhenATeamLeadExists()
        {
            var sut = new RemoveTeamLeadCommandHandler(Context);

            await sut.Handle(new RemoveTeamLeadCommand(1));

            Context.VolunteerTaskSignups.Any(x => x.IsTeamLead && x.ItineraryId == 1).ShouldBeFalse();
        }
    }
}
