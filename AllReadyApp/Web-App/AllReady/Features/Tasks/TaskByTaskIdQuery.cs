using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class TaskByTaskIdQuery : IAsyncRequest<VolunteerTask>
    {
        public int TaskId { get; set; }
    }
}
