using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class TaskByTaskIdQueryAsync : IAsyncRequest<AllReadyTask>
    {
        public int TaskId { get; set; }
    }
}
