using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using Microsoft.Extensions.OptionsModel;

namespace AllReady.Features.Notifications
{
    public class NotifyAdminForTaskSignupStatusChange : IAsyncNotificationHandler<TaskSignupStatusChanged>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;
        private readonly IOptions<GeneralSettings> _options;
        
        public NotifyAdminForTaskSignupStatusChange(AllReadyContext context, IMediator mediator, IOptions<GeneralSettings> options)
        {
            _context = context;
            _mediator = mediator;
            _options = options;
        }

        public async Task Handle(TaskSignupStatusChanged notification)
        {
            var taskSignup = await _context.TaskSignups
                .Include(ts => ts.Task)
                    .ThenInclude(t => t.Event).ThenInclude(a => a.Organizer)
                .Include(ts => ts.Task)
                    .ThenInclude(t => t.Event).ThenInclude(a => a.Campaign).ThenInclude(c => c.CampaignContacts).ThenInclude(cc => cc.Contact)
                .Include(ts => ts.User)
                .SingleAsync(ts => ts.Id == notification.SignupId)
                .ConfigureAwait(false);

            var volunteer = taskSignup.User;
            var task = taskSignup.Task;
            var campaignEvent = task.Event;
            var campaign = campaignEvent.Campaign;

            var campaignContact = campaign.CampaignContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary);
            var adminEmail = campaignContact?.Contact.Email;

            if (!string.IsNullOrWhiteSpace(adminEmail))
            {
                var link = $"View event: http://{_options.Value.SiteBaseUrl}/Admin/Task/Details/{taskSignup.Task.Id}";

                var subject = volunteer.FirstName != null && volunteer.LastName != null ? $"{volunteer.FirstName} {volunteer.LastName}" : volunteer.Email;

                var message = $@"A volunteer's status has changed for a task.
                    Volunteer: {volunteer.FirstName} {volunteer.LastName} ({volunteer.Email})
                    New status: {taskSignup.Status}
                    Task: {task.Name} {link}
                    Event: {campaignEvent.Name}
                    Campaign: {campaign.Name}";

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
