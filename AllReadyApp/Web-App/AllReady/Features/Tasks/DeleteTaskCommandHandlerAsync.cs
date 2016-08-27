using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class DeleteTaskCommandHandlerAsync : AsyncRequestHandler<DeleteTaskCommandAsync>
    {
        private readonly AllReadyContext dataContext;

        public DeleteTaskCommandHandlerAsync(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        protected override async Task HandleCore(DeleteTaskCommandAsync message)
        {
            var toDelete = this.dataContext.Tasks.Where(t => t.Id == message.TaskId).SingleOrDefault();

            if (toDelete != null) {
                dataContext.Tasks.Remove(toDelete);
                await dataContext.SaveChangesAsync();
            }
        }
    }
}
