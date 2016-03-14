using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class TaskByTaskIdQueryHandler : IRequestHandler<TaskByTaskIdQuery, AllReadyTask>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public TaskByTaskIdQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public AllReadyTask Handle(TaskByTaskIdQuery message)
        {
            return dataAccess.GetTask(message.TaskId);
        }
    }
}
