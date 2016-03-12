using System;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Activity
{
    public class UpdateMyTasksHandlerAsync : AsyncRequestHandler<UpdateMyTasksCommandAsync>
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public UpdateMyTasksHandlerAsync(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        protected override async Task HandleCore(UpdateMyTasksCommandAsync command)
        {
            var currentUser = _dataAccess.GetUser(command.UserId);
            foreach (var taskSignup in command.TaskSignups)
            {
                await _dataAccess.UpdateTaskSignupAsync(new TaskSignup
                {
                    Id = taskSignup.Id,
                    StatusDateTimeUtc = DateTime.UtcNow,
                    StatusDescription = taskSignup.StatusDescription,
                    Status = taskSignup.Status,
                    Task = new AllReadyTask { Id = taskSignup.TaskId },
                    User = currentUser
                });
            }
        }
    }
}
