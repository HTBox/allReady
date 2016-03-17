using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using Microsoft.Extensions.OptionsModel;

namespace AllReady.Features.Notifications
{
    public class NotifyAdminForActivitySignup : IAsyncNotificationHandler<VolunteerInformationAdded>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;
        private readonly IOptions<GeneralSettings> _options;

        public NotifyAdminForActivitySignup(AllReadyContext context, IMediator mediator, IOptions<GeneralSettings> options)
        {
            _context = context;
            _mediator = mediator;
            _options = options;
        }

        public async Task Handle(VolunteerInformationAdded notification)
        {
            var volunteer = _context.Users.Single(u => u.Id == notification.UserId);
            var activity = _context.Activities.Single(a => a.Id == notification.ActivityId);
            var campaign = _context.Campaigns
                .Include(c => c.CampaignContacts).ThenInclude(cc => cc.Contact)
                .Single(c => c.Id == activity.CampaignId);
            var link = $"View activity: http://{_options.Value.SiteBaseUrl}/Admin/Activity/Details/{activity.Id}";

            var subject = $"A volunteer has signed up for {activity.Name}";

            var campaignContact = campaign.CampaignContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary);
            var adminEmail = campaignContact?.Contact.Email;

            if (!string.IsNullOrWhiteSpace(adminEmail))
            {
                var message = $"Your {campaign.Name} campaign activity '{activity.Name}' has a new volunteer. {volunteer.UserName} can be reached at {volunteer.Email}. {link}";
                var command = new NotifyVolunteersCommand
                {
                    ViewModel = new NotifyVolunteersViewModel
                    {
                        EmailMessage = message,
                        HtmlMessage = message,
                        EmailRecipients = new List<string> { adminEmail },
                        Subject = subject
                    }
                };

                await _mediator.SendAsync(command).ConfigureAwait(false);
            }
        }
    }
}
