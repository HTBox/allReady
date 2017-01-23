using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class UpdateTaskCommand : IAsyncRequest
    {
        public VolunteerTask AllReadyTask { get; set; }
    }
}
