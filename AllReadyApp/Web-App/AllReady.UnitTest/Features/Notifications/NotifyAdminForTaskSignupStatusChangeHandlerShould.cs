using System.Collections.Generic;
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
            const int taskSignupId = 1001;
            const int taskId = 12314;
            const string email = "kmad@allready.com";
            const string userMail = "damk@allready.com";

            var mediator = new Mock<IMediator>();

            var options = new Mock<IOptions<GeneralSettings>>();
            options.Setup(o => o.Value).Returns(new GeneralSettings {SiteBaseUrl = "allready.com"});

            var notification = new TaskSignupStatusChanged {SignupId = taskSignupId};

            var context = Context;
            var taskSignup = CreateTaskSignup(taskSignupId, taskId, email, userMail: userMail);
            context.TaskSignups.Add(taskSignup);
            context.SaveChanges();

            var target = new NotifyAdminForTaskSignupStatusChangeHandler(context, mediator.Object, options.Object);
            await target.Handle(notification);

            mediator.VerifyAll();
        }

        [Fact]
        public async void UsesUserFirstAndLastNameAsSubjectWhenProvided()
        {
            const int taskSignupId = 1001;
            const int taskId = 12314;
            const string email = "kmad@allready.com";
            const string firstName = "Simon";
            const string lastName = "Says";
            const string userEmail = "damk@allready.com";

            var mediator = new Mock<IMediator>();

            var options = new Mock<IOptions<GeneralSettings>>();
            options.Setup(o => o.Value).Returns(new GeneralSettings { SiteBaseUrl = "allready.com" });

            var notification = new TaskSignupStatusChanged { SignupId = taskSignupId };

            var taskSignup = CreateTaskSignup(taskSignupId, taskId, email, firstName, lastName, userEmail);
            Context.TaskSignups.Add(taskSignup);
            Context.SaveChanges();

            var target = new NotifyAdminForTaskSignupStatusChangeHandler(Context, mediator.Object, options.Object);
            await target.Handle(notification);

            mediator.Verify(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(y => y.ViewModel.Subject == $"{firstName} {lastName}")));
        }

        [Fact]
        public async void NotPassANotifyVolunteersCommandToTheMediatorWhenEmailIsEmpty()
        {
            const int taskSignupId = 1001;
            const int taskId = 12314;
            const string email = "";

            var mediator = new Mock<IMediator>();
            var options = new Mock<IOptions<GeneralSettings>>();

            var notification = new TaskSignupStatusChanged { SignupId = taskSignupId };

            var context = Context;
            var taskSignup = CreateTaskSignup(taskSignupId, taskId, email);
            context.TaskSignups.Add(taskSignup);
            context.SaveChanges();

            var target = new NotifyAdminForTaskSignupStatusChangeHandler(context, mediator.Object, options.Object);
            await target.Handle(notification);

            mediator.Verify(m => m.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }

        [Fact]
        public async void NotPassANotifyVolunteersCommandToTheMediatorWhenCampaignContactIsNull()
        {
            const int taskSignupId = 1001;
            const int taskId = 12314;

            var mediator = new Mock<IMediator>();
            var options = new Mock<IOptions<GeneralSettings>>();

            var notification = new TaskSignupStatusChanged { SignupId = taskSignupId };

            var context = Context;
            var taskSignup = CreateTaskSignupWithoutCampaignContact(taskSignupId, taskId);
            context.TaskSignups.Add(taskSignup);
            context.SaveChanges();

            var target = new NotifyAdminForTaskSignupStatusChangeHandler(context, mediator.Object, options.Object);
            await target.Handle(notification);

            mediator.Verify(m => m.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }

        [Fact]
        public async void NotPassANotifyVolunteersCommandToTheMediatorWhenCampaignContactsIsNull()
        {
            const int taskSignupId = 1001;
            const int taskId = 12314;

            var mediator = new Mock<IMediator>();
            var options = new Mock<IOptions<GeneralSettings>>();

            var notification = new TaskSignupStatusChanged { SignupId = taskSignupId };

            var context = Context;
            var taskSignup = CreateTaskSignupWithoutCampaignContacts(taskSignupId, taskId);
            context.TaskSignups.Add(taskSignup);
            context.SaveChanges();

            var target = new NotifyAdminForTaskSignupStatusChangeHandler(context, mediator.Object, options.Object);
            await target.Handle(notification);

            mediator.Verify(m => m.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }

        private static TaskSignup CreateTaskSignup(int taskSignupId, int taskId , string email, string firstName = null, string lastName = null, string userMail = null)
        {
            var taskSignup = new TaskSignup
            {
                Id = taskSignupId,
                User = new ApplicationUser
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = userMail
                },
                Task = new AllReadyTask
                {
                    Id = taskId,
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

            return taskSignup;
        }

        private static TaskSignup CreateTaskSignupWithoutCampaignContact(int taskSignupId, int taskId, string firstName = null, string lastName = null, string userMail = null)
        {
            var taskSignup = CreateTaskSignup(taskSignupId, taskId, null, firstName, lastName, userMail);
            taskSignup.Task.Event.Campaign.CampaignContacts = new List<CampaignContact>();
            return taskSignup;
        }

        private static TaskSignup CreateTaskSignupWithoutCampaignContacts(int taskSignupId, int taskId, string firstName = null, string lastName = null, string userMail = null)
        {
            var taskSignup = CreateTaskSignup(taskSignupId, taskId, null, firstName, lastName, userMail);
            taskSignup.Task.Event.Campaign = new Campaign();
            return taskSignup;
        }
    }
}
