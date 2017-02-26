using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Events
{
    public class MessageEventVolunteersCommandHandler : AsyncRequestHandler<MessageEventVolunteersCommand>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;

        public MessageEventVolunteersCommandHandler(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        protected override async Task HandleCore(MessageEventVolunteersCommand message)
        {
            var users =
                _context.VolunteerTaskSignups.AsNoTracking()
                .Include(a => a.User)
                .Include(a => a.VolunteerTask)
                .Where(a => a.VolunteerTask.EventId == message.ViewModel.EventId).ToList();

            // send all notifications to the queue
            var smsRecipients = new List<string>();
            var emailRecipients = new List<string>();

            smsRecipients.AddRange(users.Where(u => u.User.PhoneNumberConfirmed).Select(v => v.User.PhoneNumber));
            emailRecipients.AddRange(users.Where(u => u.User.EmailConfirmed).Select(v => v.User.Email));
            
            var command = new NotifyVolunteersCommand
            {
                // todo: what information do we add about the task?
                // todo: should we use a template from the email service provider?
                // todo: what about non-English volunteers?
                ViewModel = new NotifyVolunteersViewModel
                {
                    SmsMessage = message.ViewModel.Message,
                    SmsRecipients = smsRecipients,
                    EmailMessage = message.ViewModel.Message,
                    HtmlMessage = message.ViewModel.Message,
                    EmailRecipients = emailRecipients,
                    Subject = message.ViewModel.Subject
                }
            };

            await _mediator.SendAsync(command);
        }
    }
}
