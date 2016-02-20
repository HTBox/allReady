using AllReady.Areas.Admin.Features.Activities;
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

namespace AllReady.UnitTest.Activities
{
    public class MessageVolunteers : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            Organization htb = new Organization()
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };
            Campaign firePrev = new Campaign()
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb
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
            context.Organizations.Add(htb);
            context.Activities.Add(queenAnne);
            
            var activitySignups = new List<ActivitySignup>();
            activitySignups.Add(new ActivitySignup { Activity = queenAnne, User = user1, SignupDateTime = DateTime.UtcNow });
            activitySignups.Add(new ActivitySignup { Activity = queenAnne, User = user2, SignupDateTime = DateTime.UtcNow });

            context.ActivitySignup.AddRange(activitySignups);
            context.SaveChanges();
        }

        [Fact]
        public void SendMessageToAssignedVolunteers()
        {
            var command = new MessageActivityVolunteersCommand
            {
                Model = new MessageActivityVolunteersModel
                {
                    ActivityId = 1,
                    Message = "This is my message",
                    Subject = "This is my subject"
                }
            };

            var bus = new Mock<IMediator>();
            
            
            var handler = new MessageActivityVolunteersCommandHandler(Context, bus.Object);
            var result = handler.Handle(command);

            bus.Verify(b => b.SendAsync(It.Is<NotifyVolunteersCommand>(notifyCommand =>
                   notifyCommand.ViewModel != null &&
                   notifyCommand.ViewModel.EmailMessage == "This is my message" &&
                   notifyCommand.ViewModel.Subject == "This is my subject" &&
                   notifyCommand.ViewModel.EmailRecipients.Count() == 2 &&
                   notifyCommand.ViewModel.EmailRecipients.Contains("blah@1.com") &&
                   notifyCommand.ViewModel.EmailRecipients.Contains("blah@2.com")

            )), Times.Once());
            
        }
    }
}
