using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Models;
using Xunit;
using System.Linq;
using Shouldly;

namespace AllReady.UnitTest.Areas.Admin.Features.Itineraries
{
    public class SetTeamLeadCommandHandlerShould : InMemoryContextTest
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
                Itinerary = itinerary1
            };

            var itinerary2= new Itinerary
            {
                Id = 2,
                Name = "Test Itinerary 2"
            };

            var task2 = new VolunteerTask
            {
                Id = 2,
                Name = "Test Task"
            };

            var taskSignup2 = new VolunteerTaskSignup
            {
                Id = 2,
                VolunteerTask = task2,
                Itinerary = itinerary2,
                IsTeamLead = true
            };
            
            var taskSignup3 = new VolunteerTaskSignup
            {
                Id = 3,
                VolunteerTask = task2,
                Itinerary = itinerary2,
            };

            Context.Add(itinerary1);
            Context.Add(task1);
            Context.Add(taskSignup1);

            Context.Add(itinerary2);
            Context.Add(task2);
            Context.Add(taskSignup2);
            Context.Add(taskSignup3);

            Context.SaveChanges();
        }

        [Fact]
        public async Task SetsTeamLead_WhenNoPriorTeamLead()
        {
            var sut = new SetTeamLeadCommandHandler(Context);

            await sut.Handle(new SetTeamLeadCommand(1, 1));

            Context.VolunteerTaskSignups.Any(x => x.IsTeamLead && x.ItineraryId == 1).ShouldBeTrue();
        }

        [Fact]
        public async Task ReturnsSuccessResult_WhenTeamLeadChanged()
        {
            var sut = new SetTeamLeadCommandHandler(Context);

            var result = await sut.Handle(new SetTeamLeadCommand(1, 1));

            result.ShouldBe(SetTeamLeadResult.Success);
        }

        [Fact]
        public async Task ReplacesTeamLead_WhenPriorTeamLead()
        {
            var sut = new SetTeamLeadCommandHandler(Context);

            await sut.Handle(new SetTeamLeadCommand(2, 3));

            Context.VolunteerTaskSignups.Count(x => x.IsTeamLead && x.ItineraryId == 2).ShouldBe(1);

            var teamLead = Context.VolunteerTaskSignups.FirstOrDefault(x => x.IsTeamLead);

            teamLead.Id.ShouldBe(3);
        }

        [Fact]
        public async Task ReturnsFailureResult_WhenVolunteerTaskIdNotFound()
        {
            var sut = new SetTeamLeadCommandHandler(Context);

            var result = await sut.Handle(new SetTeamLeadCommand(1, 400));

            result.ShouldBe(SetTeamLeadResult.VolunteerTaskSignupNotFound);
        }
    }
}
