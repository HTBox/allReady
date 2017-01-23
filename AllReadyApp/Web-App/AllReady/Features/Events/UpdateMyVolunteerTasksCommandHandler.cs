using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Events
{
    public class UpdateMyVolunteerTasksCommandHandler : AsyncRequestHandler<UpdateMyVolunteerTasksCommand>
    {
        private readonly AllReadyContext dbContext;
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public UpdateMyVolunteerTasksCommandHandler(AllReadyContext dbContext)
        {
            this.dbContext = dbContext;
        }

        protected override async Task HandleCore(UpdateMyVolunteerTasksCommand command)
        {
            var currentUser = dbContext.Users.SingleOrDefault(u => u.Id == command.UserId);

            foreach (var volunteerTaskSignupViewModel in command.VolunteerTaskSignups)
            {
                dbContext.VolunteerTaskSignups.Update(new VolunteerTaskSignup
                {
                    Id = volunteerTaskSignupViewModel.Id,
                    StatusDateTimeUtc = DateTimeUtcNow(),
                    StatusDescription = volunteerTaskSignupViewModel.StatusDescription,
                    Status = (Models.VolunteerTaskStatus)Enum.Parse(typeof(Models.VolunteerTaskStatus), volunteerTaskSignupViewModel.Status),
                    VolunteerTask = new VolunteerTask { Id = volunteerTaskSignupViewModel.VolunteerTaskId },
                    User = currentUser
                });
                await dbContext.SaveChangesAsync();
            }
        }
    }
}