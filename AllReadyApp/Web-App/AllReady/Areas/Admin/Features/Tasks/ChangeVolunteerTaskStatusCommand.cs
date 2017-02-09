using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class ChangeVolunteerTaskStatusCommand : IAsyncRequest<VolunteerTaskChangeResult>
    {
        public int VolunteerTaskId { get; set; }
        public string UserId { get; set; }
        public VolunteerTaskStatus VolunteerTaskStatus { get; set; }
        public string VolunteerTaskStatusDescription { get; set; }
    }
}
