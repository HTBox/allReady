using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class UpdateTaskCommandHandler : AsyncRequestHandler<UpdateTaskCommand>
    {
        private readonly AllReadyContext dataContext;

        public UpdateTaskCommandHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        protected override async Task HandleCore(UpdateTaskCommand message) {
            var theTask = message.AllReadyTask;
            //First remove any skills that are no longer associated with this task
            var tsToRemove = dataContext.TaskSkills.Where(ts => ts.TaskId == theTask.Id && (theTask.RequiredSkills == null ||
                                                                                         !theTask.RequiredSkills.Any(ts1 => ts1.SkillId == ts.SkillId)));
            dataContext.TaskSkills.RemoveRange(tsToRemove);
            dataContext.Tasks.Update(message.AllReadyTask);
            await dataContext.SaveChangesAsync();
        }
    }
}
