using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class DeleteTaskCommandHandler : AsyncRequestHandler<DeleteTaskCommand>
    {
        private readonly AllReadyContext dataContext;

        public DeleteTaskCommandHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        protected override async Task HandleCore(DeleteTaskCommand message)
        {
            var toDelete = this.dataContext.Tasks.Where(t => t.Id == message.TaskId).SingleOrDefault();

            if (toDelete != null) {
                dataContext.Tasks.Remove(toDelete);
                await dataContext.SaveChangesAsync();
            }
        }
    }
}
