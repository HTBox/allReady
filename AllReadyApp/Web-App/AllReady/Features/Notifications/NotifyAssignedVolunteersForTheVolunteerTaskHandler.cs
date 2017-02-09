using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Notifications
{
    public class NotifyAssignedVolunteersForTheVolunteerTaskHandler : IAsyncNotificationHandler<VolunteerTaskAssignedToVolunteersNotification>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;

        public NotifyAssignedVolunteersForTheVolunteerTaskHandler(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task Handle(VolunteerTaskAssignedToVolunteersNotification notification)
        {
            var users = await _context.Users.Where(x => notification.NewlyAssignedVolunteers.Contains(x.Id)).ToListAsync();
            var smsRecipients = users.Where(u => u.PhoneNumberConfirmed).Select(v => v.PhoneNumber).ToList();
            var emailRecipients = users.Where(u => u.EmailConfirmed).Select(v => v.Email).ToList();
            var command = new NotifyVolunteersCommand
            {
                ViewModel = new NotifyVolunteersViewModel
                {
                    SmsMessage = "You've been assigned a task from AllReady.",
                    SmsRecipients = smsRecipients,
                    EmailMessage = "You've been assigned a task from AllReady.",
                    HtmlMessage = "You've been assigned a task from AllReady.",
                    EmailRecipients = emailRecipients,
                    Subject = "You've been assigned a task from AllReady."
                }
            };
            await _mediator.SendAsync(command);
        }
    }
}