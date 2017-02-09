using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class OrganizationIdByTaskIdQueryHandlerShould : InMemoryContextTest
    {
        private readonly VolunteerTask volunteerTask;
        private const int VolunteerTaskId = 1;
        private const int OrganizationId = 2;

        public OrganizationIdByTaskIdQueryHandlerShould()
        {
            volunteerTask = new VolunteerTask { Id = VolunteerTaskId, Organization = new Organization { Id = OrganizationId } };

            Context.VolunteerTasks.Add(volunteerTask);
            Context.VolunteerTasks.Add(new VolunteerTask { Id = 2 });
            Context.SaveChanges();
        }

        [Fact]
        public async Task ReturnCorrectData()
        {
            var message = new OrganizationIdByVolunteerTaskIdQuery { VolunteerTaskId = VolunteerTaskId };

            var sut = new OrganizationIdByVolunteerTaskIdQueryHandler(Context);
            var organizationId = await sut.Handle(message);

            Assert.Equal(organizationId, volunteerTask.Organization.Id);
        }
    }
}