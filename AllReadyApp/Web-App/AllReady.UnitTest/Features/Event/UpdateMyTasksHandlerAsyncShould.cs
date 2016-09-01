using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AllReady.Features.Events;
using AllReady.Models;
using AllReady.ViewModels.Event;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{

    // FRAGILE: create lots of AllReadyContext to defeat change tracking, see https://docs.efproject.net/en/latest/miscellaneous/testing.html
    public class UpdateMyTasksHandlerAsyncShould : InMemoryContextTest {


        [Fact]
        public async Task CanSaveZeroTasks() {
            var options = this.CreateNewContextOptions();

            const string userId = "1";
            var user = new ApplicationUser() {Id = userId};

            using (var context = new AllReadyContext(options)) {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            var message = new UpdateMyTasksCommandAsync { UserId =userId, TaskSignups = new List<TaskSignupViewModel>() };

            using (var context = new AllReadyContext(options)) {
                var sut = new UpdateMyTasksHandlerAsync(context);
                await sut.Handle(message);
            }

            using (var context = new AllReadyContext(options)) {
                var taskSignups = context.TaskSignups.Count();
                Assert.Equal(taskSignups, 0);
            }
        }

        [Fact]
        public async Task InvokeUpdateTaskSignupAsyncForEachTaskSignupViewModelOnCommand() {
            var options = this.CreateNewContextOptions();

            const string userId = "1";
            const int firstId = 1;
            const int secondId = 2;

            var user = new ApplicationUser() {Id = userId};
            var taskSignupViewModels = new List<TaskSignupViewModel> {new TaskSignupViewModel() {Id = firstId}, new TaskSignupViewModel() {Id = secondId}};

            using (var context = new AllReadyContext(options)) {
                context.Users.Add(user);
                context.TaskSignups.Add(new TaskSignup {Id = firstId});
                context.TaskSignups.Add(new TaskSignup {Id = secondId});
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options)) {
                var sut = new UpdateMyTasksHandlerAsync(context) {DateTimeUtcNow = () => DateTime.UtcNow};
                await sut.Handle(new UpdateMyTasksCommandAsync {TaskSignups = taskSignupViewModels});
            }

            using (var context = new AllReadyContext(options)) {
                var signup1 = context.TaskSignups.FirstOrDefault(x => x.Id == firstId);
                Assert.Equal(signup1 != null, true);
                var signup2 = context.TaskSignups.FirstOrDefault(x => x.Id == secondId);
                Assert.Equal(signup2 != null, true);
            }
        }

        [Fact]
        public async Task InvokeUpdateTaskSignupAsyncWithTheCorrectParametersForEachTaskSignupViewModelOnCommand()
        {
            var options = this.CreateNewContextOptions();

            const string userId = "1";
            const int taskSignupId = 1;
            var user = new ApplicationUser() {Id = userId};
            var dateTimeUtcNow = DateTime.UtcNow;
            var taskSignupViewModels = new List<TaskSignupViewModel>
            {
                new TaskSignupViewModel { Id = taskSignupId, StatusDescription = "statusDescription1", Status = "Status1", TaskId = 1, }
            };

            var message = new UpdateMyTasksCommandAsync { TaskSignups = taskSignupViewModels, UserId = userId};

            using (var context = new AllReadyContext(options)) {
                context.Users.Add(user);
                context.TaskSignups.Add(new TaskSignup {Id = taskSignupId});
                context.Tasks.Add(new AllReadyTask {Id = 1});
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options)) {
                var sut = new UpdateMyTasksHandlerAsync(context) {DateTimeUtcNow = () => dateTimeUtcNow};
                await sut.Handle(message);
            }

            using (var context = new AllReadyContext(options)) {
                var signup = context.TaskSignups.FirstOrDefault(x => x.Id == taskSignupId);
                Assert.Equal(signup != null, true);
            }
        }
    }
}

