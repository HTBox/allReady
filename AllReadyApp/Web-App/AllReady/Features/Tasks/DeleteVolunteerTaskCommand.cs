using MediatR;

namespace AllReady.Features.Tasks
{
    public class DeleteVolunteerTaskCommand : IAsyncRequest
    {
        public int VolunteerTaskId { get; set; }
    }
}
