using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class UpdateVolunteerTaskCommandHandler : AsyncRequestHandler<UpdateVolunteerTaskCommand>
    {
        private readonly AllReadyContext dataContext;

        public UpdateVolunteerTaskCommandHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        protected override async Task HandleCore(UpdateVolunteerTaskCommand message) {
            var @task = message.VolunteerTask;
            
            //First remove any skills that are no longer associated with this task
            var tsToRemove = dataContext.TaskSkills.Where(ts => ts.VolunteerTaskId == @task.Id && (@task.RequiredSkills == null || !@task.RequiredSkills.Any(ts1 => ts1.SkillId == ts.SkillId)));
            dataContext.TaskSkills.RemoveRange(tsToRemove);

            dataContext.Tasks.Update(message.VolunteerTask);

            await dataContext.SaveChangesAsync();
        }
    }
}
