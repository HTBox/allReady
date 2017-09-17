using MediatR;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class RemoveTeamMemberCommand : IAsyncRequest<bool>
    {
        public int VolunteerTaskSignupId { get; set; }
    }
}
