using System.Threading.Tasks;
using AllReady.Configuration;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AllReady.Features.Notifications
{
    public class ItineraryVolunteerListUpdatedHandler : IAsyncNotificationHandler<ItineraryVolunteerListUpdated>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;
        private readonly IOptions<GeneralSettings> _generalSettings;

        public ItineraryVolunteerListUpdatedHandler(AllReadyContext context, IMediator mediator, IOptions<GeneralSettings> generalSettings)
        {
            _context = context;
            _mediator = mediator;
            _generalSettings = generalSettings;
        }

        public async Task Handle(ItineraryVolunteerListUpdated notification)
        {
            var volunteerTaskSignup = await _context.VolunteerTaskSignups
                .AsNoTracking()
                .Include(x => x.User)
                .SingleAsync(ts => ts.Id == notification.VolunteerTaskSignupId);

            var itinerary = await _context.Itineraries
                .AsNoTracking()
                .SingleAsync(x => x.Id == notification.ItineraryId);

            var emailAddress = volunteerTaskSignup.User.Email;
            var phoneNumber = volunteerTaskSignup.User.PhoneNumber;
            var itineraryDate = itinerary.Date;
            var volunteerDashboardLink = $"{_generalSettings.Value.SiteBaseUrl}v";

            var updateType = notification.UpdateType == UpdateType.VolunteerAssigned ? "assigned" : "unassigned";
            var toFrom = notification.UpdateType == UpdateType.VolunteerAssigned ? "to" : "from";

            var notifyVolunteersViewModel = new NotifyVolunteersViewModel
            {
                SmsMessage = $"You’ve been {updateType} {toFrom} a team for {itineraryDate} {volunteerDashboardLink}"
            };
            notifyVolunteersViewModel.SmsRecipients.Add(phoneNumber);

            var emailMessage = $"The volunteer organizer has {updateType} you {toFrom} a team for {itineraryDate}. See your {volunteerDashboardLink} for more information.";

            notifyVolunteersViewModel.Subject = $"You've been {updateType} {toFrom} a team for {itineraryDate}";
            notifyVolunteersViewModel.EmailRecipients.Add(emailAddress);
            notifyVolunteersViewModel.EmailMessage = emailMessage;
            notifyVolunteersViewModel.HtmlMessage = emailMessage;

            await _mediator.SendAsync(new NotifyVolunteersCommand { ViewModel = notifyVolunteersViewModel });
        }
    }
}