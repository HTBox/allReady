using System.Security.Claims;
using AllReady.Features.Event;
using AllReady.Models;
using Microsoft.AspNetCore.Identity;
using Moq;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class ShowEventQueryHandlerShould
    {
        [Fact]
        public void InvokeGetEventWithTheCorrectEventId()
        {
            var message = new ShowEventQuery { EventId = 1 };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new ShowEventQueryHandler(dataAccess.Object, null, null);
            sut.Handle(message);

            dataAccess.Verify(x => x.GetEvent(message.EventId), Times.Once);
        }

        [Fact]
        public void ReturnNullWhenEventIsNotFound()
        {
            var showEventQuery = new ShowEventQuery { EventId = 1 };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetEvent(showEventQuery.EventId)).Returns<Models.Event>(null);

            var sut = new ShowEventQueryHandler(dataAccess.Object, null, null);
            var viewModel = sut.Handle(showEventQuery);

            viewModel.ShouldBeNull();
        }

        [Fact]
        public void ReturnNullWhenEventsCampaignIslocked()
        {
            var query = new ShowEventQuery { EventId = 1 };
            var mockDbAccess = new Mock<IAllReadyDataAccess>();
            var expectedEvent = new Models.Event { Campaign = new Campaign { Locked = true } };
            mockDbAccess.Setup(x => x.GetEvent(query.EventId)).Returns(expectedEvent);

            var sut = new ShowEventQueryHandler(mockDbAccess.Object, null, null);
            var viewModel = sut.Handle(query);

            viewModel.ShouldBeNull();
        }

        [Fact]
        public void InvokeGetUserIdWithTheCorrectUser_WhenEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            var message = new ShowEventQuery { EventId = 1, User = new ClaimsPrincipal() };
            var @event = new Models.Event { Campaign = new Campaign { Locked = false } };

            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetEvent(message.EventId)).Returns(@event);
            dataAccess.Setup(x => x.GetUser(It.IsAny<string>())).Returns(new ApplicationUser());

            var userManager = CreateUserManagerMock();

            var sut = new ShowEventQueryHandler(dataAccess.Object, userManager.Object, null);
            sut.Handle(message);

            userManager.Verify(x => x.GetUserId(message.User), Times.Once);
        }

        [Fact(Skip = "NotImplemented")]
        public void InvokeGetUserWithTheCorrectUserId_WhenEventIsNotNullAndEventsCampaignIsUnlocked()
        {
        }

        //TODO: tests for this line: eventViewModel.UserSkills = appUser?.AssociatedSkills?.Select(us => new SkillViewModel(us.Skill)).ToList();
        [Fact(Skip = "NotImplemented")]
        public void AssignUserSkillsToNull_WhenEventIsNotNullAndEventsCampaignIsUnlocked()
        {
            //var userManager = CreateUserManagerMock();
            //userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Failed()));
            //userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));
            //userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).Returns(() => Task.FromResult(It.IsAny<string>()));
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignUserSkillsToAppUsersAssociatedSkills_WhenAppUserIsNotNull_AndEventIsNotNullAndEventsCampaignIsUnlocked()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void InvokeGetEventSignupsWithCorrectParameters_WhenEventIsNotNullAndEventsCampaignIsUnlocked()
        {
        }

        //var assignedTasks = @event.Tasks.Where(t => t.AssignedVolunteers.Any(au => au.User.Id == userId)).ToList();

        //eventViewModel.UserTasks = new List<TaskViewModel>(assignedTasks.Select(data => new TaskViewModel(data, userId)).OrderBy(task => task.StartDateTime));

        //var unassignedTasks = @event.Tasks.Where(t => t.AssignedVolunteers.All(au => au.User.Id != userId)).ToList();

        //eventViewModel.Tasks = new List<TaskViewModel>(unassignedTasks.Select(data => new TaskViewModel(data, userId)).OrderBy(task => task.StartDateTime));

        private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            return new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        }
    }
}
