using AllReady.Features.Tasks;
using AllReady.Models;
using MediatR;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.ViewModels.Shared;
using Xunit;

namespace AllReady.UnitTest.Features.Tasks
{
    using Event = AllReady.Models.Event;

    public class TaskSignupHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task Result_ShouldBe_ClosedTaskFailure_IfTaskIsClosed()
        {
            var mockMediator = new Mock<IMediator>();
            var message = new VolunteerTaskSignupCommand { TaskSignupModel = new VolunteerTaskSignupViewModel { VolunteerTaskId = 1, EventId = 1, UserId = "abc" } };

            var sut = new VolunteerTaskSignupCommandHandler(mockMediator.Object, Context);
            var result = await sut.Handle(message);

            Assert.Equal(TaskResultStatus.Failure_ClosedTask, result.Status);
            Assert.Equal(0, Context.VolunteerTaskSignups.Count());
        }

        [Fact]
        public async Task Result_ShouldBe_CampaignNotFound_IfCampaignIdDoesNotExist()
        {
            var mockMediator = new Mock<IMediator>();
            var message = new VolunteerTaskSignupCommand { TaskSignupModel = new VolunteerTaskSignupViewModel { VolunteerTaskId = 1, EventId = 100, UserId = "abc" } };

            var sut = new VolunteerTaskSignupCommandHandler(mockMediator.Object, Context);
            var result = await sut.Handle(message);

            Assert.Equal(TaskResultStatus.Failure_EventNotFound, result.Status);
            Assert.Equal(0, Context.VolunteerTaskSignups.Count());
        }

        [Fact]
        public async Task Result_ShouldBe_TaskNotFound_IfTaskIdDoesNotExist()
        {
            var mockMediator = new Mock<IMediator>();
            var message = new VolunteerTaskSignupCommand { TaskSignupModel = new VolunteerTaskSignupViewModel { VolunteerTaskId = 100, EventId = 1, UserId = "abc" } };

            var sut = new VolunteerTaskSignupCommandHandler(mockMediator.Object, Context);
            var result = await sut.Handle(message);

            Assert.Equal(TaskResultStatus.Failure_TaskNotFound, result.Status);
            Assert.Equal(0, Context.VolunteerTaskSignups.Count());
        }

        [Fact]
        public async Task Result_ShouldBe_Success_IfTaskIsNotClosed()
        {
            var mockMediator = new Mock<IMediator>();
            var message = new VolunteerTaskSignupCommand { TaskSignupModel = new VolunteerTaskSignupViewModel { VolunteerTaskId = 2, EventId = 1, UserId = "abc" } };

            var sut = new VolunteerTaskSignupCommandHandler(mockMediator.Object, Context);
            var result = await sut.Handle(message);

            Assert.Equal(TaskResultStatus.Success, result.Status);
            Assert.Equal(1, Context.VolunteerTaskSignups.Count());
        }

        protected override void LoadTestData()
        {
            Context.Users.Add(new ApplicationUser { Id = "abc" });

            var campaignEvent = new Event { Id = 1, Name = "Some Event" };
            Context.Events.Add(campaignEvent);

            Context.VolunteerTasks.Add(new VolunteerTask { Id = 1, Name = "Closed Task", EndDateTime = DateTime.UtcNow.AddDays(-100), Event = campaignEvent });
            Context.VolunteerTasks.Add(new VolunteerTask { Id = 2, Name = "Open Task", EndDateTime = DateTime.UtcNow.AddDays(100), Event = campaignEvent });

            Context.SaveChanges();
        }
    }
}
