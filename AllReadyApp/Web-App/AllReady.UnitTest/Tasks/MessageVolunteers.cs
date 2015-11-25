using AllReady.Areas.Admin.Features.Activities;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllReady.UnitTest.Tasks
{
    public class MessageVolunteers : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            Tenant htb = new Tenant()
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };
            Campaign firePrev = new Campaign()
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingTenant = htb
            };
            Activity queenAnne = new Activity()
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
            context.Tenants.Add(htb);
            
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


            var taskSignups = new List<TaskSignup>();
            taskSignups.Add(new TaskSignup() { Task = task, User = user1 });
            taskSignups.Add(new TaskSignup() { Task = task, User = user2 });
            context.TaskSignups.AddRange(taskSignups);

            context.SaveChanges();
        }

        [Fact]
        public void SendMessageToAssignedVolunteers()
        {
            var expectedMessage = "This is my message for all you task peeps";
            var expectedSubject = "This is my subject";
            var command = new MessageTaskVolunteersCommand
            {
                Model = new MessageTaskVolunteersModel
                {
                    TaskId = 1,
                    Message = expectedMessage,
                    Subject = expectedSubject
                }
            };

            var bus = new Mock<IMediator>();
            
            
            var handler = new MessageTaskVolunteersCommandHandler(Context, bus.Object);
            var result = handler.Handle(command);

            bus.Verify(b => b.Send(It.Is<NotifyVolunteersCommand>(notifyCommand =>
                   notifyCommand.ViewModel != null &&
                   notifyCommand.ViewModel.EmailMessage == expectedMessage &&
                   notifyCommand.ViewModel.Subject == expectedSubject &&
                   notifyCommand.ViewModel.EmailRecipients.Count() == 2 &&
                   notifyCommand.ViewModel.EmailRecipients.Contains("blah@1.com") &&
                   notifyCommand.ViewModel.EmailRecipients.Contains("blah@2.com")

            )), Times.Once());
            
        }
    }
}
