using AllReady.Areas.Admin.Models;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Collections.Generic;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class MessageTaskVolunteersCommandHandler : RequestHandler<MessageTaskVolunteersCommand>
    {
        private AllReadyContext _context;
        private IMediator _bus;

        public MessageTaskVolunteersCommandHandler(AllReadyContext context, IMediator bus)
        {
            _context = context;
            _bus = bus;
        }

        protected override void HandleCore(MessageTaskVolunteersCommand message)
        {
            var users =
                _context.TaskSignups.AsNoTracking()
                .Include(a => a.User)
                .Where(a => a.Task.Id == message.Model.TaskId).ToList();


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
                    EmailRecipients = emailRecipients,
                    Subject = message.Model.Subject
                }
            };

            _bus.Send(command);
        }
    }
}
