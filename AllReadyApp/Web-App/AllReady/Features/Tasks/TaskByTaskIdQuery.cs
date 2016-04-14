using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class TaskByTaskIdQuery : IRequest<AllReadyTask>
    {
        public int TaskId { get; set; }
    }
}
