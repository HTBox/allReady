using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Tasks;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Features.Tasks
{
    public class TasksByApplicationUserIdQueryHandlerShould : InMemoryContextTest
    {
        private readonly VolunteerTasksByApplicationUserIdQuery message;
        private readonly VolunteerTaskSignup task;
        private readonly VolunteerTask alreadyTask;
        private readonly VolunteerTasksByApplicationUserIdQueryHandler sut;


        public TasksByApplicationUserIdQueryHandlerShould()
        {
            message = new VolunteerTasksByApplicationUserIdQuery { ApplicationUserId = Guid.NewGuid().ToString() };
            alreadyTask = new VolunteerTask { Name = "name" };
            task = new VolunteerTaskSignup { User = new ApplicationUser { Id = message.ApplicationUserId }, VolunteerTask = alreadyTask };

            Context.Add(alreadyTask);
            Context.Add(task);
            Context.SaveChanges();

            sut = new VolunteerTasksByApplicationUserIdQueryHandler(Context);
        }


        [Fact]
        public async Task ReturnCorrectAmount()
        {
            var result = await sut.Handle(message);
            Assert.Single(result);
        }

        [Fact]
        public async Task ReturnCorrectData()
        {
            var result = await sut.Handle(message);
            Assert.Same(alreadyTask, result.First());
        }

        [Fact]
        public async Task ReturnCorrectType()
        {
            var result = await sut.Handle(message);
            Assert.IsType<VolunteerTask>(result.First());
        }
    }
}