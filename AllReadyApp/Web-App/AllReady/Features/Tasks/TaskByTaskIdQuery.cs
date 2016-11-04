using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class TaskByTaskIdQuery : IAsyncRequest<AllReadyTask>
    {
        public int TaskId { get; set; }
    }
}
