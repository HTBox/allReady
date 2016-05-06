using System;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Event
{
    public class UpdateMyTasksHandlerAsync : AsyncRequestHandler<UpdateMyTasksCommandAsync>
    {
        private readonly IAllReadyDataAccess _dataAccess;
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public UpdateMyTasksHandlerAsync(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        protected override async Task HandleCore(UpdateMyTasksCommandAsync command)
        {
            var currentUser = _dataAccess.GetUser(command.UserId);

            foreach (var taskSignupViewModel in command.TaskSignups)
            {
                await _dataAccess.UpdateTaskSignupAsync(new TaskSignup
                {
                    Id = taskSignupViewModel.Id,
                    StatusDateTimeUtc = DateTimeUtcNow(),
                    StatusDescription = taskSignupViewModel.StatusDescription,
                    Status = taskSignupViewModel.Status,
                    Task = new AllReadyTask { Id = taskSignupViewModel.TaskId },
                    User = currentUser
                }).ConfigureAwait(false);
            }
        }
    }
}
