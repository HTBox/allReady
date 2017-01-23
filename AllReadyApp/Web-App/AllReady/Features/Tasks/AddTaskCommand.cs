using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class AddTaskCommand : IAsyncRequest
    {
        public VolunteerTask AllReadyTask { get; set; }
    }
}
