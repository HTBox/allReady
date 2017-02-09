using System.Collections.Generic;
using AllReady.Configuration;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Notifications
{
    public class NotifyAdminForTaskSignupStatusChangeHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async void PassANotifyVolunteersCommandToTheMediatorWhenEmailIsProvided()
        {
            const int volunteerTaskSignupId = 1001;
            const int volunteerTaskId = 12314;
            const string email = "kmad@allready.com";
            const string userMail = "damk@allready.com";

            var mediator = new Mock<IMediator>();

            var options = new Mock<IOptions<GeneralSettings>>();
            options.Setup(o => o.Value).Returns(new GeneralSettings {SiteBaseUrl = "allready.com"});

            var notification = new VolunteerTaskSignupStatusChanged {SignupId = volunteerTaskSignupId};

            var context = Context;
            var volunteerTaskSignup = CreateTaskSignup(volunteerTaskSignupId, volunteerTaskId, email, userMail: userMail);
            context.VolunteerTaskSignups.Add(volunteerTaskSignup);
            context.SaveChanges();

            var target = new NotifyAdminForVolunteerTaskSignupStatusChangeHandler(context, mediator.Object, options.Object);
            await target.Handle(notification);

            mediator.VerifyAll();
        }

        [Fact]
        public async void UsesUserFirstAndLastNameAsSubjectWhenProvided()
        {
            const int volunteerTaskSignupId = 1001;
            const int volunteerTaskId = 12314;
            const string email = "kmad@allready.com";
            const string firstName = "Simon";
            const string lastName = "Says";
            const string userEmail = "damk@allready.com";

            var mediator = new Mock<IMediator>();

            var options = new Mock<IOptions<GeneralSettings>>();
            options.Setup(o => o.Value).Returns(new GeneralSettings { SiteBaseUrl = "allready.com" });

            var notification = new VolunteerTaskSignupStatusChanged { SignupId = volunteerTaskSignupId };

            var volunteerTaskSignup = CreateTaskSignup(volunteerTaskSignupId, volunteerTaskId, email, firstName, lastName, userEmail);
            Context.VolunteerTaskSignups.Add(volunteerTaskSignup);
            Context.SaveChanges();

            var target = new NotifyAdminForVolunteerTaskSignupStatusChangeHandler(Context, mediator.Object, options.Object);
            await target.Handle(notification);

            mediator.Verify(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(y => y.ViewModel.Subject == $"{firstName} {lastName}")));
        }

        [Fact]
        public async void NotPassANotifyVolunteersCommandToTheMediatorWhenEmailIsEmpty()
        {
            const int volunteerTaskSignupId = 1001;
            const int volunteerTaskId = 12314;
            const string email = "";

            var mediator = new Mock<IMediator>();
            var options = new Mock<IOptions<GeneralSettings>>();

            var notification = new VolunteerTaskSignupStatusChanged { SignupId = volunteerTaskSignupId };

            var context = Context;
            var volunteerTaskSignup = CreateTaskSignup(volunteerTaskSignupId, volunteerTaskId, email);
            context.VolunteerTaskSignups.Add(volunteerTaskSignup);
            context.SaveChanges();

            var target = new NotifyAdminForVolunteerTaskSignupStatusChangeHandler(context, mediator.Object, options.Object);
            await target.Handle(notification);

            mediator.Verify(m => m.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }

        [Fact]
        public async void NotPassANotifyVolunteersCommandToTheMediatorWhenCampaignContactIsNull()
        {
            const int volunteerTaskSignupId = 1001;
            const int volunteerTaskId = 12314;

            var mediator = new Mock<IMediator>();
            var options = new Mock<IOptions<GeneralSettings>>();

            var notification = new VolunteerTaskSignupStatusChanged { SignupId = volunteerTaskSignupId };

            var context = Context;
            var volunteerTaskSignup = CreateTaskSignupWithoutCampaignContact(volunteerTaskSignupId, volunteerTaskId);
            context.VolunteerTaskSignups.Add(volunteerTaskSignup);
            context.SaveChanges();

            var target = new NotifyAdminForVolunteerTaskSignupStatusChangeHandler(context, mediator.Object, options.Object);
            await target.Handle(notification);

            mediator.Verify(m => m.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }

        [Fact]
        public async void NotPassANotifyVolunteersCommandToTheMediatorWhenCampaignContactsIsNull()
        {
            const int volunteerTaskSignupId = 1001;
            const int volunteerTaskId = 12314;

            var mediator = new Mock<IMediator>();
            var options = new Mock<IOptions<GeneralSettings>>();

            var notification = new VolunteerTaskSignupStatusChanged { SignupId = volunteerTaskSignupId };

            var context = Context;
            var volunteerTaskSignup = CreateTaskSignupWithoutCampaignContacts(volunteerTaskSignupId, volunteerTaskId);
            context.VolunteerTaskSignups.Add(volunteerTaskSignup);
            context.SaveChanges();

            var target = new NotifyAdminForVolunteerTaskSignupStatusChangeHandler(context, mediator.Object, options.Object);
            await target.Handle(notification);

            mediator.Verify(m => m.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }

        private static VolunteerTaskSignup CreateTaskSignup(int volunteerTaskSignupId, int volunteerTaskId , string email, string firstName = null, string lastName = null, string userMail = null)
        {
            var volunteerTaskSignup = new VolunteerTaskSignup
            {
                Id = volunteerTaskSignupId,
                User = new ApplicationUser
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = userMail
                },
                VolunteerTask = new VolunteerTask
                {
                    Id = volunteerTaskId,
                    Event = new AllReady.Models.Event
                    {
                        Campaign = new Campaign
                        {
                            CampaignContacts = new List<CampaignContact>
                            {
                                new CampaignContact
                                {
                                    ContactType = (int) ContactTypes.Primary,
                                    Contact = new Contact { Email = email }
                                }
                            }
                        }
                    }
                }
            };

            return volunteerTaskSignup;
        }

        private static VolunteerTaskSignup CreateTaskSignupWithoutCampaignContact(int volunteerTaskSignupId, int volunteerTaskId, string firstName = null, string lastName = null, string userMail = null)
        {
            var volunteerTaskSignup = CreateTaskSignup(volunteerTaskSignupId, volunteerTaskId, null, firstName, lastName, userMail);
            volunteerTaskSignup.VolunteerTask.Event.Campaign.CampaignContacts = new List<CampaignContact>();
            return volunteerTaskSignup;
        }

        private static VolunteerTaskSignup CreateTaskSignupWithoutCampaignContacts(int volunteerTaskSignupId, int volunteerTaskId, string firstName = null, string lastName = null, string userMail = null)
        {
            var volunteerTaskSignup = CreateTaskSignup(volunteerTaskSignupId, volunteerTaskId, null, firstName, lastName, userMail);
            volunteerTaskSignup.VolunteerTask.Event.Campaign = new Campaign();
            return volunteerTaskSignup;
        }
    }
}
