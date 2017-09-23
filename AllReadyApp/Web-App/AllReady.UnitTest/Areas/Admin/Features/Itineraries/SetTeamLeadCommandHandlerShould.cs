using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Models;
using Xunit;
using System.Linq;
using MediatR;
using Moq;
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

            var volunteerTaskUser1 = new ApplicationUser()
            {
                Id = "B8C2A338-837E-4157-B9F2-B3D5A072917B",
                Email = "volunteer1@example.com",
                PhoneNumber = "+1 (999) 999-9999",
                FirstName = "FirstName100",
                LastName = "LastName100"
            };

            var taskSignup1 = new VolunteerTaskSignup
            {
                Id = 1,
                VolunteerTask = task1,
                Itinerary = itinerary1,
                User = volunteerTaskUser1
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

            var volunteerTaskUser2 = new ApplicationUser()
            {
                Id = "B8C2A338-837E-4157-B9F2-B3D5A072917B",
                Email = "volunteer2@example.com",
                PhoneNumber = "+1 (999) 999-9999",
                FirstName = "FirstName200",
                LastName = "LastName200"
            };

            var taskSignup2 = new VolunteerTaskSignup
            {
                Id = 2,
                VolunteerTask = task2,
                Itinerary = itinerary2,
                User = volunteerTaskUser2,
                IsTeamLead = true
            };

            var volunteerTaskUser3 = new ApplicationUser()
            {
                Id = "65DBF162-074D-4E11-8258-6E12BC6B06B8",
                Email = "volunteer3@example.com",
                PhoneNumber = "+1 (999) 999-9999",
                FirstName = "FirstName300",
                LastName = "LastName300"
            };

            var taskSignup3 = new VolunteerTaskSignup
            {
                Id = 3,
                VolunteerTask = task2,
                Itinerary = itinerary2,
                User = volunteerTaskUser3
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
            var mockMediator = new Mock<IMediator>();

            var sut = new SetTeamLeadCommandHandler(Context, mockMediator.Object);
            var itineraryUrl = "https://localhost/Admin/Itinerary/Details/1";

            await sut.Handle(new SetTeamLeadCommand(1, 1, itineraryUrl));

            Context.VolunteerTaskSignups.Any(x => x.IsTeamLead && x.ItineraryId == 1).ShouldBeTrue();
        }

        [Fact]
        public async Task ReturnsSuccessResult_WhenTeamLeadChanged()
        {
            var mockMediator = new Mock<IMediator>();

            var sut = new SetTeamLeadCommandHandler(Context, mockMediator.Object);
            var itineraryUrl = "https://localhost/Admin/Itinerary/Details/1";

            var result = await sut.Handle(new SetTeamLeadCommand(1, 1, itineraryUrl));

            result.ShouldBe(SetTeamLeadResult.Success);
        }

        [Fact]
        public async Task ReplacesTeamLead_WhenPriorTeamLead()
        {
            var mockMediator = new Mock<IMediator>();

            var sut = new SetTeamLeadCommandHandler(Context, mockMediator.Object);
            var itineraryUrl = "https://localhost/Admin/Itinerary/Details/2";

            await sut.Handle(new SetTeamLeadCommand(2, 3, itineraryUrl));

            Context.VolunteerTaskSignups.Count(x => x.IsTeamLead && x.ItineraryId == 2).ShouldBe(1);

            var teamLead = Context.VolunteerTaskSignups.FirstOrDefault(x => x.IsTeamLead);

            teamLead.Id.ShouldBe(3);
        }

        [Fact]
        public async Task ReturnsFailureResult_WhenVolunteerTaskIdNotFound()
        {
            var mockMediator = new Mock<IMediator>();

            var sut = new SetTeamLeadCommandHandler(Context, mockMediator.Object);
            var itineraryUrl = "https://localhost/Admin/Itinerary/Details/1";

            var result = await sut.Handle(new SetTeamLeadCommand(1, 400, itineraryUrl));

            result.ShouldBe(SetTeamLeadResult.VolunteerTaskSignupNotFound);
        }
    }
}
