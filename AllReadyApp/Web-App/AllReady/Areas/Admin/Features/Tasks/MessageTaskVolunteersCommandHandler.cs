using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class MessageTaskVolunteersCommandHandler : AsyncRequestHandler<MessageTaskVolunteersCommand>
    {
        private AllReadyContext _context;
        private IMediator _mediator;

        public MessageTaskVolunteersCommandHandler(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        protected override async Task HandleCore(MessageTaskVolunteersCommand message)
        {
            var users = await _context.VolunteerTaskSignups.AsNoTracking()
                .Include(a => a.User)
                .Where(a => a.VolunteerTask.Id == message.Model.VolunteerTaskId).ToListAsync();

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
                    SmsMessage = message.Model.Message,
                    SmsRecipients = smsRecipients,
                    EmailMessage = message.Model.Message,
                    HtmlMessage = message.Model.Message,
                    EmailRecipients = emailRecipients,
                    Subject = message.Model.Subject
                }
            };

            await _mediator.SendAsync(command);
        }
    }
}