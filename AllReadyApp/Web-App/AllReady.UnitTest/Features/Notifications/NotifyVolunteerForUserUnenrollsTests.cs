using AllReady.Areas.Admin.Models;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Notifications
{
    public class NotifyVolunteerForUserUnenrollsShould
    {
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IOptions<GeneralSettings>> _generalSettings;
        private readonly NotifyVolunteerForUserUnenrolls _handler;

        public NotifyVolunteerForUserUnenrollsShould()
        {
            _mediator = new Mock<IMediator>();
            _generalSettings = new Mock<IOptions<GeneralSettings>>();
            _generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());
            _handler = new NotifyVolunteerForUserUnenrolls(_mediator.Object, _generalSettings.Object);
        }

        private EventDetailForNotificationModel GetEventDetailForNotificationModel()
        {
            var volunteer1 = new ApplicationUser
            {
                Id = "volunteer1@example.com",
                UserName = "volunteer1@example.com",
                FirstName = "volunteer1",
                LastName = "volunteer1_LastName",
                Email = "volunteer1@example.com"
            };

            var volunteer2 = new ApplicationUser
            {
                Id = "voluntee2@example.com",
                UserName = "volunteer2@example.com",
                FirstName = "volunteer2_LastName",
                LastName = "volunteer2",
                Email = "volunteer2@example.com"
            };

            var organization1 = new Organization
            {
                Id = 1,
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };

            var campaign1 = new Campaign
            {
                Id = 1,
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = organization1,
                CampaignContacts = new List<CampaignContact>()
            };

            organization1.Campaigns.Add(campaign1);

            var contact1 = new Contact
            {
                Id = 1,
                FirstName = "contact1",
                LastName = "contact1",
                Email = "contact1@example.com",
                PhoneNumber = "111-111-1111"
            };

            var campaignContact1 = new CampaignContact
            {
                CampaignId = campaign1.Id,
                Campaign = campaign1,
                ContactId = contact1.Id,
                Contact = contact1,
                ContactType = (int)ContactTypes.Primary
            };

            campaign1.CampaignContacts.Add(campaignContact1);

            var campaignEvent1 = new AllReady.Models.Event
            {
                Id = 1,
                EventType = EventType.Rally,
                Name = "Queen Anne Fire Prevention Day",
                Description = "Queen Anne Fire Prevention Day Description",
                Campaign = campaign1,
                CampaignId = campaign1.Id,
                NumberOfVolunteersRequired = 2,
                StartDateTime = DateTime.UtcNow.Date.AddMonths(-1),
                EndDateTime = DateTime.UtcNow.Date.AddMonths(2),
                Location = new Location { Id = 1 },
                UsersSignedUp = new List<EventSignup>(),
                Tasks = new List<AllReadyTask>()
            };

            var eventSignup1 = new EventSignup { Event = campaignEvent1, User = volunteer1, SignupDateTime = DateTime.UtcNow.AddDays(-7) };
            var eventSignup2 = new EventSignup { Event = campaignEvent1, User = volunteer2, SignupDateTime = DateTime.UtcNow.AddDays(-14) };
            campaignEvent1.UsersSignedUp.AddRange(new List<EventSignup> { eventSignup1, eventSignup2 });

            var task1 = new AllReadyTask
            {
                Event = campaignEvent1,
                Name = "Task # 1",
                Description = "Description of a very important task",
                StartDateTime = DateTime.Now.AddDays(3),
                EndDateTime = DateTime.Now.AddDays(5),
                NumberOfVolunteersRequired = 2,
                Organization = organization1,
                AssignedVolunteers = new List<TaskSignup>()
            };

            var taskSignup1 = new TaskSignup { Task = task1, User = volunteer1 };
            var taskSignup2 = new TaskSignup { Task = task1, User = volunteer2 };

            task1.AssignedVolunteers.AddRange(new List<TaskSignup> { taskSignup1, taskSignup2 });

            campaignEvent1.Tasks.Add(task1);

            var result = new EventDetailForNotificationModel
            {
                EventId = campaignEvent1.Id,
                EventType = campaignEvent1.EventType,
                CampaignName = campaignEvent1.Campaign.Name,
                CampaignContacts = campaignEvent1.Campaign.CampaignContacts,
                Volunteer = volunteer1,
                EventName = campaignEvent1.Name,
                Description = campaignEvent1.Description,
                UsersSignedUp = campaignEvent1.UsersSignedUp,
                NumberOfVolunteersRequired = campaignEvent1.NumberOfVolunteersRequired,
                Tasks = campaignEvent1.Tasks.Select(t => new TaskSummaryModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    StartDateTime = t.StartDateTime,
                    EndDateTime = t.EndDateTime,
                    NumberOfVolunteersRequired = t.NumberOfVolunteersRequired,
                    AssignedVolunteers = t.AssignedVolunteers.Select(assignedVolunteer => new VolunteerModel
                    {
                        UserId = assignedVolunteer.User.Id,
                        UserName = assignedVolunteer.User.UserName,
                        HasVolunteered = true,
                        Status = assignedVolunteer.Status,
                        PreferredEmail = assignedVolunteer.PreferredEmail,
                        PreferredPhoneNumber = assignedVolunteer.PreferredPhoneNumber,
                        AdditionalInfo = assignedVolunteer.AdditionalInfo
                    }).ToList()
                }).OrderBy(t => t.StartDateTime).ThenBy(t => t.Name).ToList(),
            };

            return result;
        }

        private UserUnenrolls GetNotification(string userId)
        {
            return new UserUnenrolls
            {
                UserId = userId,
                EventId = 1,
                TaskIds = new List<int> { 1 }
            };
        }

        [Fact]
        public async Task NotifyVolunteerForUserUnenrolls_SentToPreferredEmail()
        {
            var userId = "volunteer1@example.com";
            var preferredEmail = "volunteer1_preferred@example.com";
            NotifyVolunteersCommand command = null;

            var notification = GetNotification(userId);
            var model = GetEventDetailForNotificationModel();

            model.UsersSignedUp.First(u => u.User.Id == userId).PreferredEmail = preferredEmail;

            _mediator.Setup(m => m.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);
            _mediator.Setup(m => m.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .Returns((NotifyVolunteersCommand c) =>
                {
                    command = c;
                    return Task.FromResult(new Unit());
                });

            await _handler.Handle(notification);

            command.ShouldNotBeNull();
            command.ViewModel.ShouldNotBeNull();
            command.ViewModel.EmailRecipients.ShouldNotBeNull();
            command.ViewModel.EmailRecipients.Count.ShouldBe(1);
            command.ViewModel.EmailRecipients.ShouldNotContain(userId);
            command.ViewModel.EmailRecipients.ShouldContain(preferredEmail);
            command.ViewModel.Subject.ShouldNotBeNullOrEmpty();
            command.ViewModel.Subject.ShouldBe("allReady Event Un-enrollment Confirmation");
            command.ViewModel.EmailMessage.ShouldNotBeNullOrEmpty();
            command.ViewModel.EmailMessage.ShouldContain(model.CampaignName);
            command.ViewModel.EmailMessage.ShouldContain(model.EventName);
            command.ViewModel.HtmlMessage.ShouldNotBeNullOrEmpty();
            command.ViewModel.HtmlMessage.ShouldBe<string>(command.ViewModel.EmailMessage);
        }

        [Fact]
        public async Task NotifyVolunteerForUserUnenrolls_SentToUserEmail_PreferredEmailIsNull()
        {
            var userId = "volunteer1@example.com";
            NotifyVolunteersCommand command = null;

            var notification = GetNotification(userId);
            var model = GetEventDetailForNotificationModel();

            model.UsersSignedUp.First(u => u.User.Id == userId).PreferredEmail = null;

            _mediator.Setup(m => m.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);
            _mediator.Setup(m => m.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .Returns((NotifyVolunteersCommand c) =>
                {
                    command = c;
                    return Task.FromResult(new Unit());
                });

            await _handler.Handle(notification);

            command.ShouldNotBeNull();
            command.ViewModel.ShouldNotBeNull();
            command.ViewModel.EmailRecipients.ShouldNotBeNull();
            command.ViewModel.EmailRecipients.Count.ShouldBe(1);
            command.ViewModel.EmailRecipients.ShouldContain(userId);
            command.ViewModel.Subject.ShouldNotBeNullOrEmpty();
            command.ViewModel.Subject.ShouldBe("allReady Event Un-enrollment Confirmation");
            command.ViewModel.EmailMessage.ShouldNotBeNullOrEmpty();
            command.ViewModel.EmailMessage.ShouldContain(model.CampaignName);
            command.ViewModel.EmailMessage.ShouldContain(model.EventName);
            command.ViewModel.HtmlMessage.ShouldNotBeNullOrEmpty();
            command.ViewModel.HtmlMessage.ShouldBe<string>(command.ViewModel.EmailMessage);
        }

        [Fact]
        public async Task NotifyVolunteerForUserUnenrolls_SentToUserEmail_PreferredEmailIsEmpty()
        {
            var userId = "volunteer1@example.com";
            NotifyVolunteersCommand command = null;

            var notification = GetNotification(userId);
            var model = GetEventDetailForNotificationModel();

            model.UsersSignedUp.First(u => u.User.Id == userId).PreferredEmail = string.Empty;

            _mediator.Setup(m => m.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);
            _mediator.Setup(m => m.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .Returns((NotifyVolunteersCommand c) =>
                {
                    command = c;
                    return Task.FromResult(new Unit());
                });

            await _handler.Handle(notification);

            command.ShouldNotBeNull();
            command.ViewModel.ShouldNotBeNull();
            command.ViewModel.EmailRecipients.ShouldNotBeNull();
            command.ViewModel.EmailRecipients.Count.ShouldBe(1);
            command.ViewModel.EmailRecipients.ShouldContain(userId);
            command.ViewModel.Subject.ShouldNotBeNullOrEmpty();
            command.ViewModel.Subject.ShouldBe("allReady Event Un-enrollment Confirmation");
            command.ViewModel.EmailMessage.ShouldNotBeNullOrEmpty();
            command.ViewModel.EmailMessage.ShouldContain(model.CampaignName);
            command.ViewModel.EmailMessage.ShouldContain(model.EventName);
            command.ViewModel.HtmlMessage.ShouldNotBeNullOrEmpty();
            command.ViewModel.HtmlMessage.ShouldBe<string>(command.ViewModel.EmailMessage);
        }

        [Fact]
        public async Task NotifyVolunteerForUserUnenrolls_SentToUserEmail_PreferredEmailIsWhiteSpace()
        {
            var userId = "volunteer1@example.com";
            NotifyVolunteersCommand command = null;

            var notification = GetNotification(userId);
            var model = GetEventDetailForNotificationModel();

            model.UsersSignedUp.First(u => u.User.Id == userId).PreferredEmail = "   ";

            _mediator.Setup(m => m.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);
            _mediator.Setup(m => m.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .Returns((NotifyVolunteersCommand c) =>
                {
                    command = c;
                    return Task.FromResult(new Unit());
                });

            await _handler.Handle(notification);

            command.ShouldNotBeNull();
            command.ViewModel.ShouldNotBeNull();
            command.ViewModel.EmailRecipients.ShouldNotBeNull();
            command.ViewModel.EmailRecipients.Count.ShouldBe(1);
            command.ViewModel.EmailRecipients.ShouldContain(userId);
            command.ViewModel.Subject.ShouldNotBeNullOrEmpty();
            command.ViewModel.Subject.ShouldBe("allReady Event Un-enrollment Confirmation");
            command.ViewModel.EmailMessage.ShouldNotBeNullOrEmpty();
            command.ViewModel.EmailMessage.ShouldContain(model.CampaignName);
            command.ViewModel.EmailMessage.ShouldContain(model.EventName);
            command.ViewModel.HtmlMessage.ShouldNotBeNullOrEmpty();
            command.ViewModel.HtmlMessage.ShouldBe<string>(command.ViewModel.EmailMessage);
        }

        [Fact]
        public async Task NotifyVolunteerForUserUnenrolls_NotSent_UserEmailIsNull()
        {
            var userId = "volunteer1@example.com";
            NotifyVolunteersCommand command = null;

            var notification = GetNotification(userId);
            var model = GetEventDetailForNotificationModel();

            model.Volunteer.Email = null;

            _mediator.Setup(m => m.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);
            _mediator.Setup(m => m.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .Returns((NotifyVolunteersCommand c) =>
                {
                    command = c;
                    return Task.FromResult(new Unit());
                });

            await _handler.Handle(notification);

            command.ShouldBeNull();
        }

        [Fact]
        public async Task NotifyVolunteerForUserUnenrolls_NotSent_UserEmailIsEmpty()
        {
            var userId = "volunteer1@example.com";
            NotifyVolunteersCommand command = null;

            var notification = GetNotification(userId);
            var model = GetEventDetailForNotificationModel();

            model.Volunteer.Email = string.Empty;

            _mediator.Setup(m => m.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);
            _mediator.Setup(m => m.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .Returns((NotifyVolunteersCommand c) =>
                {
                    command = c;
                    return Task.FromResult(new Unit());
                });

            await _handler.Handle(notification);

            command.ShouldBeNull();
        }

        [Fact]
        public async Task NotifyVolunteerForUserUnenrolls_NotSent_UserEmailIsWhiteSpace()
        {
            var userId = "volunteer1@example.com";
            NotifyVolunteersCommand command = null;

            var notification = GetNotification(userId);
            var model = GetEventDetailForNotificationModel();

            model.Volunteer.Email = "  ";

            _mediator.Setup(m => m.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);
            _mediator.Setup(m => m.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .Returns((NotifyVolunteersCommand c) =>
                {
                    command = c;
                    return Task.FromResult(new Unit());
                });

            await _handler.Handle(notification);

            command.ShouldBeNull();
        }

        [Fact]
        public async Task NotifyVolunteerForUserUnenrolls_NotSent_UserNotSignedUp()
        {
            var userId = "volunteer1@example.com";
            NotifyVolunteersCommand command = null;

            var notification = GetNotification(userId);
            var model = GetEventDetailForNotificationModel();

            model.UsersSignedUp.RemoveAll(u => u.User.Id == model.Volunteer.Id);

            _mediator.Setup(m => m.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);
            _mediator.Setup(m => m.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .Returns((NotifyVolunteersCommand c) =>
                {
                    command = c;
                    return Task.FromResult(new Unit());
                });

            await _handler.Handle(notification);

            command.ShouldBeNull();
        }

        //[Fact]
        //public void NotifyVolunteerForUserUnenrolls_NotSent_EventIsNull()
        //{
        //    var userId = "volunteer1@example.com";
        //    NotifyVolunteersCommand command = null;

        //    var notification = GetNotification(userId);
        //    EventDetailForNotificationModel model = null;

        //    _mediator.Setup(m => m.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);
        //    _mediator.Setup(m => m.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
        //        .Returns((NotifyVolunteersCommand c) => {
        //            command = c;
        //            return Task.FromResult(new Unit());
        //        });

        //    Should.Throw<NullReferenceException>(async() => await _handler.Handle(notification));            
        //}

        [Fact]
        public async Task NotifyVolunteerForUserUnenrolls_NotSent_EventIsEmpty()
        {
            var userId = "volunteer1@example.com";
            NotifyVolunteersCommand command = null;

            var notification = GetNotification(userId);
            var model = new EventDetailForNotificationModel();

            _mediator.Setup(m => m.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);
            _mediator.Setup(m => m.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .Returns((NotifyVolunteersCommand c) =>
                {
                    command = c;
                    return Task.FromResult(new Unit());
                });

            await _handler.Handle(notification);

            command.ShouldBeNull();
        }


        [Fact]
        public async Task SendEventDetailForNotificationQueryAsyncWithCorrectParameters()
        {
            var notification = new UserUnenrolls
            {
                UserId = "user1@example.com",
                EventId = 111
            };

            var model = new EventDetailForNotificationModel();

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);

            var mockSettings = new Mock<IOptions<GeneralSettings>>();
            mockSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var handler = new NotifyVolunteerForUserUnenrolls(mockMediator.Object, mockSettings.Object);
            await handler.Handle(notification);

            mockMediator.Verify(x => x.SendAsync(It.Is<EventDetailForNotificationQueryAsync>(n => n.EventId == notification.EventId && n.UserId == notification.UserId)), Times.Once);
        }
    }
}
