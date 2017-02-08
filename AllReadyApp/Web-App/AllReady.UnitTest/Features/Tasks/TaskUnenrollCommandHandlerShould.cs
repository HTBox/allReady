﻿using AllReady.Features.Tasks;
using AllReady.Models;
using MediatR;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Tasks
{
    using Event = AllReady.Models.Event;

    public class TaskUnenrollCommandHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task Result_ShouldBe_Success_IfTaskSignupExists()
        {
            var mockMediator = new Mock<IMediator>();
            var message = new VolunteerTaskUnenrollCommand { TaskId = 1, UserId = "abc" };

            var sut = new VolunteerTaskUnenrollCommandHandler(mockMediator.Object, Context);
            var result = await sut.Handle(message);

            Assert.Equal("success", result.Status);
            Assert.NotNull(result.Task);
        }

        [Fact]
        public async Task Result_ShouldBe_Failure_IfTaskIdDoesNotExist()
        {
            var mockMediator = new Mock<IMediator>();
            var message = new VolunteerTaskUnenrollCommand { TaskId = 100, UserId = "abc" };

            var sut = new VolunteerTaskUnenrollCommandHandler(mockMediator.Object, Context);
            var result = await sut.Handle(message);

            Assert.Equal("failure", result.Status);
        }

        [Fact]
        public async Task TaskSignUp_ShouldBe_Deleted()
        {
            var mockMediator = new Mock<IMediator>();
            var message = new VolunteerTaskUnenrollCommand { TaskId = 1, UserId = "abc" };

            var sut = new VolunteerTaskUnenrollCommandHandler(mockMediator.Object, Context);
            var result = await sut.Handle(message);

            Assert.Equal(0, Context.TaskSignups.Count());
        }

        protected override void LoadTestData()
        {
            var user = new ApplicationUser { Id = "abc" };
            Context.Users.Add(user);

            var campaignEvent = new Event { Id = 1, Name = "Some Event" };
            Context.Events.Add(campaignEvent);

            var @task = new VolunteerTask { Id = 1, Name = "Some Task", EndDateTime = DateTime.UtcNow.AddDays(100), Event = campaignEvent };
            Context.Tasks.Add(@task);
           
            Context.TaskSignups.Add(new VolunteerTaskSignup { VolunteerTask = @task, User = user });
        
            Context.SaveChanges();
        }
    }
}
