using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Notifications
{
    public class NotifyAssignedVolunteersForTheTask : IAsyncNotificationHandler<TaskAssignedToVolunteersNotification>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;

        public NotifyAssignedVolunteersForTheTask(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task Handle(TaskAssignedToVolunteersNotification notification)
        {
            var users = _context.Users.Where(x => notification.NewlyAssignedVolunteers.Contains(x.Id)).ToList();
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