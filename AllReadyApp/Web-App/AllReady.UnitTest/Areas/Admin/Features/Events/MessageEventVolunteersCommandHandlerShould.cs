using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.ViewModels.Event;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Events
{
    public class MessageEventVolunteersCommandHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
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

            var queenAnne = new Event()
            {
                Id = 1,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { Id = 1 },
                RequiredSkills = new List<EventSkill>(),
            };

            var username1 = $"blah@1.com";
            var username2 = $"blah@2.com";

            var user1 = new ApplicationUser { UserName = username1, Email = username1, EmailConfirmed = true };
            Context.Users.Add(user1);
            var user2 = new ApplicationUser { UserName = username2, Email = username2, EmailConfirmed = true };
            Context.Users.Add(user2);

            var volunteerTask = new VolunteerTask
            {
                Id = 1,
                Name = "Task 1",
                Event = queenAnne,
            };

            var volunteerTaskSignup = new VolunteerTaskSignup
            {
                Id = 1,
                User = user1,
                VolunteerTask = volunteerTask
            };

            htb.Campaigns.Add(firePrev);            
            Context.Organizations.Add(htb);
            Context.Events.Add(queenAnne);
            Context.VolunteerTasks.Add(volunteerTask);
            Context.VolunteerTaskSignups.Add(volunteerTaskSignup);

            Context.SaveChanges();
        }

        [Fact]
        public async Task SendMessageToAssignedVolunteers()
        {
            var command = new MessageEventVolunteersCommand
            {
                ViewModel = new MessageEventVolunteersViewModel
                {
                    EventId = 1,
                    Message = "This is my message",
                    Subject = "This is my subject"
                }
            };

            var mediator = new Mock<IMediator>();
            
            var handler = new MessageEventVolunteersCommandHandler(Context, mediator.Object);
            await handler.Handle(command);

            mediator.Verify(b => b.SendAsync(It.Is<NotifyVolunteersCommand>(notifyCommand =>
                   notifyCommand.ViewModel != null &&
                   notifyCommand.ViewModel.EmailMessage == "This is my message" &&
                   notifyCommand.ViewModel.Subject == "This is my subject" &&
                   notifyCommand.ViewModel.EmailRecipients.Count == 1 &&
                   notifyCommand.ViewModel.EmailRecipients.Contains("blah@1.com")

            )), Times.Once());
        }
    }
}