using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.UnitTest.Features.Campaigns;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class MessageTaskVolunteersCommandHandlerAsyncTests : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var htb = new Organization()
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };

            var firePrev = new Campaign()
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb
            };

            var queenAnne = new Activity()
            {
                Id = 1,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { Id = 1 },
                RequiredSkills = new List<ActivitySkill>(),
            };

            var username1 = $"blah@1.com";
            var username2 = $"blah@2.com";

            var user1 = new ApplicationUser { UserName = username1, Email = username1, EmailConfirmed = true };
            context.Users.Add(user1);
            var user2 = new ApplicationUser { UserName = username2, Email = username2, EmailConfirmed = true };
            context.Users.Add(user2);

            htb.Campaigns.Add(firePrev);            
            context.Organizations.Add(htb);
            
            var task = new AllReadyTask()
            {
                Activity = queenAnne,
                Description = "Description of a very important task",
                Name = "Task # ",
                EndDateTime = DateTime.Now.AddDays(1),
                StartDateTime = DateTime.Now.AddDays(-3)
            };
            queenAnne.Tasks.Add(task);
            context.Activities.Add(queenAnne);

            var taskSignups = new List<TaskSignup>
            {
                new TaskSignup { Task = task, User = user1 },
                new TaskSignup { Task = task, User = user2 }
            };
            context.TaskSignups.AddRange(taskSignups);

            context.SaveChanges();
        }

        [Fact]
        public async Task SendMessageToAssignedVolunteers()
        {
            const string expectedMessage = "This is my message for all you task peeps";
            const string expectedSubject = "This is my subject";
            var command = new MessageTaskVolunteersCommandAsync
            {
                Model = new MessageTaskVolunteersModel
                {
                    TaskId = 1,
                    Message = expectedMessage,
                    Subject = expectedSubject
                }
            };

            var mediator = new Mock<IMediator>();
            
            var handler = new MessageTaskVolunteersCommandHandlerAsync(Context, mediator.Object);
            await handler.Handle(command);

            mediator.Verify(b => b.SendAsync(It.Is<NotifyVolunteersCommand>(notifyCommand =>
                   notifyCommand.ViewModel != null &&
                   notifyCommand.ViewModel.EmailMessage == expectedMessage &&
                   notifyCommand.ViewModel.Subject == expectedSubject &&
                   notifyCommand.ViewModel.EmailRecipients.Count == 2 &&
                   notifyCommand.ViewModel.EmailRecipients.Contains("blah@1.com") &&
                   notifyCommand.ViewModel.EmailRecipients.Contains("blah@2.com")

            )), Times.Once());   
        }
    }
}
