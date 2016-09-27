﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class MessageTaskVolunteersCommandHandlerAsync : AsyncRequestHandler<MessageTaskVolunteersCommandAsync>
    {
        private AllReadyContext _context;
        private IMediator _mediator;

        public MessageTaskVolunteersCommandHandlerAsync(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        protected override async Task HandleCore(MessageTaskVolunteersCommandAsync message)
        {
            var users = await _context.TaskSignups.AsNoTracking()
                .Include(a => a.User)
                .Where(a => a.Task.Id == message.Model.TaskId).ToListAsync();

            // send all notifications to the queue
            var smsRecipients = new List<string>();
            var emailRecipients = new List<string>();

            //mgmccarthy: since we're allowing people to volunteer w/out confirming their email and phone, then we should allowing messaging them w/out confirming their email and phone
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

            await _mediator.SendAsync(command).ConfigureAwait(false);
        }
    }
}