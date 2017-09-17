using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class VolunteerTaskByVolunteerTaskIdQuery : IAsyncRequest<VolunteerTask>
    {
        public int VolunteerTaskId { get; set; }
    }
}
