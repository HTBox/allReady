using MediatR;

namespace AllReady.Features.Notifications
{
    public class VolunteerInformationAdded : IAsyncNotification
    {
        public int ActivityId { get; set; }
        public string UserId { get; set; }

    }
}
