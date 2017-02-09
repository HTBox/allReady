using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Configuration;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AllReady.Features.Notifications
{
    public class NotifyAdminForTaskSignupStatusChangeHandler : IAsyncNotificationHandler<TaskSignupStatusChanged>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;
        private readonly IOptions<GeneralSettings> _options;
        
        public NotifyAdminForTaskSignupStatusChangeHandler(AllReadyContext context, IMediator mediator, IOptions<GeneralSettings> options)
        {
            _context = context;
            _mediator = mediator;
            _options = options;
        }

        public async Task Handle(TaskSignupStatusChanged notification)
        {
            var volunteerTaskSignup = await _context.TaskSignups
                .Include(ts => ts.VolunteerTask)
                    .ThenInclude(t => t.Event).ThenInclude(a => a.Organizer)
                .Include(ts => ts.VolunteerTask)
                    .ThenInclude(t => t.Event).ThenInclude(a => a.Campaign).ThenInclude(c => c.CampaignContacts).ThenInclude(cc => cc.Contact)
                .Include(ts => ts.User)
                .SingleAsync(ts => ts.Id == notification.SignupId);

            var volunteer = volunteerTaskSignup.User;
            var volunteerTask = volunteerTaskSignup.VolunteerTask;
            var campaignEvent = volunteerTask.Event;
            var campaign = campaignEvent.Campaign;

            var campaignContact = campaign.CampaignContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary);
            var adminEmail = campaignContact?.Contact.Email;

            if (!string.IsNullOrWhiteSpace(adminEmail))
            {
                var link = $"View event: http://{_options.Value.SiteBaseUrl}/Admin/Task/Details/{volunteerTaskSignup.VolunteerTask.Id}";

                var message = $@"A volunteer's status has changed for a task.
                    Volunteer: {volunteer.Name} ({volunteer.Email})
                    New status: {volunteerTaskSignup.Status}
                    Task: {volunteerTask.Name} {link}
                    Event: {campaignEvent.Name}
                    Campaign: {campaign.Name}";

                var command = new NotifyVolunteersCommand
                {
                    ViewModel = new NotifyVolunteersViewModel
                    {
                        EmailMessage = message,
                        HtmlMessage = message,
                        EmailRecipients = new List<string> { adminEmail },
                        Subject = $"{volunteer.FirstName} {volunteer.LastName}"
                    }
                };

                await _mediator.SendAsync(command);
            }
        }
    }
}
