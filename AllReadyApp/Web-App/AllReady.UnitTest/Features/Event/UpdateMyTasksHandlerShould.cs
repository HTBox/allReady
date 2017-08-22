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
    public class UpdateMyTasksHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task CanSaveZeroTasks()
        {
            var options = CreateNewContextOptions();

            const string userId = "1";
            var user = new ApplicationUser {Id = userId};

            using (var context = new AllReadyContext(options))
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            var message = new UpdateMyVolunteerTasksCommand { UserId =userId, VolunteerTaskSignups = new List<VolunteerTaskSignupViewModel>() };

            using (var context = new AllReadyContext(options))
            {
                var sut = new UpdateMyVolunteerTasksCommandHandler(context);
                await sut.Handle(message);
            }

            using (var context = new AllReadyContext(options))
            {
                var volunteerTaskSignups = context.VolunteerTaskSignups.Count();
                Assert.Equal(0, volunteerTaskSignups);
            }
        }

        [Fact]
        public async Task InvokeUpdateTaskSignupAsyncForEachTaskSignupViewModelOnCommand()
        {
            var options = CreateNewContextOptions();

            const string userId = "1";
            const int firstId = 1;
            const int secondId = 2;

            var user = new ApplicationUser {Id = userId};
            var volunteerTaskSignupViewModels = new List<VolunteerTaskSignupViewModel>
            {
                new VolunteerTaskSignupViewModel { Id = firstId, Status = "Accepted" },
                new VolunteerTaskSignupViewModel { Id = secondId, Status = "Accepted" }
            };

            using (var context = new AllReadyContext(options))
            {
                context.Users.Add(user);
                context.VolunteerTaskSignups.Add(new VolunteerTaskSignup { Id = firstId });
                context.VolunteerTaskSignups.Add(new VolunteerTaskSignup { Id = secondId });
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new UpdateMyVolunteerTasksCommandHandler(context) { DateTimeUtcNow = () => DateTime.UtcNow };
                await sut.Handle(new UpdateMyVolunteerTasksCommand { VolunteerTaskSignups = volunteerTaskSignupViewModels });
            }

            using (var context = new AllReadyContext(options))
            {
                var signup1 = context.VolunteerTaskSignups.FirstOrDefault(x => x.Id == firstId);
                Assert.True(signup1 != null);
                var signup2 = context.VolunteerTaskSignups.FirstOrDefault(x => x.Id == secondId);
                Assert.True(signup2 != null);
            }
        }

        [Fact]
        public async Task InvokeUpdateTaskSignupAsyncWithTheCorrectParametersForEachTaskSignupViewModelOnCommand()
        {
            var options = CreateNewContextOptions();

            const string userId = "1";
            const int volunteerTaskSignupId = 1;
            var user = new ApplicationUser {Id = userId};
            var dateTimeUtcNow = DateTime.UtcNow;
            var taskSignupViewModels = new List<VolunteerTaskSignupViewModel>
            {
                new VolunteerTaskSignupViewModel { Id = volunteerTaskSignupId, StatusDescription = "statusDescription1", Status = "Accepted", VolunteerTaskId = 1 }
            };

            var message = new UpdateMyVolunteerTasksCommand { VolunteerTaskSignups = taskSignupViewModels, UserId = userId};

            using (var context = new AllReadyContext(options))
            {
                context.Users.Add(user);
                context.VolunteerTaskSignups.Add(new VolunteerTaskSignup { Id = volunteerTaskSignupId });
                context.VolunteerTasks.Add(new VolunteerTask { Id = 1 });
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new UpdateMyVolunteerTasksCommandHandler(context) { DateTimeUtcNow = () => dateTimeUtcNow };
                await sut.Handle(message);
            }

            using (var context = new AllReadyContext(options))
            {
                var signup = context.VolunteerTaskSignups.FirstOrDefault(x => x.Id == volunteerTaskSignupId);
                Assert.True(signup != null);
            }
        }
    }
}