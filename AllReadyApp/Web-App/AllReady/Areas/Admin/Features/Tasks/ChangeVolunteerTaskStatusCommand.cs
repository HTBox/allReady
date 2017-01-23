using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class ChangeVolunteerTaskStatusCommand : IAsyncRequest<VolunteerTaskChangeResult>
    {
        public int TaskId { get; set; }
        public string UserId { get; set; }
        public VolunteerTaskStatus TaskStatus { get; set; }
        public string TaskStatusDescription { get; set; }
    }
}
