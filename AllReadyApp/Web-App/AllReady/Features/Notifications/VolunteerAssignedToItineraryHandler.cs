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
            var taskSignup = await _context.TaskSignups
                .AsNoTracking()
                .Include(x => x.User)
                .SingleAsync(ts => ts.Id == notification.TaskSignupId)
                .ConfigureAwait(false);

            var emailAddress = !string.IsNullOrWhiteSpace(taskSignup.PreferredEmail) ? taskSignup.PreferredEmail : taskSignup.User?.Email;
            var sms = !string.IsNullOrWhiteSpace(taskSignup.PreferredPhoneNumber) ? taskSignup.PreferredPhoneNumber : taskSignup.User?.PhoneNumber;

            if (string.IsNullOrWhiteSpace(emailAddress) && string.IsNullOrWhiteSpace(sms)) return;

            var emailMessage = "The volunteer organizer has assigned you to a team for [date]. See your [allReady Volunteer Dashboard] for more information.";

            var notifyVolunteersViewModel = new NotifyVolunteersViewModel();
            if (!string.IsNullOrWhiteSpace(sms))
            {
                notifyVolunteersViewModel.SmsMessage = "You’ve been assigned to a team for [date] [link to volunteer dashboard]";
                notifyVolunteersViewModel.SmsRecipients.Add(sms);
            }
            if (!string.IsNullOrWhiteSpace(emailAddress))
            {
                notifyVolunteersViewModel.Subject = "You've been assigned to a team for [date]";
                notifyVolunteersViewModel.EmailRecipients.Add(emailAddress);
                notifyVolunteersViewModel.EmailMessage = emailMessage;
                notifyVolunteersViewModel.HtmlMessage = emailMessage;
            }

            await _mediator.SendAsync(new NotifyVolunteersCommand { ViewModel = notifyVolunteersViewModel }).ConfigureAwait(false);
        }
    }
}
