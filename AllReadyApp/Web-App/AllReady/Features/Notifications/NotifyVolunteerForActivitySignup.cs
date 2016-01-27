using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllReady.Areas.Admin.Features.Activities;
using AllReady.Features.Login;
using AllReady.Models;
using MediatR;
using Microsoft.Extensions.OptionsModel;

namespace AllReady.Features.Notifications
{
    public class NotifyVolunteerForActivitySignup : INotificationHandler<VolunteerInformationAdded>
    {
        private readonly IMediator _bus;
        private readonly IOptions<GeneralSettings> _options;

        public NotifyVolunteerForActivitySignup(IMediator bus, IOptions<GeneralSettings> options)
        {
            _bus = bus;
            _options = options;
        }

        public void Handle(VolunteerInformationAdded notification)
        {
            var model = _bus.Send(new ActivityDetailForNotificationQuery {ActivityId = notification.ActivityId});

            var signup = model.UsersSignedUp.FirstOrDefault(s => s.User.Id == notification.UserId);

            if (signup == null || !signup.User.EmailConfirmed) return;

            var activityLink = $"View activity: {_options.Value.SiteBaseUrl}Admin/Activity/Details/{model.ActivityId}";
            var subject = "allReady Activity Enrollment Confirmation";

            var message = new StringBuilder();
            message.AppendLine($"This is to confirm that you have volunteered to participate in the following activity:");
            message.AppendLine();
            message.AppendLine($"   Campaign: {model.CampaignName}");
            message.AppendLine($"   Activity: {model.ActivityName} ({activityLink})");
            message.AppendLine();
            message.AppendLine($"Thanks for volunteering. Your help is appreciated.");

            var command = new NotifyVolunteersCommand
            {
                ViewModel = new NotifyVolunteersViewModel
                {
                    EmailMessage = message.ToString(),
                    HtmlMessage = message.ToString(),
                    EmailRecipients = new List<string> { signup.User.Email },
                    Subject = subject
                }
            };
            _bus.Send(command);
        }
    }
}