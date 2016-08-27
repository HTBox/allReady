using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class UpdateTaskCommandHandlerAsync : AsyncRequestHandler<UpdateTaskCommandAsync>
    {
        private readonly AllReadyContext dataContext;

        public UpdateTaskCommandHandlerAsync(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        protected override async Task HandleCore(UpdateTaskCommandAsync message) {
            var task = message.AllReadyTask;
            //First remove any skills that are no longer associated with this task
            var tsToRemove = dataContext.TaskSkills.Where(ts => ts.TaskId == task.Id && (task.RequiredSkills == null ||
                                                                                         !task.RequiredSkills.Any(ts1 => ts1.SkillId == ts.SkillId)));
            dataContext.TaskSkills.RemoveRange(tsToRemove);
            dataContext.Tasks.Update(message.AllReadyTask);
            await dataContext.SaveChangesAsync();
        }
    }
}
