using MediatR;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class SetTeamLeadCommand : IAsyncRequest<SetTeamLeadResult>
    {
        public SetTeamLeadCommand(int itineraryId, int volunteerTaskId)
        {
            ItineraryId = itineraryId;
            VolunteerTaskId = volunteerTaskId;
        }

        public int ItineraryId { get; }
        public int VolunteerTaskId { get; }
    }

    public enum SetTeamLeadResult
    {
        Success,
        VolunteerTaskSignupNotFound,
        SaveChangesFailed
    }
}
