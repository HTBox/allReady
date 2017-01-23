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
        private readonly TasksByApplicationUserIdQuery message;
        private readonly VolunteerTaskSignup task;
        private readonly VolunteerTask alreadyTask;
        private readonly TasksByApplicationUserIdQueryHandler sut;


        public TasksByApplicationUserIdQueryHandlerShould()
        {
            message = new TasksByApplicationUserIdQuery { ApplicationUserId = Guid.NewGuid().ToString() };
            alreadyTask = new VolunteerTask { Name = "name" };
            task = new VolunteerTaskSignup { User = new ApplicationUser { Id = message.ApplicationUserId }, VolunteerTask = alreadyTask };

            Context.Add(alreadyTask);
            Context.Add(task);
            Context.SaveChanges();

            sut = new TasksByApplicationUserIdQueryHandler(Context);
        }


        [Fact]
        public async Task ReturnCorrectAmount()
        {
            var result = await sut.Handle(message);
            Assert.Equal(1, result.Count);
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