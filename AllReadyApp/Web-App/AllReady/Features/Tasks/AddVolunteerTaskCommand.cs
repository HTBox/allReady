using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class AddVolunteerTaskCommand : IAsyncRequest
    {
        public VolunteerTask VolunteerTask { get; set; }
    }
}
