using MediatR;

namespace AllReady.Features.Notifications
{
    public class VolunteerAssignedToItinerary : IAsyncNotification
    {
        public int ItineraryId { get; set; }
        public int TaskSignupId { get; set; }
    }
}
