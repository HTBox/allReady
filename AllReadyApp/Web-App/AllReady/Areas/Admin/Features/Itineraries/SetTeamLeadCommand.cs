using MediatR;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class SetTeamLeadCommand : IAsyncRequest<SetTeamLeadResult>
    {
        public SetTeamLeadCommand(int itineraryId, int volunteerTaskId, string itineraryUrl)
        {
            ItineraryId = itineraryId;
            VolunteerTaskId = volunteerTaskId;
            ItineraryUrl = itineraryUrl;
        }

        public int ItineraryId { get; }
        public int VolunteerTaskId { get; }
        public string ItineraryUrl { get; }
    }

    public enum SetTeamLeadResult
    {
        Success,
        VolunteerTaskSignupNotFound,
        SaveChangesFailed
    }
}
