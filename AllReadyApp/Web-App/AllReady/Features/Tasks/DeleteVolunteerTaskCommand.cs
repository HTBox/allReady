using MediatR;

namespace AllReady.Features.Tasks
{
    public class DeleteVolunteerTaskCommand : IAsyncRequest
    {
        public int TaskId { get; set; }
    }
}
