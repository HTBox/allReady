using MediatR;

namespace AllReady.Features.Tasks
{
    public class VolunteerTaskUnenrollCommand : IAsyncRequest<VolunteerTaskUnenrollResult>
    {
        public int TaskId { get; set; }
        public string UserId { get; set; }
    }
}
