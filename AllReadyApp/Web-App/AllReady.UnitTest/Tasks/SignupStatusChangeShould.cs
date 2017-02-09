using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Tasks
{
    public class SignupStatusChangeShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var htb = new Organization
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };

            var firePrev = new Campaign
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb
            };

            var queenAnne = new Event
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

            const string username1 = "blah@1.com";

            var user1 = new ApplicationUser { UserName = username1, Email = username1, EmailConfirmed = true };
            Context.Users.Add(user1);

            htb.Campaigns.Add(firePrev);
            Context.Organizations.Add(htb);
            Context.Events.Add(queenAnne);

            var newTask = new VolunteerTask
            {
                Event = queenAnne,
                Description = "Description of a very important task",
                Name = "Task # 1",
                EndDateTime = DateTime.Now.AddDays(5),
                StartDateTime = DateTime.Now.AddDays(3),
                Organization = htb
            };

            newTask.AssignedVolunteers.Add(new VolunteerTaskSignup
            {
                VolunteerTask = newTask,
                User = user1
            });

            Context.VolunteerTasks.Add(newTask);

            Context.SaveChanges();
        }

        [Fact]
        public async Task VolunteerAcceptsTask()
        {
            var mediator = new Mock<IMediator>();

            var volunteerTask = Context.VolunteerTasks.First();
            var user = Context.Users.First();
            var command = new ChangeVolunteerTaskStatusCommand
            {
                VolunteerTaskId = volunteerTask.Id,
                UserId = user.Id,
                VolunteerTaskStatus = VolunteerTaskStatus.Accepted
            };
            var handler = new ChangeVolunteerTaskStatusCommandHandler(Context, mediator.Object);
            await handler.Handle(command);

            var volunteerTaskSignup = Context.VolunteerTaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<VolunteerTaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == volunteerTaskSignup.Id)), Times.Once());
        }
    }
}
