using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Events
{
    public class UpdateMyTasksCommandHandler : AsyncRequestHandler<UpdateMyTasksCommand>
    {
        private readonly AllReadyContext dbContext;
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public UpdateMyTasksCommandHandler(AllReadyContext DbContext)
        {
            this.dbContext = DbContext;
        }

        protected override async Task HandleCore(UpdateMyTasksCommand command)
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
                await this.dbContext.SaveChangesAsync();
            }
        }
    }
}
