using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class UpdateVolunteerTaskCommand : IAsyncRequest
    {
        public VolunteerTask VolunteerTask { get; set; }
    }
}
