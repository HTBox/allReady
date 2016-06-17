using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Features.Notifications
{
    public class VolunteerAssignedToItineraryHandler : IAsyncNotificationHandler<VolunteerAssignedToItinerary>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;

        public VolunteerAssignedToItineraryHandler(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task Handle(VolunteerAssignedToItinerary notification)
        {
            var taskSignup = await _context.TaskSignups.SingleAsync(ts => ts.Id == notification.TaskSignupId).ConfigureAwait(false);

            var emailAddress = !string.IsNullOrWhiteSpace(taskSignup.PreferredEmail) ? taskSignup.PreferredEmail : taskSignup.User?.Email;
            var sms = !string.IsNullOrWhiteSpace(taskSignup.PreferredPhoneNumber) ? taskSignup.PreferredPhoneNumber : taskSignup.User?.PhoneNumber;

            if (string.IsNullOrWhiteSpace(emailAddress) && string.IsNullOrWhiteSpace(sms)) return;

            //construct subject and body of message
            var subject = "You've been assigned to an intinerary";
            var message = "You've been assigned to an Intinerary";

            var notifyVolunteersViewModel = new NotifyVolunteersViewModel();
            if (!string.IsNullOrWhiteSpace(sms))
            {
                notifyVolunteersViewModel.SmsMessage = message;
                notifyVolunteersViewModel.SmsRecipients.Add(sms);
            }
            if(!string.IsNullOrWhiteSpace(emailAddress))
            {
                notifyVolunteersViewModel.Subject = subject;
                notifyVolunteersViewModel.EmailRecipients.Add(emailAddress);
                notifyVolunteersViewModel.EmailMessage = message;
                notifyVolunteersViewModel.HtmlMessage = message;
            }

            await _mediator.SendAsync(new NotifyVolunteersCommand { ViewModel = notifyVolunteersViewModel }).ConfigureAwait(false);
        }
    }
}
