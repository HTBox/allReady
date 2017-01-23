using MediatR;

namespace AllReady.Features.Tasks
{
    public class VolunteerTaskUnenrollCommand : IAsyncRequest<VolunteerTaskUnenrollResult>
    {
        public int VolunteerTaskId { get; set; }
        public string UserId { get; set; }
    }
}
