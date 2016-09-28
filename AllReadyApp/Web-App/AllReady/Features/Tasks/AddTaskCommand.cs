using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class AddTaskCommand : IAsyncRequest
    {
        public AllReadyTask AllReadyTask { get; set; }
    }
}
