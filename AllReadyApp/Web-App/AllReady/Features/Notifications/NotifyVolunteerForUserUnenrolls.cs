using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.OptionsModel;

namespace AllReady.Features.Notifications
{
    public class NotifyVolunteerForUserUnenrolls : IAsyncNotificationHandler<UserUnenrolls>
    {
        private readonly IMediator _mediator;
        private readonly IOptions<GeneralSettings> _options;

        public NotifyVolunteerForUserUnenrolls(IMediator mediator, IOptions<GeneralSettings> options)
        {
            _mediator = mediator;
            _options = options;
        }

        public async Task Handle(UserUnenrolls notification)
        {
            var model = await _mediator.SendAsync(new ActivityDetailForNotificationQueryAsync { ActivityId = notification.ActivityId, UserId = notification.UserId })
                .ConfigureAwait(false);

            var signup = model.UsersSignedUp?.FirstOrDefault(s => s.User.Id == notification.UserId);
            if (signup == null)
            {
                return;
            }
            
            var emailRecipient = !string.IsNullOrWhiteSpace(signup.PreferredEmail)
                ? signup.PreferredEmail
                : signup.User?.Email;
            if (string.IsNullOrWhiteSpace(emailRecipient))
            {
                return;
            }
            
            var activityLink = $"View activity: {_options.Value.SiteBaseUrl}Admin/Activity/Details/{model.ActivityId}";

            var message = new StringBuilder();
            message.AppendLine("This is to confirm that you have elected to un-enroll from the following activity:");
            message.AppendLine();
            message.AppendLine($"   Campaign: {model.CampaignName}");
            message.AppendLine($"   Activity: {model.ActivityName} ({activityLink})");
            message.AppendLine();
            message.AppendLine("Thanks for letting us know that you will not be participating.");

            var command = new NotifyVolunteersCommand
            {
                ViewModel = new NotifyVolunteersViewModel
                {
                    EmailMessage = message.ToString(),
                    HtmlMessage = message.ToString(),
                    EmailRecipients = new List<string> { emailRecipient },
                    Subject = "allReady Activity Un-enrollment Confirmation"
                }
            };

            await _mediator.SendAsync(command).ConfigureAwait(false);
        }
    }
}