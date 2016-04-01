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
                    .ThenInclude(t => t.Activity).ThenInclude(a => a.Organizer)
                .Include(ts => ts.Task)
                    .ThenInclude(t => t.Activity).ThenInclude(a => a.Campaign).ThenInclude(c => c.CampaignContacts).ThenInclude(cc => cc.Contact)
                .Include(ts => ts.User)
                .SingleAsync(ts => ts.Id == notification.SignupId)
                .ConfigureAwait(false);

            var volunteer = taskSignup.User;
            var task = taskSignup.Task;
            var activity = task.Activity;
            var campaign = activity.Campaign;

            var campaignContact = campaign.CampaignContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary);
            var adminEmail = campaignContact?.Contact.Email;

            if (!string.IsNullOrWhiteSpace(adminEmail))
            {
                var link = $"View activity: http://{_options.Value.SiteBaseUrl}/Admin/Task/Details/{taskSignup.Task.Id}";
                var subject = $"Task status changed ({taskSignup.Status}) for volunteer {volunteer.Name ?? volunteer.Email}";

                var message = $@"A volunteer's status has changed for a task.
                                    Volunteer: {volunteer.Name} ({volunteer.Email})
                                    New status: {taskSignup.Status}
                                    Task: {task.Name} {link}
                                    Activity: {activity.Name}
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
