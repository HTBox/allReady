using MediatR;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class AddTeamMemberCommand : IAsyncRequest<bool>
    {
        public int ItineraryId { get; set; }
        public int VolunteerTaskSignupId { get; set; }
    }
}
