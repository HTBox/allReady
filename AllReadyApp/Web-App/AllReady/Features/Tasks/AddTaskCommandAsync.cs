using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class AddTaskCommandAsync : IAsyncRequest
    {
        public AllReadyTask AllReadyTask { get; set; }
    }
}
