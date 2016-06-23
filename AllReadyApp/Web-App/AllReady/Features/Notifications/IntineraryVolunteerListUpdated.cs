using MediatR;

namespace AllReady.Features.Notifications
{
    public class IntineraryVolunteerListUpdated : IAsyncNotification
    {
        public int TaskSignupId { get; set; }
        public int ItineraryId { get; set; }
        public UpdateType UpdateType { get; set; }
    }

    public enum UpdateType
    {
        VolunteerAssigned,
        VolnteerUnassigned
    }
}
