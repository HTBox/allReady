﻿using System.Threading.Tasks;
using AllReady.Features.Tasks;
using AllReady.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Xunit;

namespace AllReady.UnitTest.Features.Tasks
{
    public class UpdateTaskCommandHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task HandleInvokesUpdateTaskAsyncWithCorrectData()
        {
            var options = this.CreateNewContextOptions();

            const int taskId = 1;
            var message = new UpdateTaskCommand { AllReadyTask = new AllReadyTask {Id = taskId} };

            using (var context = new AllReadyContext(options)) {
                context.Tasks.Add(new AllReadyTask {
                    Id = taskId,
                    RequiredSkills = new List<TaskSkill> {
                        new TaskSkill()
                    }
                });
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options)) {
                var sut = new UpdateTaskCommandHandler(context);
                await sut.Handle(message);
            }

            using (var context = new AllReadyContext(options)) {
                var theTask = context.Tasks.Include(t => t.RequiredSkills).FirstOrDefault(t => t.Id == taskId);
                Assert.NotNull(theTask);
                Assert.Equal(theTask.RequiredSkills.Count, 0);
            }
        }
    }
}
