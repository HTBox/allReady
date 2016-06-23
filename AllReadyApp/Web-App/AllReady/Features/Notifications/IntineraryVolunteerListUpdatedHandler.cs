using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using Microsoft.Extensions.OptionsModel;

namespace AllReady.Features.Notifications
{
    public class IntineraryVolunteerListUpdatedHandler : IAsyncNotificationHandler<IntineraryVolunteerListUpdated>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;
        private readonly IOptions<GeneralSettings> _generalSettings;

        public IntineraryVolunteerListUpdatedHandler(AllReadyContext context, IMediator mediator, IOptions<GeneralSettings> generalSettings)
        {
            _context = context;
            _mediator = mediator;
            _generalSettings = generalSettings;
        }

        public async Task Handle(IntineraryVolunteerListUpdated notification)
        {
            //when volunteer is unassigned, the TaskSignup's ItineraryId is set to null in RemoveTeamMemberCommandHandlerAsync, so in an unassign scenario, the .Include to 
            //Itinerary will pull back null, which, in turn, throws a null reference exception below when trying to retrieve taskSignup.Itinerary.[PROPERTY_NAME]
            //this is why we have a separate query for Intinerary by IntineraryId supplied on the notification
            var taskSignup = await _context.TaskSignups
                .AsNoTracking()
                .Include(x => x.User)
                .SingleAsync(ts => ts.Id == notification.TaskSignupId)
                .ConfigureAwait(false);

            var itinerary = await _context.Itineraries
                .AsNoTracking()
                .SingleAsync(x => x.Id == notification.ItineraryId)
                .ConfigureAwait(false);

            var emailAddress = !string.IsNullOrWhiteSpace(taskSignup.PreferredEmail) ? taskSignup.PreferredEmail : taskSignup.User?.Email;
            var phoneNumber = !string.IsNullOrWhiteSpace(taskSignup.PreferredPhoneNumber) ? taskSignup.PreferredPhoneNumber : taskSignup.User?.PhoneNumber;

            if (string.IsNullOrWhiteSpace(emailAddress) && string.IsNullOrWhiteSpace(phoneNumber))
                return;

            var itineraryDate = itinerary.Date;
            var volunteerDashboardLink = $"{_generalSettings.Value.SiteBaseUrl}v";
            var updateType = notification.UpdateType == UpdateType.VolunteerAssigned ? "assigned" : "unassigned";
            var toFrom = notification.UpdateType == UpdateType.VolunteerAssigned ? "to" : "from";

            var notifyVolunteersViewModel = new NotifyVolunteersViewModel();
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                notifyVolunteersViewModel.SmsMessage = $"You’ve been {updateType} {toFrom} a team for {itineraryDate} {volunteerDashboardLink}";
                notifyVolunteersViewModel.SmsRecipients.Add(phoneNumber);
            }
            if (!string.IsNullOrWhiteSpace(emailAddress))
            {
                var emailMessage = $"The volunteer organizer has {updateType} you {toFrom} a team for {itineraryDate}. See your {volunteerDashboardLink} for more information.";
                notifyVolunteersViewModel.Subject = $"You've been {updateType} {toFrom} a team for {itineraryDate}";
                notifyVolunteersViewModel.EmailRecipients.Add(emailAddress);
                notifyVolunteersViewModel.EmailMessage = emailMessage;
                notifyVolunteersViewModel.HtmlMessage = emailMessage;
            }

            await _mediator.SendAsync(new NotifyVolunteersCommand { ViewModel = notifyVolunteersViewModel }).ConfigureAwait(false);
        }
    }
}
