using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class UpdateTaskCommandAsync : IAsyncRequest
    {
        public AllReadyTask AllReadyTask { get; set; }
    }
}
