using System;
using System.Threading.Tasks;
using System.Linq;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Event
{
    public class UpdateMyTasksHandlerAsync : AsyncRequestHandler<UpdateMyTasksCommandAsync>
    {
        private readonly AllReadyContext dbContext;
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public UpdateMyTasksHandlerAsync(AllReadyContext DbContext)
        {
            this.dbContext = DbContext;
        }

        protected override async Task HandleCore(UpdateMyTasksCommandAsync command)
        {
            var currentUser = this.dbContext.Users.SingleOrDefault(u => u.Id == command.UserId);

            foreach (var taskSignupViewModel in command.TaskSignups)
            {
                this.dbContext.TaskSignups.Update(new TaskSignup {
                    Id = taskSignupViewModel.Id,
                    StatusDateTimeUtc = DateTimeUtcNow(),
                    StatusDescription = taskSignupViewModel.StatusDescription,
                    Status = taskSignupViewModel.Status,
                    Task = new AllReadyTask { Id = taskSignupViewModel.TaskId },
                    User = currentUser
                });
                await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
