using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class AssignVolunteerToTaskCommand : IAsyncRequest
    {
        public int VolunteerTaskId { get; set; }
        public string UserId { get; set; }
        public bool NotifyUser { get; set; }
    }
}
